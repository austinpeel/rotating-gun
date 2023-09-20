using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class SandboxAutoFire : MonoBehaviour
{
    [SerializeField] private Button fireButton;

    private void OnEnable()
    {
        Toggle toggle = GetComponent<Toggle>();
        toggle.onValueChanged.AddListener(HandleToggleChanged);
    }

    private void OnDisable()
    {
        Toggle toggle = GetComponent<Toggle>();
        toggle.onValueChanged.RemoveListener(HandleToggleChanged);
    }

    public void HandleToggleChanged(bool isOn)
    {
        if (!fireButton) return;

        fireButton.interactable = !isOn;
        if (fireButton.TryGetComponent<CursorHoverUI>(out var cursorHover))
        {
            cursorHover.enabled = !isOn;
        }
    }
}
