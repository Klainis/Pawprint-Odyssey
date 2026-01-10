using System.Collections;
using UnityEngine;
using UnityEngine.U2D;

public class CollapsingPlatform : MonoBehaviour
{
    [Header("Destroy parameters")]
    [SerializeField] private float _timeBeforeHide = 1.5f;
    [SerializeField] private float _timeToReveal = 1.5f;

    [Header("Shake parameters")]
    //[SerializeField] private float _initialShakeDuration = 0.1f;
    [SerializeField] private float _shakeMagnitude = 0.25f;

    [Header("Particles")]
    [SerializeField] private ParticleSystem _destroyParticale;

    private ParticleSystem _destroyParticaleInstance;

    private SpriteShapeRenderer spriteRenderer;
    private Collider2D platformCollider;
    private Collider2D collapseTriggerCollider;
    private Vector3 _initialPosition;

    private bool isHidden = false;
    private float _shakeDuration;

    public bool IsHidden { get { return isHidden; } }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteShapeRenderer>();
        platformCollider = GetComponent<Collider2D>();
        collapseTriggerCollider = GetComponentInChildren<CollapseTrigger>().GetComponent<Collider2D>();
    }

    private void Start()
    {
        _initialPosition = transform.localPosition;
        _shakeDuration = _timeBeforeHide;
    }

    public void ChangePlatformState(bool toHidden)
    {
        isHidden = toHidden;
        spriteRenderer.enabled = !toHidden;
        platformCollider.enabled = !toHidden;
        collapseTriggerCollider.enabled = !toHidden;
    }

    public IEnumerator Shake()
    {
        while (_shakeDuration >= 0)
        {
            transform.localPosition = _initialPosition + Random.insideUnitSphere * _shakeMagnitude;
            _shakeDuration -= Time.deltaTime;
            yield return null;
        }

        transform.localPosition = _initialPosition;
    }

    private void InstantiateParticles()
    {
        Quaternion particleRotation = new Quaternion(-90, 0, 0, 0);
        _destroyParticaleInstance = Instantiate(_destroyParticale, transform.position, particleRotation);
        Debug.Log(_destroyParticaleInstance.transform.rotation);
    }

    public IEnumerator WaitAndChangeState()
    {
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(Shake());
        yield return new WaitForSeconds(_timeBeforeHide);
        ChangePlatformState(true);
        InstantiateParticles();

        yield return new WaitForSeconds(_timeToReveal);
        while (Physics2D.OverlapBox(transform.position, platformCollider.bounds.size, 0, LayerMask.GetMask("Player")) != null)
            yield return null;
        ChangePlatformState(false);
        _shakeDuration = _timeBeforeHide;
    }
}
