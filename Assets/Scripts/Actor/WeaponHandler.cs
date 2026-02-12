using System.Collections.Generic;
using UnityEngine;

namespace Actor
{
    public class WeaponHandler : MonoBehaviour
    {
        public Transform weaponHolder;
        public Actor.Item.Weapon.WeaponController EquippedWeapon { get; private set; }

        public List<GameObject> slottedWeapons = new List<GameObject>();
        private int maxWeaponSlots = 3;

        private bool isAttackReady;
        private float attackReady;

        private void Awake()
        {
            if (weaponHolder.transform.childCount > 0)
            {
                foreach (var weapon in weaponHolder.GetComponentsInChildren<Item.Weapon.WeaponController>())
                {
                    AddWeapon(weapon.gameObject);
                }
            }
        }

        public void AssignOwnerId(string id)
        {
            foreach(var weapon in slottedWeapons)
            {
                weapon.GetComponent<Item.Weapon.WeaponController>().SetOwnerID(id);
            }
        }

        public void UpdateAttackTimer()
        {
            attackReady += Time.deltaTime;

            if(EquippedWeapon != null)
            {
                isAttackReady = EquippedWeapon.rate < attackReady;
            }
        }

        public void Attack()
        {
            if (EquippedWeapon == null)
                return;

            if (!isAttackReady)
                return;

            if (EquippedWeapon.Type == Item.Weapon.WeaponType.Range && EquippedWeapon.GetComponent<Item.Weapon.Range.GunController>().curAmmo <= 0)
                return;

            EquippedWeapon.Attack();
            attackReady = 0;
        }

        public void EquipWeapon(int idx)
        {
            foreach(var weapon in slottedWeapons)
            {
                weapon.SetActive(false);
                weapon.GetComponent<Item.Weapon.WeaponController>().IsEquiped = false;
            }

            slottedWeapons[idx].GetComponent<CapsuleCollider>().enabled = false;
            slottedWeapons[idx].SetActive(true);
            EquippedWeapon = slottedWeapons[idx].GetComponent<Item.Weapon.WeaponController>();
            EquippedWeapon.IsEquiped = true;
        }

        public bool CanAddWeapon()
        {
            return slottedWeapons.Count < maxWeaponSlots;
        }

        public void AddWeapon(GameObject newWeapon)
        {
            if (slottedWeapons.Contains(newWeapon)) return;

            newWeapon.transform.SetParent(weaponHolder);

            slottedWeapons.Add(newWeapon);
            if (TryGetComponent<Actor.CharacterController>(out var handler))
            {
                handler.AssignWeaponOwnerID();
            }

            if (EquippedWeapon == null)
            {
                EquipWeapon(0);
            }
            else
            {
                newWeapon.SetActive(false);
            }
        }

        public void RemoveWeapon(GameObject weaponToRemove)
        {
            if (slottedWeapons.Contains(weaponToRemove))
            {
                slottedWeapons.Remove(weaponToRemove);
            }
        }

        public Actor.Item.Weapon.WeaponType GetEquipWeaponType()
        {
            return EquippedWeapon.Type;
        }
    }
}