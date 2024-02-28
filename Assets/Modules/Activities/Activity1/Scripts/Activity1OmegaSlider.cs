// ----------------------------------------------------------------------------------------------------------
// Author: Austin Peel
//
// © All rights reserved. ECOLE POLYTECHNIQUE FEDERALE DE LAUSANNE, Switzerland, Section de Physique, 2024.
// See the LICENSE.md file for more details.
// ----------------------------------------------------------------------------------------------------------
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class Activity1OmegaSlider : CenteredSlider
{
    public float[] allowedValues = new float[] { -1f, 0f, 1f };

    protected override void OnEnable()
    {
        base.OnEnable();
        onValueChanged.AddListener(SnapToValue);
    }

    protected override void OnDisable()
    {
        onValueChanged.RemoveListener(SnapToValue);
        base.OnDisable();
    }

    protected override void Start()
    {
        base.Start();
        Initialize();
    }

    private void Initialize()
    {
        float minValue = Mathf.Infinity;
        float maxValue = -Mathf.Infinity;

        for (int i = 0; i < allowedValues.Length; i++)
        {
            float value = allowedValues[i];
            if (value < minValue) minValue = value;
            if (value > maxValue) maxValue = value;
        }

        this.minValue = minValue;
        this.maxValue = maxValue;

        UpdateFillArea(value);
    }

    public void SnapToValue(float value)
    {
        float closestValue = allowedValues[0];
        float smallestDistance = Mathf.Abs(value - closestValue);
        for (int i = 1; i < allowedValues.Length; i++)
        {
            float distance = Mathf.Abs(value - allowedValues[i]);
            if (distance < smallestDistance)
            {
                closestValue = allowedValues[i];
                smallestDistance = distance;
            }
        }

        // Debug.Log("Snapping to " + closestValue);
        this.value = closestValue;
    }
}
