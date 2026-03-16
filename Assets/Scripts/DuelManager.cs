using Unity.Netcode;
using UnityEngine.SceneManagement;

public class DuelManager : NetworkBehaviour
{
    public static DuelManager Instance;

    public NetworkVariable<int> readyPlayerCount = new NetworkVariable<int>();


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

    [Rpc(SendTo.Server)]
    public void RequestStartDuelServerRpc(string sceneName)
    {
        NetworkManager.Singleton.SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }

    [Rpc(SendTo.Server)]
    public void SubmitReadyPlayerServerRpc(bool isReady, RpcParams rpcParams = default)
    {
        if (isReady)
        {
            readyPlayerCount.Value++;
        }
        else
        {
            readyPlayerCount.Value--;
        }
    }

    private void OnReadyPlayerCountChanged(int previous, int current)
    {
        if (readyPlayerCount.Value == ActorManager.Instance.GetPlayerCount() - 1)
        {
            UIManager.Instance.SetDuelStartButtonInteractable(true);
        }
        else
        {
            UIManager.Instance.SetDuelStartButtonInteractable(false);
        }
    }
}
