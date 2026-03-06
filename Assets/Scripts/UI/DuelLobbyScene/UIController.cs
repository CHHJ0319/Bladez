using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace UI.DuelLobbyScene
{
    public class UIController : MonoBehaviour
    {
        [Header("Network Panel")]
        public Transform networkPanel;
        public Button hostButton;
        public Button clientButton;

        [Header("Duel Lobby")]
        public Transform duelLobbyUI;
        public Button duelStartButton;

        public TMP_Text statusText;

        private bool isDuelReady = false;

        void Awake()
        {
            hostButton.onClick.AddListener(OnHostButtonClicked);
            //clientButton.onClick.AddListener(OnClientButtonClicked);

            UIManager.Instance.SetDuelLobbySceneUIController(this);
        }

        public void Initialize()
        {
            SetActiveNetworkPanel(true);

            //duelStartButton.gameObject.SetActive(true);
            //duelStartButton.onClick.AddListener(() => OnDuelStartButtonClicked(isDuelHost));

            //var buttonTitle = duelStartButton.GetComponentInChildren<TextMeshProUGUI>();
            //if (isDuelHost)
            //{
            //    buttonTitle.text = "Start";
            //    duelStartButton.interactable = false;

            //    joinCode.gameObject.SetActive(true);
            //}
            //else
            //{
            //    buttonTitle.text = "Ready";
            //}
        }

        private void OnHostButtonClicked()
        {
            StartCoroutine(GameManager.Instance.ConfigureTransportAndStartNgoAsHost());
            SetActiveNetworkPanel(false);
            ActorManager.Instance.ShowDuelLobbyPads();
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

        private void OnClientButtonClicked()
        {
            //StartCoroutine(GameManager.Instance.ConfigureTransportAndStartNgoAsConnectingPlayer(joinCode));
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
    }
}
