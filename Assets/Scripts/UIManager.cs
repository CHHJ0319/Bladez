using Actor.UI;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : NetworkBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Buttons")]
    public Button hostButton;
    public Button clientButton;
    public Button serverButton;

    [Header("Status")]
    public TMP_Text statusText;

    [Header("PlayerUI")]
    public Button StartButton;
    public GaugeBar hpBar;
    public GaugeBar staminaBar;

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

    public void UpdatePlayerHPBar(float hp, float maxHP)
    {
        if (hpBar != null)
        {
            hpBar.UpdateGaugeBar(hp, maxHP);
        }
    }

    private void OnHostButtonClicked()
    {
        NetworkManager.Singleton.StartHost();
        //ActorManager.Instance.DropItemsServer();
    }
    private void OnClientButtonClicked()
    {
        NetworkManager.Singleton.StartClient();
        //ActorManager.Instance.DropItemsClinet();
    }
    private void OnServerButtonClicked()
    {
        NetworkManager.Singleton.StartServer();
        //ActorManager.Instance.DropItemsServer();
    }

    private void UpdateUI()
    {
        if (NetworkManager.Singleton == null)
        {
            SetNetworkButtons(false);
            SetStatusText("NetworkManager not found");
            return;
        }

        if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
        {
            SetNetworkButtons(true);
            SetStatusText("Not connected");
        }
        else
        {
            SetNetworkButtons(false);
            UpdateStatusLabels();
        }
    }

    private void SetNetworkButtons(bool state)
    {
        hostButton.gameObject.SetActive(state);
        clientButton.gameObject.SetActive(state);
        serverButton.gameObject.SetActive(state);
    }

    private void SetStatusText(string text)
    {
        if (statusText != null) statusText.text = text;
    }

    private void UpdateStatusLabels()
    {
        var mode = NetworkManager.Singleton.IsHost ? "Host" : NetworkManager.Singleton.IsServer ? "Server" : "Client";
        string transport = "Transport: " + NetworkManager.Singleton.NetworkConfig.NetworkTransport.GetType().Name;
        string modeText = "Mode: " + mode;
        SetStatusText($"{transport}\n{modeText}");
    }
}