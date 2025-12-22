using Asteroids.Scripts.Core;
using Asteroids.Scripts.Framework.Pooling;
using UnityEngine;

namespace Asteroids.Scripts.Gameplay
{
    public sealed class PooledParticleVfx : MonoBehaviour, IPoolable
    {
        [SerializeField] private ParticleSystem _particleSystem;
        [SerializeField] private float _extraDelaySeconds = 0.05f;

        private float _despawnTimer;

        private void Awake()
        {
            if (_particleSystem == null)
            {
                _particleSystem = GetComponent<ParticleSystem>();
            }
        }
        
        private void Update()
        {
            if (_despawnTimer <= 0f)
            {
                return;
            }

            _despawnTimer -= Time.deltaTime;
            if (_despawnTimer > 0f)
            {
                return;
            }

            if (!GameplayBootstrap.HasInstance())
            {
                gameObject.SetActive(false);
                return;
            }

            GameplayBootstrap.Instance.Pool.Despawn(gameObject);
        }
        
#region IPoolable
        public void OnSpawned()
        {
            if (_particleSystem == null)
            {
                _despawnTimer = 0.2f;
                return;
            }

            _particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            _particleSystem.Play(true);

            float duration = _particleSystem.main.duration;
            float lifetime = _particleSystem.main.startLifetime.constantMax;

            _despawnTimer = duration + lifetime + _extraDelaySeconds;
        }

        public void OnDespawned()
        {
            _despawnTimer = 0f;

            if (_particleSystem != null)
            {
                _particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }
        }
#endregion
    }
}