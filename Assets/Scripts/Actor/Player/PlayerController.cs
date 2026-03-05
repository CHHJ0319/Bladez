using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Actor.Player
{
    public class PlayerController : CharacterController
    {
        [Header("Player Properties")]
        public CinemachineCamera tpsCamera;

        private PlayerInput playerInput;

        private PlayerInputHandler playerInputHandler;
  
        public bool IsDuelHost { get; private set; } = false;

		void Start()
        {
            Initialize();

            int currentPlayerCount = ActorManager.Instance.GetPlayerCount();
            ActorManager.Instance.AddPlayer(this);
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            float h = playerInputHandler.Horizontal;
            float v = playerInputHandler.Vertical;

            MoveWithPlayerInput(h, v);
            JumpWithPlayerInput();
            SlidingWithPlayerInput();
            AttackWithPlayerInput();
			InteractWithPlayerInput();
            QuiclSlotWithPlayerInput();
        }

        protected override void Initialize()
        {
            base.Initialize();

            playerInput = GetComponent<PlayerInput>();
            playerInputHandler = GetComponent<PlayerInputHandler>();

            OnSceneLoaded();
        }

        private void MoveWithPlayerInput(float horizontal, float vertical)
        {
            CalculateVelocity(vertical);
            if(!Util.SceneChecker.CheckCurrnetScene(Util.SceneList.DuelLobbyScene))
            {
                transform.localPosition += velocity * Time.fixedDeltaTime;
            }
            transform.Rotate(0, horizontal * rotateSpeed, 0);

            characterAnimator.UpdateMovementAnimation(horizontal, vertical);
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

        private void CalculateVelocity(float vertical)
        {
            velocity = new Vector3(0, 0, vertical);
            velocity = transform.TransformDirection(velocity);

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


