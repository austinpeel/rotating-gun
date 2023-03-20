using UnityEngine;
using UnityEngine.UI;

public class TheoryDynamicsUI : MonoBehaviour
{
    public SimulationState simState;

    [Header("Equations")]
    public RectTransform rLab;
    public RectTransform rGun;
    public RectTransform vLab;
    public RectTransform vGun;
    public RectTransform omegaLab;
    public RectTransform omegaGun;
    public RectTransform fCoriolis;
    public RectTransform fCentrifugal;

    [Header("Omega Sign")]
    public Image omegaSign;
    public Sprite plus;
    public Sprite minus;

    [Header("Texts")]
    public RectTransform fictitiousForces;
    public RectTransform noFictitiousForces;

    [Header("Divider")]
    public RectTransform verticalDivider;

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
        if (omegaLab) omegaLab.gameObject.SetActive(frameIsLab);
        if (omegaGun) omegaGun.gameObject.SetActive(!frameIsLab);
        if (fCoriolis) fCoriolis.gameObject.SetActive(!frameIsLab);
        if (fCentrifugal) fCentrifugal.gameObject.SetActive(!frameIsLab);
        if (fictitiousForces) fictitiousForces.gameObject.SetActive(!frameIsLab);
        if (noFictitiousForces) noFictitiousForces.gameObject.SetActive(frameIsLab);
        if (verticalDivider) verticalDivider.gameObject.SetActive(!frameIsLab);

        // if (omegaUnitVector)
        // {
        //     omegaUnitVector.sprite = frameIsLab ? x3 : y3;
        //     omegaUnitVector.SetNativeSize();
        // }
    }
}
