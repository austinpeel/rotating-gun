using UnityEngine;

[RequireComponent(typeof(Vector))]
public class AngularVelocity : MonoBehaviour
{
    public SimulationState simState;

    [Header("Visual Properties")]
    public float scaleFactor = 1;

    [Header("Plinth")]
    public Material plinthMaterial;
    public float defaultAlpha = 1;
    public float reducedAlpha = 0.2f;

    [Header("Labels")]
    public Transform omegaLabelLabFrame;
    public Transform omegaLabelGunFrame;
    public SpriteRenderer orientationLabel;
    public Sprite intoPlaneSprite;
    public Sprite outOfPlaneSprite;

    private Vector vector;

    private void Awake()
    {
        TryGetComponent(out vector);

        HandleOmegaChange();
        SetLabelVisibilities();
    }

    private void OnEnable()
    {
        SimulationState.OnChangeOmega += HandleOmegaChange;
        SimulationState.OnChangePerspective += HandlePerspectiveChange;
        SimulationState.OnChangeReferenceFrame += HandleReferenceFrameChange;
    }

    private void OnDisable()
    {
        SimulationState.OnChangeOmega -= HandleOmegaChange;
        SimulationState.OnChangePerspective -= HandlePerspectiveChange;
        SimulationState.OnChangeReferenceFrame -= HandleReferenceFrameChange;
    }

    private void UpdateVector(Vector3 components)
    {
        // Debug.Log("AngularVelocity > UpdateVector > " + simState.omega);
        if (!vector) return;

        vector.components = scaleFactor * components;
        if (simState)
        {
            float angularFrequency = simState.GetAngularFrequency();
            bool cameraIsAbove = simState.perspective == SimulationState.Perspective.Above;
            if (angularFrequency > 0 && cameraIsAbove) vector.components = Vector3.zero;
        }
        vector.Redraw();

        // Adjust plinth transparency to see the vector through it if necessary
        SetMaterialAlpha(vector.components.y > 0 ? defaultAlpha : reducedAlpha);

        SetLabelVisibilities();
    }

    public void HandleOmegaChange()
    {
        if (simState) UpdateVector(simState.omega);
    }

    public void HandlePerspectiveChange()
    {
        HandleOmegaChange();  // To set whether the vector should be visible
        SetLabelVisibilities();
    }

    public void HandleReferenceFrameChange()
    {
        SetLabelVisibilities();
    }

    private void SetLabelVisibilities()
    {
        if (!simState) return;

        float angularFrequency = simState.GetAngularFrequency();
        bool cameraIsAbove = simState.perspective == SimulationState.Perspective.Above;
        bool frameIsLab = simState.referenceFrame == SimulationState.ReferenceFrame.Lab;

        if (orientationLabel)
        {
            bool outOfPlane = angularFrequency > 0;
            orientationLabel.sprite = outOfPlane ? outOfPlaneSprite : intoPlaneSprite;
            orientationLabel.gameObject.SetActive(cameraIsAbove);
        }

        if (omegaLabelLabFrame)
        {
            bool positive = angularFrequency > 0;
            Vector3 position = omegaLabelLabFrame.transform.position;
            position.y = cameraIsAbove ? 2 : (positive ? 2 : -2.5f);
            omegaLabelLabFrame.transform.position = position;
            omegaLabelLabFrame.gameObject.SetActive(frameIsLab);
        }

        if (omegaLabelGunFrame)
        {

            omegaLabelGunFrame.gameObject.SetActive(false);
            bool positive = angularFrequency > 0;
            Vector3 position = omegaLabelGunFrame.transform.position;
            position.y = cameraIsAbove ? 2 : (positive ? 2 : -2.5f);
            omegaLabelGunFrame.transform.position = position;
            omegaLabelGunFrame.gameObject.SetActive(!frameIsLab);
        }
    }

    public void SetMaterialAlpha(float alpha)
    {
        if (!plinthMaterial) return;

        Color color = plinthMaterial.color;
        color.a = alpha;
        plinthMaterial.color = color;
    }

    // public float GetMaterialAlpha()
    // {
    //     return plinthMaterial ? plinthMaterial.color.a : 1;
    // }

    // public void HandleStartPerspectiveChange()
    // {
    //     // Debug.Log("Heard starting " + perspective);
    //     if (!simState) return;

    //     if (simState.perspective == SimulationState.Perspective.Above)
    //     {
    //         SetOrientationLabel(false, simState.currentOmega.y >= 0);
    //     }
    // }

    // public void HandleEndPerspectiveChange()
    // {
    //     // Debug.Log("Heard ending " + perspective);
    //     if (!simState) return;

    //     if (simState.perspective == SimulationState.Perspective.Side)
    //     {
    //         bool visible = simState.currentOmega.y < 0;
    //         bool intoThePlane = simState.currentOmega.y < 0;

    //         SetOrientationLabel(visible, intoThePlane);
    //     }
    // }

    // private void SetOrientationLabel(bool visible, bool intoThePlane)
    // {
    //     if (!orientation || !intoPlane || !outOfPlane) return;

    //     orientation.sprite = intoThePlane ? intoPlane : outOfPlane;
    //     Debug.Log(intoThePlane ? "Into Plane" : "Out of Plane");
    //     orientation.gameObject.SetActive(visible);
    // }
}
