using System.Collections;
using UnityEngine;

public class PlayerParrying : MonoBehaviour
{
    [Header("Paramaters")]
    [SerializeField] private float _parryingTime = 0.5f;
    [SerializeField] private float _parryingCooldown = 1.0f;
    [SerializeField] private int _knockBackForce = 15;
    [SerializeField] private AudioClip _successfullParryingClip;
    [SerializeField] private GameObject _parryingShield;

    private AudioSource _audioSource;
    private Rigidbody2D _rigidBody;
    private PlayerAnimation _playerAnimation;
    private PlayerMana _playerMana;
    private static PlayerParrying instance;

    private RigidbodyConstraints2D _rigidbodyConstraints;
    private float _currentParryingTimer;
    private float _parryingCooldownTimer;
    private bool _isParrying;
    private bool _hasReflected;
    private bool _canStartNewParrying = true;

    public static PlayerParrying Instance { get { return instance; } }

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

    void Update()
    {
        HandleParryingInput();
    }

    private void HandleParryingInput()
    {
        var isHeld = PlayerInput.Instance.ParryingHeld;
        var isReleased = PlayerInput.Instance.ParryingReleased;

        if (!PlayerMove.Instance.IsGrounded) return;

        if (isReleased)
            _canStartNewParrying = true;

        if (_parryingCooldownTimer < _parryingCooldown)
        {
            _parryingCooldownTimer += Time.deltaTime;
            return;
        }

        if (isHeld && !_isParrying && _canStartNewParrying)
            StartParrying();
        if (isHeld && _isParrying)
        {
            _currentParryingTimer += Time.deltaTime;

            if (_currentParryingTimer >= _parryingTime)
                StopParrying();
        }
        if (isReleased && _isParrying)
            StopParrying();
    }

    private void StartParrying()
    {
        _parryingShield.SetActive(true);

        _isParrying = true;
        _hasReflected = false;
        _currentParryingTimer = 0;
        PlayerView.Instance.IsInvincible = true;
        PlayerMove.Instance.CanMove = false;
        _rigidBody.linearVelocity = Vector2.zero;
        _rigidBody.constraints = RigidbodyConstraints2D.FreezeAll;
        _playerAnimation.SetFloatSpeed(0);
        _playerAnimation.SetBoolIsParrying(true);
    }

    private void StopParrying()
    {
        _parryingShield.SetActive(false);

        _isParrying = false;
        _canStartNewParrying = false;
        PlayerMove.Instance.CanMove = true;
        _rigidBody.constraints = _rigidbodyConstraints;
        _playerAnimation.SetBoolIsParrying(false);

        StartCoroutine(StayInvincible());
    }

    public void HandleParrying(int damage, int direction, GameObject enemy)
    {
        if (!_isParrying) return;
        if (_hasReflected) return;
        if (!PlayerView.Instance.IsInvincible) return;

        Debug.Log("ASDADSAD");
        _hasReflected = true;

        _playerMana.GetMana();
        var playerDamage = PlayerView.Instance.PlayerModel.Damage;
        var reflectedDamage = damage >= playerDamage ? playerDamage : damage;

        if (_successfullParryingClip != null)
            PlaySound(_successfullParryingClip);

        enemy.SendMessage("ApplyDamage", reflectedDamage);

        StopParrying();
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            _audioSource.PlayOneShot(clip);
        }
    }

    private IEnumerator StayInvincible()
    {
        yield return new WaitForSeconds(0.5f);
        PlayerView.Instance.IsInvincible = false;
    }
}
