using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

namespace Player
{
    [RequireComponent(typeof(PlayerInput))]
    [RequireComponent(typeof(WeaponHandler))]
    public class PlayerController : MonoBehaviour
    {
        public GameEnding gameEnding;

        public float animSpeed = 1.5f;
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

        private Rigidbody rb;
        private Animator anim;
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
            weaponHandler = GetComponent<WeaponHandler>();

            anim = GetComponent<Animator>();
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

            float horizontal = input.MoveInput.x;
            float vertical = input.MoveInput.y;

            InitAnim(horizontal, vertical);

            currentBaseState = anim.GetCurrentAnimatorStateInfo(0);
            currentUpperBodyState = anim.GetCurrentAnimatorStateInfo(2);
            rb.useGravity = true;


            CalculateMoveAndRotate(horizontal, vertical);

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
                Vector3 deltaMovement = velocity * anim.deltaPosition.magnitude * speedFactor;

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
                Vector3 desiredMove = velocity * anim.deltaPosition.magnitude;
                float yMovement = anim.deltaPosition.y;
                Vector3 moveDelta = desiredMove + Vector3.up * yMovement;
                rb.MovePosition(rb.position + moveDelta);
                rb.MoveRotation(rotation);
            }
        }


        void Jump()
        {
            if (currentBaseState.fullPathHash == locoState)
            {
                if (!anim.IsInTransition(0))
                {
                    anim.SetBool("Jump", true);
                }
            }
        }

        void Sliding()
        {
            if (currentBaseState.fullPathHash == locoState)
            {
                if (!anim.IsInTransition(0))
                {
                    anim.SetBool("Sliding", true);
                }
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
                anim.SetTrigger("Shot");
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
                if (!anim.IsInTransition(0))
                {
                    anim.SetBool("Reload", true);
                    weaponHandler.Reload(ref ammo);
                }
            }
        }


        void InitAnim(float h, float v)
        {
            anim.speed = animSpeed;

            float speed = new Vector2(h, v).magnitude;

            if (currentUpperBodyState.fullPathHash == ReloadingState)
            {
                speed = 0f;
            }

            anim.SetBool("IsWalking", speed > 0.01f);
        }

        void CalculateMoveAndRotate(float h, float v)
        {
            velocity.Set(h, 0f, v);
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
            if (currentUpperBodyState.fullPathHash == ReloadingState)
            {
                if (!anim.IsInTransition(0))
                {
                    anim.SetBool("Reload", false);
                }
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
                if (!anim.IsInTransition(0))
                {
                    if (useCurves)
                    {
                        float jumpHeight = anim.GetFloat("JumpHeight");
                        float gravityControl = anim.GetFloat("GravityControl");
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
                    anim.SetBool("Jump", false);
                }
            }

            else if (currentBaseState.fullPathHash == SlidingState)
            {
                if (!anim.IsInTransition(0))
                {
                    anim.SetBool("Sliding", false);
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
