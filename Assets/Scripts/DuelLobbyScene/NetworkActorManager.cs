using Unity.Netcode;
using UnityEngine;

namespace DualLobbyScene
{
    public class NetworkActorManager : NetworkBehaviour
    {
        public NetworkList<int> WeaponIndexList = new NetworkList<int>();
        public NetworkList<Vector3> WeaponPositionList = new NetworkList<Vector3>();

        void Awake()
        {
            WeaponIndexList = new NetworkList<int>();
            WeaponPositionList = new NetworkList<Vector3>();
        }

        [Rpc(SendTo.Server)]
        public void SubmitDroppedWeaponsInfoServerRpc(int[] indexList, Vector3[] positionList, RpcParams rpcParams = default)
        {
            foreach (var i in indexList)
            {
                WeaponIndexList.Add(i);
            }

            foreach (var pos in positionList)
            {
                WeaponPositionList.Add(pos);
            }
        }

        public int[] GetWeaponIndexList()
        {
            int[] indexList = new int[WeaponIndexList.Count];

            for (int i = 0; i < WeaponIndexList.Count; i++)
            {
                indexList[i] = WeaponIndexList[i];
            }

            return indexList;
        }

        public Vector3[] GetWeaponPositionList()
        {
            Vector3[] positionList = new Vector3[WeaponPositionList.Count];

            for (int i = 0; i < WeaponPositionList.Count; i++)
            {
                positionList[i] = WeaponPositionList[i];
            }

            return positionList;
        }
    }

}
