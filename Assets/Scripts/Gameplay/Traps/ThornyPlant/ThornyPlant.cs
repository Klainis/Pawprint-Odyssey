using System.Collections;
using UnityEngine;

public class ThornyPlant : MonoBehaviour {

    [Header("Основные параметры")]
    [SerializeField] private float life = 10;
    [SerializeField] private int damage = 1;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private GameObject openForm;
    [SerializeField] private GameObject closeForm;

    [Header("Параметры выстрелов")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed = 7f;

    [Header("Particles")]
    [SerializeField] private ParticleSystem damageParticale;
    [SerializeField] private ParticleSystem _playerWeaponParticle;

    private ParticleSystem _damageParticleInstance;
    private ParticleSystem _playerWeaponParticleInstance;

    private Animator animator;
    private Rigidbody2D rigidBody;
    private Transform shootPoints;

    private ScreenShaker _screenShaker;

    private int shotsPerSeries = 3;
    private float timeBetweenShots = 0.35f;
    private float timeBetweenSeries = 2.5f;
    private float lastSeriesTime = 0;
    private float delayAfterOpen = 1.2f;
    private float lastOpenTime = 0;
    private bool canShoot = false;
    private bool isShooting = false;
    private bool isInvincible = false;
    private bool isHitted = false;
    private bool isHidden = true;

    public int Damage { get { return damage; } }
    public bool IsShooting { get { return isShooting; } }
    public bool IsHidden { get { return isHidden; } }

    void Awake () {
        animator = GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody2D>();
        _screenShaker = GetComponent<ScreenShaker>();

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

    public void ChangeForm(bool toHidden)
    {
        isHidden = toHidden;
        openForm.SetActive(!toHidden);
        closeForm.SetActive(toHidden);

        if (!toHidden)
            lastOpenTime = Time.time;
    }

    public void ApplyDamage(int damage) {
		if (!isInvincible) 
		{
            //_animator.SetBool("Hit", true);
            //_damageFlash.CallDamageFlash();
            _screenShaker.Shake();

            var direction = damage / Mathf.Abs(damage);
            SpawnDamageParticles(direction);

            life -= Mathf.Abs(damage);
            StartCoroutine(HitTime());
        }
	}
    private void SpawnDamageParticles(int direction)
    {
        Vector2 vectorDirection = new Vector2(direction, 0);
        Quaternion spawnRotation = Quaternion.FromToRotation(Vector2.right, vectorDirection);
        _damageParticleInstance = Instantiate(damageParticale, transform.position, spawnRotation);
        Quaternion spawnPlayerAttackRotation = Quaternion.FromToRotation(Vector2.right, -vectorDirection);
        _playerWeaponParticleInstance = Instantiate(_playerWeaponParticle, transform.position, spawnPlayerAttackRotation);
    }


    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
			if (life > 0)
                collision.gameObject.GetComponent<PlayerView>().ApplyDamage(damage, transform.position);
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

    IEnumerator ShootRoutine()
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

    IEnumerator HitTime()
	{
		isHitted = true;
		//isInvincible = true;
		yield return new WaitForSeconds(0.1f);
		isHitted = false;
		//isInvincible = false;
	}

	IEnumerator DestroyEnemy()
	{
        isInvincible = true;
        //_animator.SetTrigger("Dead");
        gameObject.layer = LayerMask.NameToLayer("DeadEnemy");
        gameObject.tag = "isDead";
        Vector3 rotator = new Vector3(transform.rotation.x, transform.rotation.y, -90f);
        transform.rotation = Quaternion.Euler(rotator);
        //yield return new WaitForSeconds(0.25f);
        //rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        yield return new WaitForSeconds(3f);
        Destroy(gameObject);
    }
}
