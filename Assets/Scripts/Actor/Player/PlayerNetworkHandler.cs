using Unity.Android.Gradle.Manifest;
using Unity.Cinemachine;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Actor.Player
{
    public class PlayerNetworkHandler : NetworkBehaviour
    {
        public CinemachineCamera tpsCamera;

        public NetworkVariable<Vector3> Position = new NetworkVariable<Vector3>();
        public NetworkVariable<Quaternion> Rotation = new NetworkVariable<Quaternion>();

        public NetworkVariable<bool> JumpTriggered = new NetworkVariable<bool>();
        public NetworkVariable<bool> AttackTriggered = new NetworkVariable<bool>();

        private PlayerController _playerController;
        private WeaponHandler _weaponHandler;

        private PlayerInput _playerInput;

        public string ownerID;

        void Awake()
        {
            _playerController = GetComponent<PlayerController>();
            _weaponHandler = GetComponent<WeaponHandler>();

            _playerInput = GetComponent<PlayerInput>();
        }

        public override void OnNetworkSpawn()
        {
            ownerID = OwnerClientId.ToString();
            _weaponHandler.AssignOwnerId(ownerID);

            Position.OnValueChanged += OnPositionChanged;
            Rotation.OnValueChanged += OnRotationChanged;

            JumpTriggered.OnValueChanged += OnJumpTriggeredChanged;
            AttackTriggered.OnValueChanged += OnAttackTriggeredChanged;

            if (IsOwner)
            {
                _playerInput.enabled = true;
                tpsCamera.gameObject.SetActive(true);

                MoveToRandomPosition();
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
            AttackTriggered.OnValueChanged -= OnAttackTriggeredChanged;
        }

        public void OnPositionChanged(Vector3 previous, Vector3 current)
        {
            if (Position.Value != previous)
            {
                transform.position = Position.Value;

                if (!IsOwner)
                {
                    Vector3 moveDirection = (current - previous);
                    Vector3 localDirection = transform.InverseTransformDirection(moveDirection);

                    float horizontal = 0f;
                    float vertical = 0f;

                    if (Mathf.Abs(localDirection.x) > 0.0001f)
                        horizontal = localDirection.x > 0 ? 1f : -1f;

                    if (Mathf.Abs(localDirection.z) > 0.0001f)
                        vertical = localDirection.z > 0 ? 1f : -1f;
                }
            }
        }

        public void OnRotationChanged(Quaternion previous, Quaternion current)
        {
            if (Rotation.Value != previous)
            {
                transform.rotation = Rotation.Value;
            }
        }

        public void OnJumpTriggeredChanged(bool previous, bool current)
        {
            if (JumpTriggered.Value != previous && !IsOwner)
            {
                _playerController.Jump();
            }
        }

        public void OnAttackTriggeredChanged(bool previous, bool current)
        {
            if (AttackTriggered.Value != previous && !IsOwner)
            {
                _playerController.Attack();
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
        public void SubmitAttackRequestServerRpc(RpcParams rpcParams = default)
        {
            AttackTriggered.Value = !AttackTriggered.Value;
        }

        static Vector3 GetRandomPositionOnPlane()
        {
            return new Vector3(Random.Range(-3f, 3f), 1f, Random.Range(-3f, 3f));
        }

        void MoveToRandomPosition()
        {
            Vector3 pos = GetRandomPositionOnPlane();
            SubmitTransfromRequestServerRpc(pos);
        }
    }
}
