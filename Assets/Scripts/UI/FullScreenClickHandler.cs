using System.Collections;
using UnityEngine;
using UnityEngine.Diagnostics;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI {
    public class FullScreenClickHandler : MonoBehaviour, IPointerClickHandler
    {
        private AudioSource startSound;

        private void Start()
        {
            startSound = GetComponent<AudioSource>();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                startSound.Play();
                StartCoroutine(Util.SceneLoader.LoadSceneByName("FieldScene"));
            }
        }
    }
}
