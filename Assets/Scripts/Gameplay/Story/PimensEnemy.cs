using System;
using System.Collections;
using System.Collections.Generic;
using Unity.InferenceEngine;
using UnityEngine;

public class PimensEnemy : MonoBehaviour
{
    #region SerializeFields

    [Header("Main params")]
    [SerializeField] private EnemyData _data;
    [SerializeField] private bool _isInvincible = false;
    [SerializeField] private AudioClip _hitClip;

    [Header("Particles")]
    [SerializeField] private ParticleSystem _damageParticle;
    [SerializeField] private ParticleSystem _playerWeaponParticle;
    [SerializeField] private ParticleSystem _playerWeaponSliceParticle;
    [SerializeField] private ParticleSystem _playerWeapomSimpleSliceParticle;

    #endregion

    #region Variables

    private ParticleSystem _playerWeaponSimpleSliceAttackParticleInstance;
    private ParticleSystem _damageParticleInstance;
    private ParticleSystem _playerWeaponParticleInstance;
    private ParticleSystem _playerWeaponSliceParticleInstance;

    private AudioSource _audioSource;
    private Rigidbody2D _rb;

    private DamageFlash[] _damageFlash;
    private ScreenShaker _screenShaker;

    private RigidbodyConstraints2D _defaultConstraints;

    private bool _damageApplied = false;
    private bool _isEnemyCounting = false;

    #endregion

    public EnemyModel Model { get; private set; }
    public Rigidbody2D RigidBody { get { return _rb; } }
    public bool FacingRight { get; private set; } = false;

    private void Awake()
    {
        Model = new EnemyModel(_data.Life, _data.Speed, _data.Damage, _data.Reward);

        _audioSource = GetComponent<AudioSource>();
        _rb = GetComponent<Rigidbody2D>();
        _damageFlash = GetComponentsInChildren<DamageFlash>();
        _screenShaker = GetComponent<ScreenShaker>();

        _defaultConstraints = _rb.constraints;
    }

    private void FixedUpdate()
    {
        if (Model.IsDead)
        {
            StartCoroutine(DestroySelf());
            return;
        }
    }

    public void ApplyDamage(int damage)
    {
        Debug.Log("Pimens Enemy Take Damage");
        if (_isInvincible) return;

        var direction = damage / Mathf.Abs(damage);

        _damageApplied = Model.TakeDamage(Mathf.Abs(damage));

        if (_damageApplied)
        {
            PlayHitSound(_hitClip);

            foreach (var damageFlash in _damageFlash)
            {
                damageFlash.CallDamageFlash();
            }

            //_rb.linearVelocity = Vector2.zero;

            _screenShaker.Shake();

            SpawnDamageParticles(direction);
            SpawnPlayerAttakParticles(direction);
        }
    }

    private void SpawnDamageParticles(int direction)
    {
        var vectorDirection = new Vector2(direction, 0);
        var spawnRotation = Quaternion.FromToRotation(Vector2.right, vectorDirection);
        var spawnPlayerAttackRotation = Quaternion.FromToRotation(Vector2.right, -vectorDirection);

        _damageParticleInstance = Instantiate(_damageParticle, transform.position, spawnRotation);
    }

    private void SpawnPlayerAttakParticles(int direction)
    {
        var vectorDirection = new Vector2(direction, 0);
        var spawnPlayerAttackRotation = Quaternion.FromToRotation(Vector2.right, -vectorDirection);

        _playerWeaponParticleInstance = Instantiate(_playerWeaponParticle, transform.position, spawnPlayerAttackRotation, transform);
        _playerWeaponSimpleSliceAttackParticleInstance = Instantiate(_playerWeapomSimpleSliceParticle, transform.position, Quaternion.identity);
    }

    private void PlayHitSound(AudioClip clip)
    {
        if (clip != null)
        {
            _audioSource.PlayOneShot(clip);
        }
    }

    private void ChangeTag(string tag)
    {
        gameObject.tag = tag;
    }

    private void ChangeLayer(string layer)
    {
        gameObject.layer = LayerMask.NameToLayer(layer);
    }

    private IEnumerator DestroySelf()
    {
        _isInvincible = true;
        ChangeLayer("DeadEnemy");
        ChangeTag("isDead");

        if (!_isEnemyCounting)
        {
            AttackPimenManager.Instance.CountDeadEnemy();
            _isEnemyCounting = true;
        }

        _rb.linearVelocity = new Vector2(0, _rb.linearVelocity.y);
        yield return new WaitForSeconds(0.1f);

        _isEnemyCounting = false;
        Destroy(gameObject);
    }
}
