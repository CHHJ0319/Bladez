using UnityEngine;

namespace Enemy
{
    public class EnemyAttack : MonoBehaviour
    {
        void OnParticleCollision(GameObject other)
        {
            Events.EnemyEvents.Damage(other);
        }
    }
}