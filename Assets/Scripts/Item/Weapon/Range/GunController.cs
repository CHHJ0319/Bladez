using System.Collections;
using UnityEngine;

namespace Item.Weapon.Range
{
    public class GunController : WeaponController
    {
        [Header("Type")]
        public override WeaponType Type { get; protected set; } = WeaponType.Range;

        public int maxAmmo;
        public int curAmmo;

        public GameObject bullet;
        public Transform bulletPos;
        public float bulletSpeed;

        public GameObject shotEffect;

        protected override IEnumerator AttackProcess()
        {
            curAmmo--;

            audioSource.Play();

            GameObject effect = Instantiate(shotEffect, bulletPos.position, bulletPos.rotation);

            GameObject instantBulet = Instantiate(bullet, bulletPos.position, bulletPos.rotation);
            Rigidbody bulletRb = instantBulet.GetComponent<Rigidbody>();
            bulletRb.linearVelocity = bulletPos.forward * bulletSpeed;

            yield return null;
        }
    }
}