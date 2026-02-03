using Actor.Player;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Buttons")]
    public Button hostButton;
    public Button clientButton;
    public Button serverButton;

    [Header("Status")]
    public TMP_Text statusText;

    void Start()
    {
        hostButton.onClick.AddListener(OnHostButtonClicked);
        clientButton.onClick.AddListener(OnClientButtonClicked);
        serverButton.onClick.AddListener(OnServerButtonClicked);
    }

    void Update()
    {
        UpdateUI();
    }

    void OnHostButtonClicked()
    {
        NetworkManager.Singleton.StartHost();
    }
    void OnClientButtonClicked()
    {
        NetworkManager.Singleton.StartClient();
    }
    void OnServerButtonClicked()
    {
        NetworkManager.Singleton.StartServer();
    }

    void SubmitNewPosition()
    {
        if (NetworkManager.Singleton == null) return;

        if (NetworkManager.Singleton.IsServer && !NetworkManager.Singleton.IsClient)
        {
            foreach (ulong uid in NetworkManager.Singleton.ConnectedClientsIds)
            {
                var playerObject = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(uid);
                //var player = playerObject.GetComponent<NetworkPlayerController>();
                //player.Move();
            }
        }
        else if (NetworkManager.Singleton.IsClient)
        {
            var playerObject = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject();
            //var player = playerObject.GetComponent<NetworkPlayerController>();
            //player.Move();
        }
    }

    void UpdateUI()
    {
        if (NetworkManager.Singleton == null)
        {
            SetStartButtons(false);
            SetStatusText("NetworkManager not found");
            return;
        }

        if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
        {
            SetStartButtons(true);
            SetStatusText("Not connected");
        }
        else
        {
            SetStartButtons(false);
            UpdateStatusLabels();
        }
    }

    void SetStartButtons(bool state)
    {
        hostButton.gameObject.SetActive(state);
        clientButton.gameObject.SetActive(state);
        serverButton.gameObject.SetActive(state);
    }

    void SetStatusText(string text)
    {
        if (statusText != null) statusText.text = text;
    }

    void UpdateStatusLabels()
    {
        var mode = NetworkManager.Singleton.IsHost ? "Host" : NetworkManager.Singleton.IsServer ? "Server" : "Client";
        string transport = "Transport: " + NetworkManager.Singleton.NetworkConfig.NetworkTransport.GetType().Name;
        string modeText = "Mode: " + mode;
        SetStatusText($"{transport}\n{modeText}");
    }
}