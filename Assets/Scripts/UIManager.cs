using TMPro;
using UI;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
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

    private PlayerUI playerUI;
    

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
        if (playerUI != null && playerUI.hpBar != null)
        {
            playerUI.hpBar.UpdateGaugeBar(hp, maxHP);
        }
    }

    public void UpdateLobbyPlayerUI(bool isDuelHost)
    {
        playerUI.duelStartButton.onClick.AddListener(() => OnDuelStartButtonClicked(isDuelHost));

        var buttonTitle = playerUI.duelStartButton.GetComponentInChildren<TextMeshProUGUI>();
        if (isDuelHost)
        {

            buttonTitle.text = "Start";
            playerUI.duelStartButton.interactable = false;
        }
        else
        {
            buttonTitle.text = "Ready";
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

        if (playerUI.duelStartButton != null)
        {
            if (playerUI.duelStartButton.GetComponentInChildren<TextMeshProUGUI>().text == "Start")
            {
                if (GameManager.Instance.CanStartDuel)
                {
                    playerUI.duelStartButton.interactable = true;
                }
                else
                {
                    playerUI.duelStartButton.interactable = false;
                }
            }
        }
    }

    private void SetNetworkButtons(bool state)
    {
        if (SceneManager.GetActiveScene().name != "DuelLobbyScene") return;
        
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

    private void OnDuelStartButtonClicked(bool isDuelHost)
    {
        if(isDuelHost)
        {
            GameManager.Instance.RequestStartGameServerRpc("DuelScene");
        }
        else
        {
            if(playerUI.duelStartButton.GetComponentInChildren<TextMeshProUGUI>().text == "Ready")
            {
                GameManager.Instance.SubmitReadyPlayerServerRpc();
                playerUI.duelStartButton.GetComponentInChildren<TextMeshProUGUI>().text = "Cancel";
            }
            else
            {
                GameManager.Instance.SubmitUnReadyPlayerServerRpc();
                playerUI.duelStartButton.GetComponentInChildren<TextMeshProUGUI>().text = "Ready";
            }
        }
    }

    public void SetPlayerUI(UI.PlayerUI playerUI)
    {
        this.playerUI = playerUI;
    }
}