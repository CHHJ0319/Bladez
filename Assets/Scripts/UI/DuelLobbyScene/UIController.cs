using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.DuelLobbyScene
{
    public class UIController : MonoBehaviour
    {
        [Header("Network UI")]
        public Transform networkPanel;

        private Button hostButton;
        private TMP_InputField joinCodeInputfiled;
        private Button clientButton;

        public TMP_Text joinCode;

        [Header("Duel UI")]
        public Button duelStartButton;

        private void Awake()
        {
            if(networkPanel != null)
            {
                hostButton = networkPanel.GetChild(0).GetComponent<Button>();
                joinCodeInputfiled = networkPanel.GetChild(1).GetComponent<TMP_InputField>();
                clientButton = networkPanel.GetChild(2).GetComponent<Button>();
            }
        }

        void Start()
        {
            hostButton.onClick.AddListener(OnHostButtonClicked);
            clientButton.onClick.AddListener(OnClientButtonClicked);
        
            UIManager.Instance.SetDuelLobbySceneUIController(this);
        }

        public void Initialize(bool isDuelHost)
        {
            duelStartButton.gameObject.SetActive(true);
            duelStartButton.onClick.AddListener(() => OnDuelStartButtonClicked(isDuelHost));

            var buttonTitle = duelStartButton.GetComponentInChildren<TextMeshProUGUI>();
            if (isDuelHost)
            {
                buttonTitle.text = "Start";
                duelStartButton.interactable = false;

                joinCode.gameObject.SetActive(true);
            }
            else
            {
                buttonTitle.text = "Ready";
            }
        }

        public void UpdateUI(bool state)
        {
            if (duelStartButton.GetComponentInChildren<TextMeshProUGUI>().text == "Start")
            {
                if (GameManager.Instance.CanStartDuel)
                {
                    duelStartButton.interactable = true;
                }
                else
                {
                    duelStartButton.interactable = false;
                }
            }

            SetActiveNetworkPanel(state);
        }

        public void SetJoinCode(string code)
        {
            if (joinCode != null) joinCode.text = code;
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

        private void SetActiveNetworkPanel(bool isActive)
        {
            networkPanel.gameObject.SetActive(isActive);
        }

        private void OnDuelStartButtonClicked(bool isDuelHost)
        {
            if (isDuelHost)
            {
                GameManager.Instance.RequestStartGameServerRpc("DuelScene");
            }
            else
            {
                if (duelStartButton.GetComponentInChildren<TextMeshProUGUI>().text == "Ready")
                {
                    GameManager.Instance.SubmitReadyPlayerServerRpc();
                    duelStartButton.GetComponentInChildren<TextMeshProUGUI>().text = "Cancel";
                }
                else
                {
                    GameManager.Instance.SubmitUnReadyPlayerServerRpc();
                    duelStartButton.GetComponentInChildren<TextMeshProUGUI>().text = "Ready";
                }
            }
        }
    }
}
