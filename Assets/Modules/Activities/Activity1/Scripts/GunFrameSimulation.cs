using System;
using System.Collections.Generic;
using UnityEngine;

public class GunFrameSimulation : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform gunBarrel;
    public LineRenderer bulletPath;

    [Header("Lab Frame Objects")]
    public Transform directionalLight;
    public Transform ground;

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
    private float autoFireClock;

    public SimulationState.Perspective perspective;

    [Header("Play / Pause")]
    public bool isPaused;
    public float bulletPauseDistance = 5;
    private bool hasPausedOnCurrentBullet;

    public static event Action<Vector3, Vector3, Vector3, Vector3> OnPause;

    [Header("Options")]
    public bool traceBulletPath;

    // Bullets
    private List<Bullet> bullets;
    private Bullet currentBullet;
    private bool tracingBulletPath;
    private Transform bulletContainer;

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
        Activity1Vectors.OnCheckVectors += HandleCheckVectors;
    }

    private void OnDisable()
    {
        Bullet.OnOutOfBounds -= HandleBulletOutOfBounds;
        Activity1Vectors.OnCheckVectors -= HandleCheckVectors;
    }

    public void Pause()
    {
        isPaused = true;
        if (currentBullet) currentBullet.Pause();
    }

    public void Resume()
    {
        isPaused = false;
        if (currentBullet)
        {
            currentBullet.Resume();
        }
    }

    private void FixedUpdate()
    {
        if (isPaused) return;

        if (currentBullet && !hasPausedOnCurrentBullet)
        {
            if (currentBullet.Position.magnitude > bulletPauseDistance)
            {
                Pause();
                hasPausedOnCurrentBullet = true;
                OnPause?.Invoke(transform.position, currentBullet.Position, currentBullet.GetV(), Omega);
            }
        }

        if (autoFire)
        {
            autoFireClock += Time.deltaTime;

            if (autoFireClock > 1f / fireFrequency)
            {
                Fire();
                autoFireClock = 0;
            }
        }

        float deltaTheta = Omega.y * Mathf.Rad2Deg * Time.deltaTime;

        if (directionalLight)
        {
            directionalLight.RotateAround(directionalLight.position, Vector3.up, deltaTheta);
        }

        if (ground)
        {
            ground.RotateAround(ground.position, Vector3.up, deltaTheta);
        }
    }

    private void LateUpdate()
    {
        if (!currentBullet) return;

        if (!tracingBulletPath || !bulletPath) return;

        bulletPath.positionCount++;
        Vector3 newPosition = currentBullet.transform.position;
        // Compute position relative to the gun
        bulletPath.useWorldSpace = false;
        newPosition = transform.InverseTransformPoint(newPosition);

        bulletPath.SetPosition(bulletPath.positionCount - 1, newPosition);
    }

    public void Fire()
    {
        // Do nothing if there's no bullet container or if the simulation is paused
        if (!bulletContainer || isPaused) return;

        // Limit the number of bullets at a time
        if (bulletContainer.childCount >= maxNumBullets) return;

        if (bullets == null) bullets = new List<Bullet>();

        ResetBulletPath();
        tracingBulletPath = traceBulletPath;

        // Determine where the bullet should spawn
        Vector3 bulletSpawnPosition = Vector3.zero;
        Vector3 e1 = transform.right;  // Unit vector along the gun's local x-axis (barrel)
        if (gunBarrel) bulletSpawnPosition = gunBarrel.position + 0.5f * gunBarrel.localScale.y * e1;

        // Create the new bullet
        Bullet bullet = Instantiate(bulletPrefab, bulletContainer).GetComponent<Bullet>();
        bullet.name = "Bullet";
        // Account for the velocity of the gun relative to the origin
        bullet.Initialize(bulletSpawnPosition,
                          bulletSpeed * e1 + Vector3.Cross(Omega, bulletSpawnPosition - transform.position),
                          maxBulletDistance,
                          Omega);

        bullets.Add(bullet);
        currentBullet = bullet;

        hasPausedOnCurrentBullet = false;
    }

    public void SetTimeScale(float value)
    {
        Time.timeScale = value;
        timeScale = value;
    }

    public void SetOmega(float value)
    {
        // Round to nearest tenth
        value = Mathf.Round(10 * value) / 10f;

        angularFrequency = value / (2 * Mathf.PI);

        if (currentBullet) currentBullet.SetOmega(Omega);
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

    public void HandleCheckVectors(bool allVectorsCorrect)
    {
        if (allVectorsCorrect) Resume();
    }
}
