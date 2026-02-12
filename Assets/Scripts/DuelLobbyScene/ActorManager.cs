using Actor.Item;
using UnityEngine;

namespace DualLobbyScene 
{
    public class ActorManager : MonoBehaviour
    {
        public static ActorManager Instance;

        public Transform[] lobbyPlayers;

        private NetworkActorManager networkActorManager;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);

                networkActorManager = GetComponent<NetworkActorManager>();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public Transform GetLobbyPlayerTransform()
        {
            int playerCount = networkActorManager.GetPlayerCount();
            if(networkActorManager.IsClient)
            {
                networkActorManager.RequestAddPlayerServerRpc();
            }
            else
            {
                networkActorManager.AddPlayer();
            }

                return lobbyPlayers[playerCount];
        }

        
    }
}


