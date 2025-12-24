using UnityEngine;

public static class ActorManager
{
    public static void Init()
    {
        Events.EnemyEvents.OnPlayerDetected += StartChase;
    }
    public static void StartChase(Enemy.Observer observer)
    {
        Enemy.EnemyController enemy = observer.gameObject.GetComponentInParent<Enemy.EnemyController>();
        enemy.StartChase();
    }
}
