using Asteroids.Scripts.Core;
using Asteroids.Scripts.Utils;
using UnityEngine;

namespace Asteroids.Scripts.Gameplay
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class ShipWeapon : MonoBehaviour
    {
        [SerializeField] private Bullet _bulletPrefab;
        [SerializeField] private Transform _muzzle;
        [SerializeField] private float _bulletSpeed = 18f;
        [SerializeField] private float _fireCooldownSeconds = 0.12f;

        private IShipInput _shipInput;
        private Rigidbody2D _rigidbody;
        private float _cooldown;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            if (_rigidbody == null)
            {
                Log.Error("ShipWeapon: Missing Rigidbody2D.", this);
                enabled = false;
            
                return;
            }

            if (_muzzle == null)
            {
                _muzzle = transform;
            }
        }

        private async void Start()
        {
            await Wait.Until(() => GameplayBootstrap.HasInstance() && GameplayBootstrap.Instance.IsInitialized);
            
            _shipInput = GameplayBootstrap.Instance.ShipInput;
            if (_shipInput == null)
            {
                Log.Error("ShipWeapon: IShipInput service is null.", this);
                enabled = false;

                return;
            }

            if (_bulletPrefab == null)
            {
                Log.Error("ShipWeapon: Bullet prefab is not assigned.", this);
                enabled = false;
            }
        }

        private void Update()
        {
            if (_cooldown > 0)
            {
                _cooldown -= Time.deltaTime;
            }

            if (_cooldown > 0)
            {
                return;
            }
            
            if (_shipInput == null || !_shipInput.Fire)
            {
                return;
            }

            Fire();
            _cooldown = _fireCooldownSeconds;
        }

        private void Fire()
        {
            Vector2 position = _muzzle.position;
            Vector2 direction = transform.up;
            
            Vector2 shipVelocity = _rigidbody != null ? _rigidbody.linearVelocity : Vector2.zero;
            Vector2 bulletVelocity = shipVelocity + direction * _bulletSpeed;
            float rotation = transform.eulerAngles.z;
            
            GameObject go = GameplayBootstrap.Instance.Pool.Spawn(_bulletPrefab.gameObject, position, Quaternion.Euler(0f, 0f, rotation), null);
            Bullet bullet = go.GetComponent<Bullet>();
            bullet.Launch(position, rotation, bulletVelocity);
        }
    }
}