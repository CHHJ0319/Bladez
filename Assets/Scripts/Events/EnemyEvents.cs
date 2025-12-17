using System;
using UnityEngine;

namespace Events
{
    public static class EnemyEvents
    {
        public static event Action<Character.Enemy.Observer> OnPlayerDetected;

        public static void Clear()
        {
            OnPlayerDetected = null;
        }

        public static void DetectPlayer(Character.Enemy.Observer observer)
        {
            OnPlayerDetected?.Invoke(observer);
        }
    }
}
