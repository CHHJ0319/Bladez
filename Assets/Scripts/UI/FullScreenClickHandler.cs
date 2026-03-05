using UnityEngine;
using UnityEngine.Diagnostics;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI {
    public class FullScreenClickHandler : MonoBehaviour, IPointerClickHandler
    {
        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                StartCoroutine(Util.SceneLoader.LoadSceneByName("DuelLobbyScene"));
            }
        }
    }
}
