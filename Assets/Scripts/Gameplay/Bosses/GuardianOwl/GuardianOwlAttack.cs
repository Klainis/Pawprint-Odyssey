using GlobalEnums;
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
    private CapsuleCollider2D _bossCollider;

    private Vector3 _targetWavePosition;

    private float _realSpeedModifier = 1f;

    private void Awake()
    {
        _guardianOwlView = GetComponent<GuardianOwlView>();
        _player = InitializeManager.Instance.player;
        _bossCollider = GetComponent<CapsuleCollider2D>();
    }

    #region Wave Attack
    private void InstantiateWaveAttack()
    {
        if (_guardianOwlView.Model.IsDead) return;


        Debug.Log("Начали создавать волны");
        var playerFromOwl = transform.position.x - _player.transform.position.x;

        if (playerFromOwl > 0)//left wave
        {
            Vector3 particlePosition = new Vector3(transform.position.x - _bossCollider.size.x / 2, transform.position.y, transform.position.z);
            _waveAttackInstance = Instantiate(_waveAttack, particlePosition, new Quaternion(0, 0, 0, 0));

            //Смещение взято из LifeTime партикла * на Speed
            _targetWavePosition = new Vector3(_waveAttackInstance.transform.position.x - 14, 
                _waveAttackInstance.transform.position.y,
                _waveAttackInstance.transform.position.z);
        }
        else if (playerFromOwl <= 0)//right wave
        {
            Vector3 particlePosition = new Vector3(transform.position.x + _bossCollider.size.x / 2, transform.position.y, transform.position.z);
            _waveAttackInstance = Instantiate(_waveAttack, particlePosition, new Quaternion(0, 0, 0, 0));
            var main = _waveAttackInstance.main;
            main.startRotation = 180f * Mathf.Deg2Rad;

            _targetWavePosition = new Vector3(_waveAttackInstance.transform.position.x + 14,
                _waveAttackInstance.transform.position.y,
                _waveAttackInstance.transform.position.z);
        }

        if (_guardianOwlView.Model.IsDead)
        {
            Destroy(_waveAttackInstance);
        }

        StartCoroutine(CollisionWaveAttack(_waveAttackInstance));
        StartCoroutine(VelocityOfWaveAttack(_waveAttackInstance, _targetWavePosition));
    }

    private IEnumerator VelocityOfWaveAttack(ParticleSystem waveInstance, Vector3 targetPosition)
    {
        ParticleSystem localWaveInstance = waveInstance;

        if (localWaveInstance != null)
        {
            while (Vector3.Distance(localWaveInstance.transform.position, targetPosition) > 0.01f)
            {
                if (localWaveInstance == null)
                    yield return null;

                localWaveInstance.transform.position = Vector3.MoveTowards(
                    localWaveInstance.transform.position,
                    targetPosition,
                    _waveVelocity * Time.deltaTime);
                yield return null;
            }
        }
    }

    private IEnumerator CollisionWaveAttack(ParticleSystem waveInstance)
    {
        yield return new WaitForEndOfFrame();

        BoxCollider2D collider = waveInstance.gameObject.GetComponent<BoxCollider2D>();
        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[waveInstance.main.maxParticles];

        while (waveInstance != null)
        {
            int partCount = waveInstance.GetParticles(particles);
            if (partCount > 0)
            {
                var p = particles[0];
                if (p.remainingLifetime <= 0.2f)
                {
                    collider.enabled = false;
                    break;
                }
            }
            yield return null;
        }
    }

    public IEnumerator SpawnWaveAttack(int attackCount, BossStage stage)
    {
        Debug.Log("Начали создавать волны");
        while (attackCount > 0)
        {
            if (_guardianOwlView.ShouldInterrupt(stage) || _guardianOwlView.Model.IsDead)
            {
                break;
            }

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
        if (_guardianOwlView.Model.IsDead) return;


        _eyeAttackInstance = Instantiate(_eyeAttack, _player.transform.position, new Quaternion(0, 0, 0, 0));
        _colliderOfEyeAttack = _eyeAttackInstance.gameObject.GetComponent<BoxCollider2D>();
        _eyeAttackParticles = new ParticleSystem.Particle[_eyeAttackInstance.main.maxParticles];

        if (_guardianOwlView.Model.IsDead)
        {
            Destroy(_eyeAttackInstance);
        }

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
        //Debug.Log($"ВЫЗВАНА КОРУТИНА ВКЛЮЧЕНИЯ КОЛЛАЙДЕРА, партиклов:{partCount}");
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
                    //Debug.Log($"Коллайдер выключен {t}");
                    _colliderOfEyeAttack.enabled = false;
                }
                if (currentSizeValue >= 0.9)
                {
                    //Debug.Log($"Коллайдер включен {t}");
                    _colliderOfEyeAttack.enabled = true;
                }
            }
            yield return null;
        }
    }

    public IEnumerator SpawnEyeAttack(int attackTime, BossStage stage)
    {
        while (attackTime > 0)
        {
            if (_guardianOwlView.ShouldInterrupt(stage) || _guardianOwlView.Model.IsDead)
            {
                break;
            }

            yield return new WaitForSeconds(1.2f * _realSpeedModifier);
            InstantiateEyeOwlAttack();
            attackTime--;
        }
        yield return null;
    }
    #endregion

    #region Body Attack
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (!_guardianOwlView.Model.IsDead)
            {
                var playerView = collision.gameObject.GetComponent<PlayerView>();
                playerView.ApplyDamage(_guardianOwlView.Model.Damage, transform.position, gameObject);
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
                playerView.ApplyDamage(_guardianOwlView.Model.Damage, transform.position, gameObject);
            }
        }
    }
    #endregion
}
