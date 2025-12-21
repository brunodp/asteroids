using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Asteroids.Scripts.Utils
{
    public static class Wait
    {
        public static Task<bool> Until(Func<bool> predicate)
        {
            return Until(predicate, 0f, CancellationToken.None);
        }

        public static Task<bool> Until(Func<bool> predicate, float timeoutSeconds)
        {
            return Until(predicate, timeoutSeconds, CancellationToken.None);
        }

        public static Task<bool> Until(Func<bool> predicate, CancellationToken token)
        {
            return Until(predicate, 0f, token);
        }
        
        public static async Task<bool> Until(Func<bool> predicate, float timeoutSeconds, CancellationToken token)
        {
            float start = Time.realtimeSinceStartup;

            while (!predicate())
            {
                if (token.IsCancellationRequested)
                {
                    return false;
                }

                if (timeoutSeconds > 0f)
                {
                    float elapsed = Time.realtimeSinceStartup - start;
                    if (elapsed >= timeoutSeconds)
                    {
                        return false;
                    }
                }
                
                await Task.Yield();
            }

            return true;
        }
    }
}