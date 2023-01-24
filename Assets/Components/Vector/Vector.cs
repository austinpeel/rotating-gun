using UnityEngine;

public class Vector : MonoBehaviour
{
    public LineRenderer body;
    public ConeMesh head;

    public Vector3 components;
    public Color color = Color.black;
    [Min(0)] public float lineWidth = 0.2f;

    private const float headAngle = 30;

    private void OnEnable()
    {
        Redraw();
    }

    public void Redraw()
    {
        Vector3 direction = components.normalized;
        float magnitude = components.magnitude;

        // Prevent line width from exceeding 1/3 of the magnitude
        float trueWidth = Mathf.Min(magnitude / 3, lineWidth);
        float headRadius = trueWidth;
        float headHeight = headRadius / Mathf.Tan(Mathf.Deg2Rad * headAngle);

        if (head)
        {
            head.color = color;
            head.radius = 1.2f * headRadius;
            head.height = headHeight;
            head.transform.localPosition = components - headHeight * direction;
            if (direction != Vector3.zero)
            {
                head.transform.localRotation = Quaternion.LookRotation(direction);
            }
            head.Redraw();
        }

        if (body)
        {
            body.sharedMaterial = head.GetMaterial();
            body.startWidth = trueWidth;
            body.endWidth = trueWidth;
            body.SetPositions(new Vector3[] { Vector3.zero, components - headHeight * direction });
        }
    }
}
