using UnityEngine;

public class Gun : MonoBehaviour
{
    public Transform barrel;

    public void Fire(GameObject bulletPrefab, float speed)
    {
        Vector3 bulletSpawnPosition = Vector3.zero;

        // Unit vector along the gun's x-axis (barrel)
        Vector3 e1 = transform.right;

        if (barrel)
        {
            bulletSpawnPosition = barrel.position + 0.5f * barrel.localScale.y * e1;
        }

        Bullet bullet = Instantiate(bulletPrefab, transform.parent).GetComponent<Bullet>();
        bullet.transform.position = bulletSpawnPosition;
        bullet.velocity = speed * e1;
    }
}
