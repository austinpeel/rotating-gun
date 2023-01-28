using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteDissolve : MonoBehaviour
{
    public Color startColor = Color.black;
    public float dissolveTime = 1;
    public float startDelay = 0;

    private SpriteRenderer sprite;

    private void OnEnable()
    {
        sprite = GetComponent<SpriteRenderer>();
        sprite.color = startColor;
        StartCoroutine(Dissolve());
    }

    private IEnumerator Dissolve()
    {
        yield return new WaitForSeconds(startDelay);

        float time = 0;
        Color targetColor = startColor;
        targetColor.a = 0;

        while (time < dissolveTime)
        {
            time += Time.unscaledDeltaTime;
            sprite.color = Color.Lerp(startColor, targetColor, time / dissolveTime);
            yield return null;
        }

        sprite.color = targetColor;
    }
}
