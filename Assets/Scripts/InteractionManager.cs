using Unity.Cinemachine;
using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    public CinemachineInputAxisController inputController;

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
        Cursor.lockState = lockCursor ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !lockCursor;

        if (inputController != null)
        {
            inputController.enabled = lockCursor;
        }
    }
}
