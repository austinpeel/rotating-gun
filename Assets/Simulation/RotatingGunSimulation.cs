using System.Collections;
using UnityEngine;

public class RotatingGunSimulation : Slides.Simulation
{
    public Gun gun;
    public GameObject bulletPrefab;
    public SimulationState state;

    [Header("Parameters")]
    [Range(-2, 2)] public float angularFrequency = 0f;
    public float bulletSpeed = 20;
    public int maxNumBullets = 10;
    [Range(0, 1)] public float timeScale = 0.5f;

    public enum ReferenceFrame { Lab, Gun }
    public enum Perspective { Side, TopDown }

    public ReferenceFrame referenceFrame;
    public Perspective perspective;

    [Header("Options")]
    public bool cameraControlledExternally;
    public bool traceBulletPath;
    public bool showPositionVector;

    [HideInInspector] public Camera mainCamera;
    private Vector3 cameraSidePosition = new Vector3(0, 7, -20);
    private Vector3 cameraTopDownPosition = new Vector3(0, 30, 0);
    private Coroutine cameraMovement;

    private Transform bullets;

    private void Awake()
    {
        // Get reference to the main camera
        mainCamera = Camera.main;

        // Create a container for the bullets
        bullets = new GameObject("Bullets").transform;
        bullets.SetParent(transform);

        // Initialize
        SetTimeScale(timeScale);
        SetReferenceFrame(referenceFrame);
        SetPerspective(perspective);
    }

    private void Update()
    {
        // if (cameraControlledExternally) return;
        if (IsPaused) return;

        TakeRotationStep(Time.deltaTime);
    }

    private void TakeRotationStep(float deltaTime)
    {
        float angle = 360 * angularFrequency * deltaTime;

        // Increment the gun's rotation about the y-axis
        if (gun) gun.transform.RotateAround(gun.transform.position, Vector3.down, angle);

        // Rotate the camera if in the gun's reference frame
        if (referenceFrame == ReferenceFrame.Gun)
        {
            mainCamera.transform.RotateAround(gun.transform.position, Vector3.down, angle);
        }
    }

    public void Fire()
    {
        if (!bullets) return;

        // Limit the number of bullets at a time
        if (bullets.childCount >= maxNumBullets) return;

        // Fire the gun
        if (gun) gun.Fire(bulletPrefab, bulletSpeed, bullets, traceBulletPath, showPositionVector);
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
            mainCamera.transform.position = cameraPosition;
            mainCamera.transform.rotation = cameraRotation;
        }
    }

    public void SetTimeScale(float timeScale)
    {
        Time.timeScale = timeScale;
    }

    public void SetReferenceFrame(ReferenceFrame referenceFrame)
    {
        if (gun)
        {
            gun.inGunFrame = referenceFrame == ReferenceFrame.Gun;
            gun.ResetBulletPath();
        }
        this.referenceFrame = referenceFrame;
    }

    public void SetPerspective(Perspective perspective)
    {
        if (cameraControlledExternally) return;

        mainCamera.transform.position = (perspective == Perspective.Side) ? cameraSidePosition : cameraTopDownPosition;
        mainCamera.transform.LookAt(gun ? gun.transform.position : Vector3.zero);

        this.perspective = perspective;
    }

    public void TogglePerspective(float moveTime)
    {
        if (cameraControlledExternally) return;

        if (cameraMovement != null) StopCoroutine(cameraMovement);

        Vector3 targetPosition;
        if (perspective == Perspective.Side)
        {
            perspective = Perspective.TopDown;
            targetPosition = cameraTopDownPosition;
        }
        else
        {
            perspective = Perspective.Side;
            targetPosition = Quaternion.AngleAxis(mainCamera.transform.localEulerAngles.y, Vector3.up) * cameraSidePosition;
        }

        cameraMovement = StartCoroutine(MoveCamera(targetPosition, gun.transform.position, moveTime));
    }

    public void ToggleReferenceFrame()
    {
        if (cameraMovement != null) return;

        if (referenceFrame == ReferenceFrame.Lab)
        {
            SetReferenceFrame(ReferenceFrame.Gun);
        }
        else
        {
            SetReferenceFrame(ReferenceFrame.Lab);
        }
    }

    private IEnumerator MoveCamera(Vector3 targetPosition, Vector3 lookAt, float moveTime)
    {
        Vector3 startPosition = mainCamera.transform.position;
        float time = 0;

        while (time < moveTime)
        {
            time += Time.unscaledDeltaTime;
            float t = time / moveTime;
            t = t * t * (3f - 2f * t);
            mainCamera.transform.position = Vector3.Slerp(startPosition, targetPosition, t);
            mainCamera.transform.LookAt(lookAt);
            yield return null;
        }

        mainCamera.transform.position = targetPosition;
        cameraMovement = null;
    }
}
