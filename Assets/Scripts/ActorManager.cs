using Actor;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ActorManager : NetworkBehaviour
{
    public static ActorManager Instance { get; private set; }

    private Actor.Player.PlayerController ownerPlayer;

    public GameObject duelRoomPrefab;
    public Actor.DuelRoom currentDuelRoom;
    
    [Header("DuelScene")]
    public GameObject[] weaponList;
    public NetworkList<int> WeaponIndexList = new NetworkList<int>();
    public NetworkList<Vector3> WeaponPositionList = new NetworkList<Vector3>();

    private Actor.Item.DroppedItemSpawner droppedItemSpawner;
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

    public override void OnNetworkSpawn()
    {

    }

    public override void OnNetworkDespawn()
    {

    }

    private void Initialize()
    {
        WeaponIndexList = new NetworkList<int>();
        WeaponPositionList = new NetworkList<Vector3>();
    }

    public void OnSceneLoaded()
    {
        if (Util.SceneChecker.CheckCurrnetScene(Util.SceneList.DuelScene))
        {
            if (WeaponIndexList.Count == 0)
            {
                SubmitGenerateRandomWeaponListServerRpc();
            }
            SpawnWeapon();
        }
    }

    public void SetOwnerPlayer(Actor.Player.PlayerController player)
    {
        ownerPlayer = player;
    }

    #region DuelLobbyScene
    public void SetDuelRoom(Actor.DuelRoom room)
    {
        currentDuelRoom = room; 
    }

    public Transform GetDuelLobbyPlayerTransform()
    {
        int index = DuelManager.Instance.GetPlayerCount();
        return currentDuelRoom.GetDuelLobbyPlayerTransform(index);
    }
    #endregion

    #region WeaponSpawner
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
    #endregion

    private Vector3 GetRandomWeaponPosition(Vector3 center = default, float range = 30.0f, float fixedY = 0.3f)
    {
        Vector2 randomPoint = Random.insideUnitCircle * range;

        return new Vector3(center.x + randomPoint.x, fixedY, center.z + randomPoint.y);
    }
}
