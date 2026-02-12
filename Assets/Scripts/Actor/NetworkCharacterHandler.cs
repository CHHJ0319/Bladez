using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Actor
{
    public class NetworkCharacterHandler : NetworkBehaviour
    {
        public CinemachineCamera tpsCamera;

        public NetworkVariable<Vector3> Position = new NetworkVariable<Vector3>();
        public NetworkVariable<Quaternion> Rotation = new NetworkVariable<Quaternion>();

        public NetworkVariable<bool> JumpTriggered = new NetworkVariable<bool>();
        public NetworkVariable<bool> SlidingTriggered = new NetworkVariable<bool>();
        public NetworkVariable<bool> AttackTriggered = new NetworkVariable<bool>();
        public NetworkVariable<bool> PickUpTriggered = new NetworkVariable<bool>();

        public NetworkVariable<float> HP = new NetworkVariable<float>();

        private Player.PlayerController _playerController;
        private WeaponHandler _weaponHandler;

        private PlayerInput _playerInput;

        public string OwnerID { get; private set; }

        void Awake()
        {
            _playerController = GetComponent<Player.PlayerController>();
            _weaponHandler = GetComponent<WeaponHandler>();

            _playerInput = GetComponent<PlayerInput>();
        }

        public override void OnNetworkSpawn()
        {
            OwnerID = OwnerClientId.ToString();
            AssignWeaponOwnerID();

            Position.OnValueChanged += OnPositionChanged;
            Rotation.OnValueChanged += OnRotationChanged;

            JumpTriggered.OnValueChanged += OnJumpTriggeredChanged;
            SlidingTriggered.OnValueChanged += OnSlidingTriggeredChanged;
            AttackTriggered.OnValueChanged += OnAttackTriggeredChanged;
            PickUpTriggered.OnValueChanged += OnPickUpTriggeredChanged;

            HP.OnValueChanged += OnHPChanged;

            if (IsOwner)
            {
                _playerInput.enabled = true;
                _playerController.CreatePlayerUI();

                if (SceneManager.GetActiveScene().name == "DuelLobbyScene")
                {
                    tpsCamera.gameObject.SetActive(false);

                    Transform lobbyTransform = DualLobbyScene.ActorManager.Instance.GetLobbyPlayerTransform();
                    transform.localPosition = lobbyTransform.localPosition;
                    transform.localRotation = lobbyTransform.rotation;
                }
                else if (SceneManager.GetActiveScene().name == "TestScene")
                {
                    tpsCamera.gameObject.SetActive(true);
                    MoveToRandomPosition();
                }
            }
            else
            {
                tpsCamera.gameObject.SetActive(false);
                _playerInput.enabled = false;
            }
        }

        public override void OnNetworkDespawn()
        {
            Position.OnValueChanged -= OnPositionChanged;
            Rotation.OnValueChanged -= OnRotationChanged;

            JumpTriggered.OnValueChanged -= OnJumpTriggeredChanged;
            SlidingTriggered.OnValueChanged -= OnSlidingTriggeredChanged;
            AttackTriggered.OnValueChanged -= OnAttackTriggeredChanged;
            PickUpTriggered.OnValueChanged -= OnPickUpTriggeredChanged;

            HP.OnValueChanged -= OnHPChanged;
        }

        public void AssignWeaponOwnerID()
        {
            _weaponHandler.AssignOwnerId(OwnerID);
        }

        public void OnPositionChanged(Vector3 previous, Vector3 current)
        {
            if (Position.Value != previous)
            {
                if (!IsOwner)
                {
                    transform.position = Position.Value;
                }
            }
        }

        public void OnRotationChanged(Quaternion previous, Quaternion current)
        {
            if (Rotation.Value != previous)
            {
                if (!IsOwner)
                {
                    transform.rotation = Rotation.Value;
                }
            }
        }

        public void OnJumpTriggeredChanged(bool previous, bool current)
        {
            if (JumpTriggered.Value != previous && !IsOwner)
            {
                _playerController.Jump();
            }
        }

        public void OnSlidingTriggeredChanged(bool previous, bool current)
        {
            if (SlidingTriggered.Value != previous && !IsOwner)
            {
                _playerController.Sliding();
            }
        }

        public void OnAttackTriggeredChanged(bool previous, bool current)
        {
            if (AttackTriggered.Value != previous && !IsOwner)
            {
                _playerController.Attack();
            }
        }

        public void OnPickUpTriggeredChanged(bool previous, bool current)
        {
            if (PickUpTriggered.Value != previous && !IsOwner)
            {
                _playerController.PickUp();
            }
        }

        public void OnHPChanged(float previous, float current)
        {
            if (HP.Value != previous && !IsOwner)
            {
                _playerController.SetHP(HP.Value);
            }
        }

        [Rpc(SendTo.Server)]
        public void SubmitTransfromRequestServerRpc(Vector3 position, Quaternion rotation = default, RpcParams rpcParams = default)
        {
            Position.Value = position;
            Rotation.Value = rotation;
        }

        [Rpc(SendTo.Server)]
        public void SubmitJumpRequestServerRpc(RpcParams rpcParams = default)
        {
            JumpTriggered.Value = !JumpTriggered.Value;
        }

        [Rpc(SendTo.Server)]
        public void SubmitslidingRequestServerRpc(RpcParams rpcParams = default)
        {
            SlidingTriggered.Value = !SlidingTriggered.Value;
        }

        [Rpc(SendTo.Server)]
        public void SubmitAttackRequestServerRpc(RpcParams rpcParams = default)
        {
            AttackTriggered.Value = !AttackTriggered.Value;
        }

        [Rpc(SendTo.Server)]
        public void SubmitPickUpRequestServerRpc(RpcParams rpcParams = default)
        {
            PickUpTriggered.Value = !PickUpTriggered.Value;
        }

        [Rpc(SendTo.Server)]
        public void SubmitHPRequestServerRpc(float hp, RpcParams rpcParams = default)
        {
            HP.Value = hp;
        }

        private static Vector3 GetRandomPositionOnPlane()
        {
            return new Vector3(Random.Range(-3f, 3f), 1f, Random.Range(-3f, 3f));
        }

        private void MoveToRandomPosition()
        {
            Vector3 pos = GetRandomPositionOnPlane();
            transform.position = pos;
            SubmitTransfromRequestServerRpc(pos);
        }
    }
}
