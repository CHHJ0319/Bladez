using Actor.Player;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Actor
{
    public abstract class CharacterController : NetworkBehaviour
    {
        [Header("Properties")]
        public float forwardSpeed = 7.0f;
        public float backwardSpeed = 2.0f;
        public float rotateSpeed = 2.0f;
        public float jumpPower = 3.0f;
        public float slidingPower = 1.0f;

        public bool useCurves = true;
        public float useCurvesHeight = 0.5f;

        protected CharacterAnimator characterAnimator;
        private WeaponHandler weaponHandler;
        private ItemPicker itemPicker;

        private CapsuleCollider col;
        private Rigidbody rb;

        private float orgColHeight;
        private Vector3 orgVectColCenter;

        private AnimatorStateInfo currentBaseState;
        private AnimatorStateInfo currentUpperBodyState;

        protected Vector3 velocity;

        protected float maxHP = 100f;

        public NetworkVariable<Vector3> Position = new NetworkVariable<Vector3>();
        public NetworkVariable<Quaternion> Rotation = new NetworkVariable<Quaternion>();

        public NetworkVariable<bool> PickUpTriggered = new NetworkVariable<bool>();
        public NetworkVariable<bool> TakeDamageTriggered = new NetworkVariable<bool>();

        protected NetworkVariable<float> HP = new NetworkVariable<float>();

        public string OwnerID { get; private set; }

        public override void OnNetworkSpawn()
        {
            SubscribeNetworkVariables();
            Initialize();
        }

        public override void OnNetworkDespawn()
        {
            UnubscribeNetworkVariables();
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

        public void Jump()
        {
            if (currentBaseState.fullPathHash == PlayerState.LocoState
                && !characterAnimator.IsTransitioning())
            {
                rb.AddForce(Vector3.up * jumpPower, ForceMode.VelocityChange);

                if (IsOwner)
                {
                    characterAnimator.SetJump(true);
                    SubmitTransformRequestServerRpc(transform.localPosition, transform.localRotation);
                }
            }
        }

        public void Sliding()
        {
            if (currentBaseState.fullPathHash == PlayerState.LocoState
                && !characterAnimator.IsTransitioning())
            {
                rb.AddForce(velocity * slidingPower, ForceMode.VelocityChange);

                if (IsOwner)
                {
                    characterAnimator.SetSliding(true);
                    SubmitTransformRequestServerRpc(transform.localPosition, transform.localRotation);
                }
            }
        }

        public void Attack()
        {
            if (currentBaseState.fullPathHash == PlayerState.JumpState &&
                currentBaseState.fullPathHash == PlayerState.SlidingState)
                return;

            if (weaponHandler.EquippedWeapon == null)
                return;

            if (weaponHandler.GetEquipWeaponType() == Item.Weapon.WeaponType.Melee)
            {
                if (IsOwner)
                {
                    characterAnimator.PlayAttack();
                }
            }

            weaponHandler.Attack();
        }

        public void PickUp()
        {
            GameObject item = itemPicker.GetPickedUpItem();

            if (item == null) return;

            if (IsOwner)
                SubmitPickUpRequestServerRpc();

            if (item.tag == "Weapon")
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
            if(IsOwner)
            {
                float hp = HP.Value;
                hp -= damage;
                SubmitHPRequestServerRpc(hp);
            }

            if (HP.Value < 0)
            {
                Die();
            }

            characterAnimator.SetTakeDamage(true);
            ApplyKnockback(-damageDirection, knockbackForce);
        }

        public void Rest()
        {
            if (!characterAnimator.IsTransitioning())
            {
                characterAnimator.SetRest(true);
            }
        }

        public void SetHP()
        {
            if (IsOwner)
            {
                UIManager.Instance.UpdatePlayerHPBar(HP.Value, maxHP);
            }
        }

        public void Die()
        {
            Destroy(gameObject);
        }

        public void AssignWeaponOwnerID()
        {
            weaponHandler.AssignOwnerId(OwnerID);
        }

        protected virtual void Initialize()
        {
            SetupComponents();

            orgColHeight = col.height;
            orgVectColCenter = col.center;

            OwnerID = OwnerClientId.ToString();

            SubmitHPRequestServerRpc(maxHP);
        }

        protected virtual void SubscribeNetworkVariables()
        {
            Position.OnValueChanged += OnPositionChanged;
            Rotation.OnValueChanged += OnRotationChanged;

            PickUpTriggered.OnValueChanged += OnPickUpTriggeredChanged;
            TakeDamageTriggered.OnValueChanged += OnTakeDamageTriggeredChanged;

            HP.OnValueChanged += OnHPChanged;
        }

        protected virtual void UnubscribeNetworkVariables()
        {
            Position.OnValueChanged -= OnPositionChanged;
            Rotation.OnValueChanged -= OnRotationChanged;

            PickUpTriggered.OnValueChanged -= OnPickUpTriggeredChanged;
            TakeDamageTriggered.OnValueChanged -= OnTakeDamageTriggeredChanged;

            HP.OnValueChanged -= OnHPChanged;
        }

        private void SetupComponents()
        {
            col = GetComponent<CapsuleCollider>();
            rb = GetComponent<Rigidbody>();

            weaponHandler = GetComponent<WeaponHandler>();
            itemPicker = GetComponent<ItemPicker>();
            characterAnimator = GetComponent<CharacterAnimator>();
        }

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
            else if (currentBaseState.fullPathHash == PlayerState.RestState)
            {
                if (!characterAnimator.IsTransitioning())
                {
                    characterAnimator.SetRest(false);
                }
            }
            else if (currentBaseState.fullPathHash == PlayerState.TakeDamageState)
            {
                if (!characterAnimator.IsTransitioning())
                {
                    characterAnimator.SetTakeDamage(false);
                }
            }

            if (currentUpperBodyState.fullPathHash == PlayerState.AttackState1 
                || currentUpperBodyState.fullPathHash == PlayerState.AttackState2
                )
            {
                if (!characterAnimator.IsTransitioning())
                {
                    characterAnimator.StopAttack();
                }
            }
        }

        private void OnPositionChanged(Vector3 previous, Vector3 current)
        {
            if (Position.Value != previous)
            {
                if (!IsOwner)
                {
                    transform.position = Position.Value;
                }
            }
        }

        private void OnRotationChanged(Quaternion previous, Quaternion current)
        {
            if (Rotation.Value != previous)
            {
                if (!IsOwner)
                {
                    transform.rotation = Rotation.Value;
                }
            }
        }

        private void OnPickUpTriggeredChanged(bool previous, bool current)
        {
            if (PickUpTriggered.Value != previous && !IsOwner)
            {
                PickUp();
            }
        }

        private void OnTakeDamageTriggeredChanged(bool previous, bool current)
        {
            if (TakeDamageTriggered.Value != previous)
            {
                characterAnimator.SetTakeDamage(true);
            }
        }

        private void OnHPChanged(float previous, float current)
        {
            if (HP.Value != previous && !IsOwner)
            {
                SetHP();
            }
        }

        private void ApplyKnockback(Vector3 knockbackDirection, float knockbackForce)
        {
            knockbackDirection.y = 0.1f;

            rb.AddForce(knockbackDirection * knockbackForce, ForceMode.VelocityChange);
        }

        private void ResetCollider()
        {
            col.height = orgColHeight;
            col.center = orgVectColCenter;
        }

        [Rpc(SendTo.Server)]
        protected void SubmitTransformRequestServerRpc(Vector3 position, Quaternion rotation = default, RpcParams rpcParams = default)
        {
            Position.Value = position;
            Rotation.Value = rotation;
        }

        [Rpc(SendTo.Server)]
        protected void SubmitPickUpRequestServerRpc(RpcParams rpcParams = default)
        {
            PickUpTriggered.Value = !PickUpTriggered.Value;
        }

        [Rpc(SendTo.Server)]
        public void SubmitTakeDamageRequestServerRpc(RpcParams rpcParams = default)
        {
            TakeDamageTriggered.Value = !TakeDamageTriggered.Value;
        }

        [Rpc(SendTo.Server)]
        protected void SubmitHPRequestServerRpc(float hp, RpcParams rpcParams = default)
        {
            this.HP.Value = hp;
        }

        protected void EquipWeapon(int weaponIdx)
        {
            weaponHandler.EquipWeapon(weaponIdx);
        }

        private static Vector3 GetRandomPositionOnPlane()
        {
            return new Vector3(Random.Range(-3f, 3f), 1f, Random.Range(-3f, 3f));
        }


        protected void MoveToRandomPosition()
        {
            Vector3 pos = GetRandomPositionOnPlane();
            transform.position = pos;
            SubmitTransformRequestServerRpc(pos);
        }
    }
}