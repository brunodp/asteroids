using System;
using UnityEngine;

namespace Asteroids.Scripts.Gameplay.Ship.Config
{
    [CreateAssetMenu(menuName = "Asteroids/Ship Config", fileName = "ShipConfig")]
    public sealed class ShipConfig : ScriptableObject
    {
        [Header("Respawn")]
        [SerializeField] private float _respawnDelaySeconds = 1.5f;
        [SerializeField] private float _invulnerabilitySeconds = 2.0f;

        [Header("Invulnerability Blink")]
        [SerializeField] private float _blinkIntervalSeconds = 0.12f;
        [SerializeField, Range(0f, 1f)] private float _blinkAlpha = 0.5f;

        [Header("Movement")]
        [SerializeField] private MovementSettings _movement = MovementSettings.Default;

        [Header("Weapon")]
        [SerializeField] private WeaponSettings _weapon = WeaponSettings.Default;

        public float RespawnDelaySeconds => _respawnDelaySeconds;
        public float InvulnerabilitySeconds => _invulnerabilitySeconds;

        public float BlinkIntervalSeconds => _blinkIntervalSeconds;
        public float BlinkAlpha => _blinkAlpha;

        public MovementSettings Movement => _movement;
        public WeaponSettings Weapon => _weapon;

        [Serializable]
        public struct MovementSettings
        {
            [SerializeField] private float _turnSpeedDegreesPerSecond;
            [SerializeField] private float _thrustForce;
            [SerializeField] private float _maxSpeed;

            public float TurnSpeedDegreesPerSecond => _turnSpeedDegreesPerSecond;
            public float ThrustForce => _thrustForce;
            public float MaxSpeed => _maxSpeed;

            public static MovementSettings Default
            {
                get
                {
                    MovementSettings movementSettings;
                    movementSettings._turnSpeedDegreesPerSecond = 180f;
                    movementSettings._thrustForce = 8f;
                    movementSettings._maxSpeed = 12f;
                    return movementSettings;
                }
            }
        }

        [Serializable]
        public struct WeaponSettings
        {
            [SerializeField] private float _bulletSpeed;
            [SerializeField] private float _fireCooldownSeconds;
            
            public float BulletSpeed => _bulletSpeed;
            public float FireCooldownSeconds => _fireCooldownSeconds;

            public static WeaponSettings Default
            {
                get
                {
                    WeaponSettings weaponSettings;
                    weaponSettings._bulletSpeed = 18f;
                    weaponSettings._fireCooldownSeconds = 0.12f;
                    return weaponSettings;
                }
            }
        }
    }
}