// ----------------------------------------------------------------------------------------------------------
// Author: Austin Peel
//
// © All rights reserved. ECOLE POLYTECHNIQUE FEDERALE DE LAUSANNE, Switzerland, Section de Physique, 2024.
// See the LICENSE.md file for more details.
// ----------------------------------------------------------------------------------------------------------
using UnityEngine;

public class InstructionStep : MonoBehaviour
{
    [Tooltip("Non-children elements of this step")]
    public RectTransform[] panels;

    private CanvasGroup canvasGroup;

    private void Awake()
    {
        TryGetComponent(out canvasGroup);
    }

    public void Show()
    {
        if (canvasGroup)
        {
            canvasGroup.alpha = 1;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }

        foreach (var panel in panels)
        {
            panel.gameObject.SetActive(true);
        }
    }

    public void Hide()
    {
        if (canvasGroup)
        {
            canvasGroup.alpha = 0;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        foreach (var panel in panels)
        {
            panel.gameObject.SetActive(false);
        }
    }
}
