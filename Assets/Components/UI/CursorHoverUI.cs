using UnityEngine;
using UnityEngine.EventSystems;

public class CursorHoverUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private CustomCursor customCursor;

    [SerializeField] private Texture2D hoverCursor = null;
    [SerializeField] private Vector2 hotspot = default;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (customCursor)
        {
            Cursor.SetCursor(customCursor.texture, customCursor.hotspot, CursorMode.Auto);
            return;
        }

        // Display the cursor while hovering
        if (hoverCursor != null)
        {
            Cursor.SetCursor(hoverCursor, hotspot, CursorMode.Auto);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        RestoreDefault();
    }

    private void RestoreDefault()
    {
        if (customCursor)
        {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            return;
        }

        // Restore the default cursor
        if (hoverCursor != null)
        {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }
    }

    private void OnDisable()
    {
        RestoreDefault();
    }
}
