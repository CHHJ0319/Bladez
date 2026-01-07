using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerInput : MonoBehaviour
    {
        public void LockCursor()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        public void UnlockCursor()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        public Vector2 MoveInput { get; private set; }

        public void OnMove(InputValue value)
        {
            MoveInput = value.Get<Vector2>();
        }
        public void OnJump(InputValue value) => Events.PlayerEvents.Jump();
        public void OnSliding(InputValue value) => Events.PlayerEvents.Sliding();
        public void OnAttack(InputValue value) => Events.PlayerEvents.Attack();
        public void OnReload(InputValue value) => Events.PlayerEvents.Reload();

    }
}