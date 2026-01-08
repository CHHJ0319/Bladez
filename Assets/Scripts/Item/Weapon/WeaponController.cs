using System.Collections;
using UnityEngine;

namespace Item.Weapon 
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
            yield return new WaitForSeconds(0.1f);
            meleeArea.enabled = true;
            //trailEffect.enabled = true;

            audioSource.Play();

            yield return new WaitForSeconds(0.3f);
            meleeArea.enabled = false;

            yield return new WaitForSeconds(0.3f);
            //trailEffect.enabled = false;
        }
    }
}