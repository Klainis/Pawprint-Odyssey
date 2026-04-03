using System.Collections;
using UnityEngine;

public class ThornyPlant : MonoBehaviour {

    #region Variables

    [Header("Main params")]
    [SerializeField] private float _life = 10;
    [SerializeField] private int _damage = 1;
    [SerializeField] private LayerMask _playerLayer;
    [SerializeField] private GameObject _openForm;
    [SerializeField] private GameObject _closeForm;
    [SerializeField] private AudioClip _hitClip;

    [Header("Shots params")]
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private float _bulletSpeed = 5f;
    [SerializeField] private int _shotsPerSeries = 3;
    [SerializeField] private float _timeBetweenShots = 0.35f;
    [SerializeField] private float _timeBetweenSeries = 2.5f;
    [SerializeField] private float _delayAfterOpen = 1f;

    [Header("Particles")]
    [SerializeField] private ParticleSystem _damageParticle;
    [SerializeField] private ParticleSystem _playerWeaponParticle;
    [SerializeField] private ParticleSystem _playerWeaponLastSliceParticle;
    [SerializeField] private ParticleSystem _playerWeapomSimpleSliceParticle;

    private ParticleSystem _playerWeaponSimpleSliceAttackParticleInstance;
    private ParticleSystem _playerWeaponLastSliceAttackParticleInstance;
    private ParticleSystem _damageParticleInstance;
    private ParticleSystem _playerWeaponParticleInstance;

    private AudioSource _audioSource;
    private TPAnimation _plantAnimation;
    private Rigidbody2D _rigidBody;
    private Transform _shootPoints;

    private ScreenShaker _screenShaker;
    private InstantiateMoney _money;
    private PlayerAttack _playerAttack;

    private float _lastSeriesTime = 0;
    private float _lastOpenTime = 0;
    private bool _canShoot = false;
    private bool _isShooting = false;
    private bool _isInvincible = false;
    private bool _isHitted = false;
    private bool _isHidden = true;

    #endregion

    #region Properties

    public int Damage { get { return _damage; } }
    public bool IsShooting { get { return _isShooting; } }
    public bool IsHidden { get { return _isHidden; } }

    #endregion

    #region Common Methods

    void Awake () {
        _audioSource = GetComponent<AudioSource>();
        _plantAnimation = GetComponent<TPAnimation>();
        _rigidBody = GetComponent<Rigidbody2D>();
        _screenShaker = GetComponent<ScreenShaker>();
        _money = FindAnyObjectByType<InstantiateMoney>();
        _playerAttack = InitializeManager.Instance.player?.GetComponent<PlayerAttack>();

        _shootPoints = transform.Find("ShootPoints");
    }
	
	void FixedUpdate () {
		if (_life <= 0)
        {
            StartCoroutine(DestroyEnemy());
            return;
        }
        if (_isHitted)
            return;

        _canShoot = (Time.time >= _lastSeriesTime + _timeBetweenSeries)
                && (Time.time >= _lastOpenTime + _delayAfterOpen);
        if (!_isHidden && _canShoot && !_isShooting)
            StartCoroutine(ShootRoutine());
	}

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            if (_life > 0)
                collision.gameObject.GetComponent<PlayerView>().ApplyDamage(_damage, transform.position, gameObject);
    }

    #endregion

    public void ChangeForm(bool toHidden)
    {
        if (gameObject.CompareTag("isDead"))
            return;

        _isHidden = toHidden;
        _openForm.SetActive(!toHidden);
        _closeForm.SetActive(toHidden);

        if (!toHidden)
            _lastOpenTime = Time.time;
    }

    private void Shoot()
    {
        for (var i = 0; i < _shootPoints.childCount; i++)
        {
            var shootPoint = _shootPoints.GetChild(i);
            var bullet = Instantiate(_bulletPrefab, shootPoint.position, shootPoint.rotation, transform);
            _rigidBody = bullet.GetComponent<Rigidbody2D>();
            if (_rigidBody != null)
                _rigidBody.linearVelocity = shootPoint.right * _bulletSpeed;
        }
    }

    public void ApplyDamage(int damage) {
		if (!_isInvincible) 
		{
            PlayHitSound(_hitClip);
            _plantAnimation.SetTriggerHit();
            _screenShaker.Shake();

            var direction = damage / Mathf.Abs(damage);

            _life -= Mathf.Abs(damage);

            if (_life <= 0)
            {
                _money.SetReward(3);
                _money.InstantiateMon(transform.position);
            }
            StartCoroutine(HitTime());

            SpawnDamageParticles(direction);

            if (_playerAttack.AttackSeriesCount == 4)
            {
                SpawnPlayerLastAttackParticles();
            }
            else if (_playerAttack.AttackSeriesCount < 4)
            {
                SpawnPlayerAttakParticles(direction);
            }
        }
	}

    public void ApplyChargeDamage(int damage)
    {
        ApplyDamage(damage);
    }

    private void PlayHitSound(AudioClip clip)
    {
        if (clip != null)
        {
            _audioSource.PlayOneShot(clip);
        }
    }

    #region Particles

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

    private void SpawnPlayerLastAttackParticles()
    {
        _playerWeaponLastSliceAttackParticleInstance = Instantiate(_playerWeaponLastSliceParticle, transform.position, Quaternion.identity);
    }

    #endregion

    #region IEnumerators

    private IEnumerator ShootRoutine()
    {
        _isShooting = true;
        _canShoot = false;
        for (var i = 0; i < _shotsPerSeries; i++)
        {
            Shoot();
            yield return new WaitForSeconds(_timeBetweenShots);
        }
        _lastSeriesTime = Time.time;
        _isShooting = false;
    }

    private IEnumerator HitTime()
	{
		_isHitted = true;
		//_isInvincible = true;
		yield return new WaitForSeconds(0.1f);
		_isHitted = false;
		//_isInvincible = false;
	}

	private IEnumerator DestroyEnemy()
	{
        _isInvincible = true;
        //_animator.SetTrigger("Dead");
        gameObject.layer = LayerMask.NameToLayer("DeadEnemy");
        gameObject.tag = "isDead";
        ChangeForm(true);
        //Vector3 rotator = new Vector3(transform.rotation.x, transform.rotation.y, -90f);
        //transform.rotation = Quaternion.Euler(rotator);
        //yield return new WaitForSeconds(0.25f);
        //_rb.linearVelocity = new Vector2(0, _rb.linearVelocity.y);
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }

    #endregion
}
