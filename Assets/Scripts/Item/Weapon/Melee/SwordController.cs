using Actor;
using Actor.Player;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

namespace Item.Weapon.Melee
{
    public class SwordController : WeaponController
    {
        [Header("Type")]
        public override WeaponType Type { get; protected set; } = WeaponType.Melee;

        [Header("Sword Effects")]
        public TrailRenderer trailEffect;
        public ParticleSystem particle;

        protected override IEnumerator AttackProcess()
        {
            yield return new WaitForSeconds(0.0f);
            GetComponent<CapsuleCollider>().enabled = true;

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
            //audioSource.Play();

            yield return new WaitForSeconds(0.3f);

            yield return new WaitForSeconds(0.3f);
            GetComponent<CapsuleCollider>().enabled = false;
            if (trailEffect != null)
            {
                trailEffect.enabled = false;
            }
        }


        private void OnTriggerEnter(Collider other)
        {
            GameObject rootGameObject = other.transform.root.gameObject;

            if (rootGameObject.TryGetComponent(out NetworkObject netObj))
            {
                if (ownerID == rootGameObject.GetComponent<CharacterNetworkHandler>().ownerID)
                {
                    
                }
                else
                {
                    Vector3 damageDirection = (transform.position - other.transform.position).normalized;
                    rootGameObject.GetComponent<PlayerController>().TakeDamage(damage, damageDirection, knockbackForce);
                }
            }
        }
    }
}