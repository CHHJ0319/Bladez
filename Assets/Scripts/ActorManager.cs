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

    public static void StartChase(Character.Enemy.Observer observer)
    {
        Character.EnemyController enemy = observer.gameObject.GetComponentInParent<Character.EnemyController>();
        enemy.StartChase();
    }
}
