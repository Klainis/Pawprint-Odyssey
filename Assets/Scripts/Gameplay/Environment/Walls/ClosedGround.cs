using System.Collections;
using UnityEngine;

public class ClosedGround : MonoBehaviour
{
    [Header("Particles")]
    [SerializeField] private ParticleSystem _destroyParticale;

    [Header("Shake Settings")]
    [SerializeField] private float _shakeDuration = 0.5f;
    [SerializeField] private float _shakeMagnitude = 0.2f;
    [SerializeField] private bool _useLocalPosition = true;

    private Vector3 _originalPos;

    private ParticleSystem _destroyParticaleInstance;

    private Coroutine _currentShakeCoroutine;
    private Coroutine _destroyer;

    public void Shake(float duration, float magnitude)
    {
        if (_currentShakeCoroutine != null)
            StopCoroutine(_currentShakeCoroutine);

        _currentShakeCoroutine = StartCoroutine(ShakeCoroutine(duration, magnitude));
    }

    public void Shake()
    {
        Shake(_shakeDuration, _shakeMagnitude);
    }

    private IEnumerator ShakeCoroutine(float duration, float magnitude)
    {
        _originalPos = _useLocalPosition ? transform.localPosition : transform.position;

        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            Vector3 offset = new Vector3(x, y, 0);

            if (_useLocalPosition)
                transform.localPosition = _originalPos + offset;
            else
                transform.position = _originalPos + offset;

            elapsed += Time.deltaTime;
            yield return null;
        }

        if (_useLocalPosition)
            transform.localPosition = _originalPos;
        else
            transform.position = _originalPos;

        _currentShakeCoroutine = null;
    }

    private void InstantiateParticles()
    {
        Quaternion particleRotation = new Quaternion(-90, 0, 0, 0);
        _destroyParticaleInstance = Instantiate(_destroyParticale, transform.position, particleRotation);
        Debug.Log(_destroyParticaleInstance.transform.rotation);
    }

    public void StartDestroyer()
    {
        if (_destroyer != null)
        {
            StopCoroutine( _destroyer );
        }
        _destroyer = StartCoroutine(Destroyer());
    }

    private IEnumerator Destroyer()
    {
        Shake();

        yield return new WaitForSeconds(1.5f);

        InstantiateParticles();
        Destroy(gameObject);
    }
}
