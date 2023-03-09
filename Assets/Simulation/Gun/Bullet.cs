using System;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    // public GameObject positionVectorPrefab;
    public float maxDistance = 10;

    public static event Action<Bullet> OnOutOfBounds;

    public Vector3 Position => transform.position;

    private Vector3 velocity;
    public Vector3 Velocity => velocity;

    public void Initialize(Vector3 position, Vector3 velocity, float maxDistance)
    {
        transform.position = position;
        this.velocity = velocity;
        this.maxDistance = maxDistance;
    }

    private void Update()
    {
        transform.Translate(velocity * Time.deltaTime, Space.World);

        CheckForOutOfBounds();
    }

    public void CheckForOutOfBounds()
    {
        if (transform.position.magnitude >= maxDistance)
        {
            OnOutOfBounds?.Invoke(this);
        }
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }
}
