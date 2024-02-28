// ----------------------------------------------------------------------------------------------------------
// Author: Austin Peel
//
// © All rights reserved. ECOLE POLYTECHNIQUE FEDERALE DE LAUSANNE, Switzerland, Section de Physique, 2024.
// See the LICENSE.md file for more details.
// ----------------------------------------------------------------------------------------------------------
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public Transform barrel;
    public LineRenderer bulletPath;
    public SimulationState simState;

    [Header("Vectors")]
    public GameObject positionVectorLabPrefab;
    public GameObject positionVectorGunPrefab;
    public GameObject velocityVectorPrefab;
    public GameObject centrifugalForceVectorPrefab;
    public GameObject coriolisForceVectorPrefab;
    public float vectorScaleFactor = 1;

    [Header("Sound Effects")]
    public SoundEffect cannonFire;
    private AudioSource audioSource;

    private List<Bullet> bullets;

    [HideInInspector] public bool inGunFrame;
    private Bullet currentBullet;
    private bool tracingBulletPath;

    private Vector positionVector;
    private Vector velocityVector;
    private Vector centrifugalForceVector;
    private Vector coriolisForceVector;

    // Use when no SimulationState is assigned
    [HideInInspector] public Vector3 omega;

    private void Awake()
    {
        TryGetComponent(out audioSource);
    }

    private void OnEnable()
    {
        Bullet.OnOutOfBounds += HandleBulletOutOfBounds;
        Bullet.OnHitTarget += HandleBulletHitTarget;
    }

    private void OnDisable()
    {
        Bullet.OnOutOfBounds -= HandleBulletOutOfBounds;
        Bullet.OnHitTarget -= HandleBulletHitTarget;
    }

    private void LateUpdate()
    {
        if (!currentBullet) return;

        // Compute kinematical and dynamical quantities
        Vector3 position = currentBullet.Position;
        Vector3 velocity = currentBullet.Velocity;
        Vector3 centrifugalForce = Vector3.zero;
        Vector3 coriolisForce = Vector3.zero;
        if (inGunFrame)
        {
            Vector3 currentOmega = simState ? simState.omega : omega;

            // Unity is left-handed, so add instead of subtract
            velocity += Vector3.Cross(currentOmega, position);

            // Assume bullet mass = 1 kg
            centrifugalForce = -Vector3.Cross(currentOmega, Vector3.Cross(currentOmega, position));
            coriolisForce = 2 * Vector3.Cross(currentOmega, velocity);
        }

        velocity *= vectorScaleFactor;
        centrifugalForce *= 2 * vectorScaleFactor;
        coriolisForce *= 0.5f * vectorScaleFactor;

        // Update displayed vectors
        if (positionVector != null)
        {
            Vector3 offset = 0.5f * currentBullet.transform.localScale.x * position.normalized;
            positionVector.components = position - offset;
            positionVector.Redraw();
        }

        if (velocityVector != null)
        {
            velocityVector.transform.position = currentBullet.Position;
            velocityVector.components = velocity;
            velocityVector.Redraw();
        }

        if (centrifugalForceVector != null && inGunFrame)
        {
            centrifugalForceVector.transform.position = currentBullet.Position;
            centrifugalForceVector.components = centrifugalForce;
            centrifugalForceVector.Redraw();
        }

        if (coriolisForceVector != null && inGunFrame)
        {
            coriolisForceVector.transform.position = currentBullet.Position;
            coriolisForceVector.components = coriolisForce;
            coriolisForceVector.Redraw();
        }

        if (!tracingBulletPath || !bulletPath) return;

        bulletPath.positionCount++;
        Vector3 newPosition = currentBullet.transform.position;
        if (inGunFrame)
        {
            // Compute position relative to the gun
            bulletPath.useWorldSpace = false;
            newPosition = transform.InverseTransformPoint(newPosition);
        }
        else
        {
            bulletPath.useWorldSpace = true;
        }

        bulletPath.SetPosition(bulletPath.positionCount - 1, newPosition);
    }

    public void Fire(GameObject bulletPrefab,
                     float speed,
                     Transform parent,
                     float maxDistance,
                     bool tracePath = false,
                     bool showPosition = false,
                     bool showVelocity = false,
                     bool showCentrifugalForce = false,
                     bool showCoriolisForce = false,
                     bool playSoundEffect = false)
    {
        if (bullets == null) bullets = new List<Bullet>();

        ResetBulletPath();
        tracingBulletPath = tracePath;

        // Determine where the bullet should spawn
        Vector3 bulletSpawnPosition = Vector3.zero;
        Vector3 e1 = transform.right;  // Unit vector along the gun's local x-axis (barrel)
        if (barrel) bulletSpawnPosition = barrel.position + 0.5f * barrel.localScale.y * e1;

        // Create the new bullet
        Bullet bullet = Instantiate(bulletPrefab, parent).GetComponent<Bullet>();
        bullet.name = "Bullet";
        // Set the bullet velocity in the gun's reference frame
        Vector3 bulletVelocity = speed * e1;
        // Correct for the lab frame
        bulletVelocity -= Vector3.Cross(simState ? simState.omega : omega, bulletSpawnPosition);
        bullet.Initialize(bulletSpawnPosition, bulletVelocity, maxDistance);

        bullets.Add(bullet);
        currentBullet = bullet;

        // TODO clean up : what if prefabs are null ??
        if (showPosition && positionVector == null)
        {
            if (simState.referenceFrame == SimulationState.ReferenceFrame.Lab)
            {
                positionVector = CreateVector(positionVectorLabPrefab, transform.parent);
            }
            else if (simState.referenceFrame == SimulationState.ReferenceFrame.Gun)
            {
                positionVector = CreateVector(positionVectorGunPrefab, transform.parent);
            }
            positionVector.components = transform.position;
            positionVector.Redraw();
        }

        if (showVelocity && velocityVector == null)
        {
            velocityVector = CreateVector(velocityVectorPrefab, transform.parent);
            velocityVector.components = speed * e1;
            velocityVector.Redraw();
        }

        if (showCentrifugalForce && centrifugalForceVector == null)
        {
            centrifugalForceVector = CreateVector(centrifugalForceVectorPrefab, transform.parent);
            centrifugalForceVector.components = Vector3.zero;
            centrifugalForceVector.Redraw();
        }

        if (showCoriolisForce && coriolisForceVector == null)
        {
            coriolisForceVector = CreateVector(coriolisForceVectorPrefab, transform.parent);
            coriolisForceVector.components = Vector3.zero;
            coriolisForceVector.Redraw();
        }

        if (playSoundEffect && audioSource && cannonFire) cannonFire.Play(audioSource);
    }

    private Vector CreateVector(GameObject vectorPrefab, Transform parent)
    {
        Vector3 position = Vector3.zero;
        Quaternion rotation = Quaternion.identity;
        GameObject vector = Instantiate(vectorPrefab, position, rotation, parent);
        return vector.GetComponent<Vector>();
    }

    public void HandleBulletOutOfBounds(Bullet bullet)
    {
        if (bullets == null) return;

        bullets.Remove(bullet);
        bullet.Destroy();

        if (bullets.Count == 0)
        {
            currentBullet = null;
            ResetBulletPath();

            if (positionVector != null) Destroy(positionVector.gameObject);
            if (velocityVector != null) Destroy(velocityVector.gameObject);
            if (centrifugalForceVector != null) Destroy(centrifugalForceVector.gameObject);
            if (coriolisForceVector != null) Destroy(coriolisForceVector.gameObject);
        }
    }

    public void HandleBulletHitTarget(Bullet bullet)
    {
        // Destroy the bullet
        HandleBulletOutOfBounds(bullet);
    }

    public void ResetBulletPath()
    {
        if (bulletPath) bulletPath.positionCount = 0;
    }

    public void ResetFictitiousForces()
    {
        if (centrifugalForceVector != null) Destroy(centrifugalForceVector.gameObject);
        if (coriolisForceVector != null) Destroy(coriolisForceVector.gameObject);
    }
}
