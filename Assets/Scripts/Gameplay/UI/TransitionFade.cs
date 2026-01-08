using System.Collections;
using UnityEngine;

public class TransitionFade : MonoBehaviour
{
    public static TransitionFade _instance { get; private set; }

    public CanvasGroup canvasGroup;

    [SerializeField] private float fadeDuration = 1f;

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);

    }

    public void FadeIn()
    {
        StartCoroutine(FadeCanvasGroup(canvasGroup, canvasGroup.alpha, 0, fadeDuration));
    }

    public void FadeOut()
    {
        StartCoroutine(FadeCanvasGroup(canvasGroup, canvasGroup.alpha, 1, fadeDuration));
    }

    public void StartGameFadeIn()
    {
        StartCoroutine(FadeCanvasGroup(canvasGroup, canvasGroup.alpha, 0, 2*fadeDuration));
        MusicHandler.Instance.AudioStartGameFadeIn();
    }

    private IEnumerator FadeCanvasGroup (CanvasGroup cg, float startAlpha, float endAlpha, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            //cg.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime/duration);
            cg.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
            yield return null;
        }
        cg.alpha = endAlpha;
    }
}
