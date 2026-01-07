using UnityEngine;

namespace Item.Weapon
{
    public class DropWeapon : MonoBehaviour
    {
        public GameObject weaponPrefab;

        private void Start()
        {
            GameObject weapon = Instantiate(weaponPrefab, transform);

            weapon.transform.localPosition = Vector3.zero;
            weapon.transform.localRotation = Quaternion.identity;
        }

        private void Update()
        {
            transform.Rotate(Vector3.up * 20 * Time.deltaTime);
        }
    }
}
