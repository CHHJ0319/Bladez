using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.tag == "Environment" || collision.collider.gameObject.tag == "Enemy")
        {
            Destroy(gameObject);
        }
    }
}
