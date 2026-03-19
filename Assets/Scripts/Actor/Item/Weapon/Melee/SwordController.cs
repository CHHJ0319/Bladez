using System.Collections;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

namespace Actor.Item.Weapon.Melee
{
    public class SwordController : WeaponController
    {
        [Header("Type")]
        public override Data.WeaponType Type { get; protected set; } = Data.WeaponType.Melee;

        [Header("Sword Effects")]
        public TrailRenderer trailEffect;
        public ParticleSystem particle;

        [Header("Dropped Effects")]
        public DroppedEffects droppedEffects;

        private Vector3 oriPosition;
        private Vector3 oriScale;

        private void Awake()
        {
            oriPosition = transform.localPosition;
            oriScale = transform.localScale;
        }

        [Rpc(SendTo.Everyone)]
        public void SubmitItemDroppedServerRpc()
        {
            Embed();
            droppedEffects.Show(ElementType);
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

        private void Embed()
        {
            transform.localPosition = new Vector3(transform.localPosition.x, 0.8f, transform.localPosition.z);
            transform.localRotation = Quaternion.Euler(180f, 0f, 0f);
        }
    }
}