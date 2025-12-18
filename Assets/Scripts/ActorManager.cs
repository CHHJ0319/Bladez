using UnityEngine;

public class ActorManager : MonoBehaviour
{
    private void OnEnable()
    {
        Events.EnemyEvents.OnPlayerDetected += StartChase;
    }

    private void OnDisable()
    {
        Events.EnemyEvents.OnPlayerDetected -= StartChase;

    }

    public static void StartChase(Enemy.Observer observer)
    {
        Enemy.EnemyController enemy = observer.gameObject.GetComponentInParent<Enemy.EnemyController>();
        enemy.StartChase();
    }
}
