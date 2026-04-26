using System.Collections;
using UnityEngine;

public class LightAttackObject : MonoBehaviour
{
    [SerializeField] private float _lifeTime = 1f;
    [SerializeField] private float _sizeModifier = 1.3f;
    [SerializeField] private float _targetAlpha = 0.2f;

    private SpriteRenderer _spriteRender;
    private BoxCollider2D _boxCollider;

    private SpiritGuideView sgView;

    private Coroutine _lifeCoroutie;

    private void Awake()
    {
        _spriteRender = GetComponentInChildren<SpriteRenderer>();   
        sgView = FindAnyObjectByType<SpiritGuideView>();
        _boxCollider = GetComponent<BoxCollider2D>();
        
        StartLifeLooper();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (!sgView.Model.IsDead)
            {
                var playerView = collision.gameObject.GetComponent<PlayerView>();
                playerView.ApplyDamage(sgView.Model.Damage, transform.position, gameObject);
            }
        }
    }

    private void StartLifeLooper()
    {
        if (_lifeCoroutie != null)
        {
            StopCoroutine( _lifeCoroutie );
        }

        _lifeCoroutie = StartCoroutine(LifeLooper());
    }

    private IEnumerator LifeLooper()
    {
        Color startColor = _spriteRender.color;
        Vector3 startScale = transform.localScale;

        float elapsedTime = 0f;

        while (elapsedTime < _lifeTime)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / _lifeTime;

            Color currentColor = startColor;
            currentColor.a = Mathf.Lerp(1f, _targetAlpha, t);
            _spriteRender.color = currentColor;

            transform.localScale = Vector3.Lerp(startScale, startScale / _sizeModifier, t);

            if (currentColor.a < _targetAlpha/3)
            {
                _boxCollider.enabled = false;
            }

            yield return null;
        }

        _spriteRender.color = new Color(1, 1, 1, 0);
        transform.localScale = startScale / _sizeModifier;

        Destroy(gameObject);
    }
}
