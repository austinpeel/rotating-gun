using System.Collections;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class TabPanel : MonoBehaviour
{
    private RectTransform rectTransform;

    [SerializeField] private float yHidden = -200;
    [SerializeField] private float yShowing = -40;
    private float xPosition;

    private bool isHidden;

    protected void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        xPosition = rectTransform.anchoredPosition.x;
    }

    private void Start()
    {
        Hide(0);
    }

    public void SetVisibility(bool visible)
    {
        if (visible)
        {
            Hide(0.5f);
        }
        else
        {
            Show(1);
        }
    }

    public virtual void Hide(float lerpTime)
    {
        StopAllCoroutines();

        // Vector2 targetPosition = new Vector2(xPosition, -rectTransform.rect.height);
        Vector2 targetPosition = new Vector2(xPosition, yHidden);
        StartCoroutine(LerpPosition(rectTransform, targetPosition, lerpTime, 2));
        isHidden = true;
    }

    public virtual void Show(float lerpTime)
    {
        StopAllCoroutines();
        Vector2 targetPosition = new Vector2(xPosition, yShowing);
        StartCoroutine(LerpPosition(rectTransform, targetPosition, lerpTime, 2));
        isHidden = false;
    }

    private IEnumerator LerpPosition(RectTransform rectTransform,
                                     Vector2 targetPosition,
                                     float lerpTime,
                                     float easeFactor = 0)
    {
        Vector2 startPosition = rectTransform.anchoredPosition;

        float time = 0;
        while (time < lerpTime)
        {
            time += Time.unscaledDeltaTime;
            float t = EaseOut(time / lerpTime, 1 + easeFactor);
            rectTransform.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, t);
            yield return null;
        }

        rectTransform.anchoredPosition = targetPosition;
    }

    private float EaseOut(float t, float a)
    {
        return 1 - Mathf.Pow(1 - t, a);
    }
}
