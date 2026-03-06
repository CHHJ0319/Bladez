using UnityEngine;

namespace Actor.DuelLobbyScene
{
    public class ActorController : MonoBehaviour
    {
        public Transform DuelLobbyPads;
        public Transform[] lobbyPlayers;

        private void Awake()
        {
            ActorManager.Instance.SetDuelLobbySceneActorController(this);
        }

        public void Initialize()
        {

        }

        public void ShowDuelLobbyPads()
        {
            DuelLobbyPads.gameObject.SetActive(true);
        }

        public Transform GetDuelLobbyPlayerTransform()
        {
            int index = ActorManager.Instance.PlayerCount;
            return lobbyPlayers[index];
        }
    }
}
