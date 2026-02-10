using Unity.Cinemachine;
using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    public static InteractionManager Instance;

    public CinemachineInputAxisController inputController;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SetCursorState(false);
        }

        if (Input.GetMouseButtonDown(0) && Cursor.lockState != CursorLockMode.Locked)
        {
            SetCursorState(true);
        }
    }

    void SetCursorState(bool lockCursor)
    {
        //Cursor.lockState = lockCursor ? CursorLockMode.Locked : CursorLockMode.None;
        //Cursor.visible = !lockCursor;

        if (inputController != null)
        {
            inputController.enabled = lockCursor;
        }
    }
}
