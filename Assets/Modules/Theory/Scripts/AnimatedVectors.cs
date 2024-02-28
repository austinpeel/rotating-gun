// ----------------------------------------------------------------------------------------------------------
// Author: Austin Peel
//
// © All rights reserved. ECOLE POLYTECHNIQUE FEDERALE DE LAUSANNE, Switzerland, Section de Physique, 2024.
// See the LICENSE.md file for more details.
// ----------------------------------------------------------------------------------------------------------
using UnityEngine;
using UnityEngine.UI;

public class AnimatedVectors : MonoBehaviour
{
    public SimulationState simState;

    [Header("Lab Basis")]
    public RectTransform labBasis;
    public Image x1;
    public Image x2;

    [Header("Gun Basis")]
    public RectTransform gunBasis;
    public Image y1;
    public Image y2;

    private void Awake()
    {
        HandleReferenceFrameChange();
    }

    private void OnEnable()
    {
        SimulationState.OnChangeReferenceFrame += HandleReferenceFrameChange;
    }

    private void OnDisable()
    {
        SimulationState.OnChangeReferenceFrame -= HandleReferenceFrameChange;
    }

    public void HandleReferenceFrameChange()
    {
        if (!simState) return;

        bool frameIsLab = simState.referenceFrame == SimulationState.ReferenceFrame.Lab;

        if (labBasis && frameIsLab)
        {
            labBasis.localRotation = Quaternion.identity;
        }

        if (gunBasis && !frameIsLab)
        {
            gunBasis.localRotation = Quaternion.identity;
        }
    }

    private void LateUpdate()
    {
        if (!simState) return;

        switch (simState.referenceFrame)
        {
            case SimulationState.ReferenceFrame.Lab:
                if (gunBasis) gunBasis.localRotation = Quaternion.Euler(0, 0, simState.theta);
                break;
            case SimulationState.ReferenceFrame.Gun:
                if (labBasis) labBasis.localRotation = Quaternion.Euler(0, 0, -simState.theta);
                break;
            default:
                break;
        }

        if (labBasis)
        {
            float angleZ = labBasis.transform.localRotation.eulerAngles.z;
            if (x1) x1.transform.localRotation = Quaternion.Euler(0, 0, -angleZ);
            if (x2) x2.transform.localRotation = Quaternion.Euler(0, 0, -angleZ - 90);
        }

        if (gunBasis)
        {
            float angleZ = gunBasis.transform.localRotation.eulerAngles.z;
            if (y1) y1.transform.localRotation = Quaternion.Euler(0, 0, -angleZ);
            if (y2) y2.transform.localRotation = Quaternion.Euler(0, 0, -angleZ - 90);
        }
    }
}
