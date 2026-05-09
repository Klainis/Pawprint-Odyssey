using System.Collections;
using UnityEngine;

public class DeathFlash : MonoBehaviour
{
    [SerializeField] private Material _deathMaterial;
    [SerializeField] private Color _deathFlashColor;
    [SerializeField] private float _deathFlashDuration;

    private SpriteRenderer _spriteRenderer;

    private Coroutine _deathFlashCoroutine;
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void CallDeathFlashCoroutine()
    {
        if (_deathFlashCoroutine != null)
        {
            StopCoroutine( _deathFlashCoroutine );
        }

        _spriteRenderer.material = _deathMaterial;
        _spriteRenderer.material.color = _deathFlashColor;

        _deathFlashCoroutine = StartCoroutine(DeatFlasher());
    }

    private IEnumerator DeatFlasher()
    {
        float flashAmount = 0;

        float flashTime = _deathFlashDuration;
        float elapsedTime = 0;

        while (elapsedTime < flashTime)
        {
            elapsedTime += Time.deltaTime;

            flashAmount = Mathf.Lerp( 0, 1, elapsedTime / flashTime);
            _spriteRenderer.material.SetFloat("_FlashAmount", flashAmount);

            yield return null;
        }
        _spriteRenderer.material.SetFloat("_FlashAmount", 1);
    }
}
