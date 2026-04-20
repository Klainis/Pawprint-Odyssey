using System.Collections;
using UnityEngine;

public class ShakeCantBuyNode : MonoBehaviour
{
    [SerializeField] private float shakeDuration = 0.1f;
    [SerializeField] private float shakeMagnitude = 4f;
    //[SerializeField] private float dampingSpeed = 1f;

    Vector2 initialPosition;

    private Coroutine _shaker;

    void Start()
    {
        //shakeDuration = 0;
        initialPosition = transform.localPosition;
    }

    public void Shake()
    {
        if (_shaker != null)
        {
            StopCoroutine(_shaker);
            transform.localPosition = initialPosition;
        }

        _shaker = StartCoroutine(Shaker());
    }

    public IEnumerator Shaker()
    {
        Debug.Log("Начали Shake");
        float time = shakeDuration;
        float elapsedTime = 0;
        while (elapsedTime < time)
        {
            elapsedTime += Time.unscaledDeltaTime;
            transform.localPosition = initialPosition + Random.insideUnitCircle * shakeMagnitude;

            yield return null;
        }

        transform.localPosition = initialPosition;
    }
}
