using System.Collections;
using UnityEngine;

public class ThornyPlant : MonoBehaviour {

    #region Variables

    [Header("Main params")]
    [SerializeField] private float life = 10;
    [SerializeField] private int damage = 1;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private GameObject openForm;
    [SerializeField] private GameObject closeForm;

    [Header("Shots params")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed = 7f;
    [SerializeField] private int shotsPerSeries = 3;
    [SerializeField] private float timeBetweenShots = 0.35f;
    [SerializeField] private float timeBetweenSeries = 2.5f;
    [SerializeField] private float delayAfterOpen = 1f;

    [Header("Particles")]
    [SerializeField] private ParticleSystem _damageParticle;
    [SerializeField] private ParticleSystem _playerWeaponParticle;
    [SerializeField] private ParticleSystem _playerWeaponLastSliceParticle;
    [SerializeField] private ParticleSystem _playerWeapomSimpleSliceParticle;

    private ParticleSystem _playerWeaponSimpleSliceAttackParticleInstance;
    private ParticleSystem _playerWeaponLastSliceAttackParticleInstance;
    private ParticleSystem _damageParticleInstance;
    private ParticleSystem _playerWeaponParticleInstance;

    private Animator animator;
    private Rigidbody2D rigidBody;
    private Transform shootPoints;

    private ScreenShaker _screenShaker;
    private InstantiateMoney _money;
    private PlayerAttack playerAttack;

    private float lastSeriesTime = 0;
    private float lastOpenTime = 0;
    private bool canShoot = false;
    private bool isShooting = false;
    private bool isInvincible = false;
    private bool isHitted = false;
    private bool isHidden = true;

    #endregion

    #region Properties

    public int Damage { get { return damage; } }
    public bool IsShooting { get { return isShooting; } }
    public bool IsHidden { get { return isHidden; } }

    #endregion

    #region Common Methods

    void Awake () {
        animator = GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody2D>();
        _screenShaker = GetComponent<ScreenShaker>();
        _money = FindAnyObjectByType<InstantiateMoney>();
        playerAttack = InitializeManager.Instance.player?.GetComponent<PlayerAttack>();

        shootPoints = transform.Find("ShootPoints");
    }
	
	void FixedUpdate () {
		if (life <= 0)
        {
            StartCoroutine(DestroyEnemy());
            return;
        }
        if (isHitted)
            return;

        canShoot = (Time.time >= lastSeriesTime + timeBetweenSeries)
                && (Time.time >= lastOpenTime + delayAfterOpen);
        if (!isHidden && canShoot && !isShooting)
            StartCoroutine(ShootRoutine());
	}

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            if (life > 0)
                collision.gameObject.GetComponent<PlayerView>().ApplyDamage(damage, transform.position);
    }

    #endregion

    public void ChangeForm(bool toHidden)
    {
        if (gameObject.CompareTag("isDead"))
            return;

        isHidden = toHidden;
        openForm.SetActive(!toHidden);
        closeForm.SetActive(toHidden);

        if (!toHidden)
            lastOpenTime = Time.time;
    }

    private void Shoot()
    {
        for (var i = 0; i < shootPoints.childCount; i++)
        {
            var shootPoint = shootPoints.GetChild(i);
            var bullet = Instantiate(bulletPrefab, shootPoint.position, shootPoint.rotation, transform);
            rigidBody = bullet.GetComponent<Rigidbody2D>();
            if (rigidBody != null)
                rigidBody.linearVelocity = shootPoint.right * bulletSpeed;
        }
    }

    public void ApplyDamage(int damage) {
		if (!isInvincible) 
		{
            //_animator.SetBool("_Hit", true);
            //_damageFlash.CallDamageFlash();
            _screenShaker.Shake();

            var direction = damage / Mathf.Abs(damage);

            life -= Mathf.Abs(damage);

            if (life <= 0)
            {
                _money.SetReward(2);
                _money.InstantiateMon(transform.position);
            }
            StartCoroutine(HitTime());

            SpawnDamageParticles(direction);

            if (playerAttack.AttackSeriesCount == 3)
            {
                SpawnPlayerLastAttackParticles();
            }
            else if (playerAttack.AttackSeriesCount < 3)
            {
                SpawnPlayerAttakParticles(direction);
            }
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
        isShooting = true;
        canShoot = false;
        for (var i = 0; i < shotsPerSeries; i++)
        {
            Shoot();
            yield return new WaitForSeconds(timeBetweenShots);
        }
        lastSeriesTime = Time.time;
        isShooting = false;
    }

    private IEnumerator HitTime()
	{
		isHitted = true;
		//_isInvincible = true;
		yield return new WaitForSeconds(0.1f);
		isHitted = false;
		//_isInvincible = false;
	}

	private IEnumerator DestroyEnemy()
	{
        isInvincible = true;
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
