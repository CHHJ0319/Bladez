using UnityEngine;

namespace Player
{
    public class WeaponHandler : MonoBehaviour
    {
        public Weapon equipWeapon;

        private bool isFireReady;
        private float fireDelay;

        public void UpdateFireTimer()
        {
            fireDelay += Time.deltaTime;
            isFireReady = equipWeapon.rate < fireDelay;
        }
        
        public bool CanFire()
        {
            if (equipWeapon == null)
                return false;

            if (equipWeapon.curAmmo <= 0)
                return false;

            if (!isFireReady)
                return false;

            return true;
        }

        public void Fire()
        {
            equipWeapon.Use();
            fireDelay = 0;
        }

        public bool CanReload()
        {
            if (equipWeapon == null)
                return false;

            if (equipWeapon.type == Weapon.Type.Melee)
                return false;

            return true;
        }

        public void Reload(ref int curAmmo)
        {
            int reAmmo = curAmmo < equipWeapon.maxAmmo ? curAmmo : equipWeapon.maxAmmo;
            equipWeapon.curAmmo = reAmmo;
            curAmmo -= reAmmo;
        }
    }
}