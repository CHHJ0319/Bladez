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
        public Transform thirdCamTranform;

        [Header("Property")]
        public float forwardSpeed = 7.0f;
        public float rotateSpeed = 20.0f;
        public float slidingSpeed = 4.0f;
        public float jumpSpeed = 2f;

        private float hp = 100f;
        [SerializeField] private int ammo = 100;

        private bool useCurves = true;
        private float useCurvesHeight = 0.5f;

        private float orgColHeight;
        private Vector3 orgVectColCenter;

        private CharacterController controller;

        private WeaponHandler weaponHandler;
        private PlayerInput input;
        private PlayerAnimator anim;

        private AnimatorStateInfo currentBaseState;
        private AnimatorStateInfo currentUpperBodyState;

        void Start()
        {
            controller = GetComponent<CharacterController>();
            orgColHeight = controller.height;
            orgVectColCenter = controller.center;

            input = GetComponent<PlayerInput>();
            anim = GetComponent<PlayerAnimator>();
            weaponHandler = GetComponent<WeaponHandler>();

            Events.PlayerEvents.OnAttack += Attack;
            Events.PlayerEvents.OnJump += Jump;
            Events.PlayerEvents.OnSliding += Sliding;
            Events.PlayerEvents.OnReload += Reload;

            input.LockCursor();
        }

        private void Update()
        {
            input.UnlockCursor();
            weaponHandler.UpdateFireTimer();
            ApplyRotation();
        }

        void FixedUpdate()
        {
            GetAnimState();
            CalculateMove();
            HandleStateSpecificLogic();
        }

        void ApplyRotation()
        {
            float targetRotation = thirdCamTranform.eulerAngles.y;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, targetRotation, 0), rotateSpeed * Time.deltaTime);
        }

        void GetAnimState()
        {
            currentBaseState = anim.GetBaseLayerState();
            currentUpperBodyState = anim.GetUpperBodyState();
        }


        void Jump()
        {
            if (currentBaseState.fullPathHash == PrayerState.LocoState
                && !anim.IsTransitioning())
            {
                anim.PlayJump();
            }
        }

        void Sliding()
        {
            if (currentBaseState.fullPathHash == PrayerState.LocoState
                && !anim.IsTransitioning())
            {
                anim.PlaySliding();
            }
        }

        void Attack()
        {
            if (currentBaseState.fullPathHash == PrayerState.JumpState && 
                currentBaseState.fullPathHash == PrayerState.SlidingState && 
                currentBaseState.fullPathHash == PrayerState.ReloadingState)
                return;

            if(weaponHandler.CanAttack())
            {
                if(weaponHandler.GetEquipWeapon() == Weapon.WeaponType.Melee)
                {
                    anim.PlaySlash();
                }
                else if(weaponHandler.GetEquipWeapon() == Weapon.WeaponType.Range)
                {
                    anim.PlayShot();
                }

                weaponHandler.Attack();
            }

        }

        void Reload()
        {
            if (ammo == 0)
                return;

            if (currentBaseState.fullPathHash == PrayerState.JumpState &&
                currentBaseState.fullPathHash == PrayerState.SlidingState)
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
            anim.PlayMoving(moveSpeed > 0.01f);

            Vector3 moveDir = (transform.forward * vertical) + (transform.right * horizontal);

            if (!controller.isGrounded)
            {
                moveDir.y -= 9.81f * Time.deltaTime;
            }

            float speedFactor = forwardSpeed;

            if (currentUpperBodyState.fullPathHash == PrayerState.ReloadingState)
            {
                speedFactor = 0.0f;
            }
            else if (currentBaseState.fullPathHash == PrayerState.SlidingState)
            {
                speedFactor = slidingSpeed;
            }
            else if (currentBaseState.fullPathHash == PrayerState.JumpState)
            {
                speedFactor = jumpSpeed;
            }

            controller.Move(moveDir * speedFactor * Time.deltaTime);
        }

        void HandleStateSpecificLogic()
        {
            if (currentUpperBodyState.fullPathHash == PrayerState.ReloadingState
                && !anim.IsTransitioning())
            {
                anim.StopReload();
            }

            if (currentBaseState.fullPathHash == PrayerState.LocoState)
            {
                if (useCurves)
                {
                    ResetCollider();
                }
            }

            else if (currentBaseState.fullPathHash == PrayerState.JumpState)
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
                                Debug.Log("asasd " + jumpHeight);
                                controller.height = newHeight;

                                float adjCenterY = newHeight / 2f;
                                controller.center = new Vector3(0, adjCenterY, 0);
                            }
                            else
                            {
                                ResetCollider();
                            }
                        }
                    }
                    anim.StopJump();
                }
            }

            else if (currentBaseState.fullPathHash == PrayerState.SlidingState)
            {
                if (!anim.IsTransitioning())
                {
                    anim.StopSliding();
                }
            }

            else if (currentBaseState.fullPathHash == PrayerState.IdleState)
            {
                if (useCurves)
                {
                    ResetCollider();
                }
            }
        }

        void ResetCollider()
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
