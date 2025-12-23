using UnityEngine;

namespace Asteroids.Scripts.Gameplay.Asteroids.Config
{
    [CreateAssetMenu(menuName = "Asteroids/Asteroids Config", fileName = "AsteroidsConfig")]
    public sealed class AsteroidsConfig : ScriptableObject
    {
        [Header("Wave")]
        [SerializeField] private int _initialLargeCount = 4;
        [SerializeField] private int _extraAsteroidsPerWave = 1;
        [SerializeField] private float _nextWaveDelaySeconds = 1.5f;
        
        [Header("Splitting")]
        [SerializeField] private int _childrenOnSplit = 2;
        [SerializeField] private float _splitSpeed = 3.5f;
        
        [Header("Spawning")]
        [SerializeField] private float _spawnPadding = 0.75f;
        [SerializeField] private float _minSpawnSpeed = 1.5f;
        [SerializeField] private float _maxSpawnSpeed = 3.0f;
        [SerializeField] private float _minAngularVelocity = -120f;
        [SerializeField] private float _maxAngularVelocity = 120f;

        [Header("Prefabs")]
        [SerializeField] private Asteroid _largePrefab;
        [SerializeField] private Asteroid _mediumPrefab;
        [SerializeField] private Asteroid _smallPrefab;
        
        [Header("Asteroid Types")]
        [SerializeField] private AsteroidTypeModifiersConfig _typeTable;

        public int InitialLargeCount => _initialLargeCount;
        public int ChildrenOnSplit => _childrenOnSplit;
        public float SplitSpeed => _splitSpeed;
        public float SpawnPadding => _spawnPadding;
        public float MinAngularVelocity => _minAngularVelocity;
        public float MaxAngularVelocity => _maxAngularVelocity;
        public float MinSpawnSpeed => _minSpawnSpeed;
        public float MaxSpawnSpeed => _maxSpawnSpeed;
        public float NextWaveDelaySeconds => _nextWaveDelaySeconds;
        public int ExtraAsteroidsPerWave => _extraAsteroidsPerWave;
        
        public Asteroid LargePrefab => _largePrefab;
        public Asteroid MediumPrefab => _mediumPrefab;
        public Asteroid SmallPrefab => _smallPrefab;
        
        public AsteroidTypeModifiersConfig TypeTable => _typeTable;
    }
}