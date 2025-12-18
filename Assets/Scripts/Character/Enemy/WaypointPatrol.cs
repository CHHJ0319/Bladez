using UnityEngine;

namespace Character.Enemy
{
    public class WaypointPatrol : EnemyController
    {
        public Transform[] waypoints;
        public EnemyLaserCombat laserCombat;

        int m_CurrentWaypointIndex;

        void Update()
        {
            laserCombat.Fire();
            if (isChase)
            {
                navMeshAgent.SetDestination(target.position);
                //laserCombat.Fire();
            }
            else
            {
                if (navMeshAgent.remainingDistance < navMeshAgent.stoppingDistance)
                {
                    //m_CurrentWaypointIndex = (m_CurrentWaypointIndex + 1) % waypoints.Length;
                    //navMeshAgent.SetDestination(waypoints[m_CurrentWaypointIndex].position);
                }
            }

        }
    }
}
