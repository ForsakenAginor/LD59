using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Scripts.DI.Services.Global
{
    public interface IMusicPlayer
    {
        public void Pause();

        public void PlayBossFightMusic();

        public void Resume();
    }

    public class MusicPlayer : MonoBehaviour, IMusicPlayer
    {
        private readonly Queue<AudioClip> _clipQueue = new Queue<AudioClip>();

        [SerializeField] private AudioClip _bossFightClip;
        [SerializeField] private AudioClip[] _clips;
        [SerializeField] private AudioSource _source;

        private Coroutine _musicQueue;
        private bool _allClipsPreloaded = false;
        private float _volume;

        private void Awake()
        {
            StartCoroutine(PreloadAllClips());
        }

        private void Start()
        {
            StartCoroutine(StartMusicAfterPreload());
        }

        public void Pause()
        {
            _volume = _source.volume;
            _source.volume = 0;
        }

        public void Resume()
        {
            _source.volume = _volume;
        }

        public void PlayBossFightMusic()
        {
            if (_bossFightClip == null)
            {
                Debug.Log("BossFight clip not assigned");
                return;
            }

            StopCoroutine(_musicQueue);
            _source.Stop();
            _source.clip = _bossFightClip;
            _source.loop = true;
            _source.Play();
        }

        private IEnumerator PlayLoopMusic()
        {
            while (true)
            {
                _source.clip = _clipQueue.Dequeue();
                _source.Play();
                _clipQueue.Enqueue(_source.clip);
                yield return new WaitWhile(() => _source.isPlaying);
            }
        }

        private IEnumerator StartMusicAfterPreload()
        {
            yield return new WaitUntil(() => _allClipsPreloaded);
            _musicQueue = StartCoroutine(PlayLoopMusic());
        }

        private IEnumerator PreloadAllClips()
        {
            foreach (var clip in _clips)
            {
                clip.LoadAudioData();
                while (clip.loadState != AudioDataLoadState.Loaded)
                    yield return null;
            }

            if (_bossFightClip != null)
            {
                _bossFightClip.LoadAudioData();
                while (_bossFightClip.loadState != AudioDataLoadState.Loaded)
                    yield return null;
            }

            foreach (var clip in _clips)
                _clipQueue.Enqueue(clip);

            _allClipsPreloaded = true;
        }
    }
}