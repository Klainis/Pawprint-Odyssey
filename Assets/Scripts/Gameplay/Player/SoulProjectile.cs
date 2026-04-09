using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulProjectile : MonoBehaviour
{
    #region Variables

    private Vector2 _direction;
    private float _normalSpeed;
    private float _slowSpeed;
    private float _lifeTime;
    private int _hitAmount;
    private float _hitInterval;
    private int _damage;

    private float _currentSpeed;
    private float _aliveTimer = 0f;

    private List<GameObject> _enemiesInside = new List<GameObject>();
    private Dictionary<GameObject, Coroutine> _damageCoroutines = new Dictionary<GameObject, Coroutine>();

    #endregion

    #region Common Methods

    private void Start()
    {
        _currentSpeed = _normalSpeed;
    }

    private void Update()
    {
        CleanupEnemiesList();

        transform.Translate(_currentSpeed * Time.deltaTime * _direction, Space.World);

        HandleLifeTime();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var groundLayer = LayerMask.NameToLayer("Ground");

        if (collision.CompareTag("Enemy"))
        {
            _enemiesInside.Add(collision.gameObject);
            _currentSpeed = _slowSpeed;

            if (!_damageCoroutines.ContainsKey(collision.gameObject))
            {
                var damageRoutine = StartCoroutine(DealDamageOverTime(collision.gameObject));
                _damageCoroutines.Add(collision.gameObject, damageRoutine);
            }
        }
        else if (collision.gameObject.layer == groundLayer)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            _enemiesInside.Remove(collision.gameObject);

            if (_damageCoroutines.TryGetValue(collision.gameObject, out Coroutine routine))
            {
                StopCoroutine(routine);
                _damageCoroutines.Remove(collision.gameObject);
            }

            if (_enemiesInside.Count == 0)
            {
                _currentSpeed = _normalSpeed;
            }
        }
    }

    #endregion

    private void HandleLifeTime()
    {
        _aliveTimer += Time.deltaTime;
        if (_aliveTimer >= _lifeTime)
        {
            Destroy(gameObject);
        }
    }

    public void SetVariables(
        Vector2 dir, float normalSpeed, float slowSpeed,
        float lifeTime, int sendDamageAmount, float sendDamageInterval, int dmg)
    {
        _direction = dir;
        _normalSpeed = normalSpeed;
        _slowSpeed = slowSpeed;
        _lifeTime = lifeTime;
        _hitAmount = sendDamageAmount;
        _hitInterval = sendDamageInterval;
        _damage = dmg;

        _currentSpeed = _normalSpeed;
    }

    private void CleanupEnemiesList()
    {
        _enemiesInside.RemoveAll(item => item == null);

        if (_enemiesInside.Count == 0)
        {
            _currentSpeed = _normalSpeed;
        }
    }

    private IEnumerator DealDamageOverTime(GameObject enemy)
    {
        var hitsDone = 0;

        while (hitsDone < _hitAmount && enemy != null)
        {
            var damageSide = _direction.x >= 0 ? _damage : -_damage;
            enemy.SendMessage("ApplyDamage", (int)damageSide, SendMessageOptions.DontRequireReceiver);

            hitsDone++;

            yield return new WaitForSeconds(_hitInterval);
        }
    }
}
