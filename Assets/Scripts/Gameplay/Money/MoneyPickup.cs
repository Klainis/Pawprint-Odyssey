using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(CircleCollider2D))]
public class MoneyPickup : MonoBehaviour
{
    [Header("Follow")]
    [SerializeField] private float followDelay = 0.5f;
    [SerializeField] private float followSpeed = 8f;

    [Header("Pickup")]
    [SerializeField] private float pickupDistance = 0.2f;

    private MoneyCounter _moneyCounter;

    private Transform _player;
    private Rigidbody2D _rb;
    private bool _isFollowing;

    public int reward;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        _moneyCounter = _player.GetComponent<MoneyCounter>();
    }

    private void Start()
    {
        Invoke(nameof(StartFollowing), followDelay);
    }

    private void FixedUpdate()
    {
        if (!_isFollowing || _player == null) return;

        Vector2 dir = (_player.position - transform.position).normalized;
        _rb.linearVelocity = dir * followSpeed;
    }

    private void StartFollowing()
    {
        _rb.gravityScale = Mathf.Lerp(_rb.gravityScale, 0f, 0.2f);
        StartCoroutine(WaitFollowing());
    }

    private IEnumerator WaitFollowing()
    {
        yield return new WaitForSeconds(0.2f);
        _isFollowing = true;
    }

    private void Pickup()
    {
        _moneyCounter.CountMoney(reward);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Pickup();
        }
    }
}
