using Asteroids.Scripts.Core;
using Asteroids.Scripts.Framework;
using Asteroids.Scripts.Framework.Pooling;
using Asteroids.Scripts.Utils;
using UnityEngine;

namespace Asteroids.Scripts.Gameplay
{
    [RequireComponent(typeof(Collider2D)), RequireComponent(typeof(SpriteRenderer))]
    public class ShipController : MonoBehaviour
    {
        [SerializeField] private float _respawnDelaySeconds = 1.5f;
        [SerializeField] private float _invulnerabilitySeconds = 2.0f;
        [SerializeField] private ShipMovement _shipMovement;
        [SerializeField] private ShipWeapon _shipWeapon;
        
        [Header("VFX")]
        [SerializeField] private ThrustVfx _thrustVfx;
        [SerializeField] private GameObject _shipExplosionVfxPrefab;
        
        [Header("Invulnerability Blink")]
        [SerializeField] private float _blinkIntervalSeconds = 0.12f;
        [SerializeField, Range(0f, 1f)] private float _blinkAlpha = 0.5f;
        
        private Collider2D _collider;
        private SpriteRenderer _spriteRenderer;
        private GameplayController _gameplayController;
        
        private IShipInput _input;
        private IPool _pool;
        
        private bool _isDead;
        private float _respawnTimer;
        private float _invulnerabilityTimer;
        
        private float _blinkTimer;
        private bool _blinkDim;
        private float _baseAlpha;

        private bool _isRuntimeReady;
        private bool _isGameOver;
        
        private async void Awake()
        {
            _collider = GetComponent<Collider2D>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _baseAlpha = _spriteRenderer.color.a;
            
            await Wait.Until(() => GameplayBootstrap.HasInstance() && GameplayBootstrap.Instance.IsInitialized);

            _input = GameplayBootstrap.Instance.ShipInput;
            _pool = GameplayBootstrap.Instance.Pool;
            if (_input == null)
            {
                Log.Error("ShipController: IShipInput is null.", this);
                enabled = false;
                return;
            }

            _shipWeapon.Initialize(GameplayBootstrap.Instance.Pool);
            
            _isRuntimeReady = true;
        }

        private void Update()
        {
            if (!_isRuntimeReady)
            {
                return;
            }

            UpdateInvulnerabilityBlink();

            if (_isDead)
            {
                if (_isGameOver)
                {
                    return;
                }
                
                _respawnTimer -= Time.deltaTime;
                if (_respawnTimer <= 0f)
                {
                    Respawn();
                }

                return;
            }
            
            Vector2 shipVelocity = _shipMovement.GetVelocity();
            _shipWeapon.Tick(_input.Fire, shipVelocity);
        }

        private void FixedUpdate()
        {
            if (!_isRuntimeReady || _isDead)
            {
                return;
            }
            
            _shipMovement.Tick(_input.Turn, _input.Thrust);
            if (_thrustVfx != null)
            {
                _thrustVfx.SetThrust(_input.Thrust);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_isDead || _invulnerabilityTimer > 0f)
            {
                return;
            }

            Asteroid asteroid = other.GetComponent<Asteroid>();
            if (asteroid == null)
            {
                return;
            }

            if (_gameplayController == null)
            {
                Log.Error("ShipController: GameplayController is null on collision.", this);
                return;
            }

            _gameplayController.NotifyShipKilled();
        }
        
        public void Initialize(GameplayController gameplay)
        {
            _gameplayController = gameplay;
        }
        
        public void ResetForNewGame()
        {
            _isGameOver = false;
            ApplySpawnState();
        }
        
        public void BeginRespawn()
        {
            if (_isDead)
            {
                return;
            }

            _isDead = true;
            _respawnTimer = _respawnDelaySeconds;

            _collider.enabled = false;

            _invulnerabilityTimer = 0;
            _blinkTimer = 0f;
            _blinkDim = false;
            
            _spriteRenderer.enabled = false;
            
            if (_thrustVfx != null)
            {
                _thrustVfx.StopAndClear();
            }
            
            SpawnExplosionVfx();

            _shipMovement.Stop();
            _shipWeapon.ResetCooldown();
        }

        public void SetGameOver()
        {
            _isGameOver = true;
            _isDead = true;

            _collider.enabled = false;

            _invulnerabilityTimer = 0f;
            _blinkTimer = 0f;
            _blinkDim = false;
            
            _spriteRenderer.enabled = false;
            
            if (_thrustVfx != null)
            {
                _thrustVfx.StopAndClear();
            }

            SpawnExplosionVfx();

            _shipMovement.Stop();
            _shipWeapon.ResetCooldown();
        }
        
        private void SpawnExplosionVfx()
        {
            if (_shipExplosionVfxPrefab != null)
            {
                _pool.Spawn(_shipExplosionVfxPrefab, transform.position, Quaternion.identity, null);
            }
        }
        
        private void Respawn()
        {
            ApplySpawnState();
        }

        private void ApplySpawnState()
        {
            _isDead = false;
            
            _shipMovement.ResetToPose(Vector2.zero, 0f);
            _spriteRenderer.enabled = true;
            _collider.enabled = true;
            
            _invulnerabilityTimer = _invulnerabilitySeconds;

            _blinkTimer = 0f;
            _blinkDim = false;

            Color color = _spriteRenderer.color;
            color.a = _baseAlpha;
            _spriteRenderer.color = color;
        }
        
        private void UpdateInvulnerabilityBlink()
        {
            if (_invulnerabilityTimer <= 0f)
            {
                return;
            }
            
            _invulnerabilityTimer -= Time.deltaTime;
            _blinkTimer -= Time.deltaTime;

            if (_invulnerabilityTimer <= 0f)
            {
                _invulnerabilityTimer = 0f;
                _blinkTimer = 0f;
                _blinkDim = false;
                
                Color color = _spriteRenderer.color;
                color.a = _baseAlpha;
                _spriteRenderer.color = color;

                return;
            }

            if (_blinkTimer > 0f)
            {
                return;
            }

            _blinkTimer = _blinkIntervalSeconds;
            _blinkDim = !_blinkDim;

            Color blinkColor = _spriteRenderer.color;
            blinkColor.a = _blinkDim ? _blinkAlpha : _baseAlpha;
            _spriteRenderer.color = blinkColor;
        }
    }
}