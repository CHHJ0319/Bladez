using System;
using UnityEngine;

namespace Events
{
    public static class EnemyEvents
    {
        public static event Action<GameObject> OnFireHit;

        public static void Clear()
        {
            OnFireHit = null;
        }

        public static void Damage(GameObject target)
        {
            OnFireHit?.Invoke(target);
        }
    }
}
