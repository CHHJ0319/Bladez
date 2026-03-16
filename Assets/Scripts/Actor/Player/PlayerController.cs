using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Actor.Player
{
    public class PlayerController : CharacterController
    {
        [Header("Player Properties")]
        public CinemachineCamera tpsCamera;
        public float mouseSensitivity = 0.5f;

        private PlayerInput playerInput;
        private PlayerInputHandler playerInputHandler;
        private PlayerTransform playerTransform;
  
        public bool IsDuelHost { get; private set; } = false;
        public bool IsReady { get; set; } = false;

        private Vector2 lookInput;

        private float horizontal;
        private float vertical;

        protected override void Awake()
        {
            Initialize();

            playerInput = GetComponent<PlayerInput>();
            playerInputHandler = GetComponent<PlayerInputHandler>();
            playerTransform = GetComponent<PlayerTransform>();
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        public override void OnNetworkSpawn()
        {
            if(IsHost)
            {
                IsDuelHost = true;
                playerID.Value = (int)NetworkObjectId;
            }
            else
            {
                RequestSetPlayerIDServerRpc();
            }

            if (IsOwner)
            {
                playerInput.enabled = true;
                tpsCamera.gameObject.SetActive(true);

                ActorManager.Instance.SetOwnerPlayer(this);
                weaponHandler.AssignOwnerId(playerID.Value);
            }
            else
            {
                playerInput.enabled = false;
                tpsCamera.gameObject.SetActive(false);
            }

            if (Util.SceneChecker.CheckCurrnetScene(Util.SceneList.DuelLobbyScene))
            {
                tpsCamera.gameObject.SetActive(false);

                Transform lobbyTransform = ActorManager.Instance.GetDuelLobbyPlayerTransform();
                transform.localPosition = lobbyTransform.localPosition;
                transform.localRotation = lobbyTransform.rotation;
            }
            DuelManager.Instance.AddPlayer(this);
        }

        public override void OnNetworkDespawn()
        {
        }

        protected override void Update()
        {
            base.Update();

            horizontal = playerInputHandler.MoveInput.x;
            vertical = playerInputHandler.MoveInput.y;

            lookInput = playerInputHandler.LookInput;

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

        #region OnSceneLoaded
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (Util.SceneChecker.CheckCurrnetScene(Util.SceneList.DuelLobbyScene))
            {
                tpsCamera.gameObject.SetActive(false);

                Transform lobbyTransform = ActorManager.Instance.GetDuelLobbyPlayerTransform();
                transform.localPosition = lobbyTransform.localPosition;
                transform.localRotation = lobbyTransform.rotation;
            }
            else if (Util.SceneChecker.CheckCurrnetScene(Util.SceneList.DuelScene))
            {
                if(IsOwner)
                {
                    tpsCamera.gameObject.SetActive(true);
                    MoveToRandomPosition();
                }
            }
        }
        #endregion

        #region Movement
        private void MoveWithPlayerInput()
        {
            if (IsOwner)
            {
                float yaw = lookInput.x * mouseSensitivity;
                Quaternion deltaRotation = Quaternion.Euler(0, yaw, 0);
                rb.MoveRotation(rb.rotation * deltaRotation);

                characterAnimator.UpdateMovementAnimation(horizontal, vertical);

                if (!Util.SceneChecker.CheckCurrnetScene(Util.SceneList.DuelLobbyScene))
                {
                    Vector3 currentVelocity = rb.linearVelocity;
                    Vector3 newVelocity = new Vector3(velocity.x, currentVelocity.y, velocity.z);
                    rb.linearVelocity = newVelocity;
                }
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

        private void CalculateVelocity()
        {
            horizontal = playerInputHandler.MoveInput.x;
            vertical = playerInputHandler.MoveInput.y;

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
        #endregion

        [Rpc(SendTo.Server)]
        private void RequestSetPlayerIDServerRpc(RpcParams rpcParams = default)
        {
            playerID.Value = (int) NetworkObjectId;

            weaponHandler.AssignOwnerId(playerID.Value);
        }
    }
}


