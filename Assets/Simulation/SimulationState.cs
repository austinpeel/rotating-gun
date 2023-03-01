using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New Simulation State", menuName = "Simulation State", order = 50)]
public class SimulationState : ScriptableObject
{
    [Tooltip("Degrees")] public float theta;
    [Tooltip("Rad / s")] public Vector3 omega;

    public enum ReferenceFrame { Lab, Gun }
    public ReferenceFrame referenceFrame;

    public enum Perspective { Side, Above }
    public Perspective perspective;

    public static event Action OnChangeOmega;
    public static event Action OnChangePerspective;
    public static event Action OnChangeReferenceFrame;

    public void SetReferenceFrame(ReferenceFrame newReferenceFrame)
    {
        referenceFrame = newReferenceFrame;
        OnChangeReferenceFrame?.Invoke();

        // Debug.Log("SimState > SetReferenceFrame()");
    }

    public void SetPerspective(Perspective newPerspective)
    {
        perspective = newPerspective;
        OnChangePerspective?.Invoke();

        // Debug.Log("SimState > SetPerspective()");
    }

    public void SetOmega(Vector3 omega)
    {
        this.omega = omega;
        OnChangeOmega?.Invoke();

        // Debug.Log("SimState > SetOmega()");
    }

    public void SetOmegaFromFrequency(float angularFrequency)
    {
        SetOmega(angularFrequency * 2 * Mathf.PI * Vector3.up);
    }

    public float GetAngularFrequency()
    {
        return omega.y / (2 * Mathf.PI);
    }

    public void SetTheta(float theta)
    {
        this.theta = theta;
    }

    public void Reset()
    {
        theta = 0;
        omega = Vector3.up;
        referenceFrame = ReferenceFrame.Lab;
        perspective = Perspective.Side;
    }
}
