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

                Debug.Log("Hit");
            }
        }

        IEnumerator OnDamage()
        {
            yield return null;
        }
    }
}

