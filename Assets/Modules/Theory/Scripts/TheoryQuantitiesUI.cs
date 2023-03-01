using UnityEngine;
using UnityEngine.UI;

public class TheoryQuantitiesUI : MonoBehaviour
{
    public SimulationState simState;

    [Header("Equations")]
    public RectTransform rLab;
    public RectTransform rGun;
    public RectTransform vLab;
    public RectTransform vGun;
    public RectTransform fCoriolis;
    public RectTransform fCentrifugal;

    [Header("Images")]
    public Image omegaSign;
    public Image omegaUnitVector;
    public Sprite plus;
    public Sprite minus;
    public Sprite x3;
    public Sprite y3;

    [Header("Texts")]
    public RectTransform inertialForces;
    public RectTransform noInertialForces;

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

    public void HandleOmegaChange()
    {
        if (!simState) return;

        float angularFrequency = simState.GetAngularFrequency();

        if (omegaSign)
        {
            omegaSign.sprite = angularFrequency >= 0 ? plus : minus;
            omegaSign.SetNativeSize();
        }
    }

    public void HandleReferenceFrameChange()
    {
        if (!simState) return;

        bool frameIsLab = simState.referenceFrame == SimulationState.ReferenceFrame.Lab;

        if (rLab) rLab.gameObject.SetActive(frameIsLab);
        if (rGun) rGun.gameObject.SetActive(!frameIsLab);
        if (vLab) vLab.gameObject.SetActive(frameIsLab);
        if (vGun) vGun.gameObject.SetActive(!frameIsLab);
        if (fCoriolis) fCoriolis.gameObject.SetActive(!frameIsLab);
        if (fCentrifugal) fCentrifugal.gameObject.SetActive(!frameIsLab);
        if (inertialForces) inertialForces.gameObject.SetActive(!frameIsLab);
        if (noInertialForces) noInertialForces.gameObject.SetActive(frameIsLab);

        if (omegaUnitVector)
        {
            omegaUnitVector.sprite = frameIsLab ? x3 : y3;
            omegaUnitVector.SetNativeSize();
        }
    }
}
