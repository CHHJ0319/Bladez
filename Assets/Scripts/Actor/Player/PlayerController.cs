using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

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
  
        public bool IsDuelHost { get; private set; } = false;

		public override void OnNetworkSpawn()
        {
            Initialize();
            SubscribeNetworkVariables();

            if (IsOwner)
            {

                playerInput.enabled = true;
            }
            else
            {
                playerInput.enabled = false;
            }

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
        }

        protected override void Initialize()
        {
            base.Initialize();

            playerInput = GetComponent<PlayerInput>();
            playerInputHandler = GetComponent<PlayerInputHandler>();

            OnSceneLoaded();
        }

        protected override void SubscribeNetworkVariables()
        {
            base.SubscribeNetworkVariables();
        }

        protected override void UnubscribeNetworkVariables()
        {
            base.UnubscribeNetworkVariables();
        }

        private void MoveWithPlayerInput(float horizontal, float vertical)
        {
            CalculateVelocity(vertical);
            if(GameManager.Instance.GetCurrentScene() != "DuelLobbyScene")
            {
                transform.localPosition += velocity * Time.fixedDeltaTime;
            }
            transform.Rotate(0, horizontal * rotateSpeed, 0);

            if (IsOwner)
            {
                characterAnimator.UpdateMovementAnimation(horizontal, vertical);
                SubmitTransformRequestServerRpc(transform.localPosition, transform.localRotation);
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
                if (GameManager.Instance.GetCurrentScene() == "DuelLobbyScene")
                {
                    Rest();
                }
                else
                {
                    PickUp();
                }
            }
        }

        public void OnSceneLoaded()
        {
            if (GameManager.Instance.GetCurrentScene() == "DuelLobbyScene")
            {
                tpsCamera.gameObject.SetActive(false);

                Transform lobbyTransform = ActorManager.Instance.GetLobbyPlayerTransform();
                transform.localPosition = lobbyTransform.localPosition;
                transform.localRotation = lobbyTransform.rotation;

                if (ActorManager.Instance.GetCurrentPlayerCount() == 0)
                {
                    IsDuelHost = true;
                }

                if (IsOwner)
                {
                    UIManager.Instance.InitializePlayerUI(IsDuelHost);
                }
            }
            else if(GameManager.Instance.GetCurrentScene() == "DuelScene")
            {
                MoveToRandomPosition();

                if (IsOwner)
                {
                    UIManager.Instance.UpdatePlayerHPBar(HP.Value, maxHP);

                    tpsCamera.gameObject.SetActive(true);
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


