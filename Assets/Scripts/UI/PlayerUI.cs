using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class PlayerUI : MonoBehaviour
    {
        [Header("DualLobbyScene")]
        public Button duelStartButton;

        [Header("DualScene")]
        public GaugeBar hpBar;
        public GaugeBar staminaBar;
        public Image gameDefeatImage;
        public Image gameWinImage;

        void Awake()
        {
            UIManager.Instance.SetPlayerUI(this);
        }

        public void Initialize(bool isDuelHost)
        {
            duelStartButton.onClick.AddListener(() => OnDuelStartButtonClicked(isDuelHost));

            var buttonTitle = duelStartButton.GetComponentInChildren<TextMeshProUGUI>();
            if (isDuelHost)
            {

                buttonTitle.text = "Start";
                duelStartButton.interactable = false;
            }
            else
            {
                buttonTitle.text = "Ready";
            }
        }

        public void UpdateHPBar(float hp, float maxHP)
        {
            if (hpBar != null)
            {
                hpBar.UpdateGaugeBar(hp, maxHP);
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

