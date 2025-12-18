using System;
using UnityEngine;

namespace Events
{
    public static class EnemyEvents
    {
        public static event Action<Enemy.Observer> OnPlayerDetected;
        public static event Action<GameObject> OnFireHit;

        public static void Clear()
        {
            OnPlayerDetected = null;
            OnFireHit = null;
        }

        public static void DetectPlayer(Enemy.Observer observer)
        {
            OnPlayerDetected?.Invoke(observer);
        }

        public static void Damage(GameObject target)
        {
            OnFireHit?.Invoke(target);
        }
    }
}
