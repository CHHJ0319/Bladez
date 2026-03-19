using Unity.Netcode;
using UnityEngine;

namespace Actor
{
    public abstract class CharacterController : NetworkBehaviour
    {
        [Header("Character Properties")]
        public float forwardSpeed = 7.0f;
        public float backwardSpeed = 2.0f;
        public float rotateSpeed = 2.0f;
        public float jumpPower = 3.0f;
        public float slidingPower = 1.0f;

        public bool useCurves = true;
        public float useCurvesHeight = 0.5f;

        protected CharacterAnimator characterAnimator;
        protected WeaponHandler weaponHandler;
        private ItemPicker itemPicker;

        private CapsuleCollider col;
        protected Rigidbody rb;

        private float orgColHeight;
        private Vector3 orgVectColCenter;

        private AnimatorStateInfo currentBaseState;
        private AnimatorStateInfo currentUpperBodyState;

        protected Vector3 velocity;

        protected float maxHP = 100f;
        public float HP { get; private set; }

        protected virtual void Awake()
        {
            Initialize();
        }

        protected void Initialize()
        {
            SetComponents();

            orgColHeight = col.height;
            orgVectColCenter = col.center;
        }

        private void SetComponents()
        {
            col = GetComponent<CapsuleCollider>();
            rb = GetComponent<Rigidbody>();

            characterAnimator = GetComponent<CharacterAnimator>();

            weaponHandler = GetComponent<WeaponHandler>();
            itemPicker = GetComponent<ItemPicker>();
        }

        protected virtual void Update()
        {
            weaponHandler.UpdateAttackTimer();

            UpdateAnimationState();
            SetGravity(true);
            UpdateStateBehavior();
        }

        #region Update
        private void UpdateAnimationState()
        {
            currentBaseState = characterAnimator.GetBaseLayerState();
            currentUpperBodyState = characterAnimator.GetUpperBodyState();
        }

        private void SetGravity(bool active)
        {
            rb.useGravity = active;
        }

        private void UpdateStateBehavior()
        {
            if (currentBaseState.fullPathHash == CharacterAnimationState.LocoState)
            {
                if (useCurves)
                {
                    ResetCollider();
                }
            }

            else if (currentBaseState.fullPathHash == CharacterAnimationState.JumpState)
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

            else if (currentBaseState.fullPathHash == CharacterAnimationState.SlidingState)
            {
                if (!characterAnimator.IsTransitioning())
                {
                    characterAnimator.SetSliding(false);
                }
            }

            else if (currentBaseState.fullPathHash == CharacterAnimationState.IdleState)
            {
                if (useCurves)
                {
                    ResetCollider();
                }
            }
            else if (currentBaseState.fullPathHash == CharacterAnimationState.RestState)
            {
                if (!characterAnimator.IsTransitioning())
                {
                    characterAnimator.SetRest(false);
                }
            }
            else if (currentBaseState.fullPathHash == CharacterAnimationState.TakeDamageState)
            {
                if (!characterAnimator.IsTransitioning())
                {
                    characterAnimator.SetTakeDamage(false);
                }
            }

            if (currentUpperBodyState.fullPathHash == CharacterAnimationState.AttackState1
                || currentUpperBodyState.fullPathHash == CharacterAnimationState.AttackState2
                )
            {
                if (!characterAnimator.IsTransitioning())
                {
                    characterAnimator.StopAttack();
                }
            }
        }
        #endregion

        #region Movement
        public void Jump()
        {
            if (currentBaseState.fullPathHash == CharacterAnimationState.LocoState
                && !characterAnimator.IsTransitioning())
            {
                rb.AddForce(Vector3.up * jumpPower, ForceMode.VelocityChange);
                characterAnimator.SetJump(true);
            }
        }

        public void Sliding()
        {
            if (currentBaseState.fullPathHash == CharacterAnimationState.LocoState
                && !characterAnimator.IsTransitioning())
            {
                rb.AddForce(velocity * slidingPower, ForceMode.VelocityChange);
                characterAnimator.SetSliding(true);
            }
        }

        public void Attack()
        {
            if (currentBaseState.fullPathHash == CharacterAnimationState.JumpState &&
                currentBaseState.fullPathHash == CharacterAnimationState.SlidingState)
                return;

            if (weaponHandler.EquippedWeapon == null)
                return;

            if (weaponHandler.GetEquipWeaponType() == Item.Weapon.WeaponType.Melee)
            {
                characterAnimator.PlayAttack();
            }

            weaponHandler.Attack();
        }

        public void PickUp(int playerID)
        {
            GameObject item = itemPicker.GetPickedUpItem();

            if (item == null) return;

            if (item.tag == "Weapon")
            {
                if (weaponHandler.CanAddWeapon())
                {
                    GameObject weapon = item.transform.root.gameObject;
                    weaponHandler.AddWeapon(weapon, playerID);
                    itemPicker.Clear();
                }
            }
        }

        public void TakeDamage(float damage, Vector3 damageDirection, float knockbackForce)
        {
            HP -= damage;

            characterAnimator.SetTakeDamage(true);
            ApplyKnockback(-damageDirection, knockbackForce);

            if (HP < 0)
            {
                Die();
            }
        }

        public void Rest()
        {
            if (!characterAnimator.IsTransitioning())
            {
                characterAnimator.SetRest(true);
            }
        }

        private void Die()
        {
            UIManager.Instance.ShowPlayerResultUI(false);
            gameObject.SetActive(false);
        }

        private void ApplyKnockback(Vector3 knockbackDirection, float knockbackForce)
        {
            knockbackDirection.y = 0.1f;

            rb.AddForce(knockbackDirection * knockbackForce, ForceMode.VelocityChange);
        }
        #endregion

        protected void EquipWeapon(int weaponIdx)
        {
            weaponHandler.EquipWeapon(weaponIdx);
        }

        private void ResetCollider()
        {
            col.height = orgColHeight;
            col.center = orgVectColCenter;
        }

        private static Vector3 GetRandomPositionOnPlane()
        {
            return new Vector3(Random.Range(-3f, 3f), 1f, Random.Range(-3f, 3f));
        }


        protected void MoveToRandomPosition()
        {
            Vector3 pos = GetRandomPositionOnPlane();
            transform.position = pos;
        }
    }
}