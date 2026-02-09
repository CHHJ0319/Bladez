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

        private Vector3 originalWeaponScale;

        private void Start()
        {
            ElementType elementType = weaponPrefab.GetComponent<WeaponController>().ElementType;
            if(elementType != ElementType.Neutral)
            {
                dropZoneLight.color = dropZoneLightColors[(int)elementType];
                GameObject particle = Instantiate(particles[(int)elementType], transform);
            }
            
            GameObject weapon = Instantiate(weaponPrefab, transform);
            originalWeaponScale = weapon.transform.localScale;
            weapon.transform.localPosition = new Vector3(0, -1.0f, 0);
            weapon.transform.localRotation = Quaternion.identity;
            weapon.transform.localScale = Vector3.one;
        }

        private void Update()
        {
        }    
    }
}