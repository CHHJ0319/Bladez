using Actor.Item.Weapon;
using System.Xml.Serialization;
using Unity.Netcode;
using UnityEngine;

namespace Actor.Item
{
    public class DroppedItemSpawner : MonoBehaviour
    {
        public GameObject droppedWeaponPrefab;

        public GameObject[] weaponList;

        private void Awake()
        {
            
        }

        private void Start()
        {
            ActorManager.Instance.SetDroppedItemSpawner(this);
        }

        public void SpawnDroppedWeapons(int index, Vector3 pos )
        {
            GameObject newWeapon = weaponList[index];
            GameObject droppedWeapon = Instantiate(droppedWeaponPrefab, transform);
            droppedWeapon.GetComponent<DroppedWeapon>().weaponPrefab = newWeapon;
            droppedWeapon.GetComponent<DroppedWeapon>().Initialize();
            droppedWeapon.transform.localPosition = pos;
        }
    }
}