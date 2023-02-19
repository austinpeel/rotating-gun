using System;
using System.Collections;
using UnityEngine;

public class RotatingGunSimulation : Slides.Simulation
{
    public Gun gun;
    public GameObject bulletPrefab;
    public SimulationState state;

    [Header("Camera")]
    public Transform mainCamera;
    public Vector3 cameraSidePosition = new Vector3(0, 5, -20);
    public Vector3 cameraSideRotation = new Vector3(20, 0, 0);
    public Vector3 cameraAbovePosition = new Vector3(0, 30, 0);
    public Vector3 cameraAboveRotation = new Vector3(90, 0, 0);
    private Coroutine cameraMovement;

    [Header("Parameters")]
    [Range(-2, 2), Tooltip("Rotations per second")] public float angularFrequency = 0f;
    public float bulletSpeed = 20;
    public int maxNumBullets = 10;
    public float maxBulletDistance = 10;
    [Range(0, 1)] public float timeScale = 1f;
    private float runningAngle;

    public enum ReferenceFrame { Lab, Gun }
    public enum Perspective { Side, Above }

    public ReferenceFrame referenceFrame;
    public Perspective perspective;

    [Header("Options")]
    public bool cameraControlledExternally;
    public bool traceBulletPath;
    public bool showPositionVector;

    private Transform bullets;

    public static event Action<ReferenceFrame> OnChangeReferenceFrame;
    public static event Action<float> OnChangeAngle;

    private void Awake()
    {
        // Create a container for the bullets
        bullets = new GameObject("Bullets").transform;
        bullets.SetParent(transform);

        // if (mainCamera) cameraSidePosition = mainCamera.position;

        // Initialize
        SetTimeScale(timeScale);
        SetReferenceFrame(referenceFrame, 0);
        SetPerspective(perspective, 0);
    }

    private void Update()
    {
        if (IsPaused) return;

        float deltaAngle = 360 * angularFrequency * Time.deltaTime;

        runningAngle += deltaAngle;
        if (runningAngle >= 360) runningAngle -= 360;
        if (runningAngle <= 0) runningAngle += 360;
        OnChangeAngle?.Invoke(runningAngle);

        // Increment the gun's rotation about the y-axis
        if (gun) gun.transform.RotateAround(gun.transform.position, Vector3.down, deltaAngle);

        // Rotate the camera if in the gun's reference frame
        if (referenceFrame == ReferenceFrame.Gun)
        {
            mainCamera.RotateAround(gun.transform.position, Vector3.down, deltaAngle);
        }
    }

    public void Fire()
    {
        if (!bullets || IsPaused) return;

        // Limit the number of bullets at a time
        if (bullets.childCount >= maxNumBullets) return;

        // Fire the gun
        if (gun) gun.Fire(bulletPrefab, bulletSpeed, bullets, maxBulletDistance, traceBulletPath, showPositionVector);
    }

    public void Reset(Vector3 cameraPosition, Quaternion cameraRotation)
    {
        if (gun)
        {
            gun.transform.position = Vector3.zero;
            gun.transform.rotation = Quaternion.identity;
        }

        if (mainCamera)
        {
            mainCamera.position = cameraPosition;
            mainCamera.rotation = cameraRotation;
        }
    }

    public void SetTimeScale(float timeScale)
    {
        Time.timeScale = timeScale;
    }

    public void ToggleRotationDirection()
    {
        angularFrequency = -angularFrequency;
    }

    public void SetReferenceFrame(ReferenceFrame referenceFrame, float cameraMoveTime)
    {
        this.referenceFrame = referenceFrame;

        if (gun)
        {
            gun.inGunFrame = referenceFrame == ReferenceFrame.Gun;
            gun.ResetBulletPath();
        }

        // Move camera to correct position
        SetPerspective(perspective, cameraMoveTime);

        // Alert other scripts
        OnChangeReferenceFrame?.Invoke(this.referenceFrame);
    }

    public void SetPerspective(Perspective perspective, float cameraMoveTime)
    {
        if (cameraControlledExternally || !mainCamera) return;

        Vector3 targetPosition = cameraSidePosition;
        Quaternion targetRotation = Quaternion.Euler(cameraSideRotation);
        if (perspective == Perspective.Above)
        {
            targetPosition = cameraAbovePosition;
            targetRotation = Quaternion.Euler(cameraAboveRotation);
        }

        // Align the camera's x-axis with the gun's
        if (gun && referenceFrame == ReferenceFrame.Gun)
        {
            if (perspective == Perspective.Above)
            {
                targetRotation = Quaternion.AngleAxis(gun.transform.eulerAngles.y, Vector3.up) * targetRotation;
            }
            else
            {
                targetPosition = Quaternion.Euler(0, gun.transform.eulerAngles.y, 0) * targetPosition;
                targetRotation = Quaternion.AngleAxis(gun.transform.eulerAngles.y, Vector3.up) * targetRotation;
            }
        }

        this.perspective = perspective;

        if (cameraMoveTime > 0)
        {
            cameraMovement = StartCoroutine(MoveCamera(targetPosition, targetRotation, cameraMoveTime));
        }
        else
        {
            mainCamera.position = targetPosition;
            mainCamera.rotation = targetRotation;
        }
    }

    public void TogglePerspective(bool value)
    {
        Perspective newPerspective = value ? Perspective.Side : Perspective.Above;

        if (newPerspective != perspective) SetPerspective(newPerspective, 1);
    }

    public void ToggleReferenceFrame(bool value)
    {
        ReferenceFrame newFrame = value ? ReferenceFrame.Lab : ReferenceFrame.Gun;

        if (newFrame != referenceFrame) SetReferenceFrame(newFrame, 1);
    }

    private IEnumerator MoveCamera(Vector3 targetPosition, Quaternion targetRotation, float moveTime)
    {
        Pause();

        Vector3 startPosition = mainCamera.position;
        Quaternion startRotation = mainCamera.rotation;
        float time = 0;

        while (time < moveTime)
        {
            time += Time.unscaledDeltaTime;
            float t = time / moveTime;
            t = t * t * (3f - 2f * t);
            mainCamera.position = Vector3.Slerp(startPosition, targetPosition, t);
            mainCamera.rotation = Quaternion.Slerp(startRotation, targetRotation, t);
            yield return null;
        }

        mainCamera.position = targetPosition;
        mainCamera.rotation = targetRotation;
        cameraMovement = null;

        Resume();
    }
}
