using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GunFrameAnimation : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform gunBarrel;
    public LineRenderer bulletPath;

    [Header("Lab Frame Objects")]
    public Transform directionalLight;
    public Transform ground;

    [Header("Camera")]
    public Transform mainCamera;
    public Vector3 cameraPosition = new Vector3(0, 5, -20);
    public Vector3 cameraRotation = new Vector3(20, 0, 0);

    [Header("Animated Objects")]
    public Vector velocity;
    public Vector centrifugalForce;
    public Vector coriolisForce;
    public Slider omegaSlider;
    public Button fireButton;
    public Button checkButton;

    [Header("Parameters")]
    public float omegaMax = 0.6f;
    public float bulletSpeed = 20;
    private float angularFrequency;

    [Header("Play / Pause")]
    public bool isPaused;
    public float bulletPauseDistance = 4;
    private bool hasPausedOnCurrentBullet;

    public enum InstructionsAnimation { Animation1, Animation2, Animation3, None }
    [Header("Animations")]
    public InstructionsAnimation anim;
    private Coroutine currentAnimation;

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

        // Initialize camera
        if (!mainCamera) return;
        mainCamera.position = cameraPosition;
        mainCamera.rotation = Quaternion.Euler(cameraRotation);
    }

    private void OnDisable()
    {
        Reset();
        StartAnimation(InstructionsAnimation.None);
    }

    private void Start()
    {
        StartAnimation(anim);
    }

    private void Reset()
    {
        if (directionalLight) directionalLight.rotation = Quaternion.Euler(45, 0, 0);
        if (ground) ground.rotation = Quaternion.Euler(90, 0, 0);

        if (bullets != null && currentBullet != null)
        {
            bullets.Remove(currentBullet);
            currentBullet.Destroy();

            if (bullets.Count == 0)
            {
                currentBullet = null;
                ResetBulletPath();
            }
        }

        if (omegaSlider) omegaSlider.value = 0;
        if (fireButton) fireButton.interactable = true;
        if (checkButton) checkButton.interactable = false;

        Resume();
    }

    private void StartAnimation(InstructionsAnimation animation)
    {
        if (currentAnimation != null)
        {
            StopCoroutine(currentAnimation);
            currentAnimation = null;
        }

        switch (animation)
        {
            case InstructionsAnimation.Animation1:
                currentAnimation = StartCoroutine("Animation1");
                break;
            case InstructionsAnimation.Animation2:
                currentAnimation = StartCoroutine("Animation2");
                break;
            case InstructionsAnimation.Animation3:
                currentAnimation = StartCoroutine("Animation3");
                break;
            default:
                break;
        }
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

    private void Update()
    {
        if (isPaused) return;

        if (currentBullet && !hasPausedOnCurrentBullet)
        {
            if (currentBullet.Position.magnitude > bulletPauseDistance)
            {
                Pause();
                hasPausedOnCurrentBullet = true;
            }
        }
    }

    private void LateUpdate()
    {
        if (!currentBullet) return;

        if (!bulletPath) return;

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

        if (bullets == null) bullets = new List<Bullet>();

        ResetBulletPath();

        // Determine where the bullet should spawn
        Vector3 bulletSpawnPosition = Vector3.zero;
        Vector3 e1 = transform.right;  // Unit vector along the gun's local x-axis (barrel)
        if (gunBarrel) bulletSpawnPosition = gunBarrel.position + 0.5f * gunBarrel.localScale.y * e1;

        // Create the new bullet
        Bullet bullet = Instantiate(bulletPrefab, bulletContainer).GetComponent<Bullet>();
        bullet.name = "Bullet";
        bullet.Initialize(bulletSpawnPosition, bulletSpeed * e1, 100, Omega);

        bullets.Add(bullet);
        currentBullet = bullet;

        hasPausedOnCurrentBullet = false;
    }

    public void SetOmega(float value)
    {
        // Round to nearest tenth
        value = Mathf.Round(10 * value) / 10f;

        angularFrequency = value / (2 * Mathf.PI);

        if (currentBullet) currentBullet.SetOmega(Omega);
    }

    public void ResetBulletPath()
    {
        if (bulletPath) bulletPath.positionCount = 0;
    }

    private void RotateLight(float deltaTheta)
    {
        if (directionalLight)
        {
            directionalLight.RotateAround(directionalLight.position, Vector3.up, deltaTheta);
        }
    }

    private void RotateGround(float deltaTheta)
    {
        if (ground)
        {
            ground.RotateAround(ground.position, Vector3.up, deltaTheta);
        }
    }

    private IEnumerator Animation1()
    {
        Reset();

        if (omegaSlider) omegaSlider.value = 0;

        float time = 0;
        float omega = 0;
        float sliderTime = 1;

        while (time < sliderTime)
        {
            time += Time.deltaTime;
            omega = Mathf.Lerp(0, omegaMax, time / sliderTime);
            angularFrequency = omega / (2 * Mathf.PI);
            float deltaTheta = omega * Mathf.Rad2Deg * Time.deltaTime;

            if (omegaSlider) omegaSlider.value = omega;
            RotateLight(deltaTheta);
            RotateGround(deltaTheta);
            yield return null;
        }

        time = 0;
        float holdTime = 1;
        while (time < holdTime)
        {
            time += Time.deltaTime;
            float deltaTheta = omega * Mathf.Rad2Deg * Time.deltaTime;
            RotateLight(deltaTheta);
            RotateGround(deltaTheta);
            yield return null;
        }

        Fire();
        if (fireButton) fireButton.interactable = false;
        if (checkButton) checkButton.interactable = true;

        time = 0;
        holdTime = 3;
        while (time < holdTime)
        {
            time += Time.deltaTime;
            float deltaTheta = omega * Mathf.Rad2Deg * Time.deltaTime;
            RotateLight(deltaTheta);
            RotateGround(deltaTheta);

            // Break out once paused
            if (isPaused) time = holdTime;

            yield return null;
        }

        yield return new WaitForSeconds(3);

        yield return Animation1();
    }

    private IEnumerator Animation2()
    {
        yield return null;
    }

    private IEnumerator Animation3()
    {
        yield return null;
    }
}
