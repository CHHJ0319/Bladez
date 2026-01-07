using System.Collections;
using UnityEngine;

namespace Weapon 
{
    public class WeaponController : MonoBehaviour
    {
        public WeaponType Type { get; protected set; } = WeaponType.Melee;
        public int damage;
        public float rate;
        public BoxCollider meleeArea;
        public TrailRenderer trailEffect;
        public AudioSource audioSource;


        public virtual void Use()
        {
            StartCoroutine("Attack");
        }

        IEnumerator Attack()
        {
            audioSource.Play();

            yield return null;
        }
    }
}