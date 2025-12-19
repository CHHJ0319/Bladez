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

        private float hp = 100f;
        private int ammo = 100;

        private bool useCurves = true;
        private float useCurvesHeight = 0.5f;

        private Vector3 velocity;
        Quaternion rotation = Quaternion.identity;

        private float orgColHight;
        private Vector3 orgVectColCenter;

        private WeaponHandler weaponHandler;
        private PlayerInput input;
        private PlayerAnimator anim;

        private Rigidbody rb;
        private AnimatorStateInfo currentBaseState;
        private AnimatorStateInfo currentUpperBodyState;
        private CapsuleCollider col;
        private float capsuleRadius;
        private int collisionLayerMask;

        static int idleState = Animator.StringToHash("Base Layer.Idle");
        static int locoState = Animator.StringToHash("Base Layer.RUN");
        static int jumpState = Animator.StringToHash("Base Layer.Jump");
        static int SlidingState = Animator.StringToHash("Base Layer.Sliding");

        static int ReloadingState = Animator.StringToHash("UpperBody.Reloading");

        void Start()
        {
            input = GetComponent<PlayerInput>();
            anim = GetComponent<PlayerAnimator>();
            weaponHandler = GetComponent<WeaponHandler>();

            rb = GetComponent<Rigidbody>();

            col = GetComponent<CapsuleCollider>();
            orgColHight = col.height;
            orgVectColCenter = col.center;
            capsuleRadius = col.radius;

            Events.PlayerEvents.OnAttack += Attack;
            Events.PlayerEvents.OnJump += Jump;
            Events.PlayerEvents.OnSliding += Sliding;
            Events.PlayerEvents.OnReload += Reload;


            collisionLayerMask = LayerMask.GetMask("Wall");
        }

        private void Update()
        {
            weaponHandler.UpdateFireTimer();
        }

        void FixedUpdate()
        {
            FreezeRotation();

            currentBaseState = anim.GetBaseLayerState();
            currentUpperBodyState = anim.GetUpperBodyState();
            rb.useGravity = true;


            CalculateMoveAndRotate();

            HandleStateSpecificLogic();
        }

        void OnAnimatorMove()
        {
            if (currentUpperBodyState.fullPathHash == ReloadingState)
            {
            }
            else if (currentBaseState.fullPathHash == locoState || currentBaseState.fullPathHash == SlidingState)
            {
                float speedFactor = (currentBaseState.fullPathHash == SlidingState) ? slidingSpeed : forwardSpeed;
                Vector3 deltaMovement = velocity * anim.GetAnimationDistance() * speedFactor;

                float moveDistance = deltaMovement.magnitude;
                Vector3 moveDirection = deltaMovement.normalized;

                RaycastHit hit;

                Vector3 sphereOrigin = rb.position + Vector3.up * col.center.y;

                if (Physics.SphereCast(sphereOrigin, capsuleRadius, moveDirection, out hit, moveDistance, collisionLayerMask))
                {
                    float safeDistance = hit.distance - 0.001f;

                    if (safeDistance > 0)
                    {
                        deltaMovement = moveDirection * safeDistance;
                    }
                    else
                    {
                        deltaMovement = Vector3.zero;
                    }
                }

                rb.MovePosition(rb.position + deltaMovement);
                rb.MoveRotation(rotation);
            }
            else if (currentBaseState.fullPathHash == jumpState)
            {
                Vector3 desiredMove = velocity * anim.GetAnimationDistance();
                float yMovement = anim.GetVerticalDelta();
                Vector3 moveDelta = desiredMove + Vector3.up * yMovement;
                rb.MovePosition(rb.position + moveDelta);
                rb.MoveRotation(rotation);
            }
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
                currentBaseState.fullPathHash == SlidingState && 
                currentBaseState.fullPathHash == ReloadingState)
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
                currentBaseState.fullPathHash == SlidingState)
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

        void CalculateMoveAndRotate()
        {
            float horizontal = input.MoveInput.x;
            float vertical = input.MoveInput.y;

            float speed = new Vector2(horizontal, vertical).magnitude;

            if (currentUpperBodyState.fullPathHash == ReloadingState)
            {
                speed = 0f;
            }

            anim.PlayWalking(speed > 0.01f);

            velocity.Set(horizontal, 0f, vertical);
            velocity.Normalize();

            Vector3 desiredForward = Vector3.RotateTowards(transform.forward, velocity, rotateSpeed * Time.deltaTime, 0f);
            rotation = Quaternion.LookRotation(desiredForward);
        }

        void FreezeRotation()
        {
            rb.angularVelocity = Vector3.zero;
        }

        void HandleStateSpecificLogic()
        {
            if (currentUpperBodyState.fullPathHash == ReloadingState
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
                        float gravityControl = anim.GetGravityControlt();
                        if (gravityControl > 0)
                            rb.useGravity = false;

                        Ray ray = new Ray(transform.position + Vector3.up, -Vector3.up);
                        RaycastHit hitInfo = new RaycastHit();

                        if (Physics.Raycast(ray, out hitInfo))
                        {
                            if (hitInfo.distance > useCurvesHeight)
                            {
                                col.height = orgColHight - jumpHeight;
                                float adjCenterY = orgVectColCenter.y + jumpHeight;
                                col.center = new Vector3(0, adjCenterY, 0);
                            }
                            else
                            {
                                //resetCollider();
                            }
                        }
                    }
                    anim.StopJump();
                }
            }

            else if (currentBaseState.fullPathHash == SlidingState)
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
            col.height = orgColHight;
            col.center = orgVectColCenter;
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
