using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class PlayerUI : MonoBehaviour
    {
        [Header("DualScene")]
        public GaugeBar hpBar;
        public GaugeBar staminaBar;
        public Image gameDefeatImage;
        public Image gameWinImage;

        void Awake()
        {
            //UIManager.Instance.SetPlayerUI(this);
        }

        private void OnEnable()
        {
            Events.PlayerEvents.OnHPChaneged += UpdateHPBar;
        }

        private void OnDisable()
        {
            Events.PlayerEvents.OnHPChaneged -= UpdateHPBar;
        }

        private void UpdateHPBar(float hp, float maxHP)
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
    }
}

