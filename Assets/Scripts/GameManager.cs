using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;

    public NetworkVariable<int> readyPlayerCount = new NetworkVariable<int>();

    public bool CanStartDuel { get; private set; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void InitializeBeforeScene()
    {
        EventManager.Init();
    }

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

    public override void OnNetworkSpawn()
    {
        readyPlayerCount.OnValueChanged += OnReadyPlayerCountChanged;
    }

    public override void OnNetworkDespawn()
    {
        readyPlayerCount.OnValueChanged -= OnReadyPlayerCountChanged;
    }

    [Rpc(SendTo.Server)]
    public void SubmitReadyPlayerServerRpc(RpcParams rpcParams = default)
    {
        readyPlayerCount.Value++;
    }

    [Rpc(SendTo.Server)]
    public void SubmitUnReadyPlayerServerRpc(RpcParams rpcParams = default)
    {
        readyPlayerCount.Value--;
    }

    [Rpc(SendTo.Server)]
    public void SubmitStartDuelServerRpc(RpcParams rpcParams = default)
    {
        int a = 1;
    }

    private void OnReadyPlayerCountChanged(int previous, int current)
    {
        if (readyPlayerCount.Value == ActorManager.Instance.PlayerCount.Value - 1)
        {
            CanStartDuel = true;
        }
        else
        {
            CanStartDuel = false;
        }
    }
}
