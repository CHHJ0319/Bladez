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
    public TMP_InputField joinCodeInputfiled;
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
        //serverButton.onClick.AddListener(OnServerButtonClicked);
    }

    void Update()
    {
        if (GameManager.Instance.CurrentScene == "DuelLobbyScene")
        {
            UpdateUI();
        }
    }

    public void UpdatePlayerHPBar(float hp, float maxHP)
    {
        if (playerUI != null)
        {
            playerUI.UpdateHPBar(hp, maxHP);
        }
    }

    public void InitializePlayerUI(bool isDuelHost)
    {
        if (playerUI != null)
        {
            playerUI.Initialize(isDuelHost);
        }
    }

    public void ShowPlayerResultUI(bool isWinner)
    {
        if (isWinner)
        {
            playerUI.ShowWinUI();
        }
        else
        {
            playerUI.ShowDefeatUI();
        }
    }

    private void OnHostButtonClicked()
    {
        StartCoroutine(GameManager.Instance.ConfigureTransportAndStartNgoAsHost());
    }

    private void OnClientButtonClicked()
    {
        if (joinCodeInputfiled.text == "")
            return;

        string joinCode = joinCodeInputfiled.text;
        StartCoroutine(GameManager.Instance.ConfigureTransportAndStartNgoAsConnectingPlayer(joinCode));
    }

    private void OnServerButtonClicked()
    {
        NetworkManager.Singleton.StartServer();
    }

    private void UpdateUI()
    {
        if (NetworkManager.Singleton == null)
        {
            SetNetworkButtons(false);
            //SetStatusText("NetworkManager not found");
            return;
        }

        if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
        {
            SetNetworkButtons(true);
            //SetStatusText("Not connected");
        }
        else
        {
            SetNetworkButtons(false);
            //UpdateStatusLabels();
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
        //serverButton.gameObject.SetActive(state);

        joinCodeInputfiled.gameObject.SetActive(state);
    }

    public void SetStatusText(string text)
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

    

    public void SetPlayerUI(UI.PlayerUI playerUI)
    {
        this.playerUI = playerUI;
    }
}