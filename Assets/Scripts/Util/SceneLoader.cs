using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Util
{
    public static class SceneLoader
    {
        public static IEnumerator LoadSceneByName(string name)
        {
            yield return new WaitForSeconds(0.2f);

            SceneManager.LoadScene(name);
        }
    }
}
