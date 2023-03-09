using System.Collections.Generic;
using UnityEngine;

public class GunFrameSimulation : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform gunBarrel;
    public LineRenderer bulletPath;

    [Header("Camera")]
    public Transform mainCamera;
    public Vector3 cameraSidePosition = new Vector3(0, 5, -20);
    public Vector3 cameraSideRotation = new Vector3(20, 0, 0);
    public Vector3 cameraAbovePosition = new Vector3(0, 30, 0);
    public Vector3 cameraAboveRotation = new Vector3(90, 0, 0);

    [Header("Parameters")]
    public bool autoFire;
    [Tooltip("Bullets per second")] public float fireFrequency = 1;
    [Range(-2, 2), Tooltip("Rotations per second")] public float angularFrequency = 0f;
    public float bulletSpeed = 20;
    public int maxNumBullets = 10;
    public float maxBulletDistance = 10;
    [Range(0, 1)] public float timeScale = 1f;
    public float vectorScaleFactor = 1;
    private float autoFireClock;

    public SimulationState.Perspective perspective;

    [Header("Options")]
    public bool traceBulletPath;

    private List<Bullet> bullets;
    private Bullet currentBullet;
    private bool tracingBulletPath;
    private Transform bulletContainer;
    private bool isPaused = false;

    private Vector3 Omega => angularFrequency * 2 * Mathf.PI * Vector3.up;

    private void Awake()
    {
        // Create a container for the bullets
        bulletContainer = new GameObject("Bullets").transform;
        bulletContainer.SetParent(transform);

        // Initialize
        SetTimeScale(timeScale);
        SetPerspective(perspective);
    }

    private void OnEnable()
    {
        Bullet.OnOutOfBounds += HandleBulletOutOfBounds;
    }

    private void OnDisable()
    {
        Bullet.OnOutOfBounds -= HandleBulletOutOfBounds;
    }

    public void Pause()
    {
        isPaused = true;
    }

    public void Resume()
    {
        isPaused = false;
    }

    private void Update()
    {
        if (isPaused) return;

        if (autoFire)
        {
            autoFireClock += Time.deltaTime;

            if (autoFireClock > 1f / fireFrequency)
            {
                Fire(bulletSpeed, maxBulletDistance, traceBulletPath);
                autoFireClock = 0;
            }
        }

        if (!currentBullet) return;

        // Compute kinematical and dynamical quantities
        Vector3 position = currentBullet.Position;
        Vector3 velocity = currentBullet.Velocity;
        Vector3 centrifugalForce = Vector3.zero;
        Vector3 coriolisForce = Vector3.zero;

        // Unity is left-handed, so add instead of subtract
        velocity += Vector3.Cross(Omega, position);

        // Assume bullet mass = 1 kg
        centrifugalForce = -Vector3.Cross(Omega, Vector3.Cross(Omega, position));
        coriolisForce = 2 * Vector3.Cross(Omega, velocity);

        velocity *= vectorScaleFactor;
        centrifugalForce *= vectorScaleFactor;
        coriolisForce *= 0.5f * vectorScaleFactor;
    }

    private void LateUpdate()
    {
        if (!currentBullet) return;

        // // Update displayed vectors
        // if (positionVector != null)
        // {
        //     Vector3 offset = 0.5f * currentBullet.transform.localScale.x * position.normalized;
        //     positionVector.components = position - offset;
        //     positionVector.Redraw();
        // }

        // if (velocityVector != null)
        // {
        //     velocityVector.transform.position = currentBullet.Position;
        //     velocityVector.components = velocity;
        //     velocityVector.Redraw();
        // }

        // if (centrifugalForceVector != null && inGunFrame)
        // {
        //     centrifugalForceVector.transform.position = currentBullet.Position;
        //     centrifugalForceVector.components = centrifugalForce;
        //     centrifugalForceVector.Redraw();
        // }

        // if (coriolisForceVector != null && inGunFrame)
        // {
        //     coriolisForceVector.transform.position = currentBullet.Position;
        //     coriolisForceVector.components = coriolisForce;
        //     coriolisForceVector.Redraw();
        // }

        if (!tracingBulletPath || !bulletPath) return;

        bulletPath.positionCount++;
        Vector3 newPosition = currentBullet.transform.position;
        // Compute position relative to the gun
        bulletPath.useWorldSpace = false;
        newPosition = transform.InverseTransformPoint(newPosition);

        bulletPath.SetPosition(bulletPath.positionCount - 1, newPosition);
    }

    public void Fire(float speed, float maxDistance, bool tracePath = false)
    {
        // Do nothing if there's no bullet container or if the simulation is paused
        if (!bulletContainer || isPaused) return;

        // Limit the number of bullets at a time
        if (bulletContainer.childCount >= maxNumBullets) return;

        if (bullets == null) bullets = new List<Bullet>();

        ResetBulletPath();
        tracingBulletPath = tracePath;

        // Determine where the bullet should spawn
        Vector3 bulletSpawnPosition = Vector3.zero;
        Vector3 e1 = transform.right;  // Unit vector along the gun's local x-axis (barrel)
        if (gunBarrel) bulletSpawnPosition = gunBarrel.position + 0.5f * gunBarrel.localScale.y * e1;

        // Create the new bullet
        Bullet bullet = Instantiate(bulletPrefab, transform).GetComponent<Bullet>();
        bullet.name = "Bullet";
        bullet.Initialize(bulletSpawnPosition, speed * e1, maxDistance);

        bullets.Add(bullet);
        currentBullet = bullet;
    }

    public void SetTimeScale(float timeScale)
    {
        Time.timeScale = timeScale;
        this.timeScale = timeScale;
    }

    public void SetPerspective(SimulationState.Perspective perspective)
    {
        if (!mainCamera) return;

        this.perspective = perspective;

        // Compute camera position and rotation based on reference frame
        Vector3 targetPosition = cameraSidePosition;
        Quaternion targetRotation = Quaternion.Euler(cameraSideRotation);
        if (perspective == SimulationState.Perspective.Above)
        {
            targetPosition = cameraAbovePosition;
            targetRotation = Quaternion.Euler(cameraAboveRotation);
        }

        mainCamera.position = targetPosition;
        mainCamera.rotation = targetRotation;
    }

    public void ResetBulletPath()
    {
        if (bulletPath) bulletPath.positionCount = 0;
    }

    public void HandleBulletOutOfBounds(Bullet bullet)
    {
        if (bullets == null) return;

        bullets.Remove(bullet);
        bullet.Destroy();

        if (bullets.Count == 0)
        {
            currentBullet = null;
            ResetBulletPath();
        }
    }
}
