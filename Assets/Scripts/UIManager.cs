using TMPro;
using UI;
using Unity.Netcode;
using UnityEngine;

public class UIManager : NetworkBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Status")]
    public TMP_Text statusText;

    private PlayerUI playerUI;
    private UI.DuelLobbyScene.UIController duelLobbySceneUIController;


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

    void Update()
    {
        if (GameManager.Instance.CurrentScene == "DuelLobbyScene")
        {
            UpdateDuelLobbySceneUI();
        }
        else
        {
            playerUI.UpdateUI();
        }
    }

    public void UpdatePlayerHPBar(float hp, float maxHP)
    {
        if (playerUI != null)
        {
            playerUI.UpdateHPBar(hp, maxHP);
        }
    }

    public void SetDuelLobbySceneUIController(UI.DuelLobbyScene.UIController controller)
    {
        duelLobbySceneUIController = controller;
    }

    public void InitializDuelLobbySceneUI(bool isDuelHost)
    {
        if (duelLobbySceneUIController != null)
        {
            duelLobbySceneUIController.Initialize(isDuelHost);
        }
    }

    public void InitializePlayerUI()
    {
        if (playerUI != null)
        {
            playerUI.Initialize();
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

    private void UpdateDuelLobbySceneUI()
    {
        if (NetworkManager.Singleton == null)
        {
            duelLobbySceneUIController.UpdateUI(false);
            SetStatusText("NetworkManager not found");
            return;
        }

        if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
        {
            duelLobbySceneUIController.UpdateUI(true);
            SetStatusText("Not connected");
        }
        else
        {
            duelLobbySceneUIController.UpdateUI(false);
            UpdateStatusLabels();
        }
    }

    private void SetStatusText(string text)
    {
        if (statusText != null) statusText.text = text;
    }

    public void SetJoinCode(string code)
    {
        duelLobbySceneUIController.SetJoinCode(code);
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