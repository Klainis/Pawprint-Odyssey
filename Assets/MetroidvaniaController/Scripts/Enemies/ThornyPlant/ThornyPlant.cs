using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThornyPlant : MonoBehaviour {

    [Header("Основные параметры")]
    [SerializeField] private float life = 10;
    [SerializeField] private float damage = 1f;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private GameObject openForm;
    [SerializeField] private GameObject closeForm;

    [Header("Параметры выстрелов")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed = 7f;

    private Animator animator;
    private Rigidbody2D rb;
    private Transform shootPoints;

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

    public float Damage { get { return damage; } }
    public bool IsShooting { get { return isShooting; } }
    public bool IsHidden { get { return isHidden; } }

    void Awake () {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

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

    public void ChangeForm(bool hide)
    {
        isHidden = hide;
        openForm.SetActive(!hide);
        closeForm.SetActive(hide);

        if (!hide)
            lastOpenTime = Time.time;
    }

    public void ApplyDamage(float damage) {
		if (!isInvincible) 
		{
			//animator.SetBool("Hit", true);
			life -= Mathf.Abs(damage);
            StartCoroutine(HitTime());
        }
	}

	void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
			if (life > 0)
			{
                collision.gameObject.GetComponent<CharacterController2D>().ApplyDamage(damage, transform.position);
            }
        }
    }

    private void Shoot()
    {
        for (var i = 0; i < shootPoints.childCount; i++)
        {
            var shootPoint = shootPoints.GetChild(i);
            var bullet = Instantiate(bulletPrefab, shootPoint.position, shootPoint.rotation, transform);
            rb = bullet.GetComponent<Rigidbody2D>();
            if (rb != null)
                rb.linearVelocity = shootPoint.right * bulletSpeed;
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
		isInvincible = true;
		yield return new WaitForSeconds(0.1f);
		isHitted = false;
		isInvincible = false;
	}

	IEnumerator DestroyEnemy()
	{
        //animator.SetTrigger("Dead");
        gameObject.layer = LayerMask.NameToLayer("DeadEnemy");
        var rotator = new Vector3(transform.rotation.x, transform.rotation.y, -90f);
        transform.rotation = Quaternion.Euler(rotator);
        yield return new WaitForSeconds(0.25f);
		rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
		yield return new WaitForSeconds(1.5f);
		Destroy(gameObject);
	}
}
