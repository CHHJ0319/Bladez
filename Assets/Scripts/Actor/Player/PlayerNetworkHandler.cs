using System.Globalization;
using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace Actor.Player
{
    public class PlayerNetworkHandler : NetworkBehaviour
    {
        public CinemachineCamera tpsCamera;

        public NetworkVariable<Vector3> Position = new NetworkVariable<Vector3>();

        private PlayerInput _playerInput;

        public override void OnNetworkSpawn()
        {
            _playerInput = GetComponent<PlayerInput>();

            Position.OnValueChanged += OnStateChanged;

            tpsCamera.gameObject.SetActive(false);
            _playerInput.enabled = false;

            if (IsOwner)
            {
                _playerInput.enabled = true;
                tpsCamera.gameObject.SetActive(true);
                Move();
            }
        }

        public override void OnNetworkDespawn()
        {
            Position.OnValueChanged -= OnStateChanged;
        }

        public void OnStateChanged(Vector3 previous, Vector3 current)
        {
            if (Position.Value != previous)
            {
                transform.position = Position.Value;
            }
        }

        public void Move()
        {
            SubmitPositionRequestServerRpc();
        }

        [Rpc(SendTo.Server)]
        void SubmitPositionRequestServerRpc(RpcParams rpcParams = default)
        {
            var randomPosition = GetRandomPositionOnPlane();
            transform.position = randomPosition;
            Position.Value = randomPosition;
        }

        static Vector3 GetRandomPositionOnPlane()
        {
            return new Vector3(Random.Range(-3f, 3f), 1f, Random.Range(-3f, 3f));
        }
    }
}
