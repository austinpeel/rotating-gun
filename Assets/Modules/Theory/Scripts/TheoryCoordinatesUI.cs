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
    public Image thetaSign;
    public Image thetaDot;
    public Sprite plus;
    public Sprite minus;
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

        float angularFrequency = simState.GetAngularFrequency();
        bool omegaIsPositive = angularFrequency >= 0;
        bool thetaDotIsPositive = !(frameIsLab ^ omegaIsPositive);

        if (thetaSign)
        {
            thetaSign.sprite = thetaDotIsPositive ? plus : minus;
            thetaSign.SetNativeSize();
        }

        if (thetaDot)
        {
            thetaDot.sprite = thetaDotIsPositive ? thetaDotPositive : thetaDotNegative;
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

        bool frameIsLab = simState.referenceFrame == SimulationState.ReferenceFrame.Lab;
        bool omegaIsPositive = angularFrequency >= 0;
        bool thetaDotIsPositive = !(frameIsLab ^ omegaIsPositive);

        if (thetaSign)
        {
            thetaSign.sprite = thetaDotIsPositive ? plus : minus;
            thetaSign.SetNativeSize();
        }

        if (thetaDot)
        {
            thetaDot.sprite = thetaDotIsPositive ? thetaDotPositive : thetaDotNegative;
            thetaDot.SetNativeSize();
        }
    }
}
