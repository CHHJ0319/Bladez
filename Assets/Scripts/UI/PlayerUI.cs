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

        void Awake()
        {
            UIManager.Instance.SetPlayerUI(this);
        }
    }
}

