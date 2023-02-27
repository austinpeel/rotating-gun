using UnityEngine;

public class TheoryBasisVectorsUI : MonoBehaviour
{
    public RectTransform labBasis;
    public RectTransform gunBasis;
    public SimulationState simState;

    private SimulationState.ReferenceFrame currentReferenceFrame;
    public float currentAngle;

    private void OnEnable()
    {
        SimulationState.OnChangeReferenceFrame += HandleChangedReferenceFrame;
    }

    private void OnDisable()
    {
        SimulationState.OnChangeReferenceFrame -= HandleChangedReferenceFrame;
    }

    public void HandleChangedReferenceFrame()
    {
        if (!simState) return;

        switch (simState.referenceFrame)
        {
            case SimulationState.ReferenceFrame.Lab:
                if (labBasis) labBasis.localRotation = Quaternion.identity;
                break;
            case SimulationState.ReferenceFrame.Gun:
                if (gunBasis) gunBasis.localRotation = Quaternion.identity;
                break;
            default:
                break;
        }

        currentReferenceFrame = simState.referenceFrame;
    }

    private void LateUpdate()
    {
        if (!simState) return;

        currentAngle = simState.theta;
        switch (currentReferenceFrame)
        {
            case SimulationState.ReferenceFrame.Lab:
                if (gunBasis) gunBasis.localRotation = Quaternion.Euler(0, 0, currentAngle);
                break;
            case SimulationState.ReferenceFrame.Gun:
                if (labBasis) labBasis.localRotation = Quaternion.Euler(0, 0, -currentAngle);
                break;
            default:
                break;
        }
    }
}
