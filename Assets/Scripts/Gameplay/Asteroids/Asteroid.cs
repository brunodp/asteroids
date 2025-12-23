using Asteroids.Scripts.Core;
using Asteroids.Scripts.Framework;
using Asteroids.Scripts.Framework.Pooling;
using UnityEngine;

namespace Asteroids.Scripts.Gameplay.Asteroids
{
    [RequireComponent(typeof(Rigidbody2D)), RequireComponent(typeof(CircleCollider2D)), RequireComponent(typeof(SpriteRenderer))]
    public sealed class Asteroid : MonoBehaviour, IPoolable
    {
        [SerializeField] private AsteroidTier _tier = AsteroidTier.Large;
        [SerializeField] private GameObject _hitVfxPrefab;
        
        private Rigidbody2D _rigidbody;
        private CircleCollider2D _collider;
        private SpriteRenderer _spriteRenderer;
        
        private AsteroidType _type;
        private Color _baseColor;
        
        public AsteroidTier Tier => _tier;
        public AsteroidType Type => _type;
        
        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _collider = GetComponent<CircleCollider2D>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            
            if (_rigidbody == null || _collider == null || _spriteRenderer == null)
            {
                Log.Error("Asteroid: Missing required components.", this);
                enabled = false;
                return;
            }
            
            _baseColor = _spriteRenderer.color;
            
            _rigidbody.gravityScale = 0f;
            _collider.isTrigger = true;
        }

        public void Initialize(AsteroidTier tier, AsteroidType type, Vector2 position, Vector2 velocity, float angularVelocity, Color tint)
        {
            _tier = tier;
            _type = type;
            
            _rigidbody.position = position;
            _rigidbody.linearVelocity = velocity;
            _rigidbody.angularVelocity = angularVelocity;
            
            _spriteRenderer.color = tint;
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
        public void OnSpawned()
        {
            _type = AsteroidType.Normal;
            _spriteRenderer.color = _baseColor;
        }

        public void OnDespawned()
        {
            _rigidbody.linearVelocity = Vector2.zero;
            _rigidbody.angularVelocity = 0f;
        }
#endregion
    }
}