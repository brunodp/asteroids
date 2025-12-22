using UnityEngine;

namespace Asteroids.Scripts.Framework.RNG
{
    public interface IRng
    {
        int Range(int minInclusive, int maxExclusive);
        float Range(float minInclusive, float maxInclusive);
        Vector2 InsideUnitCircle();
    }
}