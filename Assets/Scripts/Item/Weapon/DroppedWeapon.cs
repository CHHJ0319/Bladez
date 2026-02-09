using UnityEngine;

namespace Item.Weapon
{
    public class DroppedWeapon : MonoBehaviour
    {
        public GameObject weaponPrefab;

        private void Start()
        {
            GameObject weapon = Instantiate(weaponPrefab, transform);

            weapon.transform.localPosition = new Vector3(0, -1.0f, 0);
            weapon.transform.localRotation = Quaternion.identity;
        }

        private void Update()
        {
        }    
    }
}