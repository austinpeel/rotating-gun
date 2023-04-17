using UnityEngine;
using UnityEngine.UI;

public class RotatingGunSlideController : Slides.SimulationSlideController
{
    public float angularFrequency;
    public SimulationState.ReferenceFrame referenceFrame;
    public bool traceBulletPath;
    public bool showPosition;

    [Header("Buttons")]
    public Button fireButton;

    [Header("Lights")]
    public GameObject labFrameLight;
    public bool activateLabFrameLight;
    public GameObject gunFrameLight;
    public bool activateGunFrameLight;

    [Header("Ground")]
    public Transform ground;
    public bool showGround;

    private RotatingGunSimulation sim;
    private bool slideHasInitialized;

    private Vector3 cameraPosition;
    private Quaternion cameraRotation;

    private void Awake()
    {
        sim = simulation as RotatingGunSimulation;
    }

    public override void InitializeSlide()
    {
        // Debug.Log("Initializing " + transform.name);

        sim.angularFrequency = angularFrequency;
        sim.SetGunOmegaFromAngularFrequency(angularFrequency);
        sim.referenceFrame = referenceFrame;
        sim.traceBulletPath = traceBulletPath;
        sim.showPosition = showPosition;

        // Reset the positions and orientations of the camera and simulation
        sim.Reset(cameraPosition, cameraRotation);

        sim.Pause();

        if (labFrameLight) labFrameLight.SetActive(activateLabFrameLight);
        if (gunFrameLight) gunFrameLight.SetActive(activateGunFrameLight);
        if (ground) ground.gameObject.SetActive(showGround);
        if (fireButton) fireButton.interactable = false;

        slideHasInitialized = true;
    }

    private void OnEnable()
    {
        CameraController.OnCameraMovementComplete += HandleCameraMovementComplete;
    }

    private void OnDisable()
    {
        CameraController.OnCameraMovementComplete -= HandleCameraMovementComplete;
    }

    public void HandleCameraMovementComplete(Vector3 cameraPosition, Quaternion cameraRotation)
    {
        if (slideHasInitialized)
        {
            // Debug.Log(transform.name + " Handle...Complete()");

            if (sim)
            {
                sim.Reset(cameraPosition, cameraRotation);
                sim.Resume();
            }

            if (fireButton) fireButton.interactable = true;
        }

        this.cameraPosition = cameraPosition;
        this.cameraRotation = cameraRotation;
    }
}
