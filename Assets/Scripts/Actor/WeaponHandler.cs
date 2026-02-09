using Item.Weapon;
using System.Collections.Generic;
using UnityEngine;

namespace Actor
{
    public class WeaponHandler : MonoBehaviour
    {
        public Transform weaponHolder;
        public Item.Weapon.WeaponController equipWeapon;

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
                weapon.GetComponent<WeaponController>().SetOwnerID(id);
            }
        }

        public void UpdateAttackTimer()
        {
            attackReady += Time.deltaTime;

            if(equipWeapon != null)
            {
                isAttackReady = equipWeapon.rate < attackReady;
            }
        }

        public void Attack()
        {
            if (equipWeapon == null)
                return;

            if (!isAttackReady)
                return;

            if (equipWeapon.Type == Item.Weapon.WeaponType.Range && equipWeapon.GetComponent<Item.Weapon.GunController>().curAmmo <= 0)
                return;

            equipWeapon.Attack();
            attackReady = 0;
        }

        public void EquipWeapon(int idx)
        {
            foreach(var weapon in slottedWeapons)
            {
                weapon.SetActive(false);
                weapon.GetComponent<Item.Weapon.WeaponController>().isEquiped = false;
            }

            slottedWeapons[idx].GetComponent<CapsuleCollider>().enabled = false;
            slottedWeapons[idx].SetActive(true);
            equipWeapon = slottedWeapons[idx].GetComponent<Item.Weapon.WeaponController>();
            equipWeapon.isEquiped = true;
        }

        public bool CanAddWeapon()
        {
            return slottedWeapons.Count < maxWeaponSlots;
        }

        public void AddWeapon(GameObject newWeapon)
        {
            if (slottedWeapons.Contains(newWeapon)) return;

            newWeapon.transform.SetParent(weaponHolder);
            //newWeapon.transform.localPosition = Vector3.zero;
            //newWeapon.transform.localRotation = Quaternion.identity;

            slottedWeapons.Add(newWeapon);
            newWeapon.GetComponent<Item.Weapon.WeaponController>().SetOwnerID(gameObject.name);

            if (equipWeapon == null)
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

        public Item.Weapon.WeaponType GetEquipWeaponType()
        {
            return equipWeapon.Type;
        }
    }
}