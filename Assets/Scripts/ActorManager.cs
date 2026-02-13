using System.Globalization;
using Unity.Netcode;
using UnityEngine;

public class ActorManager : NetworkBehaviour
{
    public static ActorManager Instance { get; private set; }

    [Header("DuelLobbyScene")]
    public Transform[] lobbyPlayers;

    [Header("DuelScene")]
    public Actor.Item.DroppedItemSpawner droppedItemSpawner;
    public NetworkList<int> WeaponIndexList = new NetworkList<int>();
    public NetworkList<Vector3> WeaponPositionList = new NetworkList<Vector3>();

    private int playerCount = 0;

    private Actor.Player.PlayerController[] playerList = new Actor.Player.PlayerController[4];

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            Initialize();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Initialize()
    {
        WeaponIndexList = new NetworkList<int>();
        WeaponPositionList = new NetworkList<Vector3>();
    }

    public Transform GetLobbyPlayerTransform()
    {
        int index = playerCount;
        return lobbyPlayers[index];
    }

    public int GetPlayerCount()
    {
        return playerCount;
    }

    public void AddPlayer(Actor.Player.PlayerController player)
    {
        playerList[playerCount] = player;
        playerCount++;
        
    }

    public int GetCurrentPlayerCount()
    {
        return playerCount;
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

    public void DropItemsServer()
    {
        if(droppedItemSpawner != null)
        {
            droppedItemSpawner.InitializeWeaponListsRandomly();
            droppedItemSpawner.SpawnDroppedWeapons();
            SubmitDroppedWeaponsInfoServerRpc(droppedItemSpawner.WeaponIndexList, droppedItemSpawner.WeaponPositionList);
        }
    }

    public void DropItemsClinet()
    {
        //int[] indexList = GetWeaponIndexList();
        //Vector3[] positionList = GetWeaponPositionList();

        //droppedItemSpawner.InitializeWeaponLists(indexList, positionList);
        //droppedItemSpawner.SpawnDroppedWeapons();
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

    public void OnSceneLoaded()
    {
        foreach(var player in playerList)
        {
            if(player != null)
            {
                player.OnSceneLoaded();
            }
        }
    }
}
