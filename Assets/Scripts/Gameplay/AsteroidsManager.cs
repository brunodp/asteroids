using System.Collections.Generic;
using Asteroids.Scripts.Framework.Pooling;
using Asteroids.Scripts.Framework.RNG;
using UnityEngine;

namespace Asteroids.Scripts.Gameplay
{
    public sealed class AsteroidsManager
    {
        private readonly AsteroidsConfig _config;
        private readonly IPool _pool;
        private readonly IRng _rng;
        private readonly Camera _camera;
        private int _currentWaveAsteroidCount;
        private int _waveNumber;
        private float _nextWaveTimer;
        private bool _pendingNextWave;
        
        private readonly HashSet<Asteroid> _asteroids = new();
        
        public int WaveNumber => _waveNumber;

        public AsteroidsManager(AsteroidsConfig config, IPool pool, Camera camera, IRng rng)
        {
            _config = config;
            _pool = pool;
            _camera = camera;
            _rng = rng;
        }

        public void Tick(float deltaTime)
        {
            if (!_pendingNextWave)
            {
                return;
            }
            
            _nextWaveTimer -= deltaTime;
            if (_nextWaveTimer > 0)
            {
                return;
            }
            
            _pendingNextWave = false;
            SpawnNextWave();
        }

        public void StartWaves()
        {
            _waveNumber = 0;
            _currentWaveAsteroidCount = 0;
            _pendingNextWave = false;
            _nextWaveTimer = 0f;
            
            SpawnNextWave();
        }
        
        public void SpawnNextWave()
        {
            _currentWaveAsteroidCount = 0;
            
            _waveNumber++;
            int largeCount = _config.InitialLargeCount + (_waveNumber - 1) * _config.ExtraAsteroidsPerWave;

            for (int i = 0; i < largeCount; i++)
            {
                Spawn(_config.LargePrefab, RandomEdgePosition(), RandomVelocity(), RandomAngularVelocity());
            }
        }
        
        public void OnAsteroidHit(Asteroid asteroid, Vector2 hitPosition)
        {
            Asteroid childPrefab = null;

            if (asteroid.Tier == AsteroidTier.Large)
            {
                childPrefab = _config.MediumPrefab;
            }
            else if (asteroid.Tier == AsteroidTier.Medium)
            {
                childPrefab = _config.SmallPrefab;
            }

            _asteroids.Remove(asteroid);
            _pool.Despawn(asteroid.gameObject);
            _currentWaveAsteroidCount--;

            if (_currentWaveAsteroidCount <= 0)
            {
                _currentWaveAsteroidCount = 0;
                _pendingNextWave = true;
                _nextWaveTimer = _config.NextWaveDelaySeconds;
            }

            if (childPrefab == null)
            {
                return;
            }

            int count = _config.ChildrenOnSplit;
            for (int i = 0; i < count; i++)
            {
                Vector2 direction = _rng.InsideUnitCircle();
                if (direction.sqrMagnitude < 0.001f)
                {
                    direction = Vector2.right;
                }
                direction.Normalize();

                Vector2 velocity = direction * _config.SplitSpeed;
                float angularVelocity = RandomAngularVelocity();

                Spawn(childPrefab, hitPosition, velocity, angularVelocity);
            }
        }

        private void Spawn(Asteroid prefab, Vector2 position, Vector2 velocity, float angularVelocity)
        {
            GameObject go = _pool.Spawn(prefab.gameObject, position, Quaternion.identity, null);
            Asteroid asteroid = go.GetComponent<Asteroid>();
            asteroid.Initialize(prefab.Tier, position, velocity, angularVelocity);

            _asteroids.Add(asteroid);
            _currentWaveAsteroidCount++;
        }
        
        private float RandomAngularVelocity()
        {
            return _rng.Range(_config.MinAngularVelocity, _config.MaxAngularVelocity);
        }

        private Vector2 RandomVelocity()
        {
            return _rng.InsideUnitCircle().normalized * _rng.Range(_config.MinSpawnSpeed, _config.MaxSpawnSpeed);
        }

        private Vector2 RandomEdgePosition()
        {
            float verticalExtent = _camera.orthographicSize;
            float horizontalExtent = verticalExtent * _camera.aspect;
            
            Vector3 cameraPosition = _camera.transform.position;
            
            float padding = _config.SpawnPadding;
            
            float leftLimit = cameraPosition.x - horizontalExtent - padding;
            float rightLimit = cameraPosition.x + horizontalExtent + padding;
            float bottomLimit = cameraPosition.y - verticalExtent - padding;
            float topLimit = cameraPosition.y + verticalExtent + padding;
            
            int edge = _rng.Range(0, 4);
            switch (edge)
            {
                case 0:
                    return new Vector2(leftLimit, _rng.Range(bottomLimit, topLimit));
                case 1:
                    return new Vector2(rightLimit, _rng.Range(bottomLimit, topLimit));
                case 2:
                    return new Vector2(_rng.Range(leftLimit, rightLimit), topLimit);
                default:
                    return new Vector2(_rng.Range(leftLimit, rightLimit), bottomLimit);
            }
        }
        
        public void ResetAll()
        {
            _pendingNextWave = false;
            _nextWaveTimer = 0f;
            _waveNumber = 0;
            _currentWaveAsteroidCount = 0;

            if (_asteroids.Count == 0)
            {
                return;
            }
            
            List<Asteroid> tmp = new(_asteroids);
            _asteroids.Clear();

            for (int i = 0; i < tmp.Count; i++)
            {
                Asteroid asteroid = tmp[i];
                if (asteroid == null)
                {
                    continue;
                }

                _pool.Despawn(asteroid.gameObject);
            }
        }
        
        public void Stop()
        {
            ResetAll();
        }
    }
}