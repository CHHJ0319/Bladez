using UnityEngine;

namespace Actor.Player
{
    public class PlayerController : CharaterController
    {
        private PlayerInputHandler playerInputHandler;

        private Vector3 velocity;

        protected override void Start()
        {
            base.Start();

            playerInputHandler = GetComponent<PlayerInputHandler>();
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            float h = playerInputHandler.Horizontal;
            float v = playerInputHandler.Vertical;

            characterAnimator.UpdateMovementAnimation(h, v);

            CalculateVelocity(v);
            ApplyMovement(h);
            Jump();

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

        void ApplyMovement(float horizontal)
        {
            transform.localPosition += velocity * Time.fixedDeltaTime;

            transform.Rotate(0, horizontal * rotateSpeed, 0);
        }
    }
}


