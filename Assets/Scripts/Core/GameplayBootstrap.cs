using Asteroids.Scripts.Framework.Pooling;
using Asteroids.Scripts.Utils;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Asteroids.Scripts.Core
{
    public sealed class GameplayBootstrap : Singleton<GameplayBootstrap>, IInitializable
    {
        [SerializeField] private InputActionAsset _inputActions;
        [SerializeField] private Transform _poolsParent;
        
        private ShipInputService _shipInputService;
        private PoolService _poolService;
        public bool _isInitialized;
        
        public IShipInput ShipInput => _shipInputService;
        public IPool Pool => _poolService;
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
            
            _shipInputService = new ShipInputService(_inputActions);
            Initialize();
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

        public void Initialize()
        {
            if (_isInitialized)
            {
                return;
            }
            
            _shipInputService.Enable();
            _isInitialized = true;
        }
    }
}