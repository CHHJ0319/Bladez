using TMPro;
using Unity.Netcode;
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
            accessButton.onClick.AddListener(OnAccessButtonClicked);
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
            UIManager.Instance.PlayStartSound();
            string joinCode = joincodeInputField.text;

            if (string.IsNullOrWhiteSpace(joinCode))
            {
                StartCoroutine(GameManager.Instance.ConfigureTransportAndStartNgoAsHost());
            }
            else
            {
                StartCoroutine(GameManager.Instance.ConfigureTransportAndStartNgoAsConnectingPlayer(joinCode));
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
