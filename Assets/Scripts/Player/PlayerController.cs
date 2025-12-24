using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

namespace Player
{
    [RequireComponent(typeof(PlayerInput))]
    [RequireComponent(typeof(PlayerAnimator))]
    [RequireComponent(typeof(WeaponHandler))]
    public class PlayerController : MonoBehaviour
    {
        public GameEnding gameEnding;

        public float forwardSpeed = 7.0f;
        public float rotateSpeed = 20.0f;
        public float slidingSpeed = 3.0f;
        public float jumpSpeed = 0.5f;

        private float hp = 100f;
        private int ammo = 100;

        private bool useCurves = true;
        private float useCurvesHeight = 0.5f;

        private float orgColHeight;
        private Vector3 orgVectColCenter;

        private WeaponHandler weaponHandler;
        private PlayerInput input;
        private PlayerAnimator anim;

        private AnimatorStateInfo currentBaseState;
        private AnimatorStateInfo currentUpperBodyState;

        //-------------------------------------------------------------
        private CharacterController controller;
        public Transform thirdCamTranform;

        static int idleState = Animator.StringToHash("Base Layer.Idle");
        static int locoState = Animator.StringToHash("Base Layer.RUN");
        static int jumpState = Animator.StringToHash("Base Layer.Jump");
        static int slidingState = Animator.StringToHash("Base Layer.Sliding");

        static int reloadingState = Animator.StringToHash("UpperBody.Reloading");

        void Start()
        {
            input = GetComponent<PlayerInput>();
            anim = GetComponent<PlayerAnimator>();
            weaponHandler = GetComponent<WeaponHandler>();

            controller = GetComponent<CharacterController>();
            orgColHeight = controller.height;
            orgVectColCenter = controller.center;

            Events.PlayerEvents.OnAttack += Attack;
            Events.PlayerEvents.OnJump += Jump;
            Events.PlayerEvents.OnSliding += Sliding;
            Events.PlayerEvents.OnReload += Reload;
        }

        private void Update()
        {
            weaponHandler.UpdateFireTimer();

            ApplyRotation();
        }

        void FixedUpdate()
        {
            Cursor.lockState = CursorLockMode.Locked;

            currentBaseState = anim.GetBaseLayerState();
            currentUpperBodyState = anim.GetUpperBodyState();

            CalculateMove();

            HandleStateSpecificLogic();


        }

        void ApplyRotation()
        {
            float targetRotation = thirdCamTranform.eulerAngles.y;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, targetRotation, 0), rotateSpeed * Time.deltaTime);
        }


        void Jump()
        {
            if (currentBaseState.fullPathHash == locoState
                && !anim.IsTransitioning())
            {
                anim.PlayJump();
            }
        }

        void Sliding()
        {
            if (currentBaseState.fullPathHash == locoState
                && !anim.IsTransitioning())
            {
                anim.PlaySliding();
            }
        }
        void Attack()
        {
            if (currentBaseState.fullPathHash == jumpState && 
                currentBaseState.fullPathHash == slidingState && 
                currentBaseState.fullPathHash == reloadingState)
                return;

            if(weaponHandler.CanFire())
            {
                anim.PlayFire();
                weaponHandler.Fire();
            }

        }

        void Reload()
        {
            if (ammo == 0)
                return;

            if (currentBaseState.fullPathHash == jumpState &&
                currentBaseState.fullPathHash == slidingState)
                return;

            if (weaponHandler.CanReload())
            {
                if (!anim.IsTransitioning())
                {
                    anim.PlayReload();
                }
                weaponHandler.Reload(ref ammo);
            }
        }

        void CalculateMove()
        {
            float horizontal = input.MoveInput.x;
            float vertical = input.MoveInput.y;

            float moveSpeed = new Vector2(horizontal, vertical).magnitude;
            anim.PlayWalking(moveSpeed > 0.01f);

            Vector3 moveDir = (transform.forward * vertical) + (transform.right * horizontal);

            if (!controller.isGrounded)
            {
                moveDir.y -= 9.81f * Time.deltaTime;
            }

            float speedFactor = forwardSpeed;

            if (currentUpperBodyState.fullPathHash == slidingState)
            {
                speedFactor = slidingSpeed;
            }
            else if (currentUpperBodyState.fullPathHash == jumpState)
            {
                speedFactor = jumpSpeed;
            }
            else if (currentUpperBodyState.fullPathHash == reloadingState)
            {
                speedFactor = 0.0f;
            }

            controller.Move(moveDir * speedFactor * Time.deltaTime);
        }

        void HandleStateSpecificLogic()
        {
            if (currentUpperBodyState.fullPathHash == reloadingState
                && !anim.IsTransitioning())
            {
                anim.StopReload();
            }

            if (currentBaseState.fullPathHash == locoState)
            {
                if (useCurves)
                {
                    resetCollider();
                }
            }

            else if (currentBaseState.fullPathHash == jumpState)
            {
                if (!anim.IsTransitioning())
                {
                    if (useCurves)
                    {
                        float jumpHeight = anim.GetJumpHeight();

                        Ray ray = new Ray(transform.position + Vector3.up, -Vector3.up);
                        RaycastHit hitInfo = new RaycastHit();

                        if (Physics.Raycast(ray, out hitInfo))
                        {
                            if (hitInfo.distance > useCurvesHeight)
                            {
                                float newHeight = orgColHeight - jumpHeight;
                                controller.height = newHeight;

                                float adjCenterY = newHeight / 2f;
                                controller.center = new Vector3(0, adjCenterY, 0);
                            }
                            else
                            {
                                resetCollider();
                            }
                        }
                    }
                    anim.StopJump();
                }
            }

            else if (currentBaseState.fullPathHash == slidingState)
            {
                if (!anim.IsTransitioning())
                {
                    anim.StopSliding();
                }
            }

            else if (currentBaseState.fullPathHash == idleState)
            {
                if (useCurves)
                {
                    resetCollider();
                }
            }
        }

        void resetCollider()
        {
            controller.height = orgColHeight;
            controller.center = orgVectColCenter;
        }

        public void TakeDamage(float damage)
        {
            hp -= damage;
            if (hp < 0)
            {
                gameEnding.CaughtPlayer();
            }
        }
    }
}
