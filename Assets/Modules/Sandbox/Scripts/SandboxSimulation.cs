using UnityEngine;

public class SandboxSimulation : Simulation
{
    public Cannon cannon;
    public CannonSimulationState simState;

    [Header("Parameters")]
    public bool autoFire;
    [Tooltip("Bullets per second")] public float fireFrequency = 1;
    public CannonSettings cannonSettings;

    // [Range(0, 1)] public float timeScale = 1f;
    private float autoFireClock;
    private Transform bulletContainer;

    private void Awake()
    {
        // Create a container for the bullets
        bulletContainer = new GameObject("Bullets").transform;
        bulletContainer.SetParent(transform);
    }

    private void OnDisable()
    {
        if (simState) simState.Reset();
    }

    private void InitializeCannon()
    {
        if (!cannon) return;

        // Assign simulation settings to the cannon
        cannon.settings = cannonSettings;

        // Do not allow the cannon to rotate on its own
        cannon.autoUpdate = false;

        // Collect the cannon's bullets in the bullet container object
        cannon.bulletContainer = bulletContainer;
    }

    private void Start()
    {
        InitializeCannon();
    }

    private void Update()
    {
        if (IsPaused) return;

        // Rotate the cannon about the y-axis
        if (cannon) cannon.TakeAStep(Time.deltaTime);

        if (autoFire)
        {
            autoFireClock += Time.deltaTime;

            if (autoFireClock > 1f / fireFrequency)
            {
                Fire();
                autoFireClock = 0;
            }
        }
    }

    public void LateUpdate()
    {
        if (simState)
        {
            simState.angle = cannon.CurrentAngle;
            simState.bulletScale = cannon.BulletScale;
            simState.position = cannon.BulletPosition;
            simState.velocity = cannon.BulletVelocity;
            simState.centrifugal = cannon.CentrifugalForce;
            simState.coriolis = cannon.CoriolisForce;
            simState.RedrawVectors();
        }

        if (cannon) cannon.UpdateBulletPath();
    }

    public void Fire()
    {
        // Do nothing if there's no bullet container or if the simulation is paused
        if (!bulletContainer || IsPaused) return;

        // Fire the cannon
        if (cannon) cannon.Fire();
    }

    public void Reset(Vector3 cameraPosition, Quaternion cameraRotation)
    {
        if (cannon)
        {
            cannon.transform.position = Vector3.zero;
            cannon.transform.rotation = Quaternion.identity;
        }
    }

    public void SetAngularFrequency(float angularFrequency)
    {
        cannonSettings.angularFrequency = angularFrequency;

        // Update the simulation state
        if (simState)
        {
            simState.omega = cannonSettings.Omega;
            simState.RedrawVectors();
        }
    }

    public void SetBulletSpeed(float bulletSpeed)
    {
        cannonSettings.bulletSpeed = bulletSpeed;
    }

    public void ToggleSound(bool soundIsOn)
    {
        cannonSettings.playSoundEffects = soundIsOn;
    }

    public void ToggleReferenceFrame(bool frameIsLab)
    {
        cannonSettings.inCannonFrame = !frameIsLab;
        cannon.ResetBulletPath();
    }

    public void SetBulletPathVisibility(bool isVisible)
    {
        cannonSettings.showBulletPath = isVisible;
    }
}
