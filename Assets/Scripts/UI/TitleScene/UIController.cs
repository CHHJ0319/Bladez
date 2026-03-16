using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.TitleScene
{
    public class UIController : MonoBehaviour
    {
        public AudioSource startSound;
        public BlinkingPrompt blinkingPrompt;

        [Header("Network Panel")]
        public Transform networkPanel;
        public TMP_InputField joincodeInputField;
        public Button accessButton;

        void Awake()
        {
           //accessButton.onClick.AddListener(OnAccessButtonClicked);
        }

        private void OnEnable()
        {
            Events.GameEvents.OnGameStarted += PlayStartSound;
        }

        private void OnDisable()
        {
            Events.GameEvents.OnGameStarted -= PlayStartSound;
        }

        private void Start()
        {
            UIManager.Instance.SetTitleSceneUIController(this);
        }

        public void PlayStartSound()
        {
            startSound.Play();
        }

        private void OnAccessButtonClicked()
        {
            Events.GameEvents.StartGame();
            string joinCode = joincodeInputField.text;

            if (string.IsNullOrWhiteSpace(joinCode))
            {
                GameManager.Instance.StartHost();
            }
            else
            {
                GameManager.Instance.StartClient(joinCode);
            }
        }

        public void SetActiveNetworkPanel(bool isActive)
        {
            networkPanel.gameObject.SetActive(isActive);
        }

        public void HideBlinkingPrompt()
        {
            blinkingPrompt.gameObject.SetActive(false);
        }
    }
}
