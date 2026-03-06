using System;
using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Actor.Player
{
    public class NetworkCharacterController : NetworkBehaviour
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
            //SubscribeNetworkVariables();

            if (IsOwner)
            {
                playerInput.enabled = true;
            }
            else
            {
                playerInput.enabled = false;
            }

            int currentPlayerCount = ActorManager.Instance.GetPlayerCount();
            //ActorManager.Instance.AddPlayer(this);
        }

        void FixedUpdate()
        {
            //base.FixedUpdate();

            float h = playerInputHandler.Horizontal;
            float v = playerInputHandler.Vertical;

            MoveWithPlayerInput(h, v);
            JumpWithPlayerInput();
            SlidingWithPlayerInput();
            AttackWithPlayerInput();
			InteractWithPlayerInput();
            QuiclSlotWithPlayerInput();
        }

        void Initialize()
        {
            //base.Initialize();

            playerInput = GetComponent<PlayerInput>();
            playerInputHandler = GetComponent<PlayerInputHandler>();

            OnSceneLoaded();
        }

        //protected override void SubscribeNetworkVariables()
        //{
        //    base.SubscribeNetworkVariables();
        //}

        //protected override void UnubscribeNetworkVariables()
        //{
        //    base.UnubscribeNetworkVariables();
        //}

        private void MoveWithPlayerInput(float horizontal, float vertical)
        {
            //CalculateVelocity(vertical);
            //if(GameManager.Instance.GetCurrentScene() != "DuelLobbyScene")
            //{
            //    transform.localPosition += velocity * Time.fixedDeltaTime;
            //}
            //transform.Rotate(0, horizontal * rotateSpeed, 0);

            //if (IsOwner)
            //{
            //    characterAnimator.UpdateMovementAnimation(horizontal, vertical);
            //    SubmitTransformRequestServerRpc(transform.localPosition, transform.localRotation);
            //}
        }

        private void JumpWithPlayerInput()
        {
            //if (playerInputHandler.JumpTriggered)
            //{
            //    Jump();
            //}
        }

        private void SlidingWithPlayerInput()
        {
            //if (playerInputHandler.SlidingTriggered)
            //{
            //    Sliding();
            //}
        }

        private void AttackWithPlayerInput()
        {
            //if (playerInputHandler.AttackTriggered)
            //{
            //    Attack();
            //}
        }

        private void InteractWithPlayerInput()
        {
            //if (playerInputHandler.InteractTriggered)
            //{
            //    if (GameManager.Instance.GetCurrentScene() == "DuelLobbyScene")
            //    {
            //        Rest();
            //    }
            //    else
            //    {
            //        PickUp();
            //    }
            //}
        }

        private void QuiclSlotWithPlayerInput()
        {
            //if (playerInputHandler.Quiick1Triggered)
            //{
            //    EquipWeapon(0);
            //}
            
            //if (playerInputHandler.Quiick2Triggered)
            //{
            //    EquipWeapon(1);
            //}
            
            //if (playerInputHandler.Quiick3Triggered)
            //{
            //    EquipWeapon(2);
            //}
        }

        public void OnSceneLoaded()
        {
            if (Util.SceneChecker.CheckCurrnetScene(Util.SceneList.DuelLobbyScene))
            {
                tpsCamera.gameObject.SetActive(false);

                //Transform lobbyTransform = ActorManager.Instance.GetLobbyPlayerTransform();
                //transform.localPosition = lobbyTransform.localPosition;
                //transform.localRotation = lobbyTransform.rotation;

                if (ActorManager.Instance.GetPlayerCount() == 0)
                {
                    IsDuelHost = true;
                }

                if (IsOwner)
                {
                    //UIManager.Instance.InitializDuelLobbySceneUI(IsDuelHost);
                }
            }
            else if(Util.SceneChecker.CheckCurrnetScene(Util.SceneList.DuelScene))
            {
                //MoveToRandomPosition();

                //if (IsOwner)
                //{
                //    Events.PlayerEvents.UpdateHPBar(HP.Value, maxHP);

                //    tpsCamera.gameObject.SetActive(true);
                //}
            }  
        }

        private void CalculateVelocity(float vertical)
        {
            //velocity = new Vector3(0, 0, vertical);
            //velocity = transform.TransformDirection(velocity);

            //if (vertical > 0.1f)
            //{
            //    velocity *= forwardSpeed;
            //}
            //else if (vertical < -0.1f)
            //{
            //    velocity *= backwardSpeed;
            //}
        }
    }
}


