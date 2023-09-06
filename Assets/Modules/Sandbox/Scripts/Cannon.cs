using System.Collections.Generic;
using UnityEngine;

public class Cannon : MonoBehaviour
{
    public Transform barrel;
    public GameObject bulletPrefab;
    public LineRenderer bulletPath;

    [Header("Parameters")]
    public bool autoUpdate = true;
    public CannonSettings settings;

    [Header("Sound Effects")]
    public SoundEffect cannonFire;
    private AudioSource audioSource;

    private float angle = 0;
    private List<Bullet> bulletList;
    private Bullet currentBullet;
    [HideInInspector] public Transform bulletContainer;

    public float CurrentAngle => angle;
    public Vector3 BulletScale => currentBullet ? currentBullet.transform.localScale : Vector3.zero;
    public Vector3 BulletPosition => currentBullet ? currentBullet.Position : Vector3.zero;
    public Vector3 BulletVelocity
    {
        get
        {
            Vector3 velocity = Vector3.zero;
            if (currentBullet)
            {
                velocity = currentBullet.Velocity;
                if (settings.inCannonFrame)
                {
                    velocity += Vector3.Cross(settings.Omega, currentBullet.Position);
                }
            }

            return velocity;
        }
    }
    public Vector3 CentrifugalForce
    {
        get
        {
            Vector3 centrifugalForce = Vector3.zero;
            if (currentBullet && settings.inCannonFrame)
            {
                Vector3 omega = settings.Omega;
                Vector3 omegaCrossPosition = Vector3.Cross(omega, BulletPosition);
                // Recall that Unity is left-handed
                centrifugalForce = -Vector3.Cross(omega, omegaCrossPosition);
            }

            return centrifugalForce;
        }
    }
    public Vector3 CoriolisForce
    {
        get
        {
            Vector3 coriolisForce = Vector3.zero;
            if (currentBullet && settings.inCannonFrame)
            {
                coriolisForce = 2 * Vector3.Cross(settings.Omega, BulletVelocity);
            }

            return coriolisForce;
        }
    }

    private void Awake()
    {
        TryGetComponent(out audioSource);
    }

    private void OnEnable()
    {
        Bullet.OnOutOfBounds += HandleBulletOutOfBounds;
    }

    private void OnDisable()
    {
        Bullet.OnOutOfBounds -= HandleBulletOutOfBounds;
    }

    public void TakeAStep(float deltaTime)
    {
        // Compute the increment to the rotation angle
        float deltaAngle = Mathf.Rad2Deg * settings.angularFrequency * deltaTime;

        // Update the running angle
        angle += deltaAngle;
        // Keep the angle within the range [0, 360])
        angle %= 360;
        if (angle < 0) angle += 360;

        // Rotate the cannon about the y-axis
        transform.RotateAround(transform.position, Vector3.down, deltaAngle);
    }

    private void Update()
    {
        if (autoUpdate)
        {
            TakeAStep(Time.deltaTime);
            UpdateBulletPath();
        }
    }

    public void Fire()
    {
        if (!bulletPrefab)
        {
            Debug.LogWarning("No bullet prefab assigned to the cannon");
            return;
        }

        // Clear the previous bullet path
        ResetBulletPath();

        // Create a new list of bullets if necessary
        if (bulletList == null) bulletList = new List<Bullet>();

        // Only fire if there aren't too many bullets already in the scene
        if (bulletList.Count > settings.maxNumBullets) return;

        // Determine where the bullet should spawn
        Vector3 bulletSpawnPosition = Vector3.zero;
        Vector3 e1 = transform.right;  // Unit vector along the gun's local x-axis (barrel)
        if (barrel) bulletSpawnPosition = barrel.position + 0.5f * barrel.localScale.y * e1;

        // Create the new bullet
        Transform parent = bulletContainer ? bulletContainer : transform;
        Bullet bullet = Instantiate(bulletPrefab, parent).GetComponent<Bullet>();
        bullet.name = "Bullet";

        // Set the bullet velocity in the gun's reference frame
        Vector3 bulletVelocity = settings.bulletSpeed * e1;

        // Correct for the lab frame
        bulletVelocity -= Vector3.Cross(settings.Omega, bulletSpawnPosition);

        // Set the bullet's initial conditions
        bullet.Initialize(bulletSpawnPosition, bulletVelocity, settings.maxBulletDistance);

        // Track the current bullet reference and add it to the list
        bulletList.Add(bullet);
        currentBullet = bullet;

        // Sound effect
        if (settings.playSoundEffects) PlayCannonFireAudio();
    }

    public void PlayCannonFireAudio()
    {
        if (audioSource && cannonFire) cannonFire.Play(audioSource);
    }

    public void HandleBulletOutOfBounds(Bullet bullet)
    {
        // Remove this bullet from the list
        if (bulletList != null)
        {
            bulletList.Remove(bullet);

            // When no bullets are left
            if (bulletList.Count == 0)
            {
                currentBullet = null;
                ResetBulletPath();
            }
        }

        // Destroy the game object
        bullet.Destroy();
    }

    public void UpdateBulletPath()
    {
        if (!currentBullet || !bulletPath) return;

        bulletPath.positionCount++;

        Vector3 newPosition = currentBullet.Position;
        if (settings.inCannonFrame)
        {
            // Compute the position relative to the cannon
            bulletPath.useWorldSpace = false;
            newPosition = transform.InverseTransformPoint(newPosition);
        }
        else
        {
            bulletPath.useWorldSpace = true;
        }

        // Add the new point
        bulletPath.SetPosition(bulletPath.positionCount - 1, newPosition);

        // Set the path's visibility
        bulletPath.gameObject.SetActive(settings.showBulletPath);

    }

    public void ResetBulletPath()
    {
        if (bulletPath) bulletPath.positionCount = 0;
    }

    public void SetAngularFrequency(float angularFrequency)
    {
        settings.angularFrequency = angularFrequency;
    }
}

[System.Serializable]
public class CannonSettings
{
    [Range(-5, 5), Tooltip("Rotation about y-axis [rad/s]")] public float angularFrequency = 0;
    [Range(0, 30), Tooltip("Bullet fire speed [units/s]")] public float bulletSpeed = 1;
    [Min(0), Tooltip("Maximum number of bullets allowed at one time")] public int maxNumBullets = 1;
    [Min(0), Tooltip("Maximum distance a bullet can travel [units]")] public float maxBulletDistance = 10;
    public bool playSoundEffects;
    public bool showBulletPath;
    public bool inCannonFrame;

    // Angular velocity
    public Vector3 Omega => angularFrequency * Vector3.up;
}