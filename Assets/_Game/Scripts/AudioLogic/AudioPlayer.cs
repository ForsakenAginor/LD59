using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Source.Scripts.AudioLogic
{
    public class AudioPlayer : MonoBehaviour
    {
        private static AudioPlayer _instance;

        [Header("Queue logic")]
        private readonly int _maxSimultaneousSounds = 10;
        private readonly Queue<AudioSource> _audioSources = new Queue<AudioSource>();
        private readonly Dictionary<AudioSource, WaitWhileCached> _cachedWaitWhiles = new Dictionary<AudioSource, WaitWhileCached>();
        [SerializeField] private AudioSource[] _audioSourcesArray;

        [Header("Audio clips")]
        [SerializeField] private AudioClip _saw;
        [SerializeField] private AudioClip _sin;
        [SerializeField] private AudioClip _digital;
        [SerializeField] private AudioClip _joker;
        [SerializeField] private AudioClip _cardSelection;
        
        [SerializeField] private AudioClip _cardHighlight;
        [SerializeField] private AudioClip _jokerSelectionAppear;
        [SerializeField] private AudioClip _winRound;
        [SerializeField] private AudioClip _loseRound;
        [SerializeField] private AudioClip _winGame;
        [SerializeField] private AudioClip _interferenceAlert;
        [SerializeField] private AudioClip _newRoundPrepare;
        [SerializeField] private AudioClip _monitorEnable;
        [SerializeField] private AudioClip _cardFlying;
        [SerializeField] private AudioClip _patternSound;

        public static AudioPlayer Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<AudioPlayer>();

                    if (_instance == null)
                        Debug.LogError("AudioPlayer not representing at scene");
                }

                return _instance;
            }
        }

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            else
            {
                _instance = this;

                if (_audioSourcesArray.Length != _maxSimultaneousSounds)
                    throw new Exception("audioSources amount is not correct");

                for (int i = 0; i < _maxSimultaneousSounds; i++)
                {
                    _audioSources.Enqueue(_audioSourcesArray[i]);
                    _cachedWaitWhiles.Add(_audioSourcesArray[i], new WaitWhileCached(() => false));
                }
            }
        }

        private void OnDestroy()
        {
            if (_instance == this)
                _instance = null;
        }

        public void PlaySaw()
        {
            PlaySound(_saw, 0.8f, true);
        }

        public void PlaySin()
        {
            PlaySound(_sin, 0.8f, true);
        }

        public void PlayDigital()
        {
            PlaySound(_digital, 0.8f, true);
        }

        public void PlayJoker()
        {
            PlaySound(_joker, 0.8f, true);
        }

        public void PlayCardSelection()
        {
            PlaySound(_cardSelection, 0.5f, true);
        }
        
        public void PlayCardHighlight()
        {
            PlaySound(_cardHighlight, 0.5f, true);
        }

        public void PlayJokerSelectionAppear()
        {
            PlaySound(_jokerSelectionAppear, 0.7f, true);
        }

        public void PlayWinRound()
        {
            PlaySound(_winRound);
        }

        public void PlayLoseRound()
        {
            PlaySound(_loseRound);
        }

        public void PlayWinGame()
        {
            PlaySound(_winGame);
        }

        public void PlayInterferenceAlert()
        {
            PlaySound(_interferenceAlert);
        }

        public void PlayNewRoundPrepare()
        {
            PlaySound(_newRoundPrepare);
        }

        public void PlayMonitorEnable()
        {
            PlaySound(_monitorEnable);
        }

        public void PlayCardFlying()
        {
            PlaySound(_cardFlying, 0.2f, true);
        }

        public void PlayPatternSound()
        {
            PlaySound(_patternSound);
        }
        
        private void PlaySound(AudioClip clip, float volumeMultiplier = 1f, bool isRandomPitch = false)
        {
            if (volumeMultiplier > 1f)
                throw new NotImplementedException();
            if (_audioSources.Count == 0)
                return;

            AudioSource source = _audioSources.Dequeue();
            source.clip = clip;

            if (isRandomPitch)
                source.pitch = Random.Range(0.75f, 1.15f);

            if (volumeMultiplier != 1f)
                source.volume = source.volume * volumeMultiplier;

            source.Play();
            StartCoroutine(ReturnToPool(source, volumeMultiplier));
        }

        private IEnumerator ReturnToPool(AudioSource source, float volumeMultiplier)
        {
            _cachedWaitWhiles[source].UpdateCondition(() => source.isPlaying);
            yield return _cachedWaitWhiles[source];
            source.pitch = 1f;
            source.volume = source.volume / volumeMultiplier;
            _audioSources.Enqueue(source);
        }
    }

    public class WaitWhileCached : CustomYieldInstruction
    {
        private Func<bool> _predicate;

        public WaitWhileCached(Func<bool> predicate)
        {
            _predicate = predicate;
        }

        public override bool keepWaiting => _predicate();

        public void UpdateCondition(Func<bool> newPredicate)
        {
            _predicate = newPredicate;
        }
    }
}