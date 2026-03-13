using UnityEngine;

namespace Actor
{
    public class DuelRoom : MonoBehaviour
    {
        public Transform DuelLobbyPads;
        public Transform[] lobbyPlayers;

        void Start()
        {
            ActorManager.Instance.SetDuelRoom(this);
        }

        public void ShowDuelLobbyPads()
        {
            DuelLobbyPads.gameObject.SetActive(true);
        }

        public Transform GetDuelLobbyPlayerTransform(int index)
        {
            return lobbyPlayers[index];
        }
    }
}
