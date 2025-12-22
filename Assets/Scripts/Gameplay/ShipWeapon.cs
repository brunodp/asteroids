using Asteroids.Scripts.Core;
using Asteroids.Scripts.Framework;
using Asteroids.Scripts.Framework.Pooling;
using UnityEngine;

namespace Asteroids.Scripts.Gameplay
{
    public class ShipWeapon : MonoBehaviour
    {
        [SerializeField] private Bullet _bulletPrefab;
        [SerializeField] private Transform _muzzle;
        [SerializeField] private float _bulletSpeed = 18f;
        [SerializeField] private float _fireCooldownSeconds = 0.12f;
        
        private IPool _pool;
        private float _cooldown;

        private void Awake()
        {
            if (_muzzle == null)
            {
                _muzzle = transform;
            }
        }

        public void Initialize(IPool pool)
        {
            _pool = pool;
        }

        public void Tick(bool fire, Vector2 shipVelocity)
        {
            if (_cooldown > 0)
            {
                _cooldown -= Time.deltaTime;
            }

            if (!fire || _cooldown > 0f)
            {
                return;
            }

            if (_pool == null)
            {
                Log.Error("ShipWeapon: Pool is not initialized.", this);
                return;
            }

            if (_bulletPrefab == null)
            {
                Log.Error("ShipWeapon: Bullet prefab is not assigned.", this);
            }
            
            Fire(shipVelocity);
            _cooldown = _fireCooldownSeconds;
        }

        private void Fire(Vector2 shipVelocity)
        {
            Vector2 position = _muzzle.position;
            Vector2 direction = transform.up;
            
            Vector2 bulletVelocity = shipVelocity + direction * _bulletSpeed;
            float rotation = transform.eulerAngles.z;
            
            GameObject go = GameplayBootstrap.Instance.Pool.Spawn(_bulletPrefab.gameObject, position, Quaternion.Euler(0f, 0f, rotation), null);
            Bullet bullet = go.GetComponent<Bullet>();
            if (bullet != null)
            {
                bullet.Launch(position, rotation, bulletVelocity);    
            }
        }
        
        public void ResetCooldown()
        {
            _cooldown = 0f;
        }
    }
}