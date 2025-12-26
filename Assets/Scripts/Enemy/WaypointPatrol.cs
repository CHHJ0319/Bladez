using UnityEngine;
using UnityEngine.AI;

namespace Enemy
{
    public class WaypointPatrol : EnemyController
    {
        public Transform[] waypoints;
        public override float ad => 20;

        int currentWaypointIndex;

        void Awake()
        {
            rb = GetComponent<Rigidbody>();
            mat = GetComponentInChildren<SkinnedMeshRenderer>().material;
            anim = GetComponent<Animator>();
            navMeshAgent = GetComponent<NavMeshAgent>();

            observer.SetPlayer(target);
        }

        void Update()
        {
            fireDelay += Time.deltaTime;
            isFireReady = fireDelay > fireRate;

            if (observer.IsPlayerDetected && !isHit)
            //if (isAttacking)
            {
                if (isFireReady)
                {
                    navMeshAgent.velocity = Vector3.zero;

                    Fire();
                    fireDelay = 0f;
                }
                else
                {
                    navMeshAgent.SetDestination(target.position);
                }
            }
            else if (!isHit)
            {
                if (navMeshAgent.remainingDistance < navMeshAgent.stoppingDistance)
                {
                    currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
                    navMeshAgent.SetDestination(waypoints[currentWaypointIndex].position);
                }
            }

        }
    }
}
