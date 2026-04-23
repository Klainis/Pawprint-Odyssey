using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

public class StunAudioController : MonoBehaviour
{
    [SerializeField] private AudioMixerSnapshot normalSnap;
    [SerializeField] private AudioMixerSnapshot stunSnap;

    [Tooltip("Время перехода в оглушение (сек)")]
    public float fadeIn = 0.05f;
    [Tooltip("Длительность эффекта (сек)")]
    public float duration = 2f;
    [Tooltip("Время восстановления (сек)")]
    public float fadeOut = 0.8f;

    private Coroutine stunCoroutine;

    public void TriggerStun()
    {
        if (stunCoroutine != null)
        {
            StopCoroutine(stunCoroutine);
        }

        stunCoroutine = StartCoroutine(ApplyStunEffect());
    }

    private IEnumerator ApplyStunEffect()
    {
        stunSnap.TransitionTo(fadeIn);

        yield return new WaitForSecondsRealtime(duration);

        normalSnap.TransitionTo(fadeOut);

        yield return new WaitForSecondsRealtime(fadeOut);
        stunCoroutine = null;
    }
}