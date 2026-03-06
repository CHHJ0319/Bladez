using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ActorManager : NetworkBehaviour
{
    public static ActorManager Instance { get; private set; }

    public GameObject duelRoomPrefab;
    private GameObject currentDuelRoom;
    
    [Header("DuelScene")]
    public GameObject[] weaponList;
    public NetworkList<int> WeaponIndexList = new NetworkList<int>();
    public NetworkList<Vector3> WeaponPositionList = new NetworkList<Vector3>();

    private Actor.Item.DroppedItemSpawner droppedItemSpawner;

    private List<Actor.Player.PlayerController> playerList = new List<Actor.Player.PlayerController>();

    private int droppedWeaponCount = 10;

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

    public void OnSceneLoaded()
    {
        foreach (var player in playerList)
        {
            if (player != null)
            {
                player.OnSceneLoaded();
            }
        }

        if (Util.SceneChecker.CheckCurrnetScene(Util.SceneList.DuelScene))
        {
            if (WeaponIndexList.Count == 0)
            {
                SubmitGenerateRandomWeaponListServerRpc();
            }
            SpawnWeapon();
        }
    }

    #region DuelLobbyScene
    public void CreateDuelRoom()
    {
        currentDuelRoom = Instantiate(duelRoomPrefab); 
    }

    public Transform GetDuelLobbyPlayerTransform()
    {
        int index = GetPlayerCount();
        return currentDuelRoom.GetComponent<Actor.DuelLobbyScene.DuelRoom>().GetDuelLobbyPlayerTransform(index);
    }
    #endregion

    public int GetPlayerCount()
    {
        return playerList.Count;
    }

    public void AddPlayer(Actor.Player.PlayerController player)
    {
        playerList.Add(player);
        
    }
    
    public void GenerateRandomWeaponList()
    {
        for (int i = 0; i < droppedWeaponCount; i++)
        {
            int randomIndex = 0;
            int randomRange = Random.Range(0, 100);
            if(randomRange <= 60)
            {
                randomIndex = 0;
            }
            else if(randomRange <= 75)
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

    private Vector3 GetRandomWeaponPosition(Vector3 center = default, float range = 30.0f, float fixedY = 0.3f)
    {
        Vector2 randomPoint = Random.insideUnitCircle * range;

        return new Vector3(center.x + randomPoint.x, fixedY, center.z + randomPoint.y);
    }

    [Rpc(SendTo.Server)]
    private void SubmitGenerateRandomWeaponListServerRpc(RpcParams rpcParams = default)
    {
        GenerateRandomWeaponList();
    }

    private void SpawnWeapon()
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

            GameObject newWeapon = weaponList[index];
            droppedItemSpawner.SpawnDroppedWeapons(newWeapon, position);
        }
    }

    public void SetDroppedItemSpawner(Actor.Item.DroppedItemSpawner spawner)
    {
        droppedItemSpawner = spawner;
    }
}
