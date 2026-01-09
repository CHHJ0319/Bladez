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
            yield return new WaitForSeconds(0.0f);
            meleeArea.gameObject.SetActive(true);
            trailEffect.enabled = true;

            // 공격 애니메이션 타이밍에 맞춰 이펙트 생성
            yield return new WaitForSeconds(0.5f);

            if (particle != null)
            {
                // 1. 생성 위치 계산
                Vector3 spawnPos = transform.position + (transform.right * 0.5f) + (transform.forward * 0.5f);

                // 2. 파티클 프리팹을 월드 공간에 생성 (검의 자식이 아님)
                ParticleSystem newVFX = Instantiate(particle, spawnPos, transform.rotation);

                // 3. 실행 및 자동 삭제 설정
                newVFX.Play();
                Destroy(newVFX.gameObject, 1.0f);
            }

            audioSource.Play();

            yield return new WaitForSeconds(0.3f);
            meleeArea.gameObject.SetActive(false);

            yield return new WaitForSeconds(0.3f);
            trailEffect.enabled = false;
        }
    }
}