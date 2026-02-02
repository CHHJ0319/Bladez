using UnityEngine;
using UnityEngine.InputSystem;

namespace Actor.Player
{
    public class PlayerInputHandler : MonoBehaviour
    {
        public float Horizontal { get; private set; }
        public float Vertical { get; private set; }

        private bool _jumpTriggered;
        public bool JumpTriggered
        {
            get
            {
                if (_jumpTriggered)
                {
                    _jumpTriggered = false;
                    return true;
                }
                return false;
            }
        }

        private bool _slidingTriggered;
        public bool SlidingTriggered
        {
            get
            {
                if (_slidingTriggered)
                {
                    _slidingTriggered = false;
                    return true;
                }
                return false;
            }
        }

        private bool _attackTriggered;
        public bool AttackTriggered
        {
            get
            {
                if (_attackTriggered)
                {
                    _attackTriggered = false;
                    return true;
                }
                return false;
            }
        }

        void OnMove(InputValue value)
        {
            Vector2 movement = value.Get<Vector2>();
            Horizontal = movement.x;
            Vertical = movement.y;
        }

        void OnJump(InputValue value)
        {
            if (value.isPressed)
            {
                _jumpTriggered = true;
            }
        }

        void OnSliding(InputValue value)
        {
            if (value.isPressed)
            {
                _slidingTriggered = true;
            }
        }

        void OnAttack(InputValue value)
        {
            if (value.isPressed)
            {
                _attackTriggered = true;
            }
        }
    }
}
