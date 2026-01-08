using UnityEngine;

namespace Item.Weapon
{
    public class DropSwordParticle : MonoBehaviour
    {
        public GameObject weaponPrefab;
        public Transform particles;

        private void Start()
        {          
            GameObject weapon = Instantiate(weaponPrefab, transform);

            weapon.transform.localPosition = Vector3.zero;
            weapon.transform.localRotation = Quaternion.Euler(0, 0, 180f);

            ApplyParticleEffect();
        }

        private void Update()
        {
            transform.Rotate(Vector3.up * 40 * Time.deltaTime);

            if(transform.childCount < 2)
            {
                Destroy();
            }
        }

        private void ApplyParticleEffect()
        {
            string prefabName = weaponPrefab.name;
            string type = prefabName.Replace("Sword", "").Replace("Weapon", "").Replace("(Clone)", "");
            string targetParticleName = type + "Particle";

            foreach (Transform child in particles)
            {
                if (child.name.Contains("Particle"))
                {
                    bool isMatch = child.name == targetParticleName;
                    child.gameObject.SetActive(isMatch);
                }
            }
        }

       
        private void Destroy()
        {
            particles.gameObject.SetActive(false);
            Destroy(gameObject);
        }
   }
}