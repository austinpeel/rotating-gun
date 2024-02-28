// ----------------------------------------------------------------------------------------------------------
// Author: Austin Peel
//
// © All rights reserved. ECOLE POLYTECHNIQUE FEDERALE DE LAUSANNE, Switzerland, Section de Physique, 2024.
// See the LICENSE.md file for more details.
// ----------------------------------------------------------------------------------------------------------
using UnityEngine;

[RequireComponent(typeof(CustomSlider))]
public class SandboxZoom : MonoBehaviour
{
    private CustomSlider slider;

    private void OnEnable()
    {
        CameraController.OnCameraMovementComplete += HandleCameraMovementComplete;
    }

    private void OnDisable()
    {
        CameraController.OnCameraMovementComplete -= HandleCameraMovementComplete;
    }

    public void HandleCameraMovementComplete(Vector3 position, Quaternion rotation)
    {
        slider = GetComponent<CustomSlider>();
        slider.value = 55 - position.magnitude;
    }
}
