using Asteroids.Scripts.Utils;
using UnityEngine.InputSystem;

namespace Asteroids.Scripts.Core
{
    public sealed class ShipInputService : IShipInput
    {
        private float _turn;
        private float _thrust;

        private readonly InputActionAsset _inputActions;
        private InputAction _turnAction;
        private InputAction _thrustAction;
        
        public float Turn => _turn;
        public float Thrust => _thrust;

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
            
            _turnAction = _inputActions.FindAction("GamePlay/Turn", true);
            _thrustAction = _inputActions.FindAction("GamePlay/Thrust", true);
            
            _turnAction.performed += OnTurnPerformed;
            _turnAction.canceled += OnTurnCanceled;
            
            _thrustAction.performed += OnThrustPerformed;
            _thrustAction.canceled += OnThrustCanceled;
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
            
            _inputActions.Disable();
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
#endregion
    }
}