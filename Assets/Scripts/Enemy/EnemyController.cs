using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Enemy
{
    public class EnemyController : MonoBehaviour
    {
        [Header("Attack Settings")]
        public ParticleSystem fireParticle;
        public float fireRate = 2.0f;

        protected float fireDelay;
        protected bool isFireReady;

        [Header("Settings")]
        public int maxHP;
        public int curHP;
        public virtual float ad => 10;

        public Transform target;

        protected bool isChase;
        protected NavMeshAgent navMeshAgent;

        Vector3 reactVec;

        Rigidbody rb;
        BoxCollider col;
        Material mat;
        Animator anim;

        private void OnEnable()
        {
            Events.EnemyEvents.OnFireHit += OnFireHit;
        }

        private void OnDisable()
        {
            Events.EnemyEvents.OnFireHit -= OnFireHit;

        }

        void Awake()
        {
            rb = GetComponent<Rigidbody>();
            col = rb.GetComponent<BoxCollider>();
            mat = GetComponentInChildren<SkinnedMeshRenderer>().material;
            anim = GetComponent<Animator>();
            navMeshAgent = GetComponent<NavMeshAgent>();
        }


        void Update()
        {
            fireDelay += Time.deltaTime;
            isFireReady = fireDelay > fireRate;

            if (isChase)
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

        private void FixedUpdate()
        {
            FreezeVelocity();
        }

        void OnTriggerEnter(Collider collider)
        {
            if(collider.tag == "Bullet")
            {
                Bullet bullet = collider.GetComponent<Bullet>();
                curHP -= bullet.damage;
                reactVec = rb.position - collider.GetComponent<Rigidbody>().position;

                StartCoroutine(OnDamage());
            }
        }

        IEnumerator OnDamage()
        {
            mat.color = Color.red;
            yield return new WaitForSeconds(0.1f);

            if(curHP > 0)
            {
                mat.color = Color.white;
            }
            else
            {
                Die();
            }
        }

        void FreezeVelocity()
        {
            if(isChase)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }

        void Die()
        {
            mat.color = Color.gray;

            anim.SetTrigger("doDie");
            isChase = false;

            reactVec = reactVec.normalized;
            reactVec += Vector3.up;
            rb.AddForce(reactVec * 5, ForceMode.Impulse);

            Destroy(gameObject, 1);
        }

        public void StartChase()
        {
            isChase = true;
        }

        protected void Fire()
        {
            if (fireParticle == null) return;

            fireParticle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            fireParticle.Play();
        }

        public void OnFireHit(GameObject target)
        {

            //Debug.Log($"{target.name}ø° ∏Ì¡ﬂ!");

            if (target.CompareTag("Player"))
            {
                Player.PlayerController player = target.gameObject.GetComponent<Player.PlayerController>();
                player.TakeDamage(ad);
   
            }
        }
    }
}

