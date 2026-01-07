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
        public static event Action<int, Vector3> OnBulletHit;
        public static event Action OnItemPickedUp;
        public static event Action<int> OnQuickSlotPressed;


        public static void Clear()
        {
            OnMove = null;
            OnAttack = null;
            OnJump = null;
            OnSliding = null;
            OnReload = null;
            OnBulletHit = null;
            OnItemPickedUp = null;
            OnQuickSlotPressed = null;
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

        public static void OnAttackHit(int damage, Vector3 bulletPos)
        {
            OnBulletHit?.Invoke(damage, bulletPos);
        }

        public static void PickUp()
        {
            OnItemPickedUp?.Invoke();
        }

        public static void ChangeWeapon(int weaponIdx)
        {
            OnQuickSlotPressed?.Invoke(weaponIdx);
        }
    }
}
