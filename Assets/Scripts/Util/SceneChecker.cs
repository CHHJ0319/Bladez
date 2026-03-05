using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Util
{
    public enum SceneList 
    {
        TitleScene, Town1Scene, DuelLobbyScene, DuelScene
    }

    public static class SceneChecker
    {
        public static bool CheckCurrnetScene(SceneList scene)
        {
            string sceneName = scene.ToString();

            if (sceneName == SceneManager.GetActiveScene().name)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}