using System;
using UnityEngine;

namespace Asteroids.Scripts.Gameplay.Asteroids
{
    [Serializable]
    public struct AsteroidTypeModifier
    {
        public int ChildrenDelta;
        public float SplitSpeedMultiplier;
        public Color Tint;
        
        public static AsteroidTypeModifier Default
        {
            get
            {
                AsteroidTypeModifier modifier;
                modifier.ChildrenDelta = 0;
                modifier.SplitSpeedMultiplier = 1f;
                modifier.Tint = Color.white;
                return modifier;
            }
        }
    }
}