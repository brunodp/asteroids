using UnityEngine;

namespace Asteroids.Scripts.Framework.RNG
{
    public sealed class RngService : IRng
    {
        private readonly bool _useFixedSeed = false;
        private readonly int _seed;
        
        public RngService(bool useFixedSeed, int seed)
        {
            _useFixedSeed = useFixedSeed;
            _seed = seed;

            if (_useFixedSeed)
            {
                Random.InitState(_seed);
            }
        }
        
        public int Range(int minInclusive, int maxExclusive)
        {
            return Random.Range(minInclusive, maxExclusive);
        }

        public float Range(float minInclusive, float maxInclusive)
        {
            return Random.Range(minInclusive, maxInclusive);
        }

        public Vector2 InsideUnitCircle()
        {
            return Random.insideUnitCircle;
        }
    }
}