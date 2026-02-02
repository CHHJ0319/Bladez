using UnityEngine;

namespace Actor.Player
{
    public class PlayerController : MonoBehaviour
    {
        public GameObject character;

        public float lookSmoother = 3.0f;
        public bool useCurves = true;
        public float useCurvesHeight = 0.5f;

        public float forwardSpeed = 7.0f;
        public float backwardSpeed = 2.0f;
        public float rotateSpeed = 2.0f;
        public float jumpPower = 3.0f;

        private CapsuleCollider col;
        private Rigidbody rb;

        private Vector3 velocity;
        private float orgColHeight;
        private Vector3 orgVectColCenter;
        private AnimatorStateInfo currentBaseState;

        private PlayerInputHandler playerInputHandler;
        private CharacterAnimator characterAnimator;

        static int idleState = Animator.StringToHash("Base Layer.Idle");
        static int locoState = Animator.StringToHash("Base Layer.Locomotion");
        static int jumpState = Animator.StringToHash("Base Layer.Jump");
        static int restState = Animator.StringToHash("Base Layer.Rest");

        void Start()
        {
            col = character.GetComponent<CapsuleCollider>();
            rb = character.GetComponent<Rigidbody>();

            playerInputHandler = GetComponent<PlayerInputHandler>();
            characterAnimator = character.GetComponent<CharacterAnimator>();

            orgColHeight = col.height;
            orgVectColCenter = col.center;
        }

        void FixedUpdate()
        {
            float h = playerInputHandler.Horizontal;
            float v = playerInputHandler.Vertical;

            SetGravity(true);
            characterAnimator.UpdateMovementAnimation(h, v);
            UpdateAnimationState();
            CalculateVelocity(v);
            Jump();
            ApplyMovement(h);
            UpdateStateBehavior();
        }

        void SetGravity(bool active)
        {
            rb.useGravity = active;
        }

        

        void UpdateAnimationState()
        {
            currentBaseState = characterAnimator.GetBaseLayerState(); ;
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

        void Jump()
        {
            if (playerInputHandler.JumpTriggered)
            {
                if (currentBaseState.fullPathHash == locoState)
                {
                    if (!characterAnimator.IsTransitioning())
                    {
                        rb.AddForce(Vector3.up * jumpPower, ForceMode.VelocityChange);
                        characterAnimator.SetJump(true);
                    }
                }
            }
        }

        void ApplyMovement(float horizontal)
        {
            transform.localPosition += velocity * Time.fixedDeltaTime;

            transform.Rotate(0, horizontal * rotateSpeed, 0);
        }

        void UpdateStateBehavior()
        {
            if (currentBaseState.fullPathHash == locoState)
            {
                if (useCurves)
                {
                    ResetCollider();
                }
            }
            else if (currentBaseState.fullPathHash == jumpState)
            {
                if (!characterAnimator.IsTransitioning())
                {
                    if (useCurves)
                    {
                        float jumpHeight = characterAnimator.GetJumpHeight();
                        float gravityControl = characterAnimator.GetJumpHeight();
                        if (gravityControl > 0)
                            rb.useGravity = false;

                        Ray ray = new Ray(transform.position + Vector3.up, -Vector3.up);
                        RaycastHit hitInfo = new RaycastHit();
                        if (Physics.Raycast(ray, out hitInfo))
                        {
                            if (hitInfo.distance > useCurvesHeight)
                            {
                                col.height = orgColHeight - jumpHeight;
                                float adjCenterY = orgVectColCenter.y + jumpHeight;
                                col.center = new Vector3(0, adjCenterY, 0);
                            }
                            else
                            {
                                ResetCollider();
                            }
                        }
                    }
                    characterAnimator.SetJump(false);
                }
            }
            else if (currentBaseState.fullPathHash == idleState)
            {
                if (useCurves)
                {
                    ResetCollider();
                }
                if (playerInputHandler.JumpTriggered)
                {
                    characterAnimator.SetJump(true);
                }
            }
            else if (currentBaseState.fullPathHash == restState)
            {
                if (!characterAnimator.IsTransitioning())
                {
                    characterAnimator.SetJump(false);
                }
            }
        }

        void ResetCollider()
        {
            col.height = orgColHeight;
            col.center = orgVectColCenter;
        }
    }
}


