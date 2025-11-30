using Newtonsoft.Json.Bson;
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

    [Space(5)]
    [SerializeField] private float _waveVelocity = 7;
    [SerializeField] private float _eyeAttackSpeedModifier = 0.6f;

    private ParticleSystem _eyeAttackInstance;
    private ParticleSystem _waveAttackInstance;

    private ParticleSystem.Particle[] _eyeAttackParticles;
    private ParticleSystem.Particle[] _waveAttackParticles;

    private GuardianOwlView _guardianOwlView;

    private Transform pivotTop;
    private Transform pivotBottom;
    private BoxCollider2D _colliderOfEyeAttack;
    private BoxCollider2D _colliderOfWaveAttack;

    private Vector3 _targetWavePosition;

    private float _realSpeedModifier = 1f;

    private void Awake()
    {
        _guardianOwlView = GetComponent<GuardianOwlView>();
        _player = InitializeManager._instance.player;
    }

    #region Wave Attack
    private void InstantiateWaveAttack()
    {
        var playerFromOwl = transform.position.x - _player.transform.position.x;

        if (playerFromOwl > 0)//left wave
        {
            Vector3 particlePosition = new Vector3(transform.position.x - transform.localScale.x, transform.position.y, transform.position.z);
            _waveAttackInstance = Instantiate(_waveAttack, particlePosition, new Quaternion(0, 0, 0, 0));

            //Ñìåùåíèå âçÿòî èç LifeTime ïàðòèêëà * íà Speed
            _targetWavePosition = new Vector3(_waveAttackInstance.transform.position.x - 14, 
                _waveAttackInstance.transform.position.y,
                _waveAttackInstance.transform.position.z);
        }
        else if (playerFromOwl <= 0)//right wave
        {
            Vector3 particlePosition = new Vector3(transform.position.x + transform.localScale.x, transform.position.y, transform.position.z);
            _waveAttackInstance = Instantiate(_waveAttack, particlePosition, new Quaternion(0, 0, 0, 0));
            var main = _waveAttackInstance.main;
            main.startRotation = 180f * Mathf.Deg2Rad;

            _targetWavePosition = new Vector3(_waveAttackInstance.transform.position.x + 14,
                _waveAttackInstance.transform.position.y,
                _waveAttackInstance.transform.position.z);
        }

        _colliderOfWaveAttack = _waveAttackInstance.gameObject.GetComponent<BoxCollider2D>();
        _waveAttackParticles = new ParticleSystem.Particle[_waveAttackInstance.main.maxParticles];
        StartCoroutine(CollisionWaveAttack());
        StartCoroutine(VelocityOfWaveAttack());
    }

    private IEnumerator VelocityOfWaveAttack()
    {
        if (_waveAttackInstance != null)
        {
            while (Vector3.Distance(_waveAttackInstance.transform.position, _targetWavePosition) > 0.01f)
            {
                if (_waveAttackInstance == null)
                    yield return null;

                _waveAttackInstance.transform.position = Vector3.MoveTowards(
                    _waveAttackInstance.transform.position,
                    _targetWavePosition,
                    _waveVelocity * Time.deltaTime);
                yield return null;
            }
        }
    }

    private IEnumerator CollisionWaveAttack()
    {
        yield return new WaitForEndOfFrame();

        int partCount = _waveAttackInstance.GetParticles(_waveAttackParticles);
        //Debug.Log($"ÂÛÇÂÀÍÀ ÊÎÐÓÒÈÍÀ ÂÊËÞ×ÅÍÈß ÊÎËËÀÉÄÅÐÀ, ïàðòèêëîâ:{partCount}");
        while (_waveAttackInstance != null && partCount > 0)
        {
            if (partCount > 0)
            {
                partCount = _waveAttackInstance.GetParticles(_waveAttackParticles);

                var p = _waveAttackParticles[0];

                var remainingLifetime = p.remainingLifetime;

                if (remainingLifetime <= 0.2f)
                {
                    _colliderOfWaveAttack.enabled = false;
                }
            }
            yield return null;
        }
    }

    public IEnumerator SpawnWaveAttack(int attackCount)
    {
        while (attackCount > 0)
        {
            yield return new WaitForSeconds(1.5f);
            InstantiateWaveAttack();
            attackCount--;
        }
        yield return null;
    }
    #endregion

    #region Eye Attack
    private void InstantiateEyeOwlAttack()
    {
        _eyeAttackInstance = Instantiate(_eyeAttack, _player.transform.position, new Quaternion(0, 0, 0, 0));
        _colliderOfEyeAttack = _eyeAttackInstance.gameObject.GetComponent<BoxCollider2D>();
        _eyeAttackParticles = new ParticleSystem.Particle[_eyeAttackInstance.main.maxParticles];

        StartCoroutine(CollisionEyeAttack());
    }

    public void ApplyEyeAtackSpeedModifier()
    {
        _realSpeedModifier = _eyeAttackSpeedModifier;
    }

    private IEnumerator CollisionEyeAttack()
    {
        yield return new WaitForEndOfFrame();

        int partCount = _eyeAttackInstance.GetParticles(_eyeAttackParticles);
        //Debug.Log($"ÂÛÇÂÀÍÀ ÊÎÐÓÒÈÍÀ ÂÊËÞ×ÅÍÈß ÊÎËËÀÉÄÅÐÀ, ïàðòèêëîâ:{partCount}");
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

                if (currentSizeValue < 0.99)
                {
                    //Debug.Log($"Êîëëàéäåð âûêëþ÷åí {t}");
                    _colliderOfEyeAttack.enabled = false;
                }
                if (currentSizeValue >= 0.9)
                {
                    //Debug.Log($"Êîëëàéäåð âêëþ÷åí {t}");
                    _colliderOfEyeAttack.enabled = true;
                }
            }
            yield return null;
        }
    }

    public IEnumerator SpawnEyeAttack(int attackTime)
    {
        while (attackTime > 0)
        {
            yield return new WaitForSeconds(1.2f * _realSpeedModifier);
            InstantiateEyeOwlAttack();
            attackTime--;
        }
        yield return null;
    }
    #endregion

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
}
