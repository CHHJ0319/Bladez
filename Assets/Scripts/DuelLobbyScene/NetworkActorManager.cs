using Unity.Netcode;
using UnityEngine;

namespace DualLobbyScene
{
    public class NetworkActorManager : NetworkBehaviour
    {
        public NetworkList<int> WeaponIndexList = new NetworkList<int>();
        public NetworkList<Vector3> WeaponPositionList = new NetworkList<Vector3>();

        private NetworkVariable<int> playerCount = new NetworkVariable<int>();

        void Awake()
        {
            WeaponIndexList = new NetworkList<int>();
            WeaponPositionList = new NetworkList<Vector3>();

            playerCount.Value = 0;
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

        public int GetPlayerCount()
        {
            return playerCount.Value;
        }

        public void AddPlayer()
        {
            playerCount.Value++;
        }

        [Rpc(SendTo.Server)]
        public void RequestAddPlayerServerRpc()
        {
            AddPlayer();
        }
    }

}
