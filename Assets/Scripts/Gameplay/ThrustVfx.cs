using UnityEngine;

namespace Asteroids.Scripts.Gameplay
{
    public sealed class ThrustVfx : MonoBehaviour
    {
        [SerializeField] private ParticleSystem _particleSystem;
        [SerializeField] private float _minRate = 5f;
        [SerializeField] private float _maxRate = 60f;

        private ParticleSystem.EmissionModule _emission;
        private bool _isSetup;

        private void Awake()
        {
            if (_particleSystem == null)
            {
                _particleSystem = GetComponent<ParticleSystem>();
            }

            if (_particleSystem == null)
            {
                enabled = false;
                return;
            }

            _emission = _particleSystem.emission;
            _emission.rateOverTime = 0f;
            _particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

            _isSetup = true;
        }

        public void SetThrust(float thrustNormalized)
        {
            if (!_isSetup)
            {
                return;
            }

            if (thrustNormalized <= 0f)
            {
                _emission.rateOverTime = 0f;

                if (_particleSystem.isPlaying)
                {
                    _particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                }

                return;
            }

            float rate = Mathf.Lerp(_minRate, _maxRate, Mathf.Clamp01(thrustNormalized));
            _emission.rateOverTime = rate;

            if (!_particleSystem.isPlaying)
            {
                _particleSystem.Play(true);
            }
        }
        
        public void StopAndClear()
        {
            if (!_isSetup)
            {
                return;
            }

            _emission.rateOverTime = 0f;
            _particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }
}