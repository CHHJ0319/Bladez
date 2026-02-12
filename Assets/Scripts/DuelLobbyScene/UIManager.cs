using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace DualLobbyScene
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance;

        [Header("Buttons")]
        public Button hostButton;
        public Button clientButton;
        public Button serverButton;

        [Header("Status")]
        public TMP_Text statusText;

        [Header("GameUI")]
        public Button StartButton;

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

        void OnHostButtonClicked()
        {
            NetworkManager.Singleton.StartHost();
        }
        void OnClientButtonClicked()
        {
            NetworkManager.Singleton.StartClient();
        }
        void OnServerButtonClicked()
        {
            NetworkManager.Singleton.StartServer();
        }

        void UpdateUI()
        {
            if (NetworkManager.Singleton == null)
            {
                SetStartButtons(false);
                SetStatusText("NetworkManager not found");
                return;
            }

            if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
            {
                SetStartButtons(true);
                SetStatusText("Not connected");
            }
            else
            {
                SetStartButtons(false);
                UpdateStatusLabels();
            }
        }

        void SetStartButtons(bool state)
        {
            hostButton.gameObject.SetActive(state);
            clientButton.gameObject.SetActive(state);
            serverButton.gameObject.SetActive(state);
        }

        void SetStatusText(string text)
        {
            if (statusText != null) statusText.text = text;
        }

        void UpdateStatusLabels()
        {
            var mode = NetworkManager.Singleton.IsHost ? "Host" : NetworkManager.Singleton.IsServer ? "Server" : "Client";
            string transport = "Transport: " + NetworkManager.Singleton.NetworkConfig.NetworkTransport.GetType().Name;
            string modeText = "Mode: " + mode;
            SetStatusText($"{transport}\n{modeText}");
        }
    }
}
