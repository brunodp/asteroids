using Asteroids.Scripts.Core;
using Asteroids.Scripts.Framework.Pooling;
using Asteroids.Scripts.Utils;
using UnityEngine;

namespace Asteroids.Scripts.Gameplay
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Bullet : MonoBehaviour, IPoolable
    {
        [SerializeField] private float _lifetimeSeconds = 1.2f;
        
        private Rigidbody2D _rigidbody;
        private float _timeLeft;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            
            Debug.Assert(_rigidbody != null, "Bullet: Missing Rigidbody2D.", this);
            
            if (_rigidbody == null)
            {
                Log.Error("Bullet: Missing Rigidbody.", this);
                enabled = false;
                return;
            }
            
            _rigidbody.gravityScale = 0;
        }

        public void Launch(Vector2 position, float rotationDegrees, Vector2 velocity)
        {
            if (_rigidbody == null)
            {
                Log.Error("Bullet: Launch called before Awake / missing Rigidbody2D.", this);
                return;
            }
            
            _rigidbody.position = position;
            _rigidbody.rotation = rotationDegrees;
            _rigidbody.linearVelocity = velocity;
            _rigidbody.angularVelocity = 0f;
        }
        
        public void OnSpawned()
        {
            _timeLeft = _lifetimeSeconds;
        }

        public void OnDespawned()
        {
            _rigidbody.linearVelocity = Vector2.zero;
            _rigidbody.angularVelocity = 0f;
        }

        private void Update()
        {
            _timeLeft -= Time.deltaTime;

            if (_timeLeft > 0f)
            {
                return;
            }

            if (!GameplayBootstrap.HasInstance())
            {
                Log.Error("Bullet: GameplayBootstrap doesn't have an instance.", this);
                gameObject.SetActive(false);
                return;
            }
            
            GameplayBootstrap.Instance.Pool.Despawn(gameObject);
        }
    }
}