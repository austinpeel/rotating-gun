using UnityEngine;
using UnityEngine.UI;

public class TheoryCoordinatesUI : MonoBehaviour
{
    public SimulationState simState;

    [Header("Transformations")]
    public RectTransform labBasisEquations;
    public RectTransform gunBasisEquations;

    [Header("Images")]
    public Image omegaSign;
    public Image omegaUnitVector;
    public Image thetaDot;
    public Sprite plus;
    public Sprite minus;
    public Sprite x3;
    public Sprite y3;
    public Sprite thetaDotPositive;
    public Sprite thetaDotNegative;

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

        if (labBasisEquations) labBasisEquations.gameObject.SetActive(frameIsLab);
        if (gunBasisEquations) gunBasisEquations.gameObject.SetActive(!frameIsLab);

        if (omegaUnitVector)
        {
            omegaUnitVector.sprite = frameIsLab ? x3 : y3;
            omegaUnitVector.SetNativeSize();
        }

        if (thetaDot)
        {
            bool omegaIsPositive = simState.GetAngularFrequency() >= 0;
            thetaDot.sprite = !(frameIsLab ^ omegaIsPositive) ? thetaDotPositive : thetaDotNegative;
            thetaDot.SetNativeSize();
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

        if (thetaDot)
        {
            bool frameIsLab = simState.referenceFrame == SimulationState.ReferenceFrame.Lab;
            bool omegaIsPositive = angularFrequency >= 0;
            thetaDot.sprite = !(frameIsLab ^ omegaIsPositive) ? thetaDotPositive : thetaDotNegative;
            thetaDot.SetNativeSize();
        }
    }
}
