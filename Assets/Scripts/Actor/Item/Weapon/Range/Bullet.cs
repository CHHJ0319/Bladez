using UnityEngine;

namespace Actor.Item.Weapon 
{
    public class Bullet : MonoBehaviour
    {
        public int damage;

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.collider.gameObject.tag == "Environment")
            {
                Destroy(gameObject);
            }
            else if (collision.collider.gameObject.tag == "Enemy")
            {
                Events.PlayerEvents.OnAttackHit(damage, transform.position);
                Destroy(gameObject);
            }
        }

        private void OnTriggerEnter(Collider collision)
        {
            if (collision.gameObject.tag == "Environment")
            {
                Destroy(gameObject);
            }
            else if (collision.gameObject.tag == "Enemy")
            {
                collision.gameObject.GetComponent<Actor.Enemy.EnemyController>().TakeDamage(damage, transform.position);
                Destroy(gameObject);
            }
        }
    }
}
