using System.Collections;
using UnityEngine;
using GlobalEnums;

public class PlayerChargeAttack : MonoBehaviour
{
    #region SerializeFields

    [Header("Parameters")]
    [SerializeField] private float _chargeTime = 2.0f;
    [SerializeField] private float _startChargeTime = 1.0f;
    [SerializeField] private int _manaCost = 15;
    [SerializeField] private AudioClip _attackChargedClip;

    [Header("Particles")]
    [SerializeField] private Transform _particleSpawnPoint;
    [SerializeField] private ParticleSystem _attackChargingParticle;
    [SerializeField] private ParticleSystem _attackChargedParticle;

    #endregion

    #region Variables

    private static PlayerChargeAttack _instance;
    private AudioSource _audioSource;
    private PlayerView _view;
    private PlayerAnimation _animation;
    private PlayerMana _mana;
    private Transform _attackCheck;

    const float _attackCheckRadius = 1.3f;

    private ParticleSystem _attackChargingParticleInstance;
    private ParticleSystem _attackChargedParticleInstance;
    private int _damage;
    private float _currentChargeTimer = 0f;
    private bool _isCharging = false;

    #endregion

    #region Properties

    public static PlayerChargeAttack Instance { get { return _instance; } }
    public bool IsCharging { get { return _isCharging; } }

    #endregion

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;

        _audioSource = GetComponent<AudioSource>();
        _view = GetComponent<PlayerView>();
        _animation = GetComponent<PlayerAnimation>();
        _mana = GetComponent<PlayerMana>();
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

        if (isHeld && !_isCharging && _view.PlayerModel.Mana >= _manaCost)
        {
            _isCharging = true;
            _currentChargeTimer = 0f;

            _animation.SetBoolIsChargingAttack(true);
        }

        if (_isCharging && isHeld)
        {
            if (GameManager.Instance.GameState == GameState.ENTERING_LEVEL)
            {
                StopCharging();
                return;
            }

            _currentChargeTimer += Time.deltaTime;
            var isCharged = _currentChargeTimer >= _chargeTime + _startChargeTime;
            var startedCharging = _currentChargeTimer >= _startChargeTime;

            if (!isCharged && startedCharging)
            {
                ResetParticleInstance(true);
                SpawnParticle(false);
            }

            if (isCharged)
            {
                PlayChargeAttackSound(_attackChargedClip);
                ResetParticleInstance(false);
                SpawnParticle(true);
            }
        }

        if (_isCharging && isReleased)
        {
            PlayerMove.Instance.CanMove = true;
            ResetParticleInstance(true);
            ResetParticleInstance(false);

            if (_currentChargeTimer < _chargeTime + _startChargeTime)
            {
                _isCharging = false;
                return;
            }

            _animation.SetBoolIsChargingAttack(false);
            ReleaseAttack();
        }
    }

    private void StopCharging()
    {
        _isCharging = false;
        _currentChargeTimer = 0f;
        _animation.SetBoolIsChargingAttack(false);

        ResetParticleInstance(true);
        ResetParticleInstance(false);
    }

    private void ReleaseAttack()
    {
        _isCharging = false;
        _mana.SpendMana("ChargeAttack", _manaCost);

        ApplyAreaDamage(_damage);

        _animation.SetTriggerAttack(3);
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

    #region Particles

    private void SpawnParticle(bool isCharged)
    {
        if (isCharged && _attackChargedParticleInstance == null)
            _attackChargedParticleInstance = InstantiateParticle(_attackChargedParticle);
        else if (!isCharged && _attackChargingParticleInstance == null)
            _attackChargingParticleInstance = InstantiateParticle(_attackChargingParticle);
    }

    private ParticleSystem InstantiateParticle(ParticleSystem particlePrefab)
    {
        var position = _particleSpawnPoint.position;
        var rotation = _particleSpawnPoint.rotation;
        return Instantiate(particlePrefab, position, rotation, _particleSpawnPoint);
    }

    private void ResetParticleInstance(bool chargedInstance)
    {
        if (chargedInstance)
        {
            if (_attackChargedParticleInstance != null)
            {
                Destroy(_attackChargedParticleInstance.gameObject);
                _attackChargedParticleInstance = null;
            }
        }
        else
        {
            if (_attackChargingParticleInstance != null)
            {
                Destroy(_attackChargingParticleInstance.gameObject);
                _attackChargingParticleInstance = null;
            }
        }
    }

    #endregion
}
