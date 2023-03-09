using UnityEngine;

public class Activity1Vectors : MonoBehaviour
{
    public DraggableVector velocity;
    public DraggableVector centrifugalForce;
    public DraggableVector coriolisForce;

    private Vector3 initialiVelocityPosition;
    private Vector3 initialiVelocityComponents;

    private Vector3 initialiCentrifugalPosition;
    private Vector3 initialiCentrifugalComponents;

    private Vector3 initialiCoriolisPosition;
    private Vector3 initialiCoriolisComponents;

    private void Awake()
    {
        if (velocity)
        {
            initialiVelocityPosition = velocity.transform.position;
            initialiVelocityComponents = velocity.components;
        }
        if (centrifugalForce)
        {
            initialiCentrifugalPosition = centrifugalForce.transform.position;
            initialiCentrifugalComponents = centrifugalForce.components;
        }
        if (coriolisForce)
        {
            initialiCoriolisPosition = coriolisForce.transform.position;
            initialiCoriolisComponents = coriolisForce.components;
        }
    }

    public void Reset()
    {
        if (velocity)
        {
            velocity.transform.position = initialiVelocityPosition;
            velocity.components = initialiVelocityComponents;
        }
        if (centrifugalForce)
        {
            centrifugalForce.transform.position = initialiCentrifugalPosition;
            centrifugalForce.components = initialiCentrifugalComponents;
        }
        if (coriolisForce)
        {
            coriolisForce.transform.position = initialiCoriolisPosition;
            coriolisForce.components = initialiCoriolisComponents;
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

    public void HandleGunFrameSimulationPaused(Vector3 bulletPosition)
    {
        Debug.Log("Bullet at " + bulletPosition);
    }
}
