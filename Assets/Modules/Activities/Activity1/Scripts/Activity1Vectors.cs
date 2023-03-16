using System.Collections.Generic;
using UnityEngine;

public class Activity1Vectors : MonoBehaviour
{
    public DraggableVector velocity;
    public DraggableVector centrifugalForce;
    public DraggableVector coriolisForce;

    private Vector3 initialVelocityPosition;
    private Vector3 initialVelocityComponents;

    private Vector3 initialCentrifugalPosition;
    private Vector3 initialCentrifugalComponents;

    private Vector3 initialCoriolisPosition;
    private Vector3 initialCoriolisComponents;

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

    public void Reset()
    {
        ResetVector(velocity, initialVelocityPosition, initialVelocityComponents);
        ResetVector(centrifugalForce, initialCentrifugalPosition, initialCentrifugalComponents);
        ResetVector(coriolisForce, initialCoriolisPosition, initialCoriolisComponents);
    }

    private void ResetVector(DraggableVector vector, Vector3 initialPosition, Vector3 initialComponents)
    {
        if (vector)
        {
            vector.transform.position = initialPosition;
            vector.components = initialComponents;
            vector.Redraw();
            vector.useStickyPoint = false;
            vector.useStickyDirections = false;
            vector.stickyDirections = new List<Vector3>();
        }
    }

    public void HandleGunFrameSimulationPaused(Vector3 simPosition, Vector3 bulletPosition, Vector3 bulletVelocity, Vector3 omega)
    {
        Vector3 velocityDirection = bulletVelocity.normalized;
        Vector3 centrifugalDirection = (bulletPosition - simPosition).normalized;
        Vector3 coriolisDirection = Vector3.Cross(omega, bulletVelocity).normalized;
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
}
