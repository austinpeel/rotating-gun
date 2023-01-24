using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Vector3 velocity;
    public float maxDistance = 10;

    private void Update()
    {
        transform.Translate(velocity * Time.deltaTime, Space.World);

        if (transform.position.magnitude >= maxDistance)
        {
            Destroy(this.gameObject);
        }
    }
}
