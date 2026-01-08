using Actor.Player;
using UnityEngine;
using UnityEngine.Windows;

namespace Actor
{
    public abstract class ActorController : MonoBehaviour
    {
        public ItemPicker itemPicker;

        [Header("Movement Settings")]
        public float forwardSpeed = 7.0f;
        public float rotateSpeed = 20.0f;
        public float slidingSpeed = 4.0f;
        public float jumpSpeed = 2f;

        protected float verticalVelocity;

        // [Header("Character Status")]
        protected float hp = 100f;
        private int ammo = 100;

        private float orgColHeight;
        private Vector3 orgVectColCenter;

        private bool useCurves = true;
        private float useCurvesHeight = 0.5f;

        protected CharacterController controller;

        private WeaponHandler weaponHandler;
        protected ActorAnimator anim;

        protected AnimatorStateInfo currentBaseState;
        protected AnimatorStateInfo currentUpperBodyState;

        protected virtual void Start()
        {
            controller = GetComponent<CharacterController>();
            orgColHeight = controller.height;
            orgVectColCenter = controller.center;

            anim = GetComponent<ActorAnimator>();
            weaponHandler = GetComponent<WeaponHandler>();
        }

        protected virtual void Update()
        {
            weaponHandler.UpdateFireTimer();
            DetectDroppedItems();
        }

        protected virtual void FixedUpdate()
        {
            GetAnimState();
            HandleStateSpecificLogic();
        }

        void DetectDroppedItems()
        {
            if(itemPicker != null)
            {
                if(itemPicker.IsItemDetected)
                {
                    PickUp();
                }
            }
        }

        void GetAnimState()
        {
            currentBaseState = anim.GetBaseLayerState();
            //currentUpperBodyState = anim.GetUpperBodyState();
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


        protected void Jump()
        {
            if (currentBaseState.fullPathHash == PrayerState.LocoState
                && !anim.IsTransitioning())
            {
                anim.PlayJump();
            }
        }

        protected void Sliding()
        {
            if (currentBaseState.fullPathHash == PrayerState.LocoState
                && !anim.IsTransitioning())
            {
                anim.PlaySliding();
            }
        }

        protected void Attack()
        {
            if (currentBaseState.fullPathHash == PrayerState.JumpState &&
                currentBaseState.fullPathHash == PrayerState.SlidingState &&
                currentBaseState.fullPathHash == PrayerState.ReloadingState)
                return;

            if (weaponHandler.CanAttack())
            {
                if (weaponHandler.GetEquipWeaponType() == Item.Weapon.WeaponType.Melee)
                {
                    anim.PlaySlash();
                }
                else if (weaponHandler.GetEquipWeaponType() == Item.Weapon.WeaponType.Range)
                {
                    anim.PlayShot();
                }

                weaponHandler.Attack();
            }

        }

        protected void Reload()
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

        protected void PickUp()
        {
            GameObject item = itemPicker.GetPickedUpItem();

            if (item != null && item.tag == "Weapon")
            {
                if (weaponHandler.CanAddWeapon())
                {
                    weaponHandler.AddWeapon(item);
                    itemPicker.IsItemDetected = false;
                }
            }
        }

        protected void EquipWeapon(int weaponIdx)
        {
            weaponHandler.EquipWeapon(weaponIdx);
        }

        protected void ApplyGravity()
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

        public virtual void TakeDamage(float damage)
        {
            hp -= damage;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("AttackRange"))
            {
                Item.Weapon.WeaponController weapon = other.transform.parent.gameObject.GetComponent<Item.Weapon.WeaponController>();
                if (weaponHandler.IsEquipWeapon(weapon))
                {
                }
                else
                {

                }
            }
        }
    }
}