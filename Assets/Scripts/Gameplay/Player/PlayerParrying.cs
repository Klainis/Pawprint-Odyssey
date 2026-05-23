using System.Collections;
using UnityEngine;

public class PlayerParrying : MonoBehaviour
{
    #region SerializeFields

    [Header("Paramaters")]
    [SerializeField] private float _successParryInvincibleTime = 1.0f;
    [SerializeField] private float _parryingTime = 0.5f;
    [SerializeField] private float _parryingCooldown = 1.0f;
    [SerializeField] private float _stopTime = 0.15f;
    [SerializeField] private int _knockBackForce = 15;
    [SerializeField] private float _stopFrameDuration = 0.1f;
    [SerializeField] private AudioClip _successParryClip;
    [SerializeField] private GameObject _parryingShield;

    #endregion

    #region Variables

    private AudioSource _audioSource;
    private Rigidbody2D _rigidBody;
    private PlayerAnimation _playerAnimation;
    private PlayerMana _playerMana;
    private HitStop _hitStop;
    private static PlayerParrying instance;

    private RigidbodyConstraints2D _rigidbodyConstraints;
    private Coroutine _smoothStopCoroutine;
    private float _currentParryingTimer;
    private float _parryingCooldownTimer;
    private bool _isParrying;
    private bool _hasReflected;
    private bool _canStartNewParrying = true;

    #endregion

    #region Properties

    public static PlayerParrying Instance { get { return instance; } }
    public bool IsParrying { get { return _isParrying; } }

    #endregion

    #region Common Methods

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        _audioSource = GetComponent<AudioSource>();
        _rigidBody = GetComponent<Rigidbody2D>();
        _rigidbodyConstraints = _rigidBody.constraints;
        _playerAnimation = GetComponent<PlayerAnimation>();
        _playerMana = GetComponent<PlayerMana>();
    }

    private void Update()
    {
        HandleParryingInput();
    }

    #endregion

    #region Parrying

    private void HandleParryingInput()
    {
        var isHeld = PlayerInput.Instance.ParryingHeld;
        var isReleased = PlayerInput.Instance.ParryingReleased;

        if (isReleased)
            _canStartNewParrying = true;

        if (_isParrying)
        {
            _currentParryingTimer += Time.deltaTime;
            if (_currentParryingTimer >= _parryingTime || isReleased)
                StopParrying();
            return;
        }

        if (_parryingCooldownTimer < _parryingCooldown)
            _parryingCooldownTimer += Time.deltaTime;
        if (PlayerMove.Instance.IsGrounded && _parryingCooldownTimer >= _parryingCooldown)
        {
            if (isHeld && _canStartNewParrying)
            {
                _canStartNewParrying = false;
                StartParrying();
            }
        }
    }

    private void StartParrying()
    {
        //if (_parryingShield != null)
        //{
        //    _parryingShield.SetActive(true);
        //}
        PlayerMove.Instance.canJump = false;
        PlayerMove.Instance.canDash = false;
        _isParrying = true;
        _hasReflected = false;
        _currentParryingTimer = 0;

        PlayerView.Instance.IsInvincible = true;
        //PlayerMove.Instance.CanMove = false;

        if (_smoothStopCoroutine != null)
            StopCoroutine(_smoothStopCoroutine);
        _smoothStopCoroutine = StartCoroutine(SmoothStop(_stopTime));
        
        _playerAnimation.SetFloatSpeed(0);
        _playerAnimation.SetBoolIsParrying(true);
    }

    private void StopParrying()
    {
        if (_smoothStopCoroutine != null)
        {
            StopCoroutine(_smoothStopCoroutine);
            _smoothStopCoroutine = null;
        }

        //if (_parryingShield != null)
        //{
        //    _parryingShield.SetActive(false);
        //}
        PlayerMove.Instance.canJump = true;
        PlayerMove.Instance.canDash = true;
        _isParrying = false;
        _parryingCooldownTimer = 0f;

        _playerAnimation.SetBoolIsParrying(false);

        //_rigidBody.constraints = _rigidbodyConstraints;
        //PlayerMove.Instance.CanMove = true;
        PlayerView.Instance.FreezePlayerWithDisableMove(false);

        if (_hasReflected)
            StartCoroutine(SuccessParryInvincibleRoutine());

        PlayerView.Instance.IsInvincible = false;
    }

    public void HandleParrying(int damage, int direction, GameObject enemy)
    {
        if (!_isParrying || _hasReflected ||
            !PlayerView.Instance.IsInvincible)
            return;

        _hasReflected = true;

        _playerMana.GetMana();
        var playerDamage = PlayerView.Instance.PlayerModel.Damage;
        var reflectedDamage = damage >= playerDamage ? playerDamage : damage;

        if (_successParryClip != null)
            PlaySound(_successParryClip);

        enemy.SendMessage("ApplyDamage", reflectedDamage);

        _hitStop.Stop(_stopFrameDuration);

        StopParrying();
    }

    #endregion

    private void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            _audioSource.PlayOneShot(clip);
        }
    }

    private IEnumerator SuccessParryInvincibleRoutine()
    {
        var playerLayer = gameObject.layer;
        var enemyLayer = LayerMask.NameToLayer("Enemy");

        Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, true);

        var spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            var color = spriteRenderer.color;
            color.a = 0.5f;
            spriteRenderer.color = color;
        }

        yield return new WaitForSeconds(_successParryInvincibleTime);

        Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, false);

        if (spriteRenderer != null)
        {
            var color = spriteRenderer.color;
            color.a = 1.0f;
            spriteRenderer.color = color;
        }
    }

    private IEnumerator SmoothStop(float duration)
    {
        var velocityX = _rigidBody.linearVelocity.x;
        var currentVelocity = 0f;

        var elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            velocityX = Mathf.SmoothDamp(velocityX, 0, ref currentVelocity, duration);
            _rigidBody.linearVelocity = new Vector2(velocityX, _rigidBody.linearVelocity.y);
            yield return null;
        }

        //_isStop = true;
        //_rigidbody.linearVelocity = new Vector2(0, 0);
        //_rigidbody.gravityScale = 0;
        PlayerView.Instance.FreezePlayerWithDisableMove(true);

    }
}
