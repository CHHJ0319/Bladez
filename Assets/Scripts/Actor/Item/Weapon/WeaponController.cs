using System.Collections;
using Unity.Netcode;
using UnityEngine;

namespace Actor.Item.Weapon 
{
    public abstract class WeaponController : NetworkBehaviour
    {
        [Header("Type")]
        public abstract WeaponType Type { get; protected set; }
        [SerializeField] private ElementType elementType;
        public ElementType ElementType
        {
            get => elementType;
            set => elementType = value;
        }

        [Header("Effects")]
        public AudioSource audioSource;

        [Header("Properties")]
        public float damage;
        public float rate;
        public float knockbackForce;

        public int OwnerID { get; private set; }
        public bool IsEquiped { get; set; }

        public void Attack()
        {
            StartCoroutine(AttackProcess());
        }

        public void SetOwnerID(int id)
        {
            OwnerID = id;
        }

        protected abstract IEnumerator AttackProcess();
    }
}