using Actor.Item.Weapon;
using System.Xml.Serialization;
using Unity.Netcode;
using UnityEngine;

namespace Actor.Item
{
    public class DroppedItemSpawner : NetworkBehaviour
    {
        private int droppedWeaponCount = 10;

        public NetworkList<int> WeaponIndexList = new NetworkList<int>();
        public NetworkList<Vector3> WeaponPositionList = new NetworkList<Vector3>();

        public GameObject[] weaponList;

        private void Awake()
        {
            WeaponIndexList = new NetworkList<int>();
            WeaponPositionList = new NetworkList<Vector3>();
        }

        private void GenerateRandomWeaponList()
        {
            for (int i = 0; i < droppedWeaponCount; i++)
            {
                int randomIndex = 0;
                int randomRange = Random.Range(0, 100);
                if (randomRange <= 60)
                {
                    randomIndex = 0;
                }
                else if (randomRange <= 75)
                {
                    randomIndex = 1;
                }
                else if (randomRange <= 90)
                {
                    randomIndex = 2;
                }
                else
                {
                    randomIndex = 3;
                }

                Vector3 randomPosition = GetRandomWeaponPosition();
                WeaponIndexList.Add(randomIndex);
                WeaponPositionList.Add(randomPosition);
            }
        }

        [Rpc(SendTo.Server)]
        public void SubmitGenerateRandomWeaponListServerRpc(RpcParams rpcParams = default)
        {
            GenerateRandomWeaponList();
        }

        public void SpawnWeapons()
        {
            if (WeaponIndexList == null || WeaponIndexList.Count == 0)
            {
                return;
            }

            if (WeaponPositionList == null || WeaponPositionList.Count == 0)
            {
                return;
            }

            for (int i = 0; i < droppedWeaponCount; i++)
            {
                int index = WeaponIndexList[i];
                Vector3 position = WeaponPositionList[i];

                SubmitSpawnDroppedWeaponsServerRpc(index, position);
            }
        }

        [Rpc(SendTo.Server)]
        public void SubmitSpawnDroppedWeaponsServerRpc(int index, Vector3 pos )
        {
            GameObject newWeapon = Instantiate(weaponList[index], pos, Quaternion.identity);

            NetworkObject netObj = newWeapon.GetComponent<NetworkObject>();
            if (netObj != null && !netObj.IsSpawned)
            {
                netObj.Spawn();
            }

            newWeapon.GetComponent<Actor.Item.Weapon.Melee.SwordController>().SubmitItemDroppedServerRpc();
        }

        private Vector3 GetRandomWeaponPosition(Vector3 center = default, float range = 30.0f, float fixedY = 0.3f)
        {
            Vector2 randomPoint = Random.insideUnitCircle * range;

            return new Vector3(center.x + randomPoint.x, fixedY, center.z + randomPoint.y);
        }
    }
}