using UnityEngine;

public static class EventManager
{
    public static void  Init()
    {
        Events.PlayerEvents.Clear();
        Events.EnemyEvents.Clear();
    }
}
