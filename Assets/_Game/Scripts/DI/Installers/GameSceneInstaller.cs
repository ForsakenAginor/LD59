using Assets.Source.Scripts.DI.Services.Game;
using Assets.Source.Scripts.DI.Services.Global;
using Assets.Source.Scripts.Utility.Pools;
using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Zenject;

namespace Assets.Source.Scripts.DI.Installers
{
    public class GameSceneInstaller : MonoInstaller
    {
        [SerializeField] private UIAudioPlayer _uiAudioPlayerPrefab;
        [SerializeField] private MusicPlayer _musicPlayerPrefab;
        [SerializeField] private AudioPlayer _audioPlayerPrefab;

        private ZenjectInstantiateWrapper _instantiateWrapper;

        public override void InstallBindings()
        {
            BindInstantiateWrapper();
            BindPoolFactory();
            BindCoroutineRunner();
            BindAudio();
            BindTimeIncrement();
            BindModelScripts();
        }

        private void BindModelScripts()
        {
            CombinationCalculator combinationCalculator = Container.Instantiate<CombinationCalculator>();
            Container.Bind<CombinationCalculator>().To<CombinationCalculator>().FromInstance(combinationCalculator).AsSingle().NonLazy();
            ScoreManager scoreManager = Container.Instantiate<ScoreManager>();
            CardTransferManager cardTransferManager = new CardTransferManager();

            Container.Bind<ScoreManager>().To<ScoreManager>().FromInstance(scoreManager).AsSingle().NonLazy();
            Container.Bind<CardTransferManager>().To<CardTransferManager>().FromInstance(cardTransferManager).AsSingle().NonLazy();
        }

        private void BindPoolFactory()
        {
            PoolableFactory poolableFactory = new (Container);
            PoolFactory factory = new(poolableFactory, transform);

            Container
                .Bind<IPoolFactory>()
                .To<PoolFactory>()
                .FromInstance(factory)
                .AsSingle()
                .NonLazy();
        }

        private void BindTimeIncrement()
        {
            GameTimeService timeService = gameObject.AddComponent<GameTimeService>();

            Container
                .Bind<IGameTimeService>()
                .To<GameTimeService>()
                .FromInstance(timeService)
                .AsSingle();
        }

        private void BindCoroutineRunner()
        {
            ZenjectCoroutineRunner runner = new(this);

            Container
                .Bind<ICoroutineRunner>()
                .To<ZenjectCoroutineRunner>()
                .FromInstance(runner)
                .AsSingle()
                .NonLazy();
        }

        private void BindInstantiateWrapper()
        {
            _instantiateWrapper = new ZenjectInstantiateWrapper(Container);

            Container.Bind<IZenjectInstantiateWrapper>()
                .To<ZenjectInstantiateWrapper>()
                .FromInstance(_instantiateWrapper)
                .AsSingle()
                .NonLazy();
        }

        private void BindAudio()
        {
            UIAudioPlayer uIAudioPlayer = Container.InstantiatePrefabForComponent<UIAudioPlayer>(_uiAudioPlayerPrefab);
            MusicPlayer musicPlayer = Container.InstantiatePrefabForComponent<MusicPlayer>(_musicPlayerPrefab);
            AudioPlayer audioPlayer = Container.InstantiatePrefabForComponent<AudioPlayer>(_audioPlayerPrefab);

            Container
                .Bind<IUIAudioPlayer>()
                .To<UIAudioPlayer>()
                .FromInstance(uIAudioPlayer)
                .AsSingle()
                .NonLazy();

            Container
                .Bind<IMusicPlayer>()
                .To<MusicPlayer>()
                .FromInstance(musicPlayer)
                .AsSingle()
                .NonLazy();

            Container
                .Bind<AudioPlayer>()
                .To<AudioPlayer>()
                .FromInstance(audioPlayer)
                .AsSingle()
                .NonLazy();
        }
    }
}