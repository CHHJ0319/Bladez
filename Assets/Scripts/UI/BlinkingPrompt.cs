using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class BlinkingPrompt : MonoBehaviour
    {
        private Image promptImage;
        [SerializeField] private float blinkSpeed = 1.0f;

        void Start()
        {
            promptImage = GetComponent<Image>();

            StartCoroutine(DoBlink());
        }

        IEnumerator DoBlink()
        {
            while (true)
            {
                for (float f = 0f; f <= 1f; f += Time.deltaTime * blinkSpeed)
                {
                    Color c = promptImage.color;
                    c.a = f;
                    promptImage.color = c;
                    yield return null;
                }

                for (float f = 1f; f >= 0f; f -= Time.deltaTime * blinkSpeed)
                {
                    Color c = promptImage.color;
                    c.a = f;
                    promptImage.color = c;
                    yield return null;
                }
            }
        }
    }
}
