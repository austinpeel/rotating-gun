using UnityEngine;

public class ActivityOverlayPanel : MonoBehaviour
{
    public bool activateOnAwake;
    public CanvasGroup canvasGroup;

    private void Awake()
    {
        if (activateOnAwake && canvasGroup)
        {
            canvasGroup.alpha = 1;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
    }
}
