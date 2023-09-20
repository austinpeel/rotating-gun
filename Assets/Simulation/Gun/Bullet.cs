using System;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float maxDistance = 10;

    public static event Action<Bullet> OnOutOfBounds;
    public static event Action<Bullet> OnHitTarget;

    public Vector3 Position => transform.position;

    private Vector3 velocity;
    public Vector3 Velocity => velocity;

    private Vector3 omega;
    // Whether to solve the non-inertial frame equations of motion numerically
    [HideInInspector] public bool gunFrameSim = false;
    [HideInInspector] public bool isAnimation = false;

    // Arrays for numerical integration
    private float[] x;
    private float[] xdot;

    private bool isPaused;

    // Hit the target
    private void OnTriggerEnter(Collider other)
    {
        OnHitTarget?.Invoke(this);
    }

    public void Initialize(Vector3 position, Vector3 velocity, float maxDistance)
    {
        transform.position = position;
        this.velocity = velocity;
        this.maxDistance = maxDistance;
    }

    // Used by GunFrameSimulation and GunFrameAnimation
    public void Initialize(Vector3 position,
                           Vector3 velocity,
                           float maxDistance,
                           Vector3 omega,
                           bool isAnimation = false)
    {
        gunFrameSim = true;

        // Initialize equations of motion arrays
        x = new float[6] { position.x, position.y, position.z, velocity.x, velocity.y, velocity.z };
        Vector3 a = ComputeAccelerations();
        xdot = new float[6] { x[3], x[4], x[5], a.x, a.y, a.z };

        this.omega = omega;
        this.maxDistance = maxDistance;
        this.isAnimation = isAnimation;
    }

    private void Update()
    {
        if (isPaused || isAnimation) return;

        if (!gunFrameSim)
        {
            transform.Translate(velocity * Time.deltaTime, Space.World);
        }

        CheckForOutOfBounds();
    }

    private void FixedUpdate()
    {
        // Solve the equations of motion in the Gun Frame
        if (!gunFrameSim || isPaused || isAnimation) return;

        // Evolve equations by a time step
        int numSubsteps = 10;
        float deltaTime = Time.fixedDeltaTime / numSubsteps;
        for (int i = 0; i < numSubsteps; i++)
        {
            TakeLeapfrogStep(deltaTime);
        }

        // Update the bullet position
        transform.position = new Vector3(x[0], x[1], x[2]);
    }

    private Vector3 ComputeAccelerations()
    {
        Vector3 position = new Vector3(x[0], x[1], x[2]);
        Vector3 velocity = new Vector3(x[3], x[4], x[5]);

        Vector3 centrifugalAcceleration = -Vector3.Cross(omega, Vector3.Cross(omega, position));
        Vector3 coriolisAcceleration = 2 * Vector3.Cross(omega, velocity);

        Vector3 totalAcceleration = centrifugalAcceleration + coriolisAcceleration;

        return totalAcceleration;
    }

    public void TakeLeapfrogStep(float deltaTime)
    {
        // Update positions with current velocities and accelerations
        x[0] += deltaTime * (xdot[0] + 0.5f * xdot[3] * deltaTime);
        x[1] += deltaTime * (xdot[1] + 0.5f * xdot[4] * deltaTime);
        x[2] += deltaTime * (xdot[2] + 0.5f * xdot[5] * deltaTime);

        // Compute new accelerations and update velocities
        Vector3 aNew = ComputeAccelerations();
        x[3] += 0.5f * (xdot[3] + aNew.x) * deltaTime;
        x[4] += 0.5f * (xdot[4] + aNew.y) * deltaTime;
        x[5] += 0.5f * (xdot[5] + aNew.z) * deltaTime;

        // Update accelerations
        xdot[0] = x[3];
        xdot[1] = x[4];
        xdot[2] = x[5];
        xdot[3] = aNew.x;
        xdot[4] = aNew.y;
        xdot[5] = aNew.z;
    }

    public void CheckForOutOfBounds()
    {
        if (transform.position.magnitude >= maxDistance)
        {
            OnOutOfBounds?.Invoke(this);
        }
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }

    public void SetOmega(Vector3 value)
    {
        omega = value;
    }

    public void Pause()
    {
        isPaused = true;
    }

    public void Resume()
    {
        isPaused = false;
    }

    public Vector3 GetX()
    {
        return new Vector3(x[0], x[1], x[2]);
    }

    public Vector3 GetV()
    {
        return new Vector3(xdot[0], xdot[1], xdot[2]);
    }
}
