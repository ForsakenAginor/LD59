using System;
using System.Collections;
using Assets.Source.Scripts.AudioLogic;
using Assets.Source.Scripts.DI.Services.Boot;
using Assets.Source.Scripts.DI.Services.Game;
using Assets.Source.Scripts.General;
using Assets.Source.Scripts.SaveSystem;
using System.Collections.Generic;
using Assets.Source.Scripts.Utility;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Assets.Source.Scripts.EntryPoint
{
    public class Root : MonoBehaviour
    {
        [SerializeField] private GameManager _gameManager;
        [SerializeField] private Button _playButton;
        [SerializeField] private Button _closeMenuButton;
        [SerializeField] private Button[] _restartButtons;
        [SerializeField] private Button _settingsButton;
        [SerializeField] private SwitchableElement _settings;
        [SerializeField] private SwitchableElement _winScreen;
        [SerializeField] private SwitchableElement _loseScreen;
        
        [Header("Other")]
        [SerializeField] private AudioSaveLoadService _soundInitializer;
        private ISceneChanger _sceneChanger;
        private SaveDataProvider _saveDataProvider;
        private List<IDataSaveLoadService> _saveLoadServices = new();
        private HealthVignetteEffect _healthVignette;
        private NoiceVignetteEffect _noiceVignette;

        [Inject]
        public void Construct(ISceneChanger sceneChanger, SaveDataProvider saveDataProvider,
            HealthVignetteEffect healthVignette, NoiceVignetteEffect noiceVignette)
        {
            _sceneChanger = sceneChanger;
            _saveDataProvider = saveDataProvider;
            _healthVignette = healthVignette;
            _noiceVignette = noiceVignette;

            _healthVignette.Enable();
            _noiceVignette.Enable();
            _saveLoadServices.Add(_soundInitializer);
        }

        private void Awake()
        {
            _playButton.onClick.AddListener(StartPlay);
            _closeMenuButton.onClick.AddListener(SaveData);

            foreach (Button restartButton in _restartButtons)
            {
                restartButton.onClick.AddListener(Restart);
            }

            _settingsButton.onClick.AddListener(OpenSettings);

            Time.timeScale = 0f;
        }

        private IEnumerator Start()
        {
            _gameManager.PlayerWon += OnPlayerWon;
            _gameManager.PlayerLose += OnPlayerLose;
            _sceneChanger.FadeOut();
            yield return null;
            LoadData();
            Time.timeScale = 1f;
        }

        private void OnDestroy()
        {
            _playButton.onClick.RemoveListener(StartPlay);
            _closeMenuButton.onClick.RemoveListener(SaveData);
            _settingsButton.onClick.RemoveListener(OpenSettings);

            foreach (Button restartButton in _restartButtons)
            {
                restartButton.onClick.RemoveListener(Restart);
            }

            _healthVignette.Disable();
            _noiceVignette.Disable();
            _gameManager.PlayerWon -= OnPlayerWon;
            _gameManager.PlayerLose -= OnPlayerLose;
        }

        private void OpenSettings()
        {
            _settings.Enable();
        }

        private void Restart()
        {
            foreach (Button restartButton in _restartButtons)
            {
                restartButton.interactable = false;
            }

            _sceneChanger.LoadScene(Scenes.Game.ToString());
        }

        private void StartPlay()
        {
            _gameManager.Init(LevelNumber.First);
        }

        private void OnPlayerLose()
        {
            _loseScreen.Enable();
        }

        private void OnPlayerWon()
        {
            _winScreen.Enable();
        }

        private void SaveData()
        {
            foreach (var service in _saveLoadServices)
            {
                service.Save();
            }

            _saveDataProvider.Save();
        }

        private void LoadData()
        {
            foreach (var service in _saveLoadServices)
            {
                service.Init(_saveDataProvider.PlayerSavedData);
            }

            foreach (var service in _saveLoadServices)
            {
                service.Load();
            }
        }
    }
}