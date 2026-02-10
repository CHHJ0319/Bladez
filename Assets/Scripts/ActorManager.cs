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

    public void DropItems()
    {
        droppedItemSpawner.SpawnWeapon();
    }
}
