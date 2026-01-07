using System.Collections;
using UnityEngine;

namespace Item.Weapon 
{
    public class GunController : WeaponController
    {
        public int maxAmmo;
        public int curAmmo;

        public GameObject bullet;
        public Transform bulletPos;
        public float bulletSpeed;

        public GameObject shotEffect;

        public void Start()
        {
            Type = WeaponType.Range;
        }

        public override void Use()
        {
            StartCoroutine("Attack");
            curAmmo--;
        }

        IEnumerator Attack()
        {
            audioSource.Play();

            GameObject effect = Instantiate(shotEffect, bulletPos.position, bulletPos.rotation);

            GameObject instantBulet = Instantiate(bullet, bulletPos.position, bulletPos.rotation);
            Rigidbody bulletRb = instantBulet.GetComponent<Rigidbody>();
            bulletRb.linearVelocity = bulletPos.forward * bulletSpeed;

            yield return null;
        }
    }
}