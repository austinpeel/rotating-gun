using System.Collections;
using UnityEngine;

public class RotatingGunSimulation : Slides.Simulation
{
    public Gun gun;
    public GameObject bulletPrefab;
    public SimulationState simState;

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

    public SimulationState.ReferenceFrame referenceFrame;
    public SimulationState.Perspective perspective;

    [Header("Options")]
    public bool cameraControlledExternally;
    public bool traceBulletPath;
    public bool showPosition;
    public bool showVelocity;
    public bool showCentrifugalForce;
    public bool showCoriolisForce;

    private Transform bulletContainer;

    private void Awake()
    {
        // Create a container for the bullets
        bulletContainer = new GameObject("Bullets").transform;
        bulletContainer.SetParent(transform);

        // Initialize
        SetTimeScale(timeScale);

        // Set simulation parameters according to SimulationState values
        if (simState)
        {
            angularFrequency = simState.GetAngularFrequency();
            runningAngle = simState.theta;
            if (gun) gun.transform.rotation = Quaternion.Euler(0, -runningAngle, 0);
            // Debug.Log("Sim Awake : frequency = " + angularFrequency);

            SetReferenceFrame(simState.referenceFrame);
            SetPerspective(simState.perspective);
        }
    }

    private void OnEnable()
    {
        SimulationState.OnChangeOmega += HandleOmegaChange;
        SimulationState.OnChangePerspective += HandlePerspectiveChange;
        SimulationState.OnChangeReferenceFrame += HandleReferenceFrameChange;
    }

    private void OnDisable()
    {
        SimulationState.OnChangeOmega -= HandleOmegaChange;
        SimulationState.OnChangePerspective -= HandlePerspectiveChange;
        SimulationState.OnChangeReferenceFrame -= HandleReferenceFrameChange;
    }

    private void Update()
    {
        if (IsPaused) return;

        float deltaAngle = 360 * angularFrequency * Time.deltaTime;

        runningAngle += deltaAngle;
        if (runningAngle >= 360) runningAngle -= 360;
        if (runningAngle <= 0) runningAngle += 360;

        // Increment the gun's rotation about the y-axis
        if (gun) gun.transform.RotateAround(gun.transform.position, Vector3.down, deltaAngle);

        // Rotate the camera if in the gun's reference frame
        if (referenceFrame == SimulationState.ReferenceFrame.Gun && mainCamera)
        {
            mainCamera.RotateAround(gun.transform.position, Vector3.down, deltaAngle);
        }

        if (simState)
        {
            simState.SetTheta(runningAngle);

            // Update sim state's omega value if angular frequency is changed in the inspector
            if (angularFrequency != simState.GetAngularFrequency())
            {
                simState.SetOmegaFromFrequency(angularFrequency);
            }
        }
    }

    public void Fire()
    {
        // Do nothing if there's no bullet container or if the simulation is paused
        if (!bulletContainer || IsPaused) return;

        // Limit the number of bullets at a time
        if (bulletContainer.childCount >= maxNumBullets) return;

        // Fire the gun
        if (gun) gun.Fire(bulletPrefab,
                          bulletSpeed,
                          bulletContainer,
                          maxBulletDistance,
                          traceBulletPath,
                          showPosition,
                          showVelocity,
                          showCentrifugalForce,
                          showCoriolisForce);
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

    public void HandleOmegaChange()
    {
        if (!simState) return;

        // Debug.Log("Sim > heard angular frequency : " + angularFrequency);
        angularFrequency = simState.GetAngularFrequency();
    }

    public void HandlePerspectiveChange()
    {
        if (!simState) return;

        // Debug.Log("Sim > heard perspective : " + simState.perspective);
        SetPerspective(simState.perspective);
    }

    public void HandleReferenceFrameChange()
    {
        if (!simState) return;

        // Debug.Log("Sim > heard reference : " + simState.referenceFrame);
        SetReferenceFrame(simState.referenceFrame);
    }

    public void SetTimeScale(float timeScale)
    {
        Time.timeScale = timeScale;
        this.timeScale = timeScale;
    }

    public void SetReferenceFrame(SimulationState.ReferenceFrame referenceFrame)
    {
        this.referenceFrame = referenceFrame;

        if (gun)
        {
            gun.inGunFrame = referenceFrame == SimulationState.ReferenceFrame.Gun;
            gun.ResetBulletPath();
        }

        // Move camera to correct position
        SetPerspective(perspective);
    }

    public void SetPerspective(SimulationState.Perspective perspective)
    {
        if (cameraControlledExternally || !mainCamera) return;

        this.perspective = perspective;

        // Compute camera position and rotation based on reference frame
        Vector3 targetPosition = cameraSidePosition;
        Quaternion targetRotation = Quaternion.Euler(cameraSideRotation);
        if (perspective == SimulationState.Perspective.Above)
        {
            targetPosition = cameraAbovePosition;
            targetRotation = Quaternion.Euler(cameraAboveRotation);
        }

        // Align the camera's x-axis with the gun's x-axis
        if (gun && referenceFrame == SimulationState.ReferenceFrame.Gun)
        {
            if (perspective == SimulationState.Perspective.Above)
            {
                targetRotation = Quaternion.AngleAxis(gun.transform.eulerAngles.y, Vector3.up) * targetRotation;
            }
            else
            {
                targetPosition = Quaternion.Euler(0, gun.transform.eulerAngles.y, 0) * targetPosition;
                targetRotation = Quaternion.AngleAxis(gun.transform.eulerAngles.y, Vector3.up) * targetRotation;
            }
        }

        mainCamera.position = targetPosition;
        mainCamera.rotation = targetRotation;

        // Move camera to correct position
        // if (cameraMoveTime > 0)
        // {
        //     cameraMovement = StartCoroutine(MoveCamera(targetPosition, targetRotation, cameraMoveTime));
        // }
        // else
        // {
        //     mainCamera.position = targetPosition;
        //     mainCamera.rotation = targetRotation;
        // }
    }

    // public void TogglePerspective(bool value)
    // {
    //     SimulationState.Perspective newPerspective = value ? SimulationState.Perspective.Side : SimulationState.Perspective.Above;

    //     // if (newPerspective != perspective) SetPerspective(newPerspective, 1);
    // }

    // public void ToggleReferenceFrame(bool value)
    // {
    //     SimulationState.ReferenceFrame newFrame = value ? SimulationState.ReferenceFrame.Lab : SimulationState.ReferenceFrame.Gun;

    //     // if (newFrame != referenceFrame) SetReferenceFrame(newFrame, 1);
    // }

    public void SetAssumptionsTabVisibility(bool tabIsActive)
    {
        if (tabIsActive)
        {
            showPosition = false;
            showVelocity = false;
            showCentrifugalForce = false;
            showCoriolisForce = false;
        }
    }

    public void SetCoordinateTabVisibility(bool tabIsActive)
    {
        if (tabIsActive)
        {
            showPosition = false;
            showVelocity = false;
            showCentrifugalForce = false;
            showCoriolisForce = false;
        }
    }

    public void SetQuantitiesTabVisibility(bool tabIsActive)
    {
        if (tabIsActive)
        {
            showPosition = true;
            showVelocity = true;
        }
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
