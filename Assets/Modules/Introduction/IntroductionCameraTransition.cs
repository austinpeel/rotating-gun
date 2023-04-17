using UnityEngine;

public class IntroductionCameraTransition : MonoBehaviour
{
    public CameraState cameraState;

    private void Awake()
    {
        if (cameraState)
        {
            Transform mainCamera = Camera.main.transform;
            mainCamera.position = cameraState.position;
            mainCamera.rotation = cameraState.rotation;
            mainCamera.localScale = cameraState.scale;
        }
    }
}
