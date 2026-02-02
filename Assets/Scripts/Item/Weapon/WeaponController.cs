using Actor.Player;
using System.Collections;
using UnityEngine;

namespace Item.Weapon 
{
    public class WeaponController : MonoBehaviour
    {
        public WeaponType Type { get; protected set; } = WeaponType.Melee;
        public float damage;
        public float knockbackForce;
        public float rate;

        public TrailRenderer trailEffect;
        public ParticleSystem particle;
        public AudioSource audioSource;

        public string ownerName;
        public bool isEquiped;

        public virtual void Attack()
        {
            StartCoroutine(AttackProcess());
        }

        IEnumerator AttackProcess()
        {
            yield return new WaitForSeconds(0.0f);

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

            yield return new WaitForSeconds(0.3f);
            if (trailEffect != null)
            {
                trailEffect.enabled = false;
            }
        }

        public void OnPickedUp(string name)
        {
            ownerName = name;
        }

        private void OnTriggerEnter(Collider other)
        {
            string targetName = "";

            GameObject rootGameObject = other.transform.root.gameObject;

            if (rootGameObject.TryGetComponent(out PlayerController playerCcontroller))
            {
                targetName = rootGameObject.name;
                //Debug.Log($"{targetName}와(과) 접촉했습니다!");
            }
            else if (rootGameObject.TryGetComponent(out RemotePlayerController remotePlayerController))
            {
                targetName = rootGameObject.name;
                Debug.Log($"{targetName}와(과) 접촉했습니다!");

                Vector3 damageDirection = (transform.position - other.transform.position).normalized;
                RemotePlayerController remotePlayer = rootGameObject.GetComponent<RemotePlayerController>();
                remotePlayer.TakeDamage(damage, damageDirection, knockbackForce);
            }
        }
    }
}