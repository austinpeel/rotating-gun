using System.Collections;
using UnityEngine;

public class RotatingGunSimulation : Slides.Simulation
{
    public Gun gun;
    public GameObject bulletPrefab;
    [Range(-1, 1)] public float angularFrequency = 0f;
    public float bulletSpeed = 200;
    [Range(0, 2)] public float timeScale = 0.1f;

    public enum ReferenceFrame { Lab, Gun }
    public ReferenceFrame referenceFrame;

    public enum Perspective { Side, TopDown }
    public Perspective perspective;

    private Camera mainCamera;
    private Vector3 cameraSidePosition = new Vector3(0, 3, -10);
    private Vector3 cameraTopDownPosition = new Vector3(0, 10, 0);

    private void Awake()
    {
        mainCamera = Camera.main;

        SynchronizeReferenceFrame();
        SynchronizeTimeScale();
        SetPerspective(perspective);
    }

    private void Update()
    {
        if (!gun) return;

        float angle = 360 * angularFrequency * Time.deltaTime;
        gun.transform.RotateAround(gun.transform.position, Vector3.up, angle);
    }

    public void Fire()
    {
        // Limit the number of bullets at a time
        if (transform.childCount > 10) return;

        if (gun) gun.Fire(bulletPrefab, bulletSpeed);
    }

    public void ChangeReferenceFrame(ReferenceFrame newReferenceFrame)
    {
        if (referenceFrame == newReferenceFrame) return;

        referenceFrame = newReferenceFrame;
        SynchronizeReferenceFrame();
    }

    public void SynchronizeReferenceFrame()
    {
        if (!gun) return;

        if (!mainCamera) mainCamera = Camera.main;

        if (referenceFrame == ReferenceFrame.Lab)
        {
            mainCamera.transform.SetParent(transform.parent);
        }
        else if (referenceFrame == ReferenceFrame.Gun)
        {
            mainCamera.transform.SetParent(gun.transform);

        }
    }

    public void SynchronizeTimeScale()
    {
        Time.timeScale = timeScale;
    }

    public void SetPerspective(Perspective perspective)
    {
        this.perspective = perspective;

        if (!mainCamera) mainCamera = Camera.main;

        mainCamera.transform.position = (perspective == Perspective.Side) ? cameraSidePosition : cameraTopDownPosition;
        mainCamera.transform.LookAt(gun ? gun.transform.position : Vector3.zero);
    }

    public void TogglePerspective(float moveTime)
    {
        StopAllCoroutines();
        Vector3 targetPosition;
        if (perspective == Perspective.Side)
        {
            perspective = Perspective.TopDown;
            targetPosition = cameraTopDownPosition;
        }
        else
        {
            perspective = Perspective.Side;
            targetPosition = cameraSidePosition;
        }
        StartCoroutine(MoveCamera(targetPosition, gun.transform.position, moveTime));
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
    }
}
