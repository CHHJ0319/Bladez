using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Actor.Player
{
    public class PlayerNetworkHandler : NetworkBehaviour
    {
        public CinemachineCamera tpsCamera;

        public NetworkVariable<Vector3> Position = new NetworkVariable<Vector3>();
        public NetworkVariable<Quaternion> Rotation = new NetworkVariable<Quaternion>();

        private CharacterAnimator _characterAnimator;
        private PlayerInput _playerInput;

        private void Awake()
        {
            _playerInput = GetComponent<PlayerInput>();
            _characterAnimator = GetComponent<CharacterAnimator>();
        }

        public override void OnNetworkSpawn()
        {
            Position.OnValueChanged += OnPositionChanged;
            Rotation.OnValueChanged += OnRotationChanged;

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
        }

        public void OnPositionChanged(Vector3 previous, Vector3 current)
        {
            if (Position.Value != previous)
            {
                transform.position = Position.Value;

                Vector3 moveDirection = (current - previous);
                //Vector3 localDirection = transform.InverseTransformDirection(moveDirection);

                if (!IsOwner)
                {
                    _characterAnimator.UpdateMovementAnimation(moveDirection.normalized.x, moveDirection.normalized.z);

                }
            }
            else
            {
                if (!IsOwner)
                {
                    _characterAnimator.UpdateMovementAnimation(0, 0);
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

        [Rpc(SendTo.Server)]
        void SubmitTransfromRequestServerRpc(Vector3 position, Quaternion rotation = default, RpcParams rpcParams = default)
        {
            Position.Value = position;
            Rotation.Value = rotation;
        }

        static Vector3 GetRandomPositionOnPlane()
        {
            return new Vector3(Random.Range(-3f, 3f), 1f, Random.Range(-3f, 3f));
        }

        public void Move(Vector3 position, Quaternion rotation)
        {
            SubmitTransfromRequestServerRpc(position, rotation);
        }

        void MoveToRandomPosition()
        {
            Vector3 pos = GetRandomPositionOnPlane();
            SubmitTransfromRequestServerRpc(pos);
        }
    }
}
