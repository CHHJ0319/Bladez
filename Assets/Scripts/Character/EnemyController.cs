using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Character
{
    public class EnemyController : MonoBehaviour
    {
        [Header("Settings")]
        public int maxHP;
        public int curHP;

        public Transform target;

        protected bool isChase;
        protected NavMeshAgent navMeshAgent;

        Vector3 reactVec;

        Rigidbody rb;
        BoxCollider col;
        Material mat;
        Animator anim;

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
            if (isChase)
            {
                navMeshAgent.SetDestination(target.position);
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
    }
}

