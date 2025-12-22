using Asteroids.Scripts.Core;
using Asteroids.Scripts.Framework.Pooling;
using Asteroids.Scripts.Framework;
using UnityEngine;

namespace Asteroids.Scripts.Gameplay
{
    [RequireComponent(typeof(Rigidbody2D)), RequireComponent(typeof(CircleCollider2D))]
    public sealed class Asteroid : MonoBehaviour, IPoolable
    {
        [SerializeField] private AsteroidTier _tier = AsteroidTier.Large;
        [SerializeField] private GameObject _hitVfxPrefab;
        
        private Rigidbody2D _rigidbody;
        private CircleCollider2D _collider;
        
        public AsteroidTier Tier => _tier;
        
        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _collider = GetComponent<CircleCollider2D>();
            
            if (_rigidbody == null)
            {
                Log.Error("Asteroid: Missing Rigidbody2D.", this);
                enabled = false;
                return;
            }
            
            if (_collider == null)
            {
                Log.Error("Asteroid: Missing CircleCollider2D.", this);
                enabled = false;
                return;
            }
            
            _rigidbody.gravityScale = 0f;
            _collider.isTrigger = true;
        }

        public void Initialize(AsteroidTier tier, Vector2 position, Vector2 velocity, float angularVelocity)
        {
            _tier = tier;
            
            _rigidbody.position = position;
            _rigidbody.linearVelocity = velocity;
            _rigidbody.angularVelocity = angularVelocity;
        }

        public void Hit(Vector2 hitPosition)
        {
            if (!GameplayBootstrap.HasInstance())
            {
                Log.Error("Asteroid: GameplayBootstrap doesn't have an instance.", this);
                gameObject.SetActive(false);
                return;
            }
            
            GameplayBootstrap.Instance.AsteroidsManager.OnAsteroidHit(this, hitPosition);
            GameplayBootstrap.Instance.Pool.Spawn(_hitVfxPrefab, transform.position, Quaternion.identity, null);
        }
        
#region IPoolable
        public void OnSpawned() { }

        public void OnDespawned()
        {
            _rigidbody.linearVelocity = Vector2.zero;
            _rigidbody.angularVelocity = 0f;
        }
#endregion
    }
}