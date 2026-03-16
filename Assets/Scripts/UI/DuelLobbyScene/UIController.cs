using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace UI.DuelLobbyScene
{
    public class UIController : MonoBehaviour
    {
        public Transform duelLobbyUI;
        public Button duelStartButton;
        public TMP_Text joinCode;

        public TMP_Text statusText;

        [Header("Network Panel")]
        public Transform networkPanel;
        public TMP_InputField joincodeInputField;
        public Button accessButton;

        private bool isDuelHost = false;
        private bool isDuelReady = false;

        void Awake()
        {
            Initialize();
        }

        private void Start()
        {
            UIManager.Instance.SetDuelLobbySceneUIController(this);
        }

        public void Initialize()
        {
            accessButton.onClick.AddListener(OnAccessButtonClicked);
        }

        public void SetDuelHostUI(string joinCode)
        {
            this.joinCode.gameObject.SetActive(true);
            this.joinCode.text = joinCode;

            duelStartButton.gameObject.SetActive(true);
            TextMeshProUGUI duelStartButtonText = duelStartButton.GetComponentInChildren<TextMeshProUGUI>();
            duelStartButtonText.text = "Start";
            duelStartButton.interactable = false;

            duelStartButton.onClick.AddListener(OnDuelStartButtonClicked);

            isDuelHost = true;
        }

        public void SetDuelClientUI()
        {
            duelStartButton.gameObject.SetActive(true);
            TextMeshProUGUI duelStartButtonText = duelStartButton.GetComponentInChildren<TextMeshProUGUI>();
            duelStartButtonText.text = "Ready";

            duelStartButton.onClick.AddListener(OnDuelReadyButtonClicked);
        }

        public void UpdateUI()
        {
            if (isDuelHost)
            {
                if (DuelManager.Instance.AreAllPlayersReady())
                {
                    duelStartButton.interactable = true;
                }
                else
                {
                    duelStartButton.interactable = false;
                }
            }

            UpdateStatusText();
        }

        #region StatusText
        public void UpdateStatusText()
        {
            if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
            {
                SetStatusText("Not connected");
            }
            else
            {
                UpdateStatusLabels();
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
        #endregion

        public void SetDuelStartButtonInteractable(bool state)
        {
            if (duelStartButton.GetComponentInChildren<TextMeshProUGUI>().text == "Start")
            {
                duelStartButton.interactable = state;
            }
        }

        private void OnDuelStartButtonClicked()
        {
            GameManager.Instance.RequestStartDuelServerRpc("DuelScene");
        }

        private void OnDuelReadyButtonClicked()
        {
            TextMeshProUGUI duelStartButtonText = duelStartButton.GetComponentInChildren<TextMeshProUGUI>();
            if(isDuelReady)
            {
                DuelManager.Instance.RequestDuelReadyServerRpc(false);

                duelStartButtonText.text = "Ready";
                isDuelReady = false;
            }
            else
            {
                DuelManager.Instance.RequestDuelReadyServerRpc(true);

                duelStartButtonText.text = "Cancel";
                isDuelReady = true;
            }
        }

        private void OnAccessButtonClicked()
        {
            string joinCode = joincodeInputField.text;

            networkPanel.gameObject.SetActive(false);

            if (string.IsNullOrWhiteSpace(joinCode))
            {
                GameManager.Instance.StartHost();
            }
            else
            {
                GameManager.Instance.StartClient(joinCode);
            }
        }
    }
}
