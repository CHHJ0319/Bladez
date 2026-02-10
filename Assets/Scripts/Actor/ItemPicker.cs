using UnityEngine;

namespace Actor
{
    public class ItemPicker : MonoBehaviour
    {
        GameObject targetObject;

        public bool IsItemDetected { get; private set; }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Weapon")
            {
                targetObject = other.gameObject;

                IsItemDetected = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.tag == "Weapon")
            {
                targetObject = null;

                IsItemDetected = false;
            }
        }

        public GameObject GetPickedUpItem()
        {
            return targetObject;
        }

        public void Clear()
        {
            targetObject = null;

            IsItemDetected = false;
        }
    }
}

