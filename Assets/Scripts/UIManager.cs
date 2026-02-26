using TMPro;
using UI;
using Unity.Netcode;
using UnityEngine;

public class UIManager : NetworkBehaviour
{
    public static UIManager Instance { get; private set; }

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
            duelLobbySceneUIController.UpdateUI();
        }
        else
        {

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

    public void SetJoinCode(string code)
    {
        duelLobbySceneUIController.SetJoinCode(code);
    }

    public void SetPlayerUI(UI.PlayerUI playerUI)
    {
        this.playerUI = playerUI;
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
}