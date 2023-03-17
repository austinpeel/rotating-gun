using UnityEngine;

public class Activity1OmegaController : MonoBehaviour
{
    public GameObject backgroundSprite;
    public GameObject intoPlaneSprite;
    public GameObject outOfPlaneSprite;

    private void Awake()
    {
        if (backgroundSprite) backgroundSprite.SetActive(false);
        if (intoPlaneSprite) intoPlaneSprite.SetActive(false);
        if (outOfPlaneSprite) outOfPlaneSprite.SetActive(false);
    }

    public void SetOmega(float value)
    {
        if (backgroundSprite) backgroundSprite.SetActive(value != 0);
        if (intoPlaneSprite) intoPlaneSprite.SetActive(value < 0);
        if (outOfPlaneSprite) outOfPlaneSprite.SetActive(value > 0);
    }
}
