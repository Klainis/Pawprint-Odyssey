using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class MusicHandler : MonoBehaviour
{
    private static MusicHandler instance;
    public static MusicHandler Instance {  get { return instance; } }

    [Header("Music")]
    [SerializeField] private AudioClip _mainMusicClip;

    [Header("Parameters")]
    [SerializeField][Range(-40f, 0f)] private float _musicVolumeInMenu = -20f;

    [SerializeField] private AudioMixer audioMaster;
    [SerializeField] private float fadeDuration = 1f;

    private AudioSource _audioSource;

    private float _initialMaxDB;

    private float _currentDB;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        audioMaster.SetFloat("MasterVolume", -80f);

        _audioSource = GetComponent<AudioSource>();
        _audioSource.clip = _mainMusicClip;
    }
    private void Update()
    {
        audioMaster.GetFloat("MasterVolume", out float currentDB);
        Debug.Log(currentDB);
    }

    public void AudioFadeOut()
    {
        Debug.Log("AUDIO FADE IN");
        audioMaster.GetFloat("MasterVolume", out float currentDB);
        StartCoroutine(FadeAudio(currentDB, -80, fadeDuration));
    }

    public void AudioFadeIn()
    {
        Debug.Log("AUDIO FADE OUT");
        audioMaster.GetFloat("MasterVolume", out float currentDB);
        StartCoroutine(FadeAudio(currentDB, 0, fadeDuration));
    }

    public void AudioStartGameFadeIn()
    {
        Debug.Log("START GAME");
        _audioSource?.Play();
        StartCoroutine(FadeAudio(-80, 0, 2 * fadeDuration));
    }

    public void AudioInMenu()
    {
        Debug.Log("IN MENU");
        audioMaster.GetFloat("MasterVolume", out float currentDB);
        StartCoroutine(FadeAudio(currentDB, _musicVolumeInMenu, 0.7f * fadeDuration));
    }

    public void AudioOutMenu()
    {
        audioMaster.GetFloat("MasterVolume", out float currentDB);
        StartCoroutine(FadeAudio(currentDB, 0f, 0.7f * fadeDuration));
    }

    private IEnumerator FadeAudio(float startVolume, float endVolume, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float dB = Mathf.Lerp(startVolume, endVolume, elapsedTime / duration);
            audioMaster.SetFloat("MasterVolume", dB);
            yield return null;
        }
        audioMaster.SetFloat("MasterVolume", endVolume);
        //if (endVolume == -80f)
        //{
        //    AudioListener.pause = true;
        //}
        //else
        //{
        //    AudioListener.pause = false;
        //}
    }
}
