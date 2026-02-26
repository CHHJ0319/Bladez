using System;
using UnityEngine;

namespace Events
{
    public static class PlayerEvents
    {
        public static event Action OnAttack;
        public static event Action OnJump;
        public static event Action OnSliding;
        public static event Action OnReload;
        public static event Action<float, float> OnHPChaneged;
        public static event Action<int> OnQuickSlotPressed;

        public static void Clear()
        {
            OnAttack = null;
            OnJump = null;
            OnSliding = null;
            OnReload = null;
            OnHPChaneged = null;

            OnQuickSlotPressed = null;
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

        public static void UpdateHPBar(float hp, float maxHP)
        {
            OnHPChaneged?.Invoke(hp, maxHP);
        }

        public static void ChangeWeapon(int weaponIdx)
        {
            OnQuickSlotPressed?.Invoke(weaponIdx);
        }
    }
}
