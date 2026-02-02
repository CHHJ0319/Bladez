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
        public CapsuleCollider capsuleCollider;

        public TrailRenderer trailEffect;
        public ParticleSystem particle;

        public AudioSource audioSource;

        public string ownerName;
        public bool isEquiped;

        public virtual void Attack()
        {
            StartCoroutine("AttackProcess");
        }

        IEnumerator AttackProcess()
        {
            yield return new WaitForSeconds(0.0f);
            meleeArea.gameObject.SetActive(true);
            if (trailEffect != null)
            {
                trailEffect.enabled = true;
            }

            yield return new WaitForSeconds(0.5f);

            if (particle != null)
            {
                Vector3 spawnPos = transform.position + (transform.right * 0.5f) + (transform.forward * 0.5f);

                ParticleSystem newVFX = Instantiate(particle, spawnPos, transform.rotation);

                newVFX.Play();
                Destroy(newVFX.gameObject, 1.0f);
            }
            audioSource.Play();

            yield return new WaitForSeconds(0.3f);
            meleeArea.gameObject.SetActive(false);

            yield return new WaitForSeconds(0.3f);
            if (trailEffect != null)
            {
                trailEffect.enabled = false;
            }
        }

        public void OnPickedUp(string name)
        {
            //capsuleCollider.enabled = false;

            ownerName = name;
        }
    }
}