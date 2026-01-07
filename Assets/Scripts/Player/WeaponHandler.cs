using Item.Weapon;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class WeaponHandler : MonoBehaviour
    {
        public Transform weaponHolder;
        public Item.Weapon.WeaponController equipWeapon;

        public List<GameObject> weapons = new List<GameObject>();
        private int maxWeaponSlots = 3;

        private bool isAttackReady;
        private float attackReady;

        private void OnEnable()
        {
            Events.PlayerEvents.OnQuickSlotPressed += EquipWeapon;
        }

        public void UpdateFireTimer()
        {
            attackReady += Time.deltaTime;

            if(equipWeapon != null)
            {
                isAttackReady = equipWeapon.rate < attackReady;
            }
        }
        
        public bool CanAttack()
        {
            if (equipWeapon == null)
                return false;

            if (!isAttackReady)
                return false;

            if (equipWeapon.Type == Item.Weapon.WeaponType.Range && equipWeapon.GetComponent<Item.Weapon.GunController>().curAmmo <= 0)
                return false;

            return true;
        }

        public void Attack()
        {
            equipWeapon.Use();
            attackReady = 0;
        }

        public bool CanReload()
        {
            if (equipWeapon == null)
                return false;

            if (equipWeapon.Type == Item.Weapon.WeaponType.Melee)
                return false;

            return true;
        }

        public void Reload(ref int curAmmo)
        {
            if (equipWeapon.Type == Item.Weapon.WeaponType.Range)
            {
                Item.Weapon.GunController gunController = equipWeapon.GetComponent<Item.Weapon.GunController>();
                int reAmmo = curAmmo < gunController.maxAmmo ? curAmmo : gunController.maxAmmo;
                gunController.curAmmo = reAmmo;
                curAmmo -= reAmmo;
            }
            
        }

        public Item.Weapon.WeaponType GetEquipWeapon()
        {
            return equipWeapon.Type;
        }

        public void EquipWeapon(int idx)
        {
            foreach(var weapon in weapons)
            {
                weapon.SetActive(false);
            }
            
            weapons[idx].SetActive(true);
            equipWeapon = weapons[idx].GetComponent<WeaponController>();
        }

        public bool CanAddWeapon()
        {
            return weapons.Count < maxWeaponSlots;
        }

        public bool AddWeapon(GameObject newWeapon)
        {
            if (!CanAddWeapon())
            {
                return false;
            }

            if (weapons.Contains(newWeapon)) return false;

            newWeapon.transform.SetParent(weaponHolder);
            newWeapon.transform.localPosition = Vector3.zero;
            newWeapon.transform.localRotation = Quaternion.identity;

            weapons.Add(newWeapon);

            if (equipWeapon == null)
            {
                EquipWeapon(0);
            }
            else
            {
                newWeapon.SetActive(false);
            }

            return true;
        }

        public void RemoveWeapon(GameObject weaponToRemove)
        {
            if (weapons.Contains(weaponToRemove))
            {
                weapons.Remove(weaponToRemove);
            }
        }
    }
}