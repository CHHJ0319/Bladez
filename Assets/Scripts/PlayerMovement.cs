using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float animSpeed = 1.5f;

    public float forwardSpeed = 7.0f;
    public float rotateSpeed = 20.0f;
    public float slidingSpeed = 3.0f;

    public bool useCurves = true;
    public float useCurvesHeight = 0.5f;

    private Vector3 velocity;
    Quaternion rotation = Quaternion.identity;

    private float orgColHight;
    private Vector3 orgVectColCenter;

    private Rigidbody rb;
    private Animator anim;
    private AnimatorStateInfo currentBaseState;
    private CapsuleCollider col;

    static int idleState = Animator.StringToHash("Base Layer.Idle");
    static int locoState = Animator.StringToHash("Base Layer.RUN");
    static int jumpState = Animator.StringToHash("Base Layer.Jump");
    static int SlidingState = Animator.StringToHash("Base Layer.Sliding");
    static int restState = Animator.StringToHash("Base Layer.Rest");

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        col = GetComponent<CapsuleCollider>();
        orgColHight = col.height;
        orgVectColCenter = col.center;
    }

    void FixedUpdate()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        InitAnim(horizontal, vertical);

        currentBaseState = anim.GetCurrentAnimatorStateInfo(0);
        rb.useGravity = true;

        CalculateMoveAndRotate(horizontal, vertical);
        HandleStateSpecificLogic();
    }

    void OnAnimatorMove()
    {

        if (currentBaseState.fullPathHash == locoState || currentBaseState.fullPathHash == SlidingState)
        {
            rb.MovePosition(rb.position + velocity * anim.deltaPosition.magnitude * forwardSpeed);
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

        else if (currentBaseState.fullPathHash == SlidingState)
        {
            rb.MovePosition(rb.position + velocity * anim.deltaPosition.magnitude * slidingSpeed);
            rb.MoveRotation(rotation);
        }
    }

    void OnAttack(InputValue value)
    {
        if (value.isPressed)
        {
            Fire();
        }
    }

    void OnJump(InputValue value)
    {
        if (currentBaseState.fullPathHash == locoState)
        {
            if (!anim.IsInTransition(0))
            {
                anim.SetBool("Jump", true);
            }
        }
    }

    void OnSliding(InputValue value)
    {
        if (currentBaseState.fullPathHash == locoState)
        {
            if (!anim.IsInTransition(0))
            {
                anim.SetBool("Sliding", true);
            }
        }
    }

    void InitAnim(float h, float v)
    {
        anim.speed = animSpeed;

        float speed = new Vector2(h, v).magnitude;
        anim.SetBool("IsWalking", speed > 0.01f);
    }

    void CalculateMoveAndRotate(float h, float v)
    {
        velocity.Set(h, 0f, v);
        velocity.Normalize();

        Vector3 desiredForward = Vector3.RotateTowards(transform.forward, velocity, rotateSpeed * Time.deltaTime, 0f);
        rotation = Quaternion.LookRotation(desiredForward);
    }

    void Fire()
    {
        anim.SetTrigger("Shot");
        Debug.Log("공격! (Player Input 컴포넌트 사용)");
    }

    void HandleStateSpecificLogic()
    {
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

            if (Input.GetButtonDown("Jump"))
            {
                anim.SetBool("Rest", true);
            }
        }

        else if (currentBaseState.fullPathHash == restState)
        {

            if (!anim.IsInTransition(0))
            {
                anim.SetBool("Rest", false);
            }
        }
    }

    void resetCollider()
    {
        col.height = orgColHight;
        col.center = orgVectColCenter;
    }
}
