using System;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class VectorClickZone : MonoBehaviour
{
    [SerializeField] private Material defaultMaterial;

    [Header("Cursor")]
    [SerializeField] private CustomCursor customCursor;

    private MeshRenderer mesh;

    public static event Action<VectorClickZone> OnZoneMouseDown;
    public static event Action<VectorClickZone> OnZoneMouseUp;

    private void Awake()
    {
        if (TryGetComponent(out mesh))
        {
            mesh.enabled = false;
        }
    }

    private void OnDisable()
    {
        RestoreDefaultCursor();
    }

    private void OnMouseEnter()
    {
        // Display the cursor while hovering
        if (customCursor) Cursor.SetCursor(customCursor.texture, customCursor.hotspot, CursorMode.Auto);

        if (mesh) mesh.enabled = true;
    }

    private void OnMouseExit()
    {
        RestoreDefaultCursor();
    }

    private void RestoreDefaultCursor()
    {
        // Restore the default cursor
        if (customCursor != null)
        {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }

        if (mesh) mesh.enabled = false;
    }

    private void OnMouseDown()
    {
        OnZoneMouseDown?.Invoke(this);
    }

    private void OnMouseUp()
    {
        OnZoneMouseUp?.Invoke(this);
    }

    public void SetColor(Color color)
    {
        if (defaultMaterial)
        {
            Material material = new Material(defaultMaterial);
            material.color = color;
            material.name = "Copy of " + defaultMaterial;
            GetComponent<MeshRenderer>().sharedMaterial = material;
        }
    }
}
