using UnityEngine;

namespace Player
{
    public class ItemPicker : MonoBehaviour
    {
        GameObject nearObj;

        private void OnTriggerStay(Collider other)
        {
            if (other.tag == "Weapon")
            {
                nearObj = other.gameObject;
                Events.PlayerEvents.PickUp();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.tag == "Weapon")
                nearObj = null;
        }

        public GameObject GetPickedUpItem()
        {
            return nearObj;
        }
    }
}

