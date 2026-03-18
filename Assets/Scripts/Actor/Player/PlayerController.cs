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

        private PlayerInputHandler playerInputHandler;
        private PlayerTransform playerTransform;

        public NetworkVariable<int> playerID = new NetworkVariable<int>();
        public bool IsDuelHost { get; private set; } = false;
        public bool IsReady { get; set; } = false;

        protected override void Awake()
        {
            Initialize();

            playerInputHandler = GetComponent<PlayerInputHandler>();
            playerTransform = GetComponent<PlayerTransform>(); 
        }

        private void Start()
        {
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
                playerID.Value = (int)OwnerClientId;
            }
            else
            {
                RequestSetPlayerIDServerRpc();
            }

            weaponHandler.AddWeapons(playerID.Value);

            if (IsOwner)
            {
                playerInputHandler.SetPlayerInputEnabled(true);
                tpsCamera.gameObject.SetActive(true);

                ActorManager.Instance.SetOwnerPlayer(this);
            }
            else
            {
                playerInputHandler.SetPlayerInputEnabled(false);
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

            CalculateVelocity();
            JumpWithPlayerInput();
            SlidingWithPlayerInput();
            AttackWithPlayerInput();
            InteractWithPlayerInput();
            QuiclSlotWithPlayerInput();
        }

        private void FixedUpdate()
        {
            MoveWithPlayerInput();
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
                float yaw = playerInputHandler.lookInput.x * mouseSensitivity;
                Quaternion deltaRotation = Quaternion.Euler(0, yaw, 0);
                rb.MoveRotation(rb.rotation * deltaRotation);

                characterAnimator.UpdateMovementAnimation(playerInputHandler.horizontal, playerInputHandler.vertical);

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
            if (playerInputHandler.jumpAction.triggered)
            {
                Jump();
            }
        }

        private void SlidingWithPlayerInput()
        {
            if (playerInputHandler.slidingAction.triggered)
            {
                Sliding();
            }
        }

        private void AttackWithPlayerInput()
        {
            if (playerInputHandler.attackAction.triggered)
            {
                Attack();
            }
        }

        private void InteractWithPlayerInput()
        {
            if (playerInputHandler.interactAction.triggered)
            {
                if (Util.SceneChecker.CheckCurrnetScene(Util.SceneList.DuelLobbyScene))
                {
                    Rest();
                }
                else
                {
                    PickUp(playerID.Value);
                }
            }
        }

        private void QuiclSlotWithPlayerInput()
        {
            if (playerInputHandler.quickSlot1Action.triggered)
            {
                EquipWeapon(0);
            }
            
            if (playerInputHandler.quickSlot2Action.triggered)
            {
                EquipWeapon(1);
            }
            
            if (playerInputHandler.quickSlot3Action.triggered)
            {
                EquipWeapon(2);
            }
        }

        private void CalculateVelocity()
        {
            velocity = (transform.forward * playerInputHandler.vertical) + (transform.right * playerInputHandler.horizontal);

            if (velocity.magnitude > 1f)
            {
                velocity.Normalize();
            }

            if (playerInputHandler.vertical > 0.1f)
            {
                velocity *= forwardSpeed;
            }
            else if (playerInputHandler.vertical < -0.1f)
            {
                velocity *= backwardSpeed;
            }
        }
        #endregion

        [Rpc(SendTo.Server)]
        private void RequestSetPlayerIDServerRpc(RpcParams rpcParams = default)
        {
            playerID.Value = (int)OwnerClientId;
        }
    }
}


