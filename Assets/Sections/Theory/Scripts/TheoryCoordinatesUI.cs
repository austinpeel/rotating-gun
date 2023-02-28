using UnityEngine;
using UnityEngine.UI;

public class TheoryCoordinatesUI : MonoBehaviour
{
    public SimulationState simState;

    [Header("Basis Vectors")]
    public RectTransform labBasisVectors;
    public RectTransform gunBasisVectors;

    [Header("Transformations")]
    public RectTransform labBasisEquations;
    public RectTransform gunBasisEquations;

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

        if (labBasisVectors && frameIsLab)
        {
            labBasisVectors.localRotation = Quaternion.identity;
        }

        if (gunBasisVectors && !frameIsLab)
        {
            gunBasisVectors.localRotation = Quaternion.identity;
        }

        if (labBasisEquations) labBasisEquations.gameObject.SetActive(frameIsLab);
        if (gunBasisEquations) gunBasisEquations.gameObject.SetActive(!frameIsLab);

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
                if (gunBasisVectors) gunBasisVectors.localRotation = Quaternion.Euler(0, 0, simState.theta);
                break;
            case SimulationState.ReferenceFrame.Gun:
                if (labBasisVectors) labBasisVectors.localRotation = Quaternion.Euler(0, 0, -simState.theta);
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
