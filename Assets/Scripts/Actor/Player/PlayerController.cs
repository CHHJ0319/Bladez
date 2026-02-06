using UnityEngine;

namespace Actor.Player
{
    public class PlayerController : CharaterController
    {
        private PlayerInputHandler playerInputHandler;

        protected override void Awake()
        {
            base.Awake();

            playerInputHandler = GetComponent<PlayerInputHandler>();

        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            float h = playerInputHandler.Horizontal;
            float v = playerInputHandler.Vertical;    

            Move(h, v);
            JumpWithPlayerInput();
            SlidingWithPlayerInput();
            AttackWithPlayerInput();
        }

        void CalculateVelocity(float vertical)
        {
            velocity = new Vector3(0, 0, vertical);
            velocity = transform.TransformDirection(velocity);

            if (vertical > 0.1f)
            {
                velocity *= forwardSpeed;
            }
            else if (vertical < -0.1f)
            {
                velocity *= backwardSpeed;
            }
        }

        void JumpWithPlayerInput()
        {
            if (playerInputHandler.JumpTriggered)
            {
                characterNetworkHandler.SubmitJumpRequestServerRpc();
                Jump();
            }
        }

        void SlidingWithPlayerInput()
        {
            if (playerInputHandler.SlidingTriggered)
            {
                Sliding();
            }
        }

        void AttackWithPlayerInput()
        {
            if (playerInputHandler.AttackTriggered)
            {
                characterNetworkHandler.SubmitAttackRequestServerRpc();
                Attack();
            }
        }

        public void Move(float horizontal, float vertical)
        {
            characterNetworkHandler.SubmitTransfromRequestServerRpc(transform.localPosition, transform.localRotation);

            characterNetworkAnimator.UpdateMovementAnimation(horizontal, vertical);

            CalculateVelocity(vertical);
            transform.localPosition += velocity * Time.fixedDeltaTime;
            transform.Rotate(0, horizontal * rotateSpeed, 0);

        }
    }
}


