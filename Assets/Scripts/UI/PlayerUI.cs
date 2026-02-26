using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class PlayerUI : MonoBehaviour
    {
        [Header("DualLobbyScene")]
        public Button duelStartButton;
        public TMP_Text joinCode;

        [Header("DualScene")]
        public GaugeBar hpBar;
        public GaugeBar staminaBar;
        public Image gameDefeatImage;
        public Image gameWinImage;

        void Awake()
        {
            UIManager.Instance.SetPlayerUI(this);
        }

        public void Initialize(bool isDuelHost, string sceneName)
        {
            if(sceneName == "DuelLobbyScene")
            {
                InitializeDuelLobbySceneUI(isDuelHost);
            }
        }

        public void SetJoinCode(string code)
        {
            if (joinCode != null) joinCode.text = code;
        }

        public void UpdateHPBar(float hp, float maxHP)
        {
            if (hpBar != null)
            {
                hpBar.UpdateGaugeBar(hp, maxHP);
            }
        }

        public void UpdateUI(string sceneName)
        {
            if (sceneName == "DuelLobbyScene")
            {
                UpdateDuelLobbySceneUI();
            }
        }

        public void ShowWinUI()
        {
            gameWinImage.gameObject.SetActive(true);
        }

        public void ShowDefeatUI()
        {
            gameDefeatImage.gameObject.SetActive(true);
        }

        private void InitializeDuelLobbySceneUI(bool isDuelHost)
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

        private void UpdateDuelLobbySceneUI()
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

