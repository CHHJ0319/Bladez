using System.Collections;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;

    private const int m_MaxConnections = 4;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            Initialize();

            Util.NetworkService.InitializeUnityServicesAsync();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    #region Initialize
    private void Initialize()
    {
        ClearEvents();
    }

    private void ClearEvents()
    {
        Events.PlayerEvents.Clear();
    }
    #endregion

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ActorManager.Instance.OnSceneLoaded();
        UIManager.Instance.OnSceneLoaded();
    }

    [Rpc(SendTo.Server)]
    public void RequestStartDuelServerRpc(string sceneName)
    {
        NetworkManager.Singleton.SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }

    #region Network Service
    public void StartHost()
    {
        StartCoroutine(GameManager.Instance.ConfigureTransportAndStartNgoAsHost());
    }

    public void StartClient(string joinCode)
    {
        StartCoroutine(GameManager.Instance.ConfigureTransportAndStartNgoAsConnectingPlayer(joinCode));
    }

    private IEnumerator ConfigureTransportAndStartNgoAsHost()
    {
        var serverRelayUtilityTask = Util.NetworkService.AllocateRelayServerAndGetJoinCode(m_MaxConnections);
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

        yield return new WaitForSeconds(2.0f);
    }

    private IEnumerator ConfigureTransportAndStartNgoAsConnectingPlayer(string relayJoinCode)
    {
        var clientRelayUtilityTask = Util.NetworkService.JoinRelayServerFromJoinCode(relayJoinCode);

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

        yield return new WaitForSeconds(2.0f);
    }
    #endregion
}
