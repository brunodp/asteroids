using Asteroids.Scripts.Framework.Pooling;
using Asteroids.Scripts.Framework.RNG;
using Asteroids.Scripts.Gameplay;
using Asteroids.Scripts.Utils;
using Asteroids.Scripts.Gameplay.Asteroids;
using Asteroids.Scripts.Gameplay.Asteroids.Config;
using Asteroids.Scripts.Gameplay.Ship.Config;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Asteroids.Scripts.Core
{
    public sealed class GameplayBootstrap : Singleton<GameplayBootstrap>, IInitializable
    {
        [SerializeField] private GameplayController _gameplayController;
        [SerializeField] private InputActionAsset _inputActions;
        [SerializeField] private AsteroidsConfig _asteroidsConfig;
        [SerializeField] private ShipConfig _shipConfig;
        [SerializeField] private Transform _poolsParent;
        
        private ShipInputService _shipInputService;
        private PoolService _poolService;
        private RngService _rngService;
        private AsteroidsManager _asteroidsManager;
        private bool _isInitialized;
        
        public IShipInput ShipInput => _shipInputService;
        public IPool Pool => _poolService;
        public IRng Rng => _rngService;
        public AsteroidsManager AsteroidsManager => _asteroidsManager;
        public GameplayController GameplayController => _gameplayController;
        public ShipConfig ShipConfig => _shipConfig;
        public bool IsInitialized => _isInitialized;

        protected override void Awake()
        {
            base.Awake();

            if (_instance != this)
            {
                return;
            }
            
            if (_poolsParent == null)
            {
                GameObject go = new GameObject("Pools");
                go.transform.SetParent(transform, false);
                _poolsParent = go.transform;
            }
            
            _poolService = new PoolService(_poolsParent);
            _rngService = new RngService(false, 0);
            _shipInputService = new ShipInputService(_inputActions);
            _asteroidsManager = new AsteroidsManager(_asteroidsConfig, _poolService, Camera.main, _rngService);
        }

        private void OnEnable()
        {
            if (_shipInputService != null)
            {
                Initialize();
            }
        }
        
        private void OnDisable()
        {
            if (_shipInputService != null)
            {
                _shipInputService.Disable();
                _isInitialized = false;
            }
        }

        private void Update()
        {
            if (!_isInitialized)
            {
                return;
            }
            
            _asteroidsManager.Tick(Time.deltaTime);
        }

        private void Initialize()
        {
            if (_isInitialized)
            {
                return;
            }
            
            _shipInputService.Enable();
            if (_gameplayController != null)
            {
                _gameplayController.Initialize(_asteroidsManager);
            }
            
            _isInitialized = true;
        }
    }
}