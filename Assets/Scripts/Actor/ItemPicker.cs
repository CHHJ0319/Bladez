using UnityEngine;

namespace Actor
{
    public class ItemPicker : MonoBehaviour
    {
        GameObject nearObj;

        public bool IsItemDetected { get; set; }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Weapon")
            {
                nearObj = other.gameObject;

                IsItemDetected = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.tag == "Weapon")
            {
                nearObj = null;

                IsItemDetected = false;
            }
        }

        public GameObject GetPickedUpItem()
        {
            return nearObj;
        }
    }
}

