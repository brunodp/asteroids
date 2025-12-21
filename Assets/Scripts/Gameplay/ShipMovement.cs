using Asteroids.Scripts.Core;
using Asteroids.Scripts.Utils;
using UnityEngine;

namespace Asteroids.Scripts.Gameplay
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class ShipMovement : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private float _turnSpeedDegreesPerSecond = 180f;
        [SerializeField] private float _thrustForce = 8f;
        [SerializeField] private float _maxSpeed = 12;
        
        private Rigidbody2D _rigidbody;
        private IShipInput _input;

        private async void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _rigidbody.gravityScale = 0f;
            _rigidbody.linearDamping = 0f;
            _rigidbody.angularDamping = 0f;
            
            await Wait.Until(() => GameplayBootstrap.HasInstance() && GameplayBootstrap.Instance.IsInitialized);
            
            _input = GameplayBootstrap.Instance.ShipInput;
            
            if (_input == null)
            {
                Log.Error($"ShipMovement: IShipInput service is null.", this);
                enabled = false;
            }
        }

        private void FixedUpdate()
        {
            if (_input == null)
            {
                return;
            }
            
            float deltaTime = Time.fixedDeltaTime;
            
            float turn = _input.Turn;
            if (turn != 0f)
            {
                float deltaAngle = -turn * _turnSpeedDegreesPerSecond * deltaTime;
                _rigidbody.MoveRotation(_rigidbody.rotation + deltaAngle);
            }
            
            float thrust = _input.Thrust;
            if (thrust != 0f)
            {
                Vector2 forward = transform.up;
                _rigidbody.AddForce(forward * (_thrustForce * thrust), ForceMode2D.Force);
            }

            Vector2 velocity = _rigidbody.linearVelocity; 
            float speed = velocity.magnitude;

            if (speed > _maxSpeed)
            {
                _rigidbody.linearVelocity = velocity * (_maxSpeed / speed); 
            }
        }
    }
}