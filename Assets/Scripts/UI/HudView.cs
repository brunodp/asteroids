using Asteroids.Scripts.Core;
using Asteroids.Scripts.Gameplay;
using Asteroids.Scripts.Utils;
using TMPro;
using UnityEngine;

namespace Asteroids.Scripts.UI
{
    public class HudView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _waveNumberText;
        [SerializeField] private TMP_Text _livesText;
        [SerializeField] private TMP_Text _gameOverText;
        
        private bool _isSubscribed;

        private void Awake()
        {
            if (_gameOverText == null)
            {
                return;
            }
        
            _gameOverText.text = "GAME OVER";
            _gameOverText.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            Subscribe();
        }

        private void OnDisable()
        {
            Unsubscribe();
        }
        
        private async void Subscribe()
        {
            await Wait.Until(GameplayBootstrap.HasInstance);
            
            if (!isActiveAndEnabled || _isSubscribed)
            {
                return;
            }
            
            GameplayController gameplayController = GameplayBootstrap.Instance.GameplayController;

            if (gameplayController != null)
            {
                gameplayController.OnWaveChanged += OnWaveChanged;
                gameplayController.OnLivesChanged += OnLivesChanged;
                gameplayController.OnGameOver += OnGameOver;
                
                OnWaveChanged(gameplayController.WaveNumber);
                OnLivesChanged(gameplayController.Lives);
                OnGameOver(gameplayController.IsGameOver);
            }
            
            _isSubscribed = true;
        }
        
        private void Unsubscribe()
        {
            if (!_isSubscribed)
            {
                return;
            }
            
            if (GameplayBootstrap.HasInstance() && GameplayBootstrap.Instance.GameplayController != null)
            {
                GameplayBootstrap.Instance.GameplayController.OnWaveChanged -= OnWaveChanged;
                GameplayBootstrap.Instance.GameplayController.OnLivesChanged -= OnLivesChanged;
                GameplayBootstrap.Instance.GameplayController.OnGameOver -= OnGameOver;                
            }
            
            _isSubscribed = false;
        }

        private void OnGameOver(bool isGameOver)
        {
            if (_gameOverText == null)
            {
                return;
            }

            _gameOverText.gameObject.SetActive(isGameOver);
        }

        private void OnLivesChanged(int lives)
        {
            if (_livesText == null)
            {
                return;
            }

            _livesText.text = $"LIVES {lives}";
        }

        private void OnWaveChanged(int waveNumber)
        {
            if (_waveNumberText == null)
            {
                return;
            }

            if (waveNumber <= 0)
            {
                _waveNumberText.text = "";
                return;
            }

            _waveNumberText.text = $"WAVE {waveNumber}";
        }
    }
}