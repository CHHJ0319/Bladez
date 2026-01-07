using UnityEngine;

namespace Player
{
    public class WeaponHandler : MonoBehaviour
    {
        public Weapon.WeaponController equipWeapon;

        private bool isAttackReady;
        private float attackReady;

        public void UpdateFireTimer()
        {
            attackReady += Time.deltaTime;

            isAttackReady = equipWeapon.rate < attackReady;
        }
        
        public bool CanFire()
        {
            if (equipWeapon == null)
                return false;

            if (!isAttackReady)
                return false;

            if (equipWeapon.Type == Weapon.WeaponType.Range && equipWeapon.GetComponent<Weapon.GunController>().curAmmo <= 0)
                return false;

            return true;
        }

        public void Fire()
        {
            equipWeapon.Use();
            attackReady = 0;
        }

        public bool CanReload()
        {
            if (equipWeapon == null)
                return false;

            if (equipWeapon.Type == Weapon.WeaponType.Melee)
                return false;

            return true;
        }

        public void Reload(ref int curAmmo)
        {
            if (equipWeapon.Type == Weapon.WeaponType.Range)
            {
                Weapon.GunController gunController = equipWeapon.GetComponent<Weapon.GunController>();
                int reAmmo = curAmmo < gunController.maxAmmo ? curAmmo : gunController.maxAmmo;
                gunController.curAmmo = reAmmo;
                curAmmo -= reAmmo;
            }
            
        }
    }
}