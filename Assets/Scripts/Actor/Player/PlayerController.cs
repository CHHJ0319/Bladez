using Actor.UI;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Actor.Player
{
    public class PlayerController : CharacterController
    {
        public CinemachineCamera tpsCamera;

        [Header("UI")]
        public GameObject hpBarPrefab;
        public GameObject staminaBarPrefab;

        private PlayerInput playerInput;

        private PlayerInputHandler playerInputHandler;
  
		private string currentScene;

        public bool IsDuelHost { get; private set; } = false;

		public override void OnNetworkSpawn()
        {
            Initialize();
            SubscribeNetworkVariables();

            if (SceneManager.GetActiveScene().name == "DuelLobbyScene")
            {
                tpsCamera.gameObject.SetActive(false);

                Transform lobbyTransform = ActorManager.Instance.GetLobbyPlayerTransform(IsOwner);
                transform.localPosition = lobbyTransform.localPosition;
                transform.localRotation = lobbyTransform.rotation;
            }

            if (IsOwner)
            {
                playerInput.enabled = true;

                UIManager.Instance.UpdatePlayerHPBar(HP.Value, maxHP);

                if (SceneManager.GetActiveScene().name == "TestScene")
                {
                    MoveToRandomPosition();
                }
            }
            else
            {
                tpsCamera.gameObject.SetActive(false);
                playerInput.enabled = false;
            }
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
        }

        protected override void Initialize()
        {
            base.Initialize();

            playerInput = GetComponent<PlayerInput>();
            playerInputHandler = GetComponent<PlayerInputHandler>();

            currentScene = SceneManager.GetActiveScene().name;

            if (currentScene == "DuelLobbyScene")
            {
                if (ActorManager.Instance.GetCurrentPlayerCount() == 0)
                {
                    IsDuelHost = true;
                }

                if(IsOwner)
                {
                    UIManager.Instance.UpdateLobbyPlayerUI(IsDuelHost);
                }
            }
        }

        private void MoveWithPlayerInput(float horizontal, float vertical)
        {
            CalculateVelocity(vertical);
            if(currentScene != "DuelLobbyScene")
            {
                transform.localPosition += velocity * Time.fixedDeltaTime;
            }
            transform.Rotate(0, horizontal * rotateSpeed, 0);

            if (IsOwner)
            {
                characterAnimator.UpdateMovementAnimation(horizontal, vertical);
                SubmitTransfromRequestServerRpc(transform.localPosition, transform.localRotation);
            }
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
                if (currentScene == "DuelLobbyScene")
                {
                    Rest();
                }
                else
                {
                    PickUp();
                }
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


