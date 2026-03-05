using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Util
{
    public static class SceneLoader
    {
        public static IEnumerator LoadSceneByName(SceneList scene)
        {
            yield return new WaitForSeconds(0.2f);

            string sceneName = scene.ToString();
            SceneManager.LoadScene(sceneName);
        }
    }
}
