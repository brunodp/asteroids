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

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _rigidbody.gravityScale = 0f;
            _rigidbody.linearDamping = 0f;
            _rigidbody.angularDamping = 0f;
        }

        public void Tick(float turn, float thrust)
        {
            float deltaTime = Time.fixedDeltaTime;

            if (turn != 0f)
            {
                float deltaAngle = -turn * _turnSpeedDegreesPerSecond * deltaTime;
                _rigidbody.MoveRotation(_rigidbody.rotation + deltaAngle);
            }
            
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

        public void Stop()
        {
            _rigidbody.linearVelocity = Vector3.zero;
            _rigidbody.angularVelocity = 0f;
        }

        public void ResetToPose(Vector3 position, float rotation)
        {
            _rigidbody.position = position;
            _rigidbody.rotation = rotation;
            Stop();
        }

        public Vector2 GetVelocity()
        {
            return _rigidbody.linearVelocity;
        }
    }
}