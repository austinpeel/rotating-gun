using UnityEngine;

[RequireComponent(typeof(Vector))]
public class AngularVelocity : MonoBehaviour
{
    public Material plinthMaterial;
    public float defaultAlpha = 1;
    public float reducedAlpha = 0.2f;

    private Vector vector;

    private void OnEnable()
    {
        TryGetComponent(out vector);
        SetMaterialAlpha(defaultAlpha);
    }

    private void OnDisable()
    {
        vector = null;
    }

    public void FlipAngularVelocity()
    {
        if (vector)
        {
            vector.components = -vector.components;
            vector.Redraw();
        }

        if (GetMaterialAlpha() == defaultAlpha)
        {
            SetMaterialAlpha(reducedAlpha);
        }
        else
        {
            SetMaterialAlpha(defaultAlpha);
        }
    }

    public void SetMaterialAlpha(float alpha)
    {
        if (!plinthMaterial) return;

        Color color = plinthMaterial.color;
        color.a = alpha;
        plinthMaterial.color = color;
    }

    public float GetMaterialAlpha()
    {
        return plinthMaterial ? plinthMaterial.color.a : 1;
    }
}
