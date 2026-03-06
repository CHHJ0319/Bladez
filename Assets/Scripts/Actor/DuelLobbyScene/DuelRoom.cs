using UnityEngine;

namespace Actor.DuelLobbyScene
{
    public class DuelRoom : MonoBehaviour
    {
        public Transform DuelLobbyPads;
        public Transform[] lobbyPlayers;

        public void Initialize()
        {

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
