using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

namespace Actor.UI
{
    public class GaugeBar : MonoBehaviour
    {
        [Header("UI Components")]
        [SerializeField] private Image gaugeBarFill;

        void Awake()
        {
                
        }


        public void UpdateGaugeBar(float currentGauge, float maxGauge)
        {
            if (gaugeBarFill != null)
            {
                float gaugeRatio = currentGauge / maxGauge;
                gaugeBarFill.fillAmount = gaugeRatio;
            }
        }
    }
}