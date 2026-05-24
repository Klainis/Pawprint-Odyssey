using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;

public class EndGameManager : MonoBehaviour
{
    private static EndGameManager _instance;
    public static EndGameManager Instance {  get { return _instance; } }

    public GameObject endGameScreenObject;
    public Volume volume;

    private Vignette _vignette;

    private Coroutine _disableVignetteCoroutine;

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
        }
        _instance = this;

        volume = FindAnyObjectByType<Volume>();

        if (volume.profile.TryGet(out Vignette vignette))
        {
            _vignette = vignette;
        }
    }

    private void Start()
    {
        if (endGameScreenObject != null)
        {
            endGameScreenObject.SetActive(false);
        }
        else
        {
            Debug.LogError("EndGameScreen Object is NULL");
        }
    }

    public void EnableEndGameScreen()
    {
        if (endGameScreenObject == null)
        {
            Debug.LogError("EndGameScreen Object is NULL");
            return;
        }

        endGameScreenObject.SetActive(true);

        StartDisableVignette();
    }

    private void StartDisableVignette()
    {
        if (_disableVignetteCoroutine != null)
        {
            StopCoroutine(_disableVignetteCoroutine);
        }
        StartCoroutine(DisableVignette(1f, 0));
    }

    private IEnumerator DisableVignette(float duration, float finishValue)
    {
        var elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            _vignette.intensity.value = Mathf.Lerp(0.181f, 0, elapsed / duration);
            yield return null;
        }

        _vignette.intensity.value = 0;
    }
}
