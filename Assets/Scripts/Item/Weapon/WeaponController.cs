using Actor;
using Actor.Player;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

namespace Item.Weapon 
{
    public abstract class WeaponController : MonoBehaviour
    {
        [Header("Type")]
        public abstract WeaponType Type { get; protected set; }
        public ElementType elementType;

        [Header("Effects")]
        public AudioSource audioSource;

        [Header("Properties")]
        public float damage;
        public float rate;
        public float knockbackForce;

        public string ownerID;
        public bool isEquiped;

        public void Attack()
        {
            StartCoroutine(AttackProcess());
        }

        protected abstract IEnumerator AttackProcess();

        public void SetOwnerID(string id)
        {
            ownerID = id;
        }
    }
}