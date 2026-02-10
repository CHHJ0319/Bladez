using Actor.Item.Weapon;
using System.Xml.Serialization;
using UnityEngine;

namespace Actor.Item
{
    public class DroppedItemSpawner : MonoBehaviour
    {
        public GameObject droppedWeaponPrefab;
        public GameObject[] weaponList;

        public int[] WeaponIndexList { get; private set; }
        public Vector3[] WeaponPositionList { get; private set; }

        private int droppedWeaponCount = 10;

        private void Awake()
        {
            WeaponIndexList = new int[droppedWeaponCount];
            WeaponPositionList = new Vector3[droppedWeaponCount];
        }

        public void SpawnDroppedWeapons()
        {
            if (weaponList == null || weaponList.Length == 0)
            {
                return;
            }

            for (int i = 0; i < droppedWeaponCount; i++)
            {
                GameObject newWeapon = weaponList[WeaponIndexList[i]];
                
                GameObject droppedWeapon = Instantiate(droppedWeaponPrefab, transform);
                droppedWeapon.GetComponent<DroppedWeapon>().weaponPrefab = newWeapon;
                droppedWeapon.GetComponent<DroppedWeapon>().Initialize();
                droppedWeapon.transform.localPosition = WeaponPositionList[i];
            }
        }

        public void InitializeWeaponListsRandomly()
        {
            for (int i = 0; i < droppedWeaponCount; i++)
            {
                int randomIndex = Random.Range(0, weaponList.Length);
                WeaponIndexList[i] = randomIndex;
                WeaponPositionList[i] = GetRandomPosition();
            }
        }

        public void InitializeWeaponLists(int[] indexList, Vector3[] positionList)
        {
            WeaponIndexList = (int[])indexList.Clone();
            WeaponPositionList = (Vector3[])positionList.Clone();
        }

        private Vector3 GetRandomPosition(Vector3 center = default, float range = 30.0f, float fixedY = 0.3f)
        {
            Vector2 randomPoint = Random.insideUnitCircle * range;

            return new Vector3(center.x + randomPoint.x, fixedY, center.z + randomPoint.y);
        }
    }
}