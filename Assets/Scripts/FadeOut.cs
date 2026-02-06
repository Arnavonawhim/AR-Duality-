using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeOut : MonoBehaviour
{
    public Image blackImage;
    public float fadeTime = 1f;

    void Start()
    {
        StartCoroutine(Fade());
    }

    IEnumerator Fade()
    {
        float t = 0;
        Color c = blackImage.color;

        while (t < fadeTime)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(1, 0, t / fadeTime);
            blackImage.color = c;
            yield return null;
        }

        blackImage.gameObject.SetActive(false);
    }
}
