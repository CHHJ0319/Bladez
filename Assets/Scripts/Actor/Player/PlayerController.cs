using UnityEngine;

namespace Actor.Player
{
    public class PlayerController : CharaterController
    {
        private PlayerInputHandler playerInputHandler;
        private PlayerNetworkHandler playerNetworkHandler;

        protected override void Start()
        {
            base.Start();

            playerInputHandler = GetComponent<PlayerInputHandler>();
            playerNetworkHandler = GetComponent<PlayerNetworkHandler>();

        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            float h = playerInputHandler.Horizontal;
            float v = playerInputHandler.Vertical;

            Move(h, v);
            Jump();
            Sliding();
            Attack();
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

        protected override void Jump()
        {
            if (playerInputHandler.JumpTriggered)
            {
                base.Jump();
            }
        }

        protected override void Sliding()
        {
            if (playerInputHandler.SlidingTriggered)
            {
                base.Sliding();
            }
        }

        protected override void Attack()
        {
            if (playerInputHandler.AttackTriggered)
            {
                base.Attack();
            }
        }

        public void Move(float horizontal, float vertical)
        {
            characterAnimator.UpdateMovementAnimation(horizontal, vertical);

            CalculateVelocity(vertical);

            transform.localPosition += velocity * Time.fixedDeltaTime;

            transform.Rotate(0, horizontal * rotateSpeed, 0);

            playerNetworkHandler.Move(transform.localPosition, transform.localRotation);
        }
    }
}


