using UnityEngine;
using UnityEngine.UI;

public class RotationToggle : MonoBehaviour
{
    public SimulationState simState;
    public Toggle option1;

    private ToggleGroup toggleGroup;

    private void Start()
    {
        Debug.Log("TODO set rotation toggle from sim state");
        // if (!simState) return;

        // if (!TryGetComponent(out toggleGroup)) return;

        // toggleGroup.SetAllTogglesOff();

        // if (!option1) return;

        // if (simState.currentOmega.y >= 0)
        // {
        //     Debug.Log("Setting option1 true");
        //     option1.isOn = true;
        // }
        // else
        // {
        //     Debug.Log("Setting option1 false");
        //     option1.isOn = false;
        // }
    }
}
