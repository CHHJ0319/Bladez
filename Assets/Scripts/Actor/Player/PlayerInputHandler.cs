using UnityEngine;
using UnityEngine.InputSystem;

namespace Actor.Player
{
    public class PlayerInputHandler : MonoBehaviour
    {
        private PlayerInput playerInput;

        public InputAction moveAction;
        public InputAction lookAction;
        public InputAction jumpAction;
        public InputAction slidingAction;
        public InputAction attackAction;
        public InputAction interactAction;
        public InputAction quickSlot1Action;
        public InputAction quickSlot2Action;
        public InputAction quickSlot3Action;
        public float horizontal => moveAction.ReadValue<Vector2>().x;
        public float vertical => moveAction.ReadValue<Vector2>().y;
        public Vector2 lookInput => lookAction.ReadValue<Vector2>();

        private void Awake()
        {
            playerInput = GetComponent<PlayerInput>();

            moveAction = playerInput.actions["Move"];
            lookAction = playerInput.actions["Look"];
            jumpAction = playerInput.actions["Jump"];
            slidingAction = playerInput.actions["Sliding"];
            attackAction = playerInput.actions["Attack"];
            interactAction = playerInput.actions["Interact"];
            quickSlot1Action = playerInput.actions["QuickSlot1"];
            quickSlot2Action = playerInput.actions["QuickSlot2"];
            quickSlot3Action = playerInput.actions["QuickSlot3"];
        }

        public void SetPlayerInputEnabled(bool isEnabled)
        {
            playerInput.enabled = isEnabled;
        }
    }
}
