using Asteroids.Scripts.Utils;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Asteroids.Scripts.Core
{
    public sealed class GameplayBootstrap : Singleton<GameplayBootstrap>, IInitializable
    {
        [SerializeField] private InputActionAsset _inputActions;
        
        private ShipInputService _shipInput;
        public bool _isInitialized;
        
        public IShipInput ShipInput => _shipInput;
        public bool IsInitialized => _isInitialized;

        protected override void Awake()
        {
            base.Awake();

            if (_instance != this)
            {
                return;   
            }

            _shipInput = new ShipInputService(_inputActions);
            Initialize();
        }

        private void OnEnable()
        {
            if (_shipInput != null)
            {
                Initialize();
            }
        }
        
        private void OnDisable()
        {
            if (_shipInput != null)
            {
                _shipInput.Disable();
                _isInitialized = false;
            }
        }

        public void Initialize()
        {
            _shipInput.Enable();
            _isInitialized = true;
        }
    }
}