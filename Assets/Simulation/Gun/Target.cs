using UnityEngine;

public class Target : MonoBehaviour
{
    public Transform bulletHole;

    public void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.collider.name);

        if (!collision.collider.CompareTag("Bullet") || !bulletHole) return;

        Vector3 point = collision.GetContact(0).point;
        bulletHole.position = point;
    }
}
