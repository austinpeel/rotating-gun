using System;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public GameObject positionVectorPrefab;
    // public GameObject velocityVectorPrefab;
    public float maxDistance = 10;

    public static event Action<Bullet> OnOutOfBounds;

    // Kinematics
    public Vector3 Position => transform.position;

    private Vector3 velocity;
    public Vector3 Velocity => velocity;

    // private Vector positionVector;

    public void Initialize(Vector3 position, Vector3 velocity, float maxDistance)
    {
        transform.position = position;
        this.velocity = velocity;
        this.maxDistance = maxDistance;

        // if (showPositionVector)
        // {
        //     positionVector = Instantiate(positionVectorPrefab, Vector3.zero, Quaternion.identity, transform.parent).GetComponent<Vector>();
        //     positionVector.components = transform.position;
        //     positionVector.Redraw();
        // }
    }

    private void Update()
    {
        transform.Translate(velocity * Time.deltaTime, Space.World);

        // if (positionVector)
        // {
        //     positionVector.components = transform.position - 0.5f * transform.localScale.x * transform.position.normalized;
        //     positionVector.Redraw();
        // }

        if (transform.position.magnitude >= maxDistance)
        {
            OnOutOfBounds?.Invoke(this);
        }
    }

    public void Destroy()
    {
        Destroy(gameObject);
        // DestroyPositionVector();
    }

    // public void DestroyPositionVector()
    // {
    //     if (positionVector) Destroy(positionVector.gameObject);
    // }
}
