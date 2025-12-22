using Asteroids.Scripts.Framework;
using UnityEngine;

namespace Asteroids.Scripts.Gameplay
{
    public sealed class ScreenWrap : MonoBehaviour
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private float _margin = 0.5f;
        
        private Rigidbody2D _rigidbody;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();

            if (_camera == null)
            {
                _camera = Camera.main;
            }
            
            if (_camera == null)
            {
                Log.Error("ScreenWrap: No camera assigned and main Camera is null.", this);
                enabled = false;
            }
        }

        private void FixedUpdate()
        {
            if (_rigidbody == null)
            {
                return;
            }
            
            Vector2 position = _rigidbody.position;
            if (TryWrap(ref position))
            {
                _rigidbody.position = position;
            }
        }

        private void LateUpdate()
        {
            if (_rigidbody != null)
            {
                return;
            }
            
            Vector2 position = transform.position;
            if (TryWrap(ref position))
            {
                transform.position = position;
            }
        }

        private bool TryWrap(ref Vector2 position)
        {
            if (_camera == null)
            {
                return false;
            }

            if (!_camera.orthographic)
            {
                return false;
            }
            
            float verticalExtent = _camera.orthographicSize;
            float horizontalExtent = verticalExtent * _camera.aspect;
            
            Vector3 cameraPosition = _camera.transform.position;
            
            float leftLimit = cameraPosition.x - horizontalExtent;
            float rightLimit = cameraPosition.x + horizontalExtent;
            float bottomLimit = cameraPosition.y - verticalExtent;
            float topLimit = cameraPosition.y + verticalExtent;

            bool changed = false;
            
            if (position.x < leftLimit - _margin)
            {
                position.x = rightLimit + _margin;
                changed = true;
            }
            else if (position.x > rightLimit + _margin)
            {
                position.x = leftLimit - _margin;
                changed = true;
            }
            
            if (position.y < bottomLimit - _margin)
            {
                position.y = topLimit + _margin;
                changed = true;
            }
            else if (position.y > topLimit + _margin)
            {
                position.y = bottomLimit - _margin;
                changed = true;
            }
            
            return changed;
        }
    }
}