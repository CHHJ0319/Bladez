using Actor.Item.Weapon;
using UnityEngine;

namespace Actor.Item
{
    public class DroppedItemSpawner : MonoBehaviour
    {
        public GameObject droppedWeaponPrefab;
        
        public GameObject[] weaponList;

        private int droppedWeaponCount = 10;

        public void SpawnWeapon()
        {
            if (weaponList == null || weaponList.Length == 0)
            {
                return;
            }

            for (int i = 0; i < droppedWeaponCount; i++)
            {
                int randomIndex = Random.Range(0, weaponList.Length);
                GameObject newWeapon = weaponList[randomIndex];
                
                GameObject droppedWeapon = Instantiate(droppedWeaponPrefab, transform);
                droppedWeapon.GetComponent<DroppedWeapon>().weaponPrefab = newWeapon;
                droppedWeapon.GetComponent<DroppedWeapon>().Initialize();
                droppedWeapon.transform.localPosition = GetRandomPosition();
            }
        }

        public Vector3 GetRandomPosition(Vector3 center = default, float range = 30.0f, float fixedY = 0.3f)
        {
            Vector2 randomPoint = Random.insideUnitCircle * range;

            return new Vector3(center.x + randomPoint.x, fixedY, center.z + randomPoint.y);
        }
    }
}