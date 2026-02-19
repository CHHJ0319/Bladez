using Unity.Netcode;
using UnityEngine;

public class ActorManager : NetworkBehaviour
{
    public static ActorManager Instance { get; private set; }

    [Header("DuelLobbyScene")]
    public Transform[] lobbyPlayers;

    [Header("DuelScene")]
    public GameObject[] weaponList;
    public NetworkList<int> WeaponIndexList = new NetworkList<int>();
    public NetworkList<Vector3> WeaponPositionList = new NetworkList<Vector3>();

    private Actor.Item.DroppedItemSpawner droppedItemSpawner;

    private int playerCount = 0;

    private Actor.Player.PlayerController[] playerList = new Actor.Player.PlayerController[4];

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

    
    public void GenerateRandomWeaponList()
    {
        for (int i = 0; i < droppedWeaponCount; i++)
        {
            int randomIndex = Random.Range(0, weaponList.Length);
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

    public void OnSceneLoaded()
    {
        foreach(var player in playerList)
        {
            if(player != null)
            {
                player.OnSceneLoaded();
            }
        }

        if (GameManager.Instance.CurrentScene == "DuelScene")
        {
            if (WeaponIndexList.Count == 0)
            {
                SubmitGenerateRandomWeaponListServerRpc();
            }
            SpawnWeapon();
        }
    }
}
