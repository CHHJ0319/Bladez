using System.Collections;
using System.Xml.Serialization;
using UnityEngine;

namespace Item.Weapon 
{
    public abstract class WeaponController : MonoBehaviour
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

        public string OwnerID { get; private set; }
        public bool IsEquiped { get; set; }

        private Vector3 originalWeaponPosition;
        private Vector3 originalWeaponScale;

        private void Awake()
        {
            originalWeaponPosition = transform.localPosition;
            originalWeaponScale = transform.localScale;
        }

        public void Attack()
        {
            StartCoroutine(AttackProcess());
        }

        public void SetOwnerID(string id)
        {
            OwnerID = id;

            transform.localPosition = originalWeaponPosition;
            transform.localScale = originalWeaponScale;
            transform.localRotation = Quaternion.identity;
        }

        protected abstract IEnumerator AttackProcess();
    }
}