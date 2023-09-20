using UnityEngine;

[CreateAssetMenu(fileName = "New Cannon Simulation State", menuName = "Cannon Simulation State", order = 51)]
public class CannonSimulationState : ScriptableObject
{
    [Tooltip("Rotation angle [deg]")] public float angle;
    [Tooltip("Scale of the current bullet")] public Vector3 bulletScale;
    [Tooltip("Angular velocity [rad/s]")] public Vector3 omega;
    [Tooltip("World space position of the current bullet")] public Vector3 position;
    [Tooltip("Velocity of the current bullet")] public Vector3 velocity;
    [Tooltip("Centrifugal force on the current bullet")] public Vector3 centrifugal;
    [Tooltip("Coriolis force on the current bullet")] public Vector3 coriolis;

    public static event System.Action OnRedrawVectors;

    public void Reset()
    {
        angle = 0;
        bulletScale = Vector3.zero;
        omega = Vector3.zero;
        position = Vector3.zero;
        velocity = Vector3.zero;
        centrifugal = Vector3.zero;
        coriolis = Vector3.zero;
    }

    public void RedrawVectors()
    {
        OnRedrawVectors?.Invoke();
    }
}
