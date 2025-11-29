using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardianOwlAttack : MonoBehaviour
{
    [Header("Main params")]
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask wallLayer;
    private GameObject _player;

    [Header("Particles")]
    [SerializeField] private ParticleSystem _eyeAttack;
    [SerializeField] private ParticleSystem _waveAttack;

    //[Header("Attack 'Ram'")]
    //[SerializeField] private float acceleratedSpeed = 10f;
    //[SerializeField] private float ramTelegraphTime = 0.25f;
    //[SerializeField] private float ramPauseBetweenSeries = 3f;

    //[Header("Attack 'Light Zone'")]
    ////[SerializeField] private GameObject lightZone;
    //[SerializeField] private float lightZoneCooldown = 3f;
    //[SerializeField] private float lightZoneChance = 0.8f;
    //[SerializeField] private float lightZoneTime = 1f;
    //[SerializeField] private float lightZoneTelegraphTime = 0.5f;

    private ParticleSystem _eyeAttackInstance;
    private ParticleSystem _waveAttackInstance;

    private ParticleSystem.Particle[] _eyeAttackParticles;
    private ParticleSystem.Particle[] _waveAttackParticles;

    public event Action OnPlayerDetected;

    private GuardianOwlView _guardianOwlView;

    private Transform pivotTop;
    private Transform pivotBottom;
    private BoxCollider2D _colliderOfEyeAttack;

    private const int maxRamSeriesCount = 3;
    private int ramSeriesCount = 0;
    private float nextLightZoneTime = 0f;
    private bool inLightZone = false;

    //public float AcceleratedSpeed { get { return acceleratedSpeed; } }

    private void Awake()
    {
        _guardianOwlView = GetComponent<GuardianOwlView>();
        _player = InitializeManager._instance.player;

        pivotTop = transform.Find("PivotTop");
        pivotBottom = transform.Find("PivotBottom");
    }

    #region Eye Attack
    public void EyeOwlAttack()
    {
        _eyeAttackInstance = Instantiate(_eyeAttack, _player.transform.position, new Quaternion(0, 0, 0, 0));
        _colliderOfEyeAttack = _eyeAttackInstance.gameObject.GetComponent<BoxCollider2D>();
        _eyeAttackParticles = new ParticleSystem.Particle[_eyeAttackInstance.main.maxParticles];

        StartCoroutine(EnableCollisionEyeAttack());
    }

    public void EyeAttackCoroutine(int amount)
    {
        StartCoroutine(SpawnEyeAttack(amount));
    }

    private IEnumerator EnableCollisionEyeAttack()
    {
        yield return new WaitForEndOfFrame();

        int partCount = _eyeAttackInstance.GetParticles(_eyeAttackParticles);
        Debug.Log($"ÂÛÇÂÀÍÀ ÊÎÐÓÒÈÍÀ ÂÊËÞ×ÅÍÈß ÊÎËËÀÉÄÅÐÀ, ïàðòèêëîâ:{partCount}");
        while (_eyeAttackInstance != null && partCount > 0)
        {
            if (partCount > 0)
            {
                partCount = _eyeAttackInstance.GetParticles(_eyeAttackParticles);

                var p = _eyeAttackParticles[0];

                var sizeOverLifetime = p.startLifetime;
                var remainingLifetime = p.remainingLifetime;

                float t = 1f - (remainingLifetime / sizeOverLifetime);
                float currentSizeValue = _eyeAttackInstance.sizeOverLifetime.size.Evaluate(t);

                if (currentSizeValue < 0.9)
                {
                    Debug.Log($"Êîëëàéäåð âûêëþ÷åí {t}");
                    _colliderOfEyeAttack.enabled = false;
                }
                if (currentSizeValue >= 0.9)
                {
                    Debug.Log($"Êîëëàéäåð âêëþ÷åí {t}");
                    _colliderOfEyeAttack.enabled = true;
                }
            }
            yield return null;
        }
    }

    private IEnumerator SpawnEyeAttack(int attackTime)
    {
        while (attackTime > 0)
        {
            yield return new WaitForSeconds(2);
            EyeOwlAttack();
            attackTime--;
        }
        yield return null;
    }
    #endregion

    public void RandomAttack(bool facingRight)
    {
        var wallHits = GetWallHits(6);
        var playerHitsSecondStage = GetPlayerHits(5, facingRight);
        //if (wallHits[0].collider == null && wallHits[1].collider == null && Time.time >= nextLightZoneTime)
        //{
        //    if (UnityEngine.Random.value <= lightZoneChance
        //        && (playerHitsSecondStage[0].collider != null || playerHitsSecondStage[1].collider != null))
        //    {
        //        StartCoroutine(LightZoneRoutine());
        //        nextLightZoneTime = Time.time + lightZoneCooldown;
        //    }
        //    else if (!inLightZone)
        //    {
        //        StartCoroutine(RamTelegraph());
        //        _guardianOwlView.IsAccelerated = true;
        //    }
        //}
    }

    public void RamAttack(List<RaycastHit2D> playerHits)
    {
        //for (var i = 0; i < playerHits.Count; i++)
        //{
        //    if (playerHits[i].collider != null)
        //    {
        //        StartCoroutine(RamTelegraph());
        //        _guardianOwlView.IsAccelerated = true;
        //        ramSeriesCount += 1;
        //        break;
        //    }
        //}
    }

    //public void CheckRamSeriesCountAndPause()
    //{
    //    if (ramSeriesCount == maxRamSeriesCount)
    //    {
    //        ramSeriesCount = 0;
    //        StartCoroutine(RamPause());
    //    }
    //}

    public List<RaycastHit2D> GetPlayerHits(float distance, bool facingRight)
    {
        var direction = facingRight ? Vector2.left : Vector2.right;
        var playerHitTop = Physics2D.Raycast(pivotTop.position, direction, distance, playerLayer);
        var playerHitBottom = Physics2D.Raycast(pivotBottom.position, direction, distance, playerLayer);
        return new List<RaycastHit2D> { playerHitTop, playerHitBottom };
    }

    public List<RaycastHit2D> GetWallHits(float distance)
    {
        var wallHitLeft = Physics2D.Raycast(pivotBottom.position, Vector2.left, distance, wallLayer);
        var wallHitRight = Physics2D.Raycast(pivotBottom.position, Vector2.right, distance, wallLayer);
        return new List<RaycastHit2D> { wallHitLeft, wallHitRight };
    }

    #region Body Attack
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (!_guardianOwlView.Model.IsDead)
            {
                var playerView = collision.gameObject.GetComponent<PlayerView>();
                playerView.ApplyDamage(_guardianOwlView.Model.Damage, transform.position);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (!_guardianOwlView.Model.IsDead)
            {
                var playerView = collision.gameObject.GetComponent<PlayerView>();
                playerView.ApplyDamage(_guardianOwlView.Model.Damage, transform.position);
            }
        }
    }
    #endregion

    //private IEnumerator RamTelegraph()
    //{
    //    // _bugAnimation.SetBoolRamTelegraph(true);
    //    var renderer = GetComponent<SpriteRenderer>();
    //    var normalColor = renderer.color;
    //    renderer.color = UnityEngine.Color.red;

    //    var normalConstraints = _guardianOwlView.RigidBody.constraints;
    //    _guardianOwlView.RigidBody.constraints = RigidbodyConstraints2D.FreezePosition;
    //    yield return new WaitForSeconds(ramTelegraphTime);
    //    _guardianOwlView.RigidBody.constraints = normalConstraints;

    //    renderer.color = normalColor;
    //}

    //private IEnumerator RamPause()
    //{
    //    //_guardianOwlView.MoveDisabled = true;

    //    var normalConstraints = _guardianOwlView.RigidBody.constraints;
    //    _guardianOwlView.RigidBody.constraints = RigidbodyConstraints2D.FreezePosition;
    //    yield return new WaitForSeconds(ramPauseBetweenSeries);
    //    _guardianOwlView.RigidBody.constraints = normalConstraints;

    //    //_guardianOwlView.MoveDisabled = false;
    //}

    //private IEnumerator LightZoneRoutine()
    //{
    //    //_animator.SetBool("LightZoneTelegraph", true);
    //    var renderer = GetComponent<SpriteRenderer>();
    //    var normalColor = renderer.color;
    //    renderer.color = UnityEngine.Color.lightYellow;

    //    inLightZone = true;
    //    var normalConstraints = _guardianOwlView.RigidBody.constraints;
    //    _guardianOwlView.RigidBody.constraints = RigidbodyConstraints2D.FreezePosition;
    //    yield return new WaitForSeconds(lightZoneTelegraphTime);
    //    lightZone.SetActive(true);
    //    yield return new WaitForSeconds(lightZoneTime);
    //    lightZone.SetActive(false);
    //    _guardianOwlView.RigidBody.constraints = normalConstraints;
    //    inLightZone = false;

    //    renderer.color = normalColor;
    //}
}
