using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheoryBasisVectors : MonoBehaviour
{
    public RectTransform labBasis;
    public RectTransform gunBasis;

    private RotatingGunSimulation.ReferenceFrame currentReferenceFrame;
    public float currentAngle;

    private void OnEnable()
    {
        RotatingGunSimulation.OnChangeReferenceFrame += HandleChangedReferenceFrame;
        RotatingGunSimulation.OnChangeAngle += HandleChangedAngle;
    }

    private void OnDisable()
    {
        RotatingGunSimulation.OnChangeReferenceFrame -= HandleChangedReferenceFrame;
        RotatingGunSimulation.OnChangeAngle -= HandleChangedAngle;
    }

    public void HandleChangedReferenceFrame(RotatingGunSimulation.ReferenceFrame referenceFrame)
    {
        Debug.Log("Heard reference frame " + referenceFrame);
        switch (referenceFrame)
        {
            case RotatingGunSimulation.ReferenceFrame.Lab:
                if (labBasis) labBasis.localRotation = Quaternion.identity;
                break;
            case RotatingGunSimulation.ReferenceFrame.Gun:
                if (gunBasis) gunBasis.localRotation = Quaternion.identity;
                break;
            default:
                break;
        }

        currentReferenceFrame = referenceFrame;
    }

    public void HandleChangedAngle(float angle)
    {
        currentAngle = angle;
        switch (currentReferenceFrame)
        {
            case RotatingGunSimulation.ReferenceFrame.Lab:
                if (gunBasis) gunBasis.localRotation = Quaternion.Euler(0, 0, angle);
                break;
            case RotatingGunSimulation.ReferenceFrame.Gun:
                if (labBasis) labBasis.localRotation = Quaternion.Euler(0, 0, -angle);
                break;
            default:
                break;
        }
    }
}
