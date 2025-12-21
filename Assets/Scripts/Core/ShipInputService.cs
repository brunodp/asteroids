using Asteroids.Scripts.Utils;
using UnityEngine.InputSystem;

namespace Asteroids.Scripts.Core
{
    public sealed class ShipInputService : IShipInput
    {
        private float _turn;
        private float _thrust;
        private bool _fire;

        private readonly InputActionAsset _inputActions;
        private InputAction _turnAction;
        private InputAction _thrustAction;
        private InputAction _fireAction;
        
        public float Turn => _turn;
        public float Thrust => _thrust;
        public bool Fire => _fire;

        public ShipInputService(InputActionAsset inputActionAsset)
        {
            _inputActions = inputActionAsset;
        }
        
        public void Enable()
        {
            if (_inputActions == null)
            {
                Log.Error("ShipInputService: InputActionAsset is null.");
                return;
            }
            
            _turnAction = _inputActions.FindAction("Gameplay/Turn", true);
            _thrustAction = _inputActions.FindAction("Gameplay/Thrust", true);
            _fireAction = _inputActions.FindAction("Gameplay/Fire", true);
            
            _turnAction.performed += OnTurnPerformed;
            _turnAction.canceled += OnTurnCanceled;
            
            _thrustAction.performed += OnThrustPerformed;
            _thrustAction.canceled += OnThrustCanceled;
            
            _fireAction.performed += OnFirePerformed;
            _fireAction.canceled += OnFireCanceled;
            
            _inputActions.Enable();
        }

        public void Disable()
        {
            if (_inputActions == null)
            {
                return;
            }
            
            if (_turnAction != null)
            {
                _turnAction.performed -= OnTurnPerformed;
                _turnAction.canceled -= OnTurnCanceled;
                _turnAction = null;
            }
            
            if (_thrustAction != null)
            {
                _thrustAction.performed -= OnThrustPerformed;
                _thrustAction.canceled -= OnThrustCanceled;
                _thrustAction = null;
            }
            
            if (_fireAction != null)
            {
                _fireAction.performed -= OnFirePerformed;
                _fireAction.canceled -= OnFireCanceled;
                _fireAction = null;
            }
            
            _inputActions.Disable();
            _fire = false;
        }

#region Handlers
        private void OnTurnPerformed(InputAction.CallbackContext context)
        {
            _turn = context.ReadValue<float>();
        }
        
        private void OnTurnCanceled(InputAction.CallbackContext context)
        {
            _turn = 0f;
        }

        private void OnThrustPerformed(InputAction.CallbackContext context)
        {
            _thrust = context.ReadValue<float>();
        }
        
        private void OnThrustCanceled(InputAction.CallbackContext context)
        {
            _thrust = 0f;
        }
        
        private void OnFirePerformed(InputAction.CallbackContext obj)
        {
            _fire = true;
        }
        
        private void OnFireCanceled(InputAction.CallbackContext obj)
        {
            _fire = false;
        }
#endregion
    }
}