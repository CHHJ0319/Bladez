using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Actor.Enemy
{
    public class EnemyController : MonoBehaviour
    {
        public Transform target;
        public Observer observer;
        public GameObject dectionMark;

        public bool isAttacking;

        public ParticleSystem fireParticle;
        public float fireRate = 2.0f;

        protected float fireDelay;
        protected bool isFireReady;

        public int maxHP;
        public int curHP;
        public virtual float ad => 10;

        protected NavMeshAgent navMeshAgent;
        protected bool isHit;

        Vector3 reactVec;

        protected Rigidbody rb;
        protected Material mat;
        protected Animator anim;

        void Awake()
        {
            rb = GetComponent<Rigidbody>();
            mat = GetComponentInChildren<SkinnedMeshRenderer>().materials[1];
            anim = GetComponent<Animator>();
            navMeshAgent = GetComponent<NavMeshAgent>();

            observer.SetPlayer(target);
        }

        private void Start()
        {
            Events.PlayerEvents.OnBulletHit += TakeDamage;
            Events.EnemyEvents.OnFireHit += OnFireHit;
        }

        void Update()
        {
            fireDelay += Time.deltaTime;
            isFireReady = fireDelay > fireRate;

            if (observer.IsPlayerDetected && !isHit)
            // if (isAttacking)
            {
                if (isFireReady)
                {
                    Fire();
                    fireDelay = 0f;
                }
                else
                {
                    navMeshAgent.SetDestination(target.position);
                }
            }
        }

        public void TakeDamage(int damage, Vector3 bulletPos)
        {
            if(!isHit)
            {
                reactVec = transform.position + bulletPos;
                reactVec = reactVec.normalized;

                StartCoroutine(OnDamage(damage));
            }
        }

        IEnumerator OnDamage(int damage)
        {
            curHP -= damage;

            ApplyKnockback();

            yield return new WaitForSeconds(0.2f);

            if(curHP > 0)
            {
                ResumeActivity();
            }
            else
            {
                Die();
            }
        }

        void ApplyKnockback()
        {
            mat.color = Color.red;

            isHit = true;
            navMeshAgent.enabled = false;
            rb.isKinematic = false;

            
            //rb.AddForce(reactVec * 5f, ForceMode.Impulse);
        }

        void ResumeActivity()
        {
            mat.color = Color.white;
            rb.linearVelocity = Vector3.zero;

            isHit = false;
            navMeshAgent.enabled = true;
            rb.isKinematic = true;
        }

        void Die()
        {
            mat.color = Color.gray;

            reactVec += Vector3.up;
            //rb.AddForce(reactVec * 5f, ForceMode.Impulse);

            anim.enabled = false;
            Destroy(gameObject, 1f);
        }

        protected void Fire()
        {
            if (fireParticle == null) return;

            fireParticle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            fireParticle.Play();
        }

        public void OnFireHit(GameObject target)
        {
            if (target.CompareTag("Player"))
            {
                Actor.Player.PlayerController player = target.gameObject.GetComponent<Actor.Player.PlayerController>();
                player.TakeDamage(ad);
   
            }
        }
    }
}

