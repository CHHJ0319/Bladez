using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI.DuelLobbyScene
{
    public class UIController : MonoBehaviour
    {
        public Transform networkPanel;

        private Button hostButton;
        private TMP_InputField joinCodeInputfiled;
        private Button clientButton;

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

        public void UpdateUI(bool state)
        {
            SetActiveNetworkPanel(state);
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
    }
}
