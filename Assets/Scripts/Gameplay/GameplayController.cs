using System;
using Asteroids.Scripts.Core;
using Asteroids.Scripts.Framework;
using Asteroids.Scripts.Utils;
using UnityEngine;

namespace Asteroids.Scripts.Gameplay
{
    public sealed class GameplayController : MonoBehaviour
    {
        [SerializeField] private int _startingLives = 3;
        [SerializeField] private ShipController _shipController;
        [SerializeField] private float _restartCooldownSeconds = 1.75f;
        
        public event Action<int> OnLivesChanged;
        public event Action<int> OnWaveChanged;
        public event Action<bool> OnGameOver;
        
        private AsteroidsManager _asteroidsManager;
        private IShipInput _input;

        private int _lives;
        private bool _isGameOver;
        private int _waveNumber;
        private float _restartCooldownTimer;

        public int Lives
        {
            get => _lives;
            private set
            {
                _lives = value;
                OnLivesChanged?.Invoke(_lives);
            }
        }

        public int WaveNumber
        {
            get => _waveNumber;
            private set 
            {
                _waveNumber = value;
                OnWaveChanged?.Invoke(_waveNumber);
            }
        } 
        
        public bool IsGameOver 
        { 
            get => _isGameOver;
            private set
            {
                _isGameOver = value;
                OnGameOver?.Invoke(_isGameOver);
            }
        }
        
        private async void Awake()
        {
            _lives = _startingLives;
            _isGameOver = false;
            _waveNumber = 0;

            if (_shipController == null)
            {
                Log.Error("GameplayController: ShipController reference is missing.", this);
                return;
            }
            _shipController.Initialize(this);
            
            await Wait.Until(() => GameplayBootstrap.HasInstance() && GameplayBootstrap.Instance.IsInitialized);
            _input = GameplayBootstrap.Instance.ShipInput;
        }
        
        private void Update()
        {
            if (_asteroidsManager == null)
            {
                return;
            }

            if (_isGameOver)
            {
                if (_restartCooldownTimer > 0f)
                {
                    _restartCooldownTimer -= Time.deltaTime;
                    return;
                }

                if (_input != null && _input.AnyStart)
                {
                    StartGame();                    
                }
            }
            
            _asteroidsManager.Tick(Time.deltaTime);
            int wave = _asteroidsManager.WaveNumber;

            if (wave != _waveNumber)
            {
                WaveNumber = wave;
            }
        }

        public void Initialize(AsteroidsManager asteroidsManager)
        {
            _asteroidsManager = asteroidsManager;
            
            IsGameOver = true;
            Lives = _startingLives;
            WaveNumber = 0;
        }
        
        public void NotifyShipKilled()
        {
            if (_isGameOver)
            {
                return;
            }

            Lives -= 1;

            if (_lives <= 0)
            {
                IsGameOver = true;
                _restartCooldownTimer = _restartCooldownSeconds;

                if (_asteroidsManager != null)
                {
                    _asteroidsManager.Stop();
                }

                if (_shipController != null)
                {
                    _shipController.SetGameOver();
                }

                return;
            }

            if (_shipController != null)
            {
                _shipController.BeginRespawn();
            }
        }

        private void StartGame()
        {
            IsGameOver = false;
            Lives = _startingLives;

            if (_asteroidsManager != null)
            {
                _asteroidsManager.ResetAll();
                _asteroidsManager.StartWaves();
                WaveNumber = _asteroidsManager.WaveNumber;
            }
            else
            {
                WaveNumber = 1;
            }
            
            if (_shipController != null)
            {
                _shipController.ResetForNewGame();
            }
        }
    }
}