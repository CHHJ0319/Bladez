using Actor.Item.Weapon;
using System.Xml.Serialization;
using UnityEngine;

namespace Actor.Item
{
    public class DroppedItemSpawner : MonoBehaviour
    {
        public GameObject droppedWeaponPrefab;

        private void Awake()
        {
            ActorManager.Instance.SetDroppedItemSpawner(this);
        }

        public void SpawnDroppedWeapons(GameObject newWeapon, Vector3 pos )
        {
            GameObject droppedWeapon = Instantiate(droppedWeaponPrefab, transform);
            droppedWeapon.GetComponent<DroppedWeapon>().weaponPrefab = newWeapon;
            droppedWeapon.GetComponent<DroppedWeapon>().Initialize();
            droppedWeapon.transform.localPosition = pos;
        }
    }
}