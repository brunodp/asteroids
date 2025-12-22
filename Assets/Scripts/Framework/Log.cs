using UnityEngine;

namespace Asteroids.Scripts.Framework
{
    public static class Log
    {
        public static void Info(string message, Object context = null)
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            Debug.Log(message, context);
#endif
        }

        public static void Warning(string message, Object context = null)
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            Debug.LogWarning(message, context);
#endif            
        }

        public static void Error(string message, Object context = null)
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            Debug.LogError(message, context);
#endif
        }
    }
}