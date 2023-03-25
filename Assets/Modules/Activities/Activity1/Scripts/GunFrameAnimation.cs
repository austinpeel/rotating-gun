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
    public DraggableVector velocity;
    public DraggableVector centrifugalForce;
    public DraggableVector coriolisForce;
    public Slider omegaSlider;
    public Button fireButton;
    public Button checkButton;

    [Header("Parameters")]
    public float omegaMax = 0.6f;
    public float bulletSpeed = 20;
    private float angularFrequency;
    public float bulletPauseDistance = 4;
    private bool isPaused;

    private enum InstructionsAnimation { Animation1, Animation2, Animation3, None }
    private InstructionsAnimation anim = InstructionsAnimation.Animation1;
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
        if (currentAnimation != null)
        {
            StopCoroutine(currentAnimation);
            currentAnimation = null;
        }

        Reset();

        // Make sure that the vectors (and click zones) are interactable
        if (velocity) velocity.MakeInteractable();
        if (centrifugalForce) centrifugalForce.MakeInteractable();
        if (coriolisForce) coriolisForce.MakeInteractable();
    }

    // private void Start()
    // {
    //     StartAnimation(anim);
    // }

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

        if (velocity) velocity.Reset();
        if (centrifugalForce) centrifugalForce.Reset();
        if (coriolisForce) coriolisForce.Reset();

        Resume();
    }

    public void StartAnimation(int animationID)
    {
        StartAnimation((InstructionsAnimation)(animationID - 1));
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
        if (currentBullet) currentBullet.Resume();
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
        // By passing omega to the bullet, it knows that this is a gun frame simulation
        // and that it should solve the equations of motion numerically
        bullet.Initialize(bulletSpawnPosition,
                          bulletSpeed * e1, // + Vector3.Cross(Omega, bulletSpawnPosition - transform.position),
                          100,
                          Omega,
                          true);

        bullets.Add(bullet);
        currentBullet = bullet;
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

        // First increase omega from 0 to its max value
        float time = 0;
        float omega = 0;
        float sliderMoveTime = 1;

        while (time < sliderMoveTime)
        {
            time += Time.fixedDeltaTime;

            // Increment omega
            omega = Mathf.Lerp(0, omegaMax, time / sliderMoveTime);
            angularFrequency = omega / (2 * Mathf.PI);
            float deltaTheta = omega * Mathf.Rad2Deg * Time.fixedDeltaTime;
            RotateLight(deltaTheta);
            RotateGround(deltaTheta);

            // Update the actual slider
            if (omegaSlider) omegaSlider.value = omega;

            yield return null;
        }

        // Hold on the rotating animation for a short time
        time = 0;
        float holdTime = 1;
        while (time < holdTime)
        {
            time += Time.fixedDeltaTime;
            float deltaTheta = omega * Mathf.Rad2Deg * Time.fixedDeltaTime;
            RotateLight(deltaTheta);
            RotateGround(deltaTheta);
            yield return null;
        }

        // Fire the gun
        Fire();
        if (fireButton) fireButton.interactable = false;
        // if (checkButton) checkButton.interactable = true;

        // Integrate the equation of motion to a given distance
        time = 0;
        holdTime = 10;
        int numSubsteps = 10;
        while (time < holdTime)
        {
            time += Time.fixedDeltaTime;
            float deltaTheta = omega * Mathf.Rad2Deg * Time.fixedDeltaTime;
            RotateLight(deltaTheta);
            RotateGround(deltaTheta);

            // Move the bullet to its next position
            float deltaTime = Time.fixedDeltaTime / numSubsteps;
            for (int i = 0; i < numSubsteps; i++)
            {
                currentBullet.TakeLeapfrogStep(deltaTime);
            }
            currentBullet.transform.position = currentBullet.GetX();

            // Trace the bullet
            if (bulletPath)
            {
                bulletPath.positionCount++;
                Vector3 newPosition = currentBullet.transform.position;
                // Compute position relative to the gun
                // bulletPath.useWorldSpace = false;
                newPosition = transform.InverseTransformPoint(newPosition);
                bulletPath.SetPosition(bulletPath.positionCount - 1, newPosition);
            }

            // Break out once the bullet has reached its max allowed distance
            if (currentBullet.Position.magnitude > bulletPauseDistance) break;

            yield return null;
        }

        // Pause for a few seconds
        yield return new WaitForSeconds(2);

        // Restart the animation
        yield return Animation1();
    }

    private IEnumerator Animation2()
    {
        Reset();

        // Set up simulation to match the end of animation 1
        if (omegaSlider) omegaSlider.value = omegaMax;
        if (fireButton) fireButton.interactable = false;
        if (checkButton) checkButton.interactable = true;

        Fire();

        int numSubsteps = 10;
        float deltaTime = Time.fixedDeltaTime / numSubsteps;
        float deltaTheta = omegaMax * Mathf.Rad2Deg * Time.fixedDeltaTime;
        while (currentBullet.GetX().magnitude < bulletPauseDistance)
        {
            // Update the bullet position (its x variable) by a time step
            for (int i = 0; i < numSubsteps; i++)
            {
                currentBullet.TakeLeapfrogStep(deltaTime);
            }

            RotateLight(deltaTheta);
            RotateGround(deltaTheta);

            // Trace the bullet
            if (bulletPath)
            {
                bulletPath.positionCount++;
                Vector3 newPosition = currentBullet.GetX();
                // Compute position relative to the gun
                // bulletPath.useWorldSpace = false;
                newPosition = transform.InverseTransformPoint(newPosition);
                bulletPath.SetPosition(bulletPath.positionCount - 1, newPosition);
            }
        }

        // Move the bullet to its final position
        currentBullet.transform.position = currentBullet.GetX();

        yield return new WaitForSeconds(1);

        // Animate translating the velocity vector to the bullet
        if (velocity)
        {
            velocity.ShowTailClickZone(false);

            float time = 0;
            float vectorMoveTime = 1;
            Vector3 startPosition = velocity.transform.position;
            Vector3 endPosition = currentBullet.transform.position;
            while (time < vectorMoveTime)
            {
                time += Time.fixedDeltaTime;
                velocity.transform.position = Vector3.Lerp(startPosition, endPosition, time / vectorMoveTime);
                yield return null;
            }

            yield return new WaitForSeconds(0.8f);

            velocity.HideTailClickZone();
            velocity.ShowHeadClickZone(false);

            time = 0;
            float vectorRotateTime = 2;
            Vector3 startComponents = velocity.components;
            Vector3 endComponents = startComponents.magnitude * currentBullet.GetV().normalized;
            endComponents = Quaternion.Euler(0, -25, 0) * endComponents;
            while (time < vectorRotateTime)
            {
                time += Time.fixedDeltaTime;
                velocity.components = Vector3.Slerp(startComponents, endComponents, time / vectorRotateTime);
                velocity.Redraw();
                yield return null;
            }
        }

        // Pause for a few seconds
        yield return new WaitForSeconds(1.2f);

        yield return Animation2();
    }

    private IEnumerator Animation3()
    {
        // Set up simulation to match the end of animation 2
        if (fireButton) fireButton.interactable = false;
        if (checkButton) checkButton.interactable = true;

        if (velocity)
        {
            velocity.Reset();
            velocity.transform.position = currentBullet.transform.position;
            Vector3 components = velocity.components.magnitude * currentBullet.GetV().normalized;
            velocity.components = Quaternion.Euler(0, -25, 0) * components;
            velocity.Redraw();
        }

        if (centrifugalForce)
        {
            centrifugalForce.Reset();
            centrifugalForce.transform.position = currentBullet.transform.position;
            centrifugalForce.components = Vector3.forward;
            centrifugalForce.Redraw();
        }

        if (coriolisForce)
        {
            coriolisForce.Reset();
            coriolisForce.transform.position = currentBullet.transform.position;
            coriolisForce.components = Vector3.left;
            coriolisForce.Redraw();
        }

        // Pause for a few seconds
        yield return new WaitForSeconds(0.8f);

        if (checkButton) checkButton.interactable = false;

        // Pause for a few seconds
        yield return new WaitForSeconds(1.2f);

        yield return Animation3();
    }
}
