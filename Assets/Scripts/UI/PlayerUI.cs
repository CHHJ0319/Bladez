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
            UIManager.Instance.SetPlayerUI(this);
        }

        public void Initialize()
        {
   
        }

        public void UpdateHPBar(float hp, float maxHP)
        {
            if (hpBar != null)
            {
                hpBar.UpdateGaugeBar(hp, maxHP);
            }
        }

        public void UpdateUI()
        {

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

