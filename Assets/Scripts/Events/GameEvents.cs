using System;

namespace Events
{
    public static class GameEvents
    {
        public static event Action OnGameStarted;

        public static void StartGame()
        {
            OnGameStarted?.Invoke();
        }
    }
}
