using UnityEngine;

public class RotatingGunSlideController : Slides.SimulationSlideController
{
    public float angularFrequency;
    public SimulationState.ReferenceFrame referenceFrame;
    public bool traceBulletPath;
    public bool showPosition;

    [Header("Lights")]
    public GameObject labFrameLight;
    public bool activateLabFrameLight;
    public GameObject gunFrameLight;
    public bool activateGunFrameLight;

    [Header("Inset Camera")]
    public Camera insetCamera;
    public bool showInsetCamera;

    [Header("Ground")]
    public Transform ground;
    public bool showGround;

    private RotatingGunSimulation sim;
    private bool slideHasInitialized;

    private Vector3 cameraPosition;
    private Quaternion cameraRotation;

    public override void InitializeSlide()
    {
        Debug.Log("Initializing " + transform.name);

        sim.angularFrequency = angularFrequency;
        sim.SetGunOmegaFromAngularFrequency(angularFrequency);
        sim.referenceFrame = referenceFrame;
        sim.traceBulletPath = traceBulletPath;
        sim.showPosition = showPosition;
        sim.Pause();

        if (labFrameLight) labFrameLight.SetActive(activateLabFrameLight);
        if (gunFrameLight) gunFrameLight.SetActive(activateGunFrameLight);
        if (insetCamera) insetCamera.gameObject.SetActive(false);
        if (ground) ground.gameObject.SetActive(showGround);

        slideHasInitialized = true;
    }

    private void OnEnable()
    {
        CameraController.OnCameraMovementComplete += HandleCameraMovementComplete;

        sim = simulation as RotatingGunSimulation;
    }

    private void OnDisable()
    {
        CameraController.OnCameraMovementComplete -= HandleCameraMovementComplete;

        // Reset the positions and orientations of the camera and simulation
        if (slideHasInitialized) sim.Reset(cameraPosition, cameraRotation);
    }

    public void HandleCameraMovementComplete(Vector3 cameraPosition, Quaternion cameraRotation)
    {
        // Debug.Log(transform.name + " sim is not null ? " + (sim != null));

        // if (enabled)
        // {
        //     this.cameraPosition = cameraPosition;
        //     this.cameraRotation = cameraRotation;

        //     if (sim) sim.Resume();

        //     if (insetCamera) insetCamera.gameObject.SetActive(showInsetCamera);
        // }

        if (slideHasInitialized)
        {
            Debug.Log(transform.name + " Handle...Complete()");

            this.cameraPosition = cameraPosition;
            this.cameraRotation = cameraRotation;

            if (sim) sim.Resume();

            if (insetCamera)
            {
                Debug.Log(transform.name + " show InsetCamera ? " + showInsetCamera);
                insetCamera.gameObject.SetActive(showInsetCamera);
            }
        }
    }
}
