using UnityEngine;

namespace Actor.Enemy
{
    public class Observer : MonoBehaviour
    {
        private Transform player;
        public bool IsPlayerDetected { get; private set; }

        bool isPlayerInRange;

        void OnTriggerEnter(Collider other)
        {
            if (other.transform == player)
            {
                isPlayerInRange = true;
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (other.transform == player)
            {
                isPlayerInRange = false;
            }
        }

        void Update()
        {
            if (isPlayerInRange)
            {
                Vector3 direction = player.position - transform.position + Vector3.up;
                Ray ray = new Ray(transform.position, direction);
                RaycastHit raycastHit;

                if (Physics.Raycast(ray, out raycastHit))
                {
                    if (raycastHit.collider.transform == player)
                    {
                        IsPlayerDetected = true;
                    }
                }
            }
        }

        public void SetPlayer(Transform target)
        {
            player = target;
        }
    }
}