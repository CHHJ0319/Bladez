using Actor.Player;
using UnityEngine;

namespace Actor
{
    public abstract class CharaterController : MonoBehaviour
    {
        public GameObject character;

        [Header("Settings")]
        public float forwardSpeed = 7.0f;
        public float backwardSpeed = 2.0f;
        public float rotateSpeed = 2.0f;
        public float jumpPower = 3.0f;
        public float slidingPower = 1.0f;

        public bool useCurves = true;
        public float useCurvesHeight = 0.5f;

        protected CharacterAnimator characterAnimator;

        private CapsuleCollider col;
        private Rigidbody rb;

        private float orgColHeight;
        private Vector3 orgVectColCenter;
        private AnimatorStateInfo currentBaseState;
        private AnimatorStateInfo currentUpperBodyState;

        protected Vector3 velocity;

        private float hp = 100f;

        /// <summary>
        /// //////////////////////////////////////////////////////
        /// </summary>

        public ItemPicker itemPicker;

        private WeaponHandler weaponHandler;


        protected virtual void Start()
        {
            col = character.GetComponent<CapsuleCollider>();
            rb = character.GetComponent<Rigidbody>();

            characterAnimator = character.GetComponent<CharacterAnimator>();

            orgColHeight = col.height;
            orgVectColCenter = col.center;

            //weaponHandler = GetComponent<WeaponHandler>();
        }

        protected virtual void Update()
        {
            //weaponHandler.UpdateFireTimer();
            DetectDroppedItems();
        }

        protected virtual void FixedUpdate()
        {
            UpdateAnimationState();
            SetGravity(true);

            UpdateStateBehavior();
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

        void UpdateAnimationState()
        {
            currentBaseState = characterAnimator.GetBaseLayerState();
            //currentUpperBodyState = characterAnimator.GetUpperBodyState();
        }

        void SetGravity(bool active)
        {
            rb.useGravity = active;
        }

        void UpdateStateBehavior()
        {
            if (currentBaseState.fullPathHash == PlayerState.LocoState)
            {
                if (useCurves)
                {
                    ResetCollider();
                }
            }

            else if (currentBaseState.fullPathHash == PlayerState.JumpState)
            {
                if (!characterAnimator.IsTransitioning())
                {
                    if (useCurves)
                    {
                        float jumpHeight = characterAnimator.GetJumpHeight();
                        float gravityControl = characterAnimator.GetGravityControl();
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

            else if (currentBaseState.fullPathHash == PlayerState.SlidingState)
            {
                if (!characterAnimator.IsTransitioning())
                {
                    characterAnimator.SetSliding(false);
                }
            }

            else if (currentBaseState.fullPathHash == PlayerState.IdleState)
            {
                if (useCurves)
                {
                    ResetCollider();
                }
            }

            else if (currentBaseState.fullPathHash == PlayerState.restState)
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


        protected virtual void Jump()
        {
            if (currentBaseState.fullPathHash == PlayerState.LocoState
                && !characterAnimator.IsTransitioning())
            {
                rb.AddForce(Vector3.up * jumpPower, ForceMode.VelocityChange);
                characterAnimator.SetJump(true);
            }
        }

        protected virtual void Sliding()
        {
            if (currentBaseState.fullPathHash == PlayerState.LocoState
                && !characterAnimator.IsTransitioning())
            {
                rb.AddForce(velocity * slidingPower, ForceMode.VelocityChange);
                characterAnimator.SetSliding(true);
            }
        }

        protected void Attack()
        {
            if (currentBaseState.fullPathHash == PlayerState.JumpState &&
                currentBaseState.fullPathHash == PlayerState.SlidingState &&
                currentBaseState.fullPathHash == PlayerState.ReloadingState)
                return;

            if (weaponHandler.CanAttack())
            {
                if (weaponHandler.GetEquipWeaponType() == Item.Weapon.WeaponType.Melee)
                {
                    characterAnimator.PlayAttack();
                }


                weaponHandler.Attack();
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
                    TakeDamage(weapon.damage);
                    characterAnimator.PlayTakeDamage();
                }
            }
        }
    }
}