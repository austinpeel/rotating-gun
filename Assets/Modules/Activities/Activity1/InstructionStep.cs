using UnityEngine;

public class InstructionStep : MonoBehaviour
{
    public RectTransform[] panels;

    public void ShowAllPanels()
    {
        foreach (var panel in panels)
        {
            panel.gameObject.SetActive(true);
        }
    }

    public void HideAllPanels()
    {
        foreach (var panel in panels)
        {
            panel.gameObject.SetActive(false);
        }
    }
}
