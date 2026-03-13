using UnityEngine;
using UnityEngine.InputSystem;

namespace Actor.Player
{
    public class PlayerInputHandler : MonoBehaviour
    {
        public Vector2 MoveInput { get; private set; }

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

        private bool _interactTriggered;
        public bool InteractTriggered
        {
            get
            {
                if (_interactTriggered)
                {
                    _interactTriggered = false;
                    return true;
                }
                return false;
            }
        }

        private bool _quiick1Triggered;
        public bool Quiick1Triggered
        {
            get
            {
                if (_quiick1Triggered)
                {
                    _quiick1Triggered = false;
                    return true;
                }
                return false;
            }
        }

        private bool _quiick2Triggered;
        public bool Quiick2Triggered
        {
            get
            {
                if (_quiick2Triggered)
                {
                    _quiick2Triggered = false;
                    return true;
                }
                return false;
            }
        }

        private bool _quiick3Triggered;
        public bool Quiick3Triggered
        {
            get
            {
                if (_quiick3Triggered)
                {
                    _quiick3Triggered = false;
                    return true;
                }
                return false;
            }
        }

        public Vector2 LookInput { get; private set; }

        void OnMove(InputValue value)
        {
            MoveInput = value.Get<Vector2>();
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
        void OnInteract(InputValue value)
        {
            if (value.isPressed)
            {
                _interactTriggered = true;
            }
        }

        void OnQuickSlot1(InputValue value)
        {
            if (value.isPressed)
            {
                _quiick1Triggered = true;
            }
        }

        void OnQuickSlot2(InputValue value)
        {
            if (value.isPressed)
            {
                _quiick2Triggered = true;
            }
        }

        void OnQuickSlot3(InputValue value)
        {
            if (value.isPressed)
            {
                _quiick3Triggered = true;
            }
        }

        public void OnLook(InputValue value)
        {
            LookInput = value.Get<Vector2>();
        }
    }
}
