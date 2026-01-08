using UnityEngine;

public class WeaponHit : MonoBehaviour
{
    public ParticleSystem hitVFXPrefab;
    public AudioClip hitSFX;
    public LayerMask enemyLayer;

    void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & enemyLayer) == 0)
            return;

        Vector3 hitPoint = other.ClosestPoint(transform.position);

        SpawnHitEffects(hitPoint);

        //// Optional: damage
        //if (other.TryGetComponent(out EnemyHealth hp))
        //{
        //    hp.TakeDamage(10);
        //}
    }

    void SpawnHitEffects(Vector3 position)
    {
        if (hitVFXPrefab)
        {
            ParticleSystem vfx =
                Instantiate(hitVFXPrefab, position, Quaternion.identity);
            vfx.Play();
            Destroy(vfx.gameObject, 2f);
        }

        if (hitSFX)
        {
            AudioSource.PlayClipAtPoint(hitSFX, position);
        }
    }
}
