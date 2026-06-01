using System.Collections;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class DamageFlash : MonoBehaviour
{
    [ColorUsage(true, true)]
    [SerializeField] private Color _flashColor = Color.white;
    [SerializeField] private float _flashTime = 0.25f;

    private SpriteRenderer _spriteRenderer;
    private Material _material;

    private Coroutine _damageFlashCoroutine;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        Init();
    }

    private void Update()
    {
        if (EndBossFightFX.Instance.isEndingBossFight)
        {
            if (_damageFlashCoroutine != null) StopCoroutine(_damageFlashCoroutine);
            CallMaxMinFlashAmount(1);
        }
    }

    private void OnEnable()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        Init();
    }

    private void Init()
    {
        _material = _spriteRenderer.material;
    }

    public void CallDamageFlash()
    {
        _damageFlashCoroutine = StartCoroutine(FlashDamage());
    }

    private IEnumerator FlashDamage()
    {
        if (EndBossFightFX.Instance.isEndingBossFight)
        {
            StopCoroutine(_damageFlashCoroutine);
            _damageFlashCoroutine = null;
        }

        SetFlashColor();

        float currentFlashAmount = 0f;
        float elapsedTime = 0f;

        while (elapsedTime < _flashTime)
        {
            elapsedTime += Time.deltaTime;

            currentFlashAmount = Mathf.Lerp(1f, 0f, (elapsedTime / _flashTime));
            SetFlashAmount(currentFlashAmount);

            yield return null;
        }

        _damageFlashCoroutine = null;
    }

    private void SetFlashColor()
    {
        _material.SetColor("_FlashColor", _flashColor);
    }

    private void SetFlashAmount(float flashAmount)
    {
        _material.SetFloat("_FlashAmount", flashAmount);
    }

    public void CallMaxMinFlashAmount(float value)
    {
        if (_damageFlashCoroutine != null) StopCoroutine(_damageFlashCoroutine);

        SetFlashAmount(value);
    }
}
