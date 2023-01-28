using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public Transform barrel;
    public LineRenderer bulletPath;

    private List<Bullet> bullets;

    [HideInInspector] public bool inGunFrame;
    private Bullet currentBullet;
    private bool tracingBulletPath;

    private void OnEnable()
    {
        Bullet.OnOutOfBounds += HandleBulletOutOfBounds;
    }

    private void OnDisable()
    {
        Bullet.OnOutOfBounds -= HandleBulletOutOfBounds;
    }

    private void Update()
    {
        if (!tracingBulletPath || !bulletPath || !currentBullet) return;

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
            return;
            // bulletPath.useWorldSpace = true;
        }
        bulletPath.SetPosition(bulletPath.positionCount - 1, newPosition);
    }

    public void Fire(GameObject bulletPrefab, float speed, Transform parent, bool tracePath = false,
                     bool showPositionVector = false)
    {
        if (bullets == null) bullets = new List<Bullet>();

        ResetBulletPath();
        tracingBulletPath = tracePath;

        // Determine where the bullet should spawn
        Vector3 bulletSpawnPosition = Vector3.zero;
        Vector3 e1 = transform.right;  // Unit vector along the gun's local x-axis (barrel)
        if (barrel) bulletSpawnPosition = barrel.position + 0.5f * barrel.localScale.y * e1;

        Bullet bullet = Instantiate(bulletPrefab, parent).GetComponent<Bullet>();
        bullet.name = "Bullet";
        bullet.Initialize(bulletSpawnPosition, speed * e1, showPositionVector);

        // Only show the latest bullet's position vector
        if (showPositionVector)
        {
            foreach (var b in bullets)
            {
                b.DestroyPositionVector();
            }
        }
        bullets.Add(bullet);
        currentBullet = bullet;
    }

    public void HandleBulletOutOfBounds(Bullet bullet)
    {
        bullets.Remove(bullet);
        bullet.Destroy();

        if (bullets.Count == 0)
        {
            currentBullet = null;
            ResetBulletPath();
        }
    }

    public void ResetBulletPath()
    {
        if (bulletPath) bulletPath.positionCount = 0;
    }
}
