using Actor;
using Actor.Item;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class ActorManager : NetworkBehaviour
{
    public static ActorManager Instance { get; private set; }

    private Actor.Player.PlayerController ownerPlayer;

    public Actor.DuelRoom currentDuelRoom;

    [Header("DuelScene")]
    public GameObject droppedItemSpawnerPrefab;
    private Actor.Item.DroppedItemSpawner droppedItemSpawner;

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
        
    }

    public void OnSceneLoaded()
    {
        if (Util.SceneChecker.CheckCurrnetScene(Util.SceneList.DuelScene))
        {
            if (IsServer)
            {
                GameObject spawner = Instantiate(droppedItemSpawnerPrefab, Vector3.zero, Quaternion.identity);
                
                NetworkObject netObj = spawner.GetComponent<NetworkObject>();
                netObj.Spawn();

                droppedItemSpawner = spawner.GetComponent<Actor.Item.DroppedItemSpawner>();

                if (droppedItemSpawner != null)
                {
                    droppedItemSpawner.SubmitGenerateRandomWeaponListServerRpc();
                    droppedItemSpawner.SpawnWeapons();
                }
            }
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
}
