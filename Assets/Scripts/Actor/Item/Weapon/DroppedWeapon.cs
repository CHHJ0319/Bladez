using UnityEngine;

namespace Actor.Item.Weapon
{
    public class DroppedEffects : MonoBehaviour
    {
        [Header("Effects")]
        public Light dropZoneLight;
        public GameObject[] particles;

        private Color[] dropZoneLightColors = new Color[]
        {
            new Color(255f / 255f, 100f / 255f, 0f / 255f),
            new Color(73f / 255f, 239f / 255f, 255f / 255f),
            new Color(244f / 255f, 255f / 255f, 86f / 255f)
        };

        public void Show(Data.ElementType type)
        {
            if (type != Data.ElementType.Neutral)
            {
                dropZoneLight.color = dropZoneLightColors[(int)type];
                GameObject particle = Instantiate(particles[(int)type], transform);
            }
        }

        private void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}