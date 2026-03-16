using Unity.Netcode;

public class UIManager : NetworkBehaviour
{
    public static UIManager Instance { get; private set; }

    private UI.PlayerUI playerUI;
    private UI.TitleScene.UIController titleSceneUIController;
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
            duelLobbySceneUIController.UpdateUI();
        }
    }

    public void OnSceneLoaded()
    {

    }

    #region TitleScene
    public void SetTitleSceneUIController(UI.TitleScene.UIController controller)
    {
        titleSceneUIController = controller;
    }
    #endregion

    #region DuelLobbyScene
    public void SetDuelLobbySceneUIController(UI.DuelLobbyScene.UIController controller)
    {
        duelLobbySceneUIController = controller;
    }

    public void SetDuelHostUI(string joinCode)
    {
        duelLobbySceneUIController.SetDuelHostUI(joinCode);
    }

    public void SetDuelClientUI()
    {
        duelLobbySceneUIController.SetDuelClientUI();
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