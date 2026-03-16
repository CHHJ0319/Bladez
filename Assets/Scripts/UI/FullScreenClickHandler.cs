using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI {
    public class FullScreenClickHandler : MonoBehaviour, IPointerClickHandler
    {
        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                Events.GameEvents.StartGame();
                StartCoroutine(Util.SceneLoader.LoadSceneByName(Util.SceneList.DuelLobbyScene));
                //UIManager.Instance.HideBlinkingPrompt();
                //UIManager.Instance.ShowNetworkPanel();
            }
        }
    }
}
