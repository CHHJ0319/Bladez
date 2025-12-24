using UnityEngine;

public class GameManager : MonoBehaviour
{
    void Awake()
    {
        ActorManager.Init();
        EventManager.Init();
    }

    void Update()
    {
        
    }
}
