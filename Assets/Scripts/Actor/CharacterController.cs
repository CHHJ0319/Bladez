using Actor.Player;
using Actor.UI;
using UnityEngine;

namespace Actor
{
    public abstract class CharaterController : MonoBehaviour
    {
        [Header("Settings")]
        public float forwardSpeed = 7.0f;
        public float backwardSpeed = 2.0f;
        public float rotateSpeed = 2.0f;
        public float jumpPower = 3.0f;
        public float slidingPower = 1.0f;

        public bool useCurves = true;
        public float useCurvesHeight = 0.5f;

        protected CharacterNetworkHandler characterNetworkHandler;
        protected CharacterNetworkAnimator characterNetworkAnimator;

        private WeaponHandler weaponHandler;
        private ItemPicker itemPicker;

        private CapsuleCollider col;
        private Rigidbody rb;

        private float orgColHeight;
        private Vector3 orgVectColCenter;
        private AnimatorStateInfo currentBaseState;
        private AnimatorStateInfo currentUpperBodyState;

        protected Vector3 velocity;

        protected float hp = 100f;
        protected float maxHP = 100f;

        protected GaugeBar hpBar;
        protected GaugeBar staminaBar;



        protected virtual void Awake()
        {
            col = GetComponent<CapsuleCollider>();
            rb = GetComponent<Rigidbody>();
            
            weaponHandler = GetComponent<WeaponHandler>();
            itemPicker = GetComponent<ItemPicker>();

            orgColHeight = col.height;
            orgVectColCenter = col.center;

            characterNetworkHandler = GetComponent<CharacterNetworkHandler>();
            characterNetworkAnimator = GetComponent<CharacterNetworkAnimator>();
        }

        protected virtual void Update()
        {
            weaponHandler.UpdateAttackTimer();
        }

        protected virtual void FixedUpdate()
        {
            UpdateAnimationState();
            SetGravity(true);
            UpdateStateBehavior();
        }

        void UpdateAnimationState()
        {
            currentBaseState = characterNetworkAnimator.GetBaseLayerState();
            currentUpperBodyState = characterNetworkAnimator.GetUpperBodyState();
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
                if (!characterNetworkAnimator.IsTransitioning())
                {
                    if (useCurves)
                    {
                        float jumpHeight = characterNetworkAnimator.GetJumpHeight();
                        float gravityControl = characterNetworkAnimator.GetGravityControl();
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
                    characterNetworkAnimator.SetJump(false);
                }
            }

            else if (currentBaseState.fullPathHash == PlayerState.SlidingState)
            {
                if (!characterNetworkAnimator.IsTransitioning())
                {
                    characterNetworkAnimator.SetSliding(false);
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
                if (!characterNetworkAnimator.IsTransitioning())
                {
                    characterNetworkAnimator.SetJump(false);
                }
            }
        }

        void ResetCollider()
        {
            col.height = orgColHeight;
            col.center = orgVectColCenter;
        }

        public void Jump()
        {
            if (currentBaseState.fullPathHash == PlayerState.LocoState
                && !characterNetworkAnimator.IsTransitioning())
            {
                rb.AddForce(Vector3.up * jumpPower, ForceMode.VelocityChange);
                characterNetworkAnimator.SetJump(true);
            }
        }

        public void Sliding()
        {
            if (currentBaseState.fullPathHash == PlayerState.LocoState
                && !characterNetworkAnimator.IsTransitioning())
            {
                rb.AddForce(velocity * slidingPower, ForceMode.VelocityChange);
                characterNetworkAnimator.SetSliding(true);
            }
        }

        public void Attack()
        {
            if (currentBaseState.fullPathHash == PlayerState.JumpState &&
                currentBaseState.fullPathHash == PlayerState.SlidingState &&
                currentBaseState.fullPathHash == PlayerState.ReloadingState)
                return;

            if (weaponHandler.EquippedWeapon == null)
                return;

            if (weaponHandler.GetEquipWeaponType() == Item.Weapon.WeaponType.Melee)
            {
                characterNetworkAnimator.PlayAttack();
            }

            weaponHandler.Attack();
        }

        protected void PickUp()
        {
            GameObject item = itemPicker.GetPickedUpItem();

            if (item == null) return;

            if(item.tag == "Weapon")
            {
                if (weaponHandler.CanAddWeapon())
                {
                    weaponHandler.AddWeapon(item);
                    itemPicker.Clear();
                }
            }
        }

        public void TakeDamage(float damage, Vector3 damageDirection, float knockbackForce)
        {
            if(characterNetworkHandler.IsOwner)
            {
                hp -= damage;
                SetHP(hp);
                characterNetworkHandler.SubmitHPRequestServerRpc(hp);

                if (hp < 0)
                {
                    Die();
                }
            }

            characterNetworkAnimator.PlayTakeDamage();
            ApplyKnockback(-damageDirection, knockbackForce);
        }

        public void SetHP(float hp)
        {
            this.hp = hp;

            if(characterNetworkHandler.IsOwner)
            {
                hpBar.UpdateGaugeBar(hp, maxHP);
            }
        }

        public void Die()
        {
            Destroy(gameObject);
        }

        private void ApplyKnockback(Vector3 knockbackDirection, float knockbackForce)
        {
            knockbackDirection.y = 0.1f;

            rb.AddForce(knockbackDirection * knockbackForce, ForceMode.VelocityChange);
        }

        protected void EquipWeapon(int weaponIdx)
        {
            weaponHandler.EquipWeapon(weaponIdx);
        }
    }
}