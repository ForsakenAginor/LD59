using Assets.Source.Scripts.DI.Services.Boot;
using Assets.Source.Scripts.SaveSystem;
using UnityEngine;
using Zenject;

namespace Assets.Source.Scripts.DI.Installers
{
    public class BootsTrapInstaller : MonoInstaller
    {
        [SerializeField] private SceneChanger _sceneChanger;
        [SerializeField] private ConfigurationProvider _configurationProvider;

        public override void InstallBindings()
        {
            InitSceneChanger();
            BindSaveLoadService();
            BindConfigurations();
        }

        private void BindConfigurations()
        {
            Container
                .Bind<ConfigurationProvider>()
                .To<ConfigurationProvider>()
                .FromInstance(_configurationProvider)
                .AsSingle()
                .NonLazy();
        }

        private void InitSceneChanger()
        {
            Container
                .Bind<ISceneChanger>()
                .To<SceneChanger>()
                .FromComponentInNewPrefab(_sceneChanger)
                .AsSingle();
        }

        private void BindSaveLoadService()
        {
            PlayerPrefsSaveLoadService saveService = new("SavedData");
            DataSerializer<SaveData> serializer = new(saveService);
            SaveDataProvider saveDataProvider = new(serializer);

            Container
                .Bind<SaveDataProvider>()
                .To<SaveDataProvider>()
                .FromInstance(saveDataProvider)
                .AsSingle()
                .NonLazy();
        }
    }
}