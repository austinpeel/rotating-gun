using UnityEngine;

[ExecuteInEditMode]
public class CameraFacingObject : MonoBehaviour
{
    private Camera mainCamera;

    private void Update()
    {
        if (!mainCamera) mainCamera = Camera.main;

        transform.forward = mainCamera.transform.forward;
    }
}
