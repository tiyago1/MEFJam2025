using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class EndSceneController : MonoBehaviour
{
    public Image blackScreen;                       // Tam ekran siyah image
    public TextMeshProUGUI[] textsToShow;           // Gösterilecek yazýlar (sýralý)
    public GameObject objectToActivate;             // Son aþamada aktif olacak obje

    public float blackScreenDuration = 3f;
    public float fadeDuration = 1.5f;
    public float waitAfterFadeIn = 1f;

    void Start()
    {
        StartCoroutine(PlayEndSequence());
    }

    IEnumerator PlayEndSequence()
    {
        // Baþlangýçta siyah ekran
        blackScreen.color = Color.black;
        foreach (var text in textsToShow)
            SetTextAlpha(text, 0f);

        objectToActivate.SetActive(false);

        yield return new WaitForSeconds(blackScreenDuration);
        //blackScreen.enabled = false; // Siyah ekraný kaldýrmak istersen

        foreach (var text in textsToShow)
        {
            yield return StartCoroutine(FadeText(text, 0f, 1f, fadeDuration)); // Fade in
            yield return new WaitForSeconds(waitAfterFadeIn);
            yield return StartCoroutine(FadeText(text, 1f, 0f, fadeDuration)); // Fade out
        }

        objectToActivate.SetActive(true);
    }

    IEnumerator FadeText(TextMeshProUGUI text, float startAlpha, float endAlpha, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            SetTextAlpha(text, alpha);
            elapsed += Time.deltaTime;
            yield return null;
        }

        SetTextAlpha(text, endAlpha);
    }

    void SetTextAlpha(TextMeshProUGUI text, float alpha)
    {
        Color c = text.color;
        text.color = new Color(c.r, c.g, c.b, alpha);
    }
}
