using UnityEngine;

public class ExampleSimulation : Slides.Simulation
{
    public Transform cube;
    public bool isRotating;
    public bool isTranslating;
    public SimulationState simState;

    private float elapsedTime;

    private void OnEnable()
    {
        if (simState)
        {
            cube.rotation = simState.rotation;
            elapsedTime = simState.time;
        }
    }

    private void FixedUpdate()
    {
        if (!cube || IsPaused) return;

        if (isRotating)
        {
            float deltaAngle = 20 * Time.fixedDeltaTime;
            cube.RotateAround(cube.position, (Vector3.up + Vector3.right).normalized, deltaAngle);
        }

        if (isTranslating)
        {
            elapsedTime += Time.fixedDeltaTime;

            float x = Mathf.Sin(elapsedTime);
            Vector3 position = cube.position;
            position.x = x;
            cube.position = position;
        }

        if (simState)
        {
            simState.rotation = cube.rotation;
            simState.time = elapsedTime;
        }
    }

    public void Reset()
    {
        elapsedTime = 0;
    }
}
