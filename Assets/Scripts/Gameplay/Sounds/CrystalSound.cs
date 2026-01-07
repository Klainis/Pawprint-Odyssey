using NUnit.Framework;
using UnityEngine;

public class CrystalSound : MonoBehaviour
{
    [SerializeField] private AudioClip _crystalSound;

    private float _pitch;
    private float _lastTime;

    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _audioSource.clip = _crystalSound;
        _audioSource.Play();
    }

    private void Update()
    {
        if (!_audioSource.isPlaying || !_audioSource.loop)
        {
            return;
        }

        if (_audioSource.time < _lastTime)
        {
            OnLoop();
        }

        _lastTime = _audioSource.time;
    }

    private void OnLoop()
    {
        _pitch = Random.Range(0.85f, 1f);
        _audioSource.pitch = _pitch;
    }

}
