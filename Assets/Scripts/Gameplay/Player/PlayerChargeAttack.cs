using System.Collections;
using UnityEngine;

public class PlayerChargeAttack : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] float _chargeTime = 1.0f;
    [SerializeField] int _manaCost = 15;
    [SerializeField] private AudioClip _attackChargedClip;

    private AudioSource _audioSource;
    private Rigidbody2D _rigidBody;
    private PlayerView _playerView;
    private PlayerAnimation _playerAnimation;
    private PlayerMana _playerMana;
    private Transform _attackCheck;

    const float _attackCheckRadius = 1.3f;

    private int _damage;
    private float _currentChargeTimer = 0f;
    private bool _isCharging = false;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _rigidBody = GetComponent<Rigidbody2D>();
        _playerView = GetComponent<PlayerView>();
        _playerAnimation = GetComponent<PlayerAnimation>();
        _playerMana = GetComponent<PlayerMana>();
        _attackCheck = transform.Find("AttackCheck");
    }

    private void Start()
    {
        _damage = PlayerView.Instance.PlayerModel.Damage * 2;
    }

    private void Update()
    {
        HandleChargeInput();
    }

    public void HandleChargeInput()
    {
        var isHeld = PlayerInput.Instance.AttackHeld;
        var isReleased = PlayerInput.Instance.AttackReleased;

        if (!PlayerMove.Instance.IsGrounded) return;

        if (isHeld && !_isCharging && _playerView.PlayerModel.Mana >= _manaCost)
        {
            _isCharging = true;
            _currentChargeTimer = 0f;

            PlayerMove.Instance.CanMove = false;
            _rigidBody.linearVelocity = Vector2.zero;

            _playerAnimation.SetFloatSpeed(0);
            _playerAnimation.SetBoolIsChargingAttack(true);
        }

        if (_isCharging && isHeld)
        {
            _currentChargeTimer += Time.deltaTime;
            if (_currentChargeTimer >= _chargeTime)
                PlayChargeAttackSound(_attackChargedClip);
        }

        if (_isCharging && isReleased)
        {
            PlayerMove.Instance.CanMove = true;
            if (_currentChargeTimer < _chargeTime) return;

            _playerAnimation.SetBoolIsChargingAttack(false);
            ReleaseAttack();
        }
    }

    private void ReleaseAttack()
    {
        _isCharging = false;
        _playerMana.SpendMana("ChargeAttack", _manaCost);

        ApplyAreaDamage(_damage);

        _playerAnimation.SetTriggerAttack(3);
        _currentChargeTimer = 0f;
    }

    private void ApplyAreaDamage(int damage)
    {
        var collidersEnemies = Physics2D.OverlapCircleAll(_attackCheck.position, _attackCheckRadius);
        foreach (var col in collidersEnemies)
        {
            if (col.CompareTag("Enemy"))
            {
                var damageDirection = col.transform.position.x - transform.position.x < 0 ? -damage : damage;
                col.SendMessage("ApplyChargeDamage", damageDirection);
            }
        }
    }

    private void PlayChargeAttackSound(AudioClip clip)
    {
        if (clip != null)
        {
            _audioSource.PlayOneShot(clip);
        }
    }
}
