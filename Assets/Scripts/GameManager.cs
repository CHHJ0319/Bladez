using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;

    public NetworkVariable<int> readyPlayerCount = new NetworkVariable<int>();

    public string CurrentScene { get; private set; }
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

            CurrentScene = SceneManager.GetActiveScene().name;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public override void OnNetworkSpawn()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;

        readyPlayerCount.OnValueChanged += OnReadyPlayerCountChanged;
    }

    public override void OnNetworkDespawn()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        readyPlayerCount.OnValueChanged -= OnReadyPlayerCountChanged;
    }

    [Rpc(SendTo.Server)]
    public void RequestStartGameServerRpc(string sceneName)
    {
        if (IsServer)
        {
            NetworkManager.Singleton.SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        }
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

    private void OnReadyPlayerCountChanged(int previous, int current)
    {
        if (readyPlayerCount.Value == ActorManager.Instance.GetPlayerCount() - 1)
        {
            CanStartDuel = true;
        }
        else
        {
            CanStartDuel = false;
        }
    }

    public string GetCurrentScene()
    {
        return CurrentScene;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        CurrentScene = SceneManager.GetActiveScene().name;

        ActorManager.Instance.OnSceneLoaded();
    }
}
