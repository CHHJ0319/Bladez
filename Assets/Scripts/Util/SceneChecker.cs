using UnityEngine.SceneManagement;

namespace Util
{
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