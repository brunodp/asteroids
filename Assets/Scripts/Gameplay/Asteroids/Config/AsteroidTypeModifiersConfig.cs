using System;
using System.Collections.Generic;
using Asteroids.Scripts.Framework.RNG;
using UnityEngine;

namespace Asteroids.Scripts.Gameplay.Asteroids.Config
{
    [CreateAssetMenu(menuName = "Asteroids/Asteroid Type Modifiers Config", fileName = "AsteroidTypeModifiersConfig")]
    public sealed class AsteroidTypeModifiersConfig : ScriptableObject
    {
        [Serializable]
        public struct Entry
        {
            public AsteroidType Type;
            [Header("Spawn weight chances on new waves"), Min(0)] public float SpawnWeight;
            public AsteroidTypeModifier Modifier;
        }
        
        [SerializeField] private List<Entry> _entries = new();
        
        private Dictionary<AsteroidType, Entry> _map;

        private void OnEnable()
        {
            BuildMap();
        }

        private void BuildMap()
        {
            _map = new Dictionary<AsteroidType, Entry>();
            
            for (int i = 0; i < _entries.Count; i++)
            {
                Entry entry = _entries[i];
                _map[entry.Type] = entry;
            }
        }

        public bool TryGetModifier(AsteroidType type, out AsteroidTypeModifier modifier)
        {
            if (_map == null)
            {
                BuildMap();
            }

            Entry entry;
            if (_map.TryGetValue(type, out entry))
            {
                modifier = entry.Modifier;
                return true;
            }

            modifier = AsteroidTypeModifier.Default;
            return false;
        }
        
        public AsteroidType RollType(IRng rng)
        {
            if (rng == null || _entries == null || _entries.Count == 0)
            {
                return AsteroidType.Normal;
            }

            float total = 0f;

            for (int i = 0; i < _entries.Count; i++)
            {
                float weight = _entries[i].SpawnWeight;
                if (weight > 0f)
                {
                    total += weight;
                }
            }

            if (total <= 0f)
            {
                return AsteroidType.Normal;
            }

            float roll = rng.Value() * total;

            float accumulator = 0f;
            for (int i = 0; i < _entries.Count; i++)
            {
                float weight = _entries[i].SpawnWeight;
                if (weight <= 0f)
                {
                    continue;
                }

                accumulator += weight;
                if (roll <= accumulator)
                {
                    return _entries[i].Type;
                }
            }

            return AsteroidType.Normal;
        }
    }
}