using System;
using System.Collections;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using Allocation = Unity.Services.Relay.Models.Allocation;
using RelayService = Unity.Services.Relay.RelayService;


public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;

    public NetworkVariable<int> readyPlayerCount = new NetworkVariable<int>();

    public string CurrentScene { get; private set; }
    public bool CanStartDuel { get; private set; }

    const int m_MaxConnections = 4;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void InitializeBeforeScene()
    {
        EventManager.Init();
    }

    async void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            try
            {
                await UnityServices.InitializeAsync();
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                var playerID = AuthenticationService.Instance.PlayerId;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

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

    public static async Task<RelayServerData> AllocateRelayServerAndGetJoinCode(int maxConnections, string region = null)
    {
        Allocation allocation;
        string createJoinCode;
        try
        {
            allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections, region);
        }
        catch (Exception e)
        {
            Debug.LogError($"Relay create allocation request failed {e.Message}");
            throw;
        }

        Debug.Log($"server: {allocation.ConnectionData[0]} {allocation.ConnectionData[1]}");
        Debug.Log($"server: {allocation.AllocationId}");

        try
        {
            createJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
        }
        catch
        {
            Debug.LogError("Relay create join code request failed");
            throw;
        }

        UIManager.Instance.SetStatusText(createJoinCode);

        return new RelayServerData(allocation, "dtls");
    }

    public IEnumerator ConfigureTransportAndStartNgoAsHost()
    {
        var serverRelayUtilityTask = AllocateRelayServerAndGetJoinCode(m_MaxConnections);
        while (!serverRelayUtilityTask.IsCompleted)
        {
            yield return null;
        }
        if (serverRelayUtilityTask.IsFaulted)
        {
            Debug.LogError("Exception thrown when attempting to start Relay Server. Server not started. Exception: " + serverRelayUtilityTask.Exception.Message);
            yield break;
        }

        var relayServerData = serverRelayUtilityTask.Result;

        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
        NetworkManager.Singleton.StartHost();
        yield return null;
    }

    public static async Task<RelayServerData> JoinRelayServerFromJoinCode(string joinCode)
    {
        JoinAllocation allocation;
        try
        {
            joinCode = joinCode.Trim().ToUpper();

            allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
        }
        catch
        {
            Debug.LogError("Relay create join code request failed");
            throw;
        }

        Debug.Log($"client: {allocation.ConnectionData[0]} {allocation.ConnectionData[1]}");
        Debug.Log($"host: {allocation.HostConnectionData[0]} {allocation.HostConnectionData[1]}");
        Debug.Log($"client: {allocation.AllocationId}");

        return new RelayServerData(allocation, "dtls");
    }

    public IEnumerator ConfigureTransportAndStartNgoAsConnectingPlayer(string relayJoinCode)
    {
        var clientRelayUtilityTask = JoinRelayServerFromJoinCode(relayJoinCode);

        while (!clientRelayUtilityTask.IsCompleted)
        {
            yield return null;
        }

        if (clientRelayUtilityTask.IsFaulted)
        {
            Debug.LogError("Exception thrown when attempting to connect to Relay Server. Exception: " + clientRelayUtilityTask.Exception.Message);
            yield break;
        }

        var relayServerData = clientRelayUtilityTask.Result;

        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

        NetworkManager.Singleton.StartClient();
        yield return null;
    }
}
