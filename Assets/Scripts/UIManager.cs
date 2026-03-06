using Unity.Netcode;

public class UIManager : NetworkBehaviour
{
    public static UIManager Instance { get; private set; }

    private UI.PlayerUI playerUI;
    private UI.DuelLobbyScene.UIController duelLobbySceneUIController;

    public void Awake()
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
        if (Util.SceneChecker.CheckCurrnetScene(Util.SceneList.DuelLobbyScene))
        {
            duelLobbySceneUIController.UpdateStatusText();
        }
        else
        {

        }
    }

    public void OnSceneLoaded()
    {
        if (Util.SceneChecker.CheckCurrnetScene(Util.SceneList.DuelLobbyScene))
        {
            InitializeDuelLobbySceneUI();
        }
    }

    #region DuelLobbyScene
    public void SetDuelLobbySceneUIController(UI.DuelLobbyScene.UIController controller)
    {
        duelLobbySceneUIController = controller;
    }

    public void InitializeDuelLobbySceneUI()
    {
        if (duelLobbySceneUIController != null)
        {
            duelLobbySceneUIController.Initialize();
        }
    }
    #endregion

    public void SetDuelStartButtonInteractable(bool state)
    {
        duelLobbySceneUIController.SetDuelStartButtonInteractable(state);
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