using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

namespace Player
{
    [RequireComponent(typeof(PlayerInputHandler))]
    [RequireComponent(typeof(PlayerAnimator))]
    [RequireComponent(typeof(WeaponHandler))]
    public class PlayerController : MonoBehaviour
    {
        public GameEnding gameEnding;
        public Transform thirdCamTranform;
        public ItemPicker itemPicker;

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
        private float verticalVelocity;

        private CharacterController controller;

        private WeaponHandler weaponHandler;
        private PlayerInputHandler input;
        private PlayerAnimator anim;

        private AnimatorStateInfo currentBaseState;
        private AnimatorStateInfo currentUpperBodyState;

        void OnEnable()
        {
            Events.PlayerEvents.OnAttack += Attack;
            Events.PlayerEvents.OnJump += Jump;
            Events.PlayerEvents.OnSliding += Sliding;
            Events.PlayerEvents.OnReload += Reload;
            Events.PlayerEvents.OnItemPickedUp += PickUp;
        }


        void Start()
        {
            controller = GetComponent<CharacterController>();
            orgColHeight = controller.height;
            orgVectColCenter = controller.center;

            input = GetComponent<PlayerInputHandler>();
            anim = GetComponent<PlayerAnimator>();
            weaponHandler = GetComponent<WeaponHandler>();

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
            //currentUpperBodyState = anim.GetUpperBodyState();
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
                if(weaponHandler.GetEquipWeapon() == Item.Weapon.WeaponType.Melee)
                {
                    anim.PlaySlash();
                }
                else if(weaponHandler.GetEquipWeapon() == Item.Weapon.WeaponType.Range)
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

        void PickUp()
        {
            GameObject item = itemPicker.GetPickedUpItem();
            if(item.tag == "Weapon")
            {
                if(weaponHandler.CanAddWeapon())
                {
                    weaponHandler.AddWeapon(item);
                }
            }
        }

        void CalculateMove()
        {
            float horizontal = input.MoveInput.x;
            float vertical = input.MoveInput.y;

            Vector3 moveDir = (transform.forward * vertical) + (transform.right * horizontal);

            float moveSpeed = new Vector2(horizontal, vertical).magnitude;
            anim.PlayMoving(moveSpeed > 0.01f);

            float speedFactor = forwardSpeed;
            if (currentUpperBodyState.fullPathHash == PrayerState.ReloadingState) speedFactor = 0.0f;
            else if (currentBaseState.fullPathHash == PrayerState.SlidingState) speedFactor = slidingSpeed;
            else if (currentBaseState.fullPathHash == PrayerState.JumpState) speedFactor = jumpSpeed;

            ApplyGravity();

            Vector3 finalMove = (moveDir * speedFactor) + (Vector3.up * verticalVelocity);

            controller.Move(finalMove * Time.deltaTime);
        }

        private void ApplyGravity()
        {
            if (controller.isGrounded)
            {
                if (verticalVelocity < 0)
                    verticalVelocity = -2f;
            }
            else
            {
                verticalVelocity -= 9.81f * Time.deltaTime;
            }
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
