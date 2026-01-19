using UnityEngine;

namespace Actor.Player
{
    [RequireComponent(typeof(ActorAnimator))]
    [RequireComponent(typeof(WeaponHandler))]
    [RequireComponent(typeof(PlayerInputHandler))]
    public class PlayerController : ActorController
    {
        public GameEnding gameEnding;
        public Transform thirdCamTranform;

        private PlayerInputHandler input;

        void OnEnable()
        {
            Events.PlayerEvents.OnJump += Jump;
            Events.PlayerEvents.OnSliding += Sliding;
            Events.PlayerEvents.OnAttack += Attack;
            Events.PlayerEvents.OnReload += Reload;
            Events.PlayerEvents.OnQuickSlotPressed += EquipWeapon;
        }

        protected override void Start()
        {
            base.Start();

            input = GetComponent<PlayerInputHandler>();
            input.LockCursor();
        }

        protected override void Update()
        {
            base.Update();

            ApplyRotation();
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            CalculateMove();
        }

        void ApplyRotation()
        {
            float targetRotation = thirdCamTranform.eulerAngles.y;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, targetRotation, 0), rotateSpeed * Time.deltaTime);
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

        public override void TakeDamage(float damage)
        {
            base.TakeDamage(damage);

            if (hp < 0)
            {
                gameEnding.CaughtPlayer();
            }
        }
    }
}
