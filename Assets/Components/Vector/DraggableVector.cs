using UnityEngine;

public class DraggableVector : Vector
{
    [SerializeField] private float dragPlaneDistance = 10f;

    [Header("Interaction Zones")]
    [SerializeField] private VectorClickZone tailClickZone;
    [SerializeField] private VectorClickZone headClickZone;

    private Vector3 initialPosition;
    private Vector3 dragStartPosition;
    private Camera mainCamera;
    private Plane dragPlane;

    private bool draggingTail;
    private bool draggingHead;

    public override void OnEnable()
    {
        base.OnEnable();

        Redraw();

        VectorClickZone.OnZoneMouseDown += HandleZoneMouseDown;
        VectorClickZone.OnZoneMouseUp += HandleZoneMouseUp;
    }

    private void OnDisable()
    {
        VectorClickZone.OnZoneMouseDown -= HandleZoneMouseDown;
        VectorClickZone.OnZoneMouseUp -= HandleZoneMouseUp;
    }

    public void HandleZoneMouseDown(string zoneName)
    {
        if (zoneName == "tail")
        {
            draggingTail = true;
        }
        else if (zoneName == "head")
        {
            draggingHead = true;
        }
    }

    public void HandleZoneMouseUp(string zoneName)
    {
        if (zoneName == "tail")
        {
            draggingTail = false;
        }
        else if (zoneName == "head")
        {
            draggingHead = false;
        }
    }

    private void Start()
    {
        mainCamera = Camera.main;
        // Create a plane at a fixed distance from the camera, perpendicular to the camera's forward direction
        Vector3 planeNormal = -mainCamera.transform.forward;
        Vector3 planePosition = mainCamera.transform.position + dragPlaneDistance * mainCamera.transform.forward;
        dragPlane = new Plane(planeNormal, planePosition);
    }

    public override void Redraw()
    {
        base.Redraw();

        if (headClickZone) headClickZone.transform.position = transform.position + components;
    }

    private void Update()
    {
        if (draggingTail || draggingHead)
        {
            // Create a ray from the mouse click position
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            // Initialise the enter variable
            float enter = 0.0f;

            if (dragPlane.Raycast(ray, out enter))
            {
                // Get the point that is clicked
                Vector3 hitPoint = ray.GetPoint(enter);

                if (draggingTail)
                {
                    // Move the vector to the clicked point
                    transform.position = hitPoint;
                }
                else
                {
                    // Update components
                    components = hitPoint - transform.position;
                    Redraw();
                }
            }
        }
    }
}
