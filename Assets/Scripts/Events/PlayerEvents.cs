using System;
using UnityEngine;

namespace Events
{
    public static class PlayerEvents
    {
        public static event Action<float, float> OnMove;
        public static event Action OnAttack;
        public static event Action OnJump;
        public static event Action OnSliding;
        public static event Action OnReload;

        public static void Clear()
        {
            OnMove = null;
            OnAttack = null;
            OnJump = null;
            OnSliding = null;
            OnReload = null;
        }

        public static void Move(float horizontal, float vertical)
        {
            OnMove?.Invoke(horizontal, vertical);
        }

        public static void Attack()
        {
            OnAttack?.Invoke();
        }

        public static void Jump()
        {
            OnJump?.Invoke();
        }

        public static void Sliding()
        {
            OnSliding?.Invoke();
        }

        public static void Reload()
        {
            OnReload?.Invoke();
        }
    }
}
