using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SGAttack : MonoBehaviour
{
    [Header("Main params")]
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private GameObject _player;

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

    public event Action OnPlayerDetected;

    private GuardianOwlView _guardianOwlView;

    private Transform pivotTop;
    private Transform pivotBottom;

    private const int maxRamSeriesCount = 3;
    private int ramSeriesCount = 0;
    private float nextLightZoneTime = 0f;
    private bool inLightZone = false;

    //public float AcceleratedSpeed { get { return acceleratedSpeed; } }

    private void Awake()
    {
        _guardianOwlView = GetComponent<GuardianOwlView>();

        pivotTop = transform.Find("PivotTop");
        pivotBottom = transform.Find("PivotBottom");
    }

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
