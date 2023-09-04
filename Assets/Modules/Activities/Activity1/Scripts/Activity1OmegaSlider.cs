using TMPro;
using UnityEngine;

[RequireComponent(typeof(CenteredSlider))]
public class Activity1OmegaSlider : MonoBehaviour
{
    public TextMeshProUGUI valueTMP;
    public float[] allowedValues = new float[] { -1f, 0f, 1f };

    private CenteredSlider slider;
    private bool hasInitialized;

    private void OnEnable()
    {
        Initialize();
    }

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        if (hasInitialized) return;

        Debug.Log("Activity1OmegaSlider > Initialize");
        slider = GetComponent<CenteredSlider>();

        float minValue = Mathf.Infinity;
        float maxValue = -Mathf.Infinity;

        for (int i = 0; i < allowedValues.Length; i++)
        {
            float value = allowedValues[i];
            if (value < minValue) minValue = value;
            if (value > maxValue) maxValue = value;
        }

        slider.minValue = minValue;
        slider.maxValue = maxValue;
        // slider.onValueChanged.AddListener(SnapToValue);
        hasInitialized = true;
    }

    public void SnapToValue(float value)
    {
        if (!slider) return;

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
        slider.value = closestValue;

        // Override the value TMP
        if (valueTMP)
        {
            string format = "0.0";
            float threshold = -0.5f * Mathf.Pow(10f, -1);
            string spacer = slider.value > threshold ? "<color=#ffffff00>-</color>" : "";
            valueTMP.text = spacer + slider.value.ToString(format);
        }
    }

    private void OnDisable()
    {
        Debug.Log("Activity1OmegaSlider > OnDisable");
        // if (slider) slider.onValueChanged.RemoveListener(SnapToValue);

        hasInitialized = false;
    }
}
