using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Activity1Vectors : MonoBehaviour
{
    public DraggableVector velocity;
    public DraggableVector centrifugalForce;
    public DraggableVector coriolisForce;

    [Header("UI")]
    public Button fireButton;
    public Button checkButton;
    public Slider omegaSlider;

    private Vector3 initialVelocityPosition;
    private Vector3 initialVelocityComponents;

    private Vector3 initialCentrifugalPosition;
    private Vector3 initialCentrifugalComponents;

    private Vector3 initialCoriolisPosition;
    private Vector3 initialCoriolisComponents;

    private bool truthIsKnown;
    private Vector3 velocityDirection;
    private Vector3 centrifugalDirection;
    private Vector3 coriolisDirection;

    public static event Action<bool> OnCheckVectors;

    private void Awake()
    {
        if (velocity)
        {
            initialVelocityPosition = velocity.transform.position;
            initialVelocityComponents = velocity.components;
        }
        if (centrifugalForce)
        {
            initialCentrifugalPosition = centrifugalForce.transform.position;
            initialCentrifugalComponents = centrifugalForce.components;
        }
        if (coriolisForce)
        {
            initialCoriolisPosition = coriolisForce.transform.position;
            initialCoriolisComponents = coriolisForce.components;
        }
    }

    private void OnEnable()
    {
        GunFrameSimulation.OnPause += HandleGunFrameSimulationPaused;
    }

    private void OnDisable()
    {
        GunFrameSimulation.OnPause -= HandleGunFrameSimulationPaused;
    }

    public void Reset(bool resetTruth)
    {
        ResetVector(velocity, initialVelocityPosition, initialVelocityComponents);
        ResetVector(centrifugalForce, initialCentrifugalPosition, initialCentrifugalComponents);
        ResetVector(coriolisForce, initialCoriolisPosition, initialCoriolisComponents);

        if (fireButton)
        {
            fireButton.interactable = resetTruth;
            if (fireButton.TryGetComponent(out CursorHoverUI cursor))
            {
                cursor.enabled = resetTruth;
            }
        }
        if (checkButton)
        {
            checkButton.interactable = !resetTruth;
            if (checkButton.TryGetComponent(out CursorHoverUI cursor))
            {
                cursor.enabled = !resetTruth;
            }
        }
        if (omegaSlider)
        {
            omegaSlider.interactable = resetTruth;
            if (omegaSlider.TryGetComponent(out CursorHoverUI cursor))
            {
                cursor.enabled = resetTruth;
            }
        }

        if (resetTruth) truthIsKnown = false;
    }

    private void ResetVector(DraggableVector vector, Vector3 initialPosition, Vector3 initialComponents)
    {
        if (vector)
        {
            vector.transform.position = initialPosition;
            vector.components = initialComponents;
            vector.Redraw();

            if (!truthIsKnown)
            {
                vector.useStickyPoint = false;
                vector.useStickyDirections = false;
                vector.stickyDirections = new List<Vector3>();
            }
        }
    }

    public void HandleGunFrameSimulationPaused(Vector3 simPosition, Vector3 bulletPosition, Vector3 bulletVelocity, Vector3 omega)
    {
        velocityDirection = bulletVelocity.normalized;
        centrifugalDirection = (bulletPosition - simPosition).normalized;
        coriolisDirection = Vector3.Cross(omega, bulletVelocity).normalized;  // Unity is left-handed
        List<Vector3> stickyDirections = new List<Vector3>
        {
            velocityDirection,
            -velocityDirection,
            centrifugalDirection,
            -centrifugalDirection,
            coriolisDirection,
            -coriolisDirection
        };

        AddStickiness(velocity, bulletPosition, stickyDirections);
        AddStickiness(centrifugalForce, bulletPosition, stickyDirections);
        AddStickiness(coriolisForce, bulletPosition, stickyDirections);

        truthIsKnown = true;
    }

    private void AddStickiness(DraggableVector vector, Vector3 point, List<Vector3> directions)
    {
        if (vector)
        {
            vector.useStickyPoint = true;
            vector.stickyPoint = point;

            vector.useStickyDirections = true;
            vector.stickyDirections = directions;
        }
    }

    private bool VectorsAreAligned(Vector3 vector1, Vector3 vector2)
    {
        float angle = Vector3.Angle(vector1, vector2);
        Debug.Log(angle);
        return angle == 0;
    }

    public void CheckAnswers()
    {
        if (!truthIsKnown)
        {
            Debug.LogWarning("Activity1Vectors > we don't yet know the truth");
            return;
        }

        // TODO What if Omega = 0?

        bool allVectorsCorrect = VectorsAreAligned(velocityDirection, velocity.components);
        allVectorsCorrect &= VectorsAreAligned(centrifugalDirection, centrifugalForce.components);
        allVectorsCorrect &= VectorsAreAligned(coriolisDirection, coriolisForce.components);

        // Debug.Log("All correct ? " + allVectorsCorrect);
        Reset(allVectorsCorrect);

        OnCheckVectors?.Invoke(allVectorsCorrect);
    }
}
