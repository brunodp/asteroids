using UnityEngine;

namespace Asteroids.Scripts.Framework.Pooling
{
    public interface IPool
    {
        void Prewarm(GameObject prefab, int count);
        GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent);
        void Despawn(GameObject instance);
    }
}