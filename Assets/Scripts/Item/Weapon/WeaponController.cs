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
        public CapsuleCollider capsuleCollider;

        public string ownerName;
        public bool isEquiped;

        public void OnPickedUp(string name)
        {
            capsuleCollider.enabled = false;

            ownerName = name;
        }

        public virtual void Use()
        {
            StartCoroutine("Attack");
        }

        IEnumerator Attack()
        {
            yield return new WaitForSeconds(0.3f);
            meleeArea.gameObject.SetActive(true);
            //trailEffect.enabled = true;

            audioSource.Play();

            yield return new WaitForSeconds(0.2f);
            meleeArea.gameObject.SetActive(false);

            yield return new WaitForSeconds(0.3f);
            //trailEffect.enabled = false;
        }
    }
}