using UnityEngine;

namespace Item.Weapon
{
    public class DroppedWeapon : MonoBehaviour
    {
        public GameObject weaponPrefab;

        [Header("Effects")]
        public Light dropZoneLight;
        public GameObject[] particles;

        private Color[] dropZoneLightColors = new Color[]
        {
            new Color(255f / 255f, 100f / 255f, 0f / 255f),
            new Color(73f / 255f, 239f / 255f, 255f / 255f),
            new Color(244f / 255f, 255f / 255f, 86f / 255f)
        };

        private WeaponController weapon;
        private ElementType elementType;
        

        private void Start()
        {
            Initialize();
            ApplyItemEffects();
            CreateWeapon();
        }

        private void Update()
        {
            CheckForDroppedWeapons();
        }

        private void Initialize()
        {
            weapon = weaponPrefab.GetComponent<WeaponController>();
            elementType = weapon.ElementType;
        }

        private void ApplyItemEffects()
        {
            if (elementType != ElementType.Neutral)
            {
                dropZoneLight.color = dropZoneLightColors[(int)elementType];
                GameObject particle = Instantiate(particles[(int)elementType], transform);
            }
        }

        private void CreateWeapon()
        {
            GameObject newWeapon = Instantiate(weaponPrefab, transform);

            newWeapon.transform.localPosition = new Vector3(0, -1.0f, 0);
            newWeapon.transform.localRotation = Quaternion.identity;
            newWeapon.transform.localScale = Vector3.one;
        }

        private void CheckForDroppedWeapons()
        {
            if (transform.childCount < 3)
            {
                Destroy(gameObject);
            }
        }
    }
}