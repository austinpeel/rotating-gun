using UnityEngine;
using UnityEngine.UI;

public class TheoryBasisVectorsUI : MonoBehaviour
{
    public RectTransform labBasis;
    public RectTransform gunBasis;
    public SimulationState simState;

    [Header("Images")]
    public Image omegaSign;
    public Image omegaUnitVector;
    public Image thetaSign;
    public Sprite plus;
    public Sprite minus;
    public Sprite x3;
    public Sprite y3;

    private SimulationState.ReferenceFrame currentReferenceFrame;

    private void Awake()
    {
        HandleOmegaChange();
        HandleReferenceFrameChange();
    }

    private void OnEnable()
    {
        SimulationState.OnChangeReferenceFrame += HandleReferenceFrameChange;
        SimulationState.OnChangeOmega += HandleOmegaChange;
    }

    private void OnDisable()
    {
        SimulationState.OnChangeReferenceFrame -= HandleReferenceFrameChange;
        SimulationState.OnChangeOmega -= HandleOmegaChange;
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

        if (omegaUnitVector)
        {
            omegaUnitVector.sprite = frameIsLab ? x3 : y3;
            omegaUnitVector.SetNativeSize();
        }

        currentReferenceFrame = simState.referenceFrame;
    }

    private void LateUpdate()
    {
        if (!simState) return;

        switch (currentReferenceFrame)
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
    }

    public void HandleOmegaChange()
    {
        if (!simState) return;

        float angularFrequency = simState.GetAngularFrequency();

        if (omegaSign)
        {
            omegaSign.sprite = angularFrequency >= 0 ? plus : minus;
            omegaSign.SetNativeSize();
        }

        if (thetaSign)
        {
            thetaSign.sprite = angularFrequency >= 0 ? plus : minus;
            thetaSign.SetNativeSize();
        }
    }
}
