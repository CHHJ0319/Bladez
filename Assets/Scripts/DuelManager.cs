using System.Collections.Generic;
using Unity.Netcode;

public class DuelManager : NetworkBehaviour
{
    public static DuelManager Instance;

    public NetworkVariable<int> playerReadyCount = new NetworkVariable<int>();
    
    private List<Actor.Player.PlayerController> playerList = new List<Actor.Player.PlayerController>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddPlayer(Actor.Player.PlayerController player)
    {
        playerList.Add(player);

    }

    public int GetPlayerCount()
    {
        return playerList.Count;
    }

    public bool AreAllPlayersReady()
    {
        if (playerList.Count == 1) return false;
        return playerList.Count - 1 == playerReadyCount.Value;
    }

    [Rpc(SendTo.Server)]
    public void RequestDuelReadyServerRpc(bool isReady, RpcParams rpcParams = default)
    {
        if (isReady)
        {
            playerReadyCount.Value++;
        }
        else
        {
            playerReadyCount.Value--;
        }
    }
}
