using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerInput : MonoBehaviour
    {
        public Vector2 MoveInput { get; private set; }

        public void OnMove(InputValue value) => MoveInput = value.Get<Vector2>();
        public void OnJump(InputValue value) => Events.PlayerEvents.Jump();
        public void OnSliding(InputValue value) => Events.PlayerEvents.Sliding();
        public void OnAttack(InputValue value) => Events.PlayerEvents.Attack();
        public void OnReload(InputValue value) => Events.PlayerEvents.Reload();

    }
}