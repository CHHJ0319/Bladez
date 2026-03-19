using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Actor
{
    public class WeaponHandler : NetworkBehaviour
    {
        public Transform weaponHolder;
        public Actor.Item.Weapon.WeaponController EquippedWeapon { get; private set; }

        public List<GameObject> slottedWeapons = new List<GameObject>();
        private int maxWeaponSlots = 3;

        private bool isAttackReady;
        private float attackReady;

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
            if(slottedWeapons.Count <= idx) return;

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

        [Rpc(SendTo.Server)]
        public void RequestAddWeaponServerRpc(int playerID)
        {
            //if (slottedWeapons.Contains(newWeapon)) return;

            //newWeapon.transform.SetParent(weaponHolder);
            //newWeapon.GetComponent<Item.Weapon.Melee.SwordController>().SetOrginTransform();
            //newWeapon.GetComponent<Item.Weapon.WeaponController>().SetOwnerID(playerID);

            //slottedWeapons.Add(newWeapon);

            //if (EquippedWeapon == null)
            //{
            //    EquipWeapon(0);
            //}
            //else
            //{
            //    newWeapon.SetActive(false);
            //}
        }

        public void AddWeapons(int playerID)
        {
            if (weaponHolder.transform.childCount > 0)
            {
                foreach (var weapon in weaponHolder.GetComponentsInChildren<Item.Weapon.WeaponController>())
                {
                    //RequestAddWeaponServerRpc(weapon.gameObject, playerID);
                }
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