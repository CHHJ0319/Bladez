using UnityEngine;

namespace Enemy
{
    public class WaypointPatrol : EnemyController
    {
        public Transform[] waypoints;
        public override float ad => 20;

        int m_CurrentWaypointIndex;

        void Update()
        {
            fireDelay += Time.deltaTime;
            isFireReady = fireDelay > fireRate;

            if (isChase)
            {
                if (isFireReady)
                {
                    navMeshAgent.isStopped = true;
                    navMeshAgent.updateRotation = false;
                    navMeshAgent.velocity = Vector3.zero;

                    Fire();
                    fireDelay = 0f;
                }
                else
                {
                    navMeshAgent.isStopped = false;
                    navMeshAgent.updateRotation = true;
                    navMeshAgent.SetDestination(target.position);
                }
            }
            else
            {
                navMeshAgent.isStopped = false;
                navMeshAgent.updateRotation = true;

                if (navMeshAgent.remainingDistance < navMeshAgent.stoppingDistance)
                {
                    m_CurrentWaypointIndex = (m_CurrentWaypointIndex + 1) % waypoints.Length;
                    navMeshAgent.SetDestination(waypoints[m_CurrentWaypointIndex].position);
                }
            }

        }
    }
}
