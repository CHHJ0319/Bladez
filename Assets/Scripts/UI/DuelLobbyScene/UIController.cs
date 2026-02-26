using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace UI.DuelLobbyScene
{
    public class UIController : MonoBehaviour
    {
        public Transform networkPanel;
        private Button hostButton;
        private TMP_InputField joinCodeInputfiled;
        private Button clientButton;

        public TMP_Text joinCode;
        public Button duelStartButton;

        public TMP_Text statusText;

        private bool isDuelReady = false;

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
                //duelStartButton.interactable = false;

                joinCode.gameObject.SetActive(true);
            }
            else
            {
                buttonTitle.text = "Ready";
            }
        }

        public void UpdateUI()
        {
            if (NetworkManager.Singleton == null)
            {
                SetActiveNetworkPanel(false);
                SetStatusText("NetworkManager not found");
                return;
            }

            if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
            {
                SetActiveNetworkPanel(true);
                SetStatusText("Not connected");
            }
            else
            {
                SetActiveNetworkPanel(false);
                UpdateStatusLabels();
            }
        }

        public void SetJoinCode(string code)
        {
            if (joinCode != null) joinCode.text = code;
        }

        public void SetDuelStartButtonInteractable(bool state)
        {
            if (duelStartButton.GetComponentInChildren<TextMeshProUGUI>().text == "Start")
            {
                duelStartButton.interactable = state;
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

        private void SetActiveNetworkPanel(bool isActive)
        {
            networkPanel.gameObject.SetActive(isActive);
        }

        private void OnDuelStartButtonClicked(bool isDuelHost)
        {
            if (isDuelHost)
            {
                GameManager.Instance.RequestStartDuelServerRpc("DuelScene");
            }
            else
            {
                if (isDuelReady)
                {
                    duelStartButton.GetComponentInChildren<TextMeshProUGUI>().text = "Ready";
                    isDuelReady = false;
                }
                else
                {
                    duelStartButton.GetComponentInChildren<TextMeshProUGUI>().text = "Cancel";
                    isDuelReady = true;
                }

                GameManager.Instance.SubmitReadyPlayerServerRpc(isDuelReady);
            }
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
}
