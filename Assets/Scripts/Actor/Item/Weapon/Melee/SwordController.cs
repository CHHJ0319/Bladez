using System.Collections;
using Unity.Netcode;
using UnityEngine;
using static UnityEngine.ParticleSystem;

namespace Actor.Item.Weapon.Melee
{
    public class SwordController : WeaponController
    {
        [Header("Type")]
        public override WeaponType Type { get; protected set; } = WeaponType.Melee;

        [Header("Sword Effects")]
        public TrailRenderer trailEffect;
        public ParticleSystem particle;

        private Vector3 oriPosition;
        private Vector3 oriScale;

        private void Awake()
        {
            oriPosition = transform.localPosition;
            oriScale = transform.localScale;
        }

        protected override IEnumerator AttackProcess()
        {
            yield return new WaitForSeconds(0.0f);
            GetComponent<CapsuleCollider>().enabled = true;

            if (trailEffect != null)
            {
                trailEffect.enabled = true;
            }

            if(particle != null)
            {
                particle.gameObject.SetActive(true);
            }

            yield return new WaitForSeconds(0.5f);

            //audioSource.Play();

            yield return new WaitForSeconds(0.3f);

            yield return new WaitForSeconds(0.3f);
            GetComponent<CapsuleCollider>().enabled = false;
            if (trailEffect != null)
            {
                trailEffect.enabled = false;
            }

            if (particle != null)
            {
                particle.gameObject.SetActive(false);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!IsEquiped) return;

            GameObject rootGameObject = other.transform.root.gameObject;

            if (rootGameObject.TryGetComponent(out NetworkObject netObj))
            {
                if (OwnerID == rootGameObject.GetComponent<Actor.Player.PlayerController>().playerID.Value)
                {
                    
                }
                else
                {
                    Vector3 damageDirection = (transform.position - other.transform.position).normalized;
                    Actor.Player.PlayerController targetPlayer = rootGameObject.GetComponent<Actor.Player.PlayerController>();
                    targetPlayer.TakeDamage(damage, damageDirection, knockbackForce);
                    //targetPlayer.SubmitTakeDamageRequestServerRpc();
                }
            }
        }

        public void SetOrginTransform()
        {
            transform.localPosition = oriPosition;
            transform.localRotation = Quaternion.identity;
            transform.localScale = oriScale;
        }
    }
}