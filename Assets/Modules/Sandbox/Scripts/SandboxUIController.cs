// ----------------------------------------------------------------------------------------------------------
// Author: Austin Peel
//
// © All rights reserved. ECOLE POLYTECHNIQUE FEDERALE DE LAUSANNE, Switzerland, Section de Physique, 2024.
// See the LICENSE.md file for more details.
// ----------------------------------------------------------------------------------------------------------
using UnityEngine;

public class SandboxUIController : MonoBehaviour
{
    public CanvasGroup[] canvasGroups;
    private bool panelsAreDisabled;

    private void OnEnable()
    {
        CameraController.OnCameraMovementComplete += HandleCameraMovementComplete;
    }

    private void OnDisable()
    {
        CameraController.OnCameraMovementComplete -= HandleCameraMovementComplete;
    }

    private void Start()
    {
        foreach (CanvasGroup cg in canvasGroups)
        {
            cg.alpha = 0.5f;
            cg.interactable = false;
            cg.blocksRaycasts = false;
        }
    }

    public void HandleCameraMovementComplete(Vector3 position, Quaternion rotation)
    {
        if (panelsAreDisabled) return;

        foreach (CanvasGroup cg in canvasGroups)
        {
            cg.alpha = 1;
            cg.interactable = true;
            cg.blocksRaycasts = true;
        }

        panelsAreDisabled = true;
    }
}
