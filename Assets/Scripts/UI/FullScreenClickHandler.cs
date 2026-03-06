using System.Collections;
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
                UIManager.Instance.PlayStartSound();
                StartCoroutine(Util.SceneLoader.LoadSceneByName(Util.SceneList.DuelLobbyScene));
                //UIManager.Instance.HideBlinkingPrompt();
                //UIManager.Instance.ShowNetworkPanel();
            }
        }
    }
}
