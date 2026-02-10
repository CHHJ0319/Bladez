using Actor.Item;
using UnityEngine;

public class ActorManager : MonoBehaviour
{
    public static ActorManager Instance;

    public DroppedItemSpawner droppedItemSpawner;

    private NetworkActorManager networkActorManager;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            networkActorManager = GetComponent<NetworkActorManager>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void DropItemsServer()
    {
        droppedItemSpawner.InitializeWeaponListsRandomly();
        droppedItemSpawner.SpawnDroppedWeapons();
        networkActorManager.SubmitDroppedWeaponsInfoServerRpc(droppedItemSpawner.WeaponIndexList, droppedItemSpawner.WeaponPositionList);
    }

    public void DropItemsClinet()
    {
        //int[] indexList = networkActorManager.GetWeaponIndexList();
        //Vector3[] positionList = networkActorManager.GetWeaponPositionList();

        //droppedItemSpawner.InitializeWeaponLists(indexList, positionList);
        //droppedItemSpawner.SpawnDroppedWeapons();
    }
}
