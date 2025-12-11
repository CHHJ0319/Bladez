using System.Collections;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum Type { Melee, Range }
    public Type type;
    public int damage;
    public float rate;
    public BoxCollider meleeArea;
    public TrailRenderer trailEffect;

    //Gun
    public int maxAmmo;
    public int curAmmo;

    public GameObject bullet;
    public Transform bulletPos;
    public float bulletSpeed;

    public GameObject shotEffect;

    public void Use()
    {
        StartCoroutine("Shot");
        curAmmo--;
    }

    IEnumerator Shot ()
    {
        GameObject effect = Instantiate(shotEffect, bulletPos.position, bulletPos.rotation);

        GameObject instantBulet = Instantiate(bullet, bulletPos.position, bulletPos.rotation);
        Rigidbody bulletRb = instantBulet.GetComponent<Rigidbody>();
        bulletRb.linearVelocity = bulletPos.forward * bulletSpeed;

        yield return null;
    }
}
