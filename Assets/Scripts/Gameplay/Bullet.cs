using Asteroids.Scripts.Core;
using Asteroids.Scripts.Framework.Pooling;
using Asteroids.Scripts.Framework;
using Asteroids.Scripts.Gameplay.Asteroids;
using UnityEngine;

namespace Asteroids.Scripts.Gameplay
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Bullet : MonoBehaviour, IPoolable
    {
        [SerializeField] private float _lifetimeSeconds = 1.2f;
        
        private Rigidbody2D _rigidbody;
        private float _timeLeft;
        private bool _isLive;
        private bool _despawnRequested;


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
        
        private void Update()
        {
            if (!_isLive)
            {
                return;
            }
            
            _timeLeft -= Time.deltaTime;

            if (_timeLeft > 0f)
            {
                return;
            }

            RequestDespawn();
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            Asteroid asteroid = other.GetComponent<Asteroid>();
            if (asteroid == null)
            {
                return;
            }

            asteroid.Hit(_rigidbody.position);

            RequestDespawn();
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
            
            _isLive = true;
        }
        
        private void RequestDespawn()
        {
            if (_despawnRequested) return;
            _despawnRequested = true;

            if (GameplayBootstrap.HasInstance())
            {
                GameplayBootstrap.Instance.Pool.Despawn(gameObject);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
        
#region IPoolable
        public void OnSpawned()
        {
            _despawnRequested = false;
            _timeLeft = _lifetimeSeconds;
            _isLive = false;
        }

        public void OnDespawned()
        {
            _rigidbody.linearVelocity = Vector2.zero;
            _rigidbody.angularVelocity = 0f;
            _isLive = false;
        }
#endregion
    }
}