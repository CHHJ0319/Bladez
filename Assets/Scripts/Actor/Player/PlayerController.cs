using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Actor.Player
{
    public class PlayerController : CharacterController
    {
        [Header("Player Properties")]
        public CinemachineCamera tpsCamera;
        public float mouseSensitivity = 0.5f;

        private PlayerInput playerInput;
        private PlayerInputHandler playerInputHandler;
  
        public bool IsDuelHost { get; private set; } = false;

        private Vector2 lookInput;
        private float horizontal;
        private float vertical;


        protected override void Awake()
        {
            Initialize();

            playerInput = GetComponent<PlayerInput>();
            playerInputHandler = GetComponent<PlayerInputHandler>();

            OnSceneLoaded();

            //int currentPlayerCount = ActorManager.Instance.GetPlayerCount();
            //ActorManager.Instance.AddPlayer(this);
        }

        protected override void Update()
        {
            lookInput = playerInputHandler.LookInput;

            horizontal = playerInputHandler.Horizontal;
            vertical = playerInputHandler.Vertical;

            CalculateVelocity();
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            MoveWithPlayerInput();
            JumpWithPlayerInput();
            SlidingWithPlayerInput();
            AttackWithPlayerInput();
            InteractWithPlayerInput();
            QuiclSlotWithPlayerInput();
        }

        private void MoveWithPlayerInput()
        {
            characterAnimator.UpdateMovementAnimation(horizontal, vertical);

            if(!Util.SceneChecker.CheckCurrnetScene(Util.SceneList.DuelLobbyScene))
            {
                Vector3 currentVelocity = rb.linearVelocity;
                Vector3 newVelocity = new Vector3(velocity.x, currentVelocity.y, velocity.z);
                rb.linearVelocity = newVelocity;
            }

            float yaw = lookInput.x * mouseSensitivity;
            Quaternion deltaRotation = Quaternion.Euler(0, yaw, 0);
            rb.MoveRotation(rb.rotation * deltaRotation);

        }

        private void JumpWithPlayerInput()
        {
            if (playerInputHandler.JumpTriggered)
            {
                Jump();
            }
        }

        private void SlidingWithPlayerInput()
        {
            if (playerInputHandler.SlidingTriggered)
            {
                Sliding();
            }
        }

        private void AttackWithPlayerInput()
        {
            if (playerInputHandler.AttackTriggered)
            {
                Attack();
            }
        }

        private void InteractWithPlayerInput()
        {
            if (playerInputHandler.InteractTriggered)
            {
                if (Util.SceneChecker.CheckCurrnetScene(Util.SceneList.DuelLobbyScene))
                {
                    Rest();
                }
                else
                {
                    PickUp();
                }
            }
        }

        private void QuiclSlotWithPlayerInput()
        {
            if (playerInputHandler.Quiick1Triggered)
            {
                EquipWeapon(0);
            }
            
            if (playerInputHandler.Quiick2Triggered)
            {
                EquipWeapon(1);
            }
            
            if (playerInputHandler.Quiick3Triggered)
            {
                EquipWeapon(2);
            }
        }

        public void OnSceneLoaded()
        {
            if (Util.SceneChecker.CheckCurrnetScene(Util.SceneList.DuelLobbyScene))
            {
                tpsCamera.gameObject.SetActive(false);

                Transform lobbyTransform = ActorManager.Instance.GetLobbyPlayerTransform();
                transform.localPosition = lobbyTransform.localPosition;
                transform.localRotation = lobbyTransform.rotation;

                if (ActorManager.Instance.GetCurrentPlayerCount() == 0)
                {
                    IsDuelHost = true;
                }

                UIManager.Instance.InitializDuelLobbySceneUI(IsDuelHost);
            }
            else if(Util.SceneChecker.CheckCurrnetScene(Util.SceneList.DuelScene))
            {
                MoveToRandomPosition();

                tpsCamera.gameObject.SetActive(true);
            }  
        }

        private void CalculateVelocity()
        {
            velocity = (transform.forward * vertical) + (transform.right * horizontal);

            if (velocity.magnitude > 1f)
            {
                velocity.Normalize();
            }

            if (vertical > 0.1f)
            {
                velocity *= forwardSpeed;
            }
            else if (vertical < -0.1f)
            {
                velocity *= backwardSpeed;
            }
        }
    }
}


