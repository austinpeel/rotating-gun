using UnityEngine;
using UnityEngine.UI;

public class TheoryUIControls : MonoBehaviour
{
    public SimulationState simState;
    public Toggle rotationToggle;
    public Toggle cameraToggle;
    public Toggle referenceToggle;

    private void Awake()
    {
        if (!simState) return;

        // Set toggles according to simulation state values
        // Do not re-update the simulation state immediately with this call
        bool rotationIsPositive = simState.omega.y >= 0;
        UpdateRotation(rotationIsPositive, false);

        bool perspectiveIsBeside = simState.perspective == SimulationState.Perspective.Side;
        UpdatePerspective(perspectiveIsBeside, false);

        bool referenceFrameIsLab = simState.referenceFrame == SimulationState.ReferenceFrame.Lab;
        UpdateReferenceFrame(referenceFrameIsLab, false);
    }

    // Called from UI toggle
    public void UpdateRotation(bool isPositive)
    {
        UpdateRotation(isPositive, true);
    }

    private void UpdateRotation(bool isPositive, bool setSimState = true)
    {
        // Debug.Log("Theory UI Controls > UpdateRotation()");
        if (rotationToggle) rotationToggle.isOn = isPositive;

        if (simState && setSimState)
        {
            Vector3 omega = (isPositive ? 1 : -1) * Mathf.Abs(simState.omega.y) * Vector3.up;
            simState.SetOmega(omega);
        }

    }

    // Called from UI toggle
    public void UpdatePerspective(bool isBeside)
    {
        UpdatePerspective(isBeside, true);
    }

    private void UpdatePerspective(bool isBeside, bool setSimState = true)
    {
        // Debug.Log("Theory UI Controls > UpdatePerspective()");
        if (cameraToggle) cameraToggle.isOn = isBeside;

        if (simState && setSimState)
        {
            var perspective = isBeside ? SimulationState.Perspective.Side : SimulationState.Perspective.Above;
            simState.SetPerspective(perspective);
        }
    }

    // Called from UI toggle
    public void UpdateReferenceFrame(bool isLab)
    {
        UpdateReferenceFrame(isLab, true);
    }

    private void UpdateReferenceFrame(bool isLab, bool setSimState = true)
    {
        // Debug.Log("Theory UI Controls > UpdateReferenceFrame()");
        if (referenceToggle) referenceToggle.isOn = isLab;

        if (simState && setSimState)
        {
            var referenceFrame = isLab ? SimulationState.ReferenceFrame.Lab : SimulationState.ReferenceFrame.Gun;
            simState.SetReferenceFrame(referenceFrame);
        }
    }
}
