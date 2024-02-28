// ----------------------------------------------------------------------------------------------------------
// Author: Austin Peel
//
// © All rights reserved. ECOLE POLYTECHNIQUE FEDERALE DE LAUSANNE, Switzerland, Section de Physique, 2024.
// See the LICENSE.md file for more details.
// ----------------------------------------------------------------------------------------------------------
using UnityEngine;

public class SandboxVectors : MonoBehaviour
{
    public CannonSimulationState simState;

    [Header("Vectors")]
    public Vector omegaVector;
    public Vector positionVector;
    public Vector velocityVector;
    public Vector centrifugalVector;
    public Vector coriolisVector;

    [Header("Visibility")]
    public bool showOmega;
    public bool showPosition;
    public bool showVelocity;
    public bool showCentrifugal;
    public bool showCoriolis;

    private void OnEnable()
    {
        CannonSimulationState.OnRedrawVectors += RedrawVectors;
    }

    private void OnDisable()
    {
        CannonSimulationState.OnRedrawVectors -= RedrawVectors;
    }

    private void Awake()
    {
        RedrawVectors();
    }

    public void RedrawVector(Vector vector, Vector3 position, Vector3 components, bool show)
    {
        if (!vector) return;

        vector.transform.position = position;
        vector.components = components;
        vector.Redraw();

        vector.gameObject.SetActive(show);
    }

    public void RedrawVectors()
    {
        if (!simState) return;

        Vector3 origin = Vector3.zero;
        RedrawVector(omegaVector, origin + 0.5f * Vector3.up, simState.omega, showOmega);
        Vector3 bulletPosition = simState.position;
        Vector3 offset = 0.5f * simState.bulletScale.x * bulletPosition.normalized;
        RedrawVector(positionVector, origin, bulletPosition - offset, showPosition);
        RedrawVector(velocityVector, bulletPosition, simState.velocity, showVelocity);
        RedrawVector(centrifugalVector, bulletPosition, simState.centrifugal, showCentrifugal);
        RedrawVector(coriolisVector, bulletPosition, simState.coriolis, showCoriolis);
    }

    // General method for setting the visibility of a vector quantity
    private void SetVectorVisibility(ref bool quantity, bool isVisible)
    {
        quantity = isVisible;
        RedrawVectors();
    }

    public void SetOmegaVisibility(bool isVisible)
    {
        SetVectorVisibility(ref showOmega, isVisible);
    }

    public void SetPositionVisibility(bool isVisible)
    {
        SetVectorVisibility(ref showPosition, isVisible);
    }

    public void SetVelocityVisibility(bool isVisible)
    {
        SetVectorVisibility(ref showVelocity, isVisible);
    }

    public void SetCentrifugalVisibility(bool isVisible)
    {
        SetVectorVisibility(ref showCentrifugal, isVisible);
    }

    public void SetCoriolisVisibility(bool isVisible)
    {
        SetVectorVisibility(ref showCoriolis, isVisible);
    }
}
