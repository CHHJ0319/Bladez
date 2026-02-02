using UnityEngine;

namespace Actor.Player
{
    public class RemotePlayerController : CharaterController
    {

        protected override void Start()
        {
            base.Start();

        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            //float h = playerInputHandler.Horizontal;
            //float v = playerInputHandler.Vertical;

            //characterAnimator.UpdateMovementAnimation(h, v);

            //CalculateVelocity(v);
            //ApplyMovement(h);
            //Jump();
            //Sliding();
            //Attack();
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
            //if (playerInputHandler.JumpTriggered)
            //{
            //    base.Jump();   
            //}
        }

        protected override void Sliding()
        {
            //if (playerInputHandler.SlidingTriggered)
            //{
            //    base.Sliding();   
            //}
        }

        protected override void Attack()
        {
            //if (playerInputHandler.AttackTriggered)
            //{
            //    base.Attack();
            //}
        }

        void ApplyMovement(float horizontal)
        {
            transform.localPosition += velocity * Time.fixedDeltaTime;

            transform.Rotate(0, horizontal * rotateSpeed, 0);
        }
    }
}


