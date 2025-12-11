using System.Collections;
using UnityEngine;

namespace Character
{
    

    public class Enemy : MonoBehaviour
    {
        public int maxHP;
        public int curHP;

        Rigidbody rb;
        BoxCollider col;
        Material mat; 

        void Awake()
        {
            rb = GetComponent<Rigidbody>();
            col = rb.GetComponent<BoxCollider>();
            mat = GetComponentInChildren<SkinnedMeshRenderer>().material;
        }

        void OnTriggerEnter(Collider collider)
        {
            if(collider.tag == "Bullet")
            {
                Bullet bullet = collider.GetComponent<Bullet>();
                curHP -= bullet.damage;
                Vector3 reactVec = rb.position - collider.GetComponent<Rigidbody>().position;

                StartCoroutine(OnDamage(reactVec));
            }
        }

        IEnumerator OnDamage(Vector3 reactVec)
        {
            mat.color = Color.red;
            yield return new WaitForSeconds(0.1f);

            if(curHP > 0)
            {
                mat.color = Color.white;
            }
            else
            {
                mat.color = Color.gray;

                reactVec = reactVec.normalized;
                reactVec += Vector3.up;
                rb.AddForce(reactVec * 5, ForceMode.Impulse);

                Destroy(gameObject, 1);
            }
        }
    }
}

