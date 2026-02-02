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
    public Button moveButton;

    [Header("Status")]
    public TMP_Text statusText;

    void Start()
    {
        hostButton.onClick.AddListener(OnHostButtonClicked);
        clientButton.onClick.AddListener(OnClientButtonClicked);
        serverButton.onClick.AddListener(OnServerButtonClicked);

        moveButton.onClick.AddListener(SubmitNewPosition);
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
            SetMoveButton(false);
            SetStatusText("NetworkManager not found");
            return;
        }

        if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
        {
            SetStartButtons(true);
            SetMoveButton(false);
            SetStatusText("Not connected");
        }
        else
        {
            SetStartButtons(false);
            SetMoveButton(true);
            UpdateStatusLabels();
        }
    }

    void SetStartButtons(bool state)
    {
        hostButton.gameObject.SetActive(state);
        clientButton.gameObject.SetActive(state);
        serverButton.gameObject.SetActive(state);
    }

    void SetMoveButton(bool state)
    {
        moveButton.gameObject.SetActive(state);
        if (state)
        {
            TMP_Text btnText = moveButton.GetComponentInChildren<TMP_Text>();
            if (btnText != null)
            {
                btnText.text = NetworkManager.Singleton.IsServer ? "Move" : "Request Position Change";
            }
        }
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