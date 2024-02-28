// ----------------------------------------------------------------------------------------------------------
// Author: Austin Peel
//
// © All rights reserved. ECOLE POLYTECHNIQUE FEDERALE DE LAUSANNE, Switzerland, Section de Physique, 2024.
// See the LICENSE.md file for more details.
// ----------------------------------------------------------------------------------------------------------
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
