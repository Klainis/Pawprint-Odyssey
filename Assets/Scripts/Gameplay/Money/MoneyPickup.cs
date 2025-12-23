using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(CircleCollider2D))]
public class MoneyPickup : MonoBehaviour
{
    [Header("Follow")]
    [SerializeField] private float followDelay = 0.5f;
    [SerializeField] private float followSpeed = 8f;

    [Header("Pickup")]
    [SerializeField] private float pickupDistance = 0.2f;

    private Transform player;
    private Rigidbody2D rb;
    private bool isFollowing;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        Invoke(nameof(StartFollowing), followDelay);
    }

    private void StartFollowing()
    {
        rb.gravityScale = Mathf.Lerp(rb.gravityScale, 0f, 0.2f);
        isFollowing = true;
    }

    private void FixedUpdate()
    {
        if (!isFollowing || player == null) return;

        Vector2 dir = (player.position - transform.position).normalized;
        rb.linearVelocity = dir * followSpeed;

        if (Vector2.Distance(transform.position, player.position) <= pickupDistance)
        {
            Pickup();
        }
    }

    private void Pickup()
    {
        // TODO: добавить деньги игроку
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
