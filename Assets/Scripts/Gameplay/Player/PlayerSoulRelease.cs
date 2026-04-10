using System.Collections;
using UnityEngine;

public class PlayerSoulRelease : MonoBehaviour
{
    #region SerializeFields

    [Header("Parameters")]
    [SerializeField] private float _shootCooldown = 1.0f;
    [SerializeField] private int _manaCost = 5;
    [SerializeField] private Transform _shootPoint;

    [Header("Projectile")]
    [SerializeField] private GameObject _projectilePrefab;
    [SerializeField] private float _normalSpeed = 15.0f;
    [SerializeField] private float _slowSpeed = 7.5f;
    [SerializeField] private float _lifeTime = 3.0f;
    [SerializeField] private int _hitAmount = 3;
    [SerializeField] private float _hitInterval = 0.3f;
    [SerializeField] private int _damage = 1;

    #endregion

    #region Variables

    private readonly Vector2 _shootPointOffset = new(1f, 1.35f);

    private Rigidbody2D _rigidbody;
    private PlayerAnimation _playerAnimation;
    private PlayerMana _playerMana;
    private float _lastShootTime = 0f;
    private float _startGravityScale;

    public bool _isStop { get; set; } = false;

    #endregion

    #region Common Methods

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _playerAnimation = GetComponent<PlayerAnimation>();
        _playerMana = GetComponent<PlayerMana>();
    }

    private void Start()
    {
        _startGravityScale = _rigidbody.gravityScale;
    }

    private void Update()
    {
        if (_isStop)
        {
            _rigidbody.linearVelocity = Vector2.zero;
            _rigidbody.gravityScale = 0f;
        }

        HandleShootInput();
    }

    #endregion

    #region Main Logic

    private void HandleShootInput()
    {
        var shootPressed = PlayerInput.Instance.ShootPressed;
        if (!shootPressed) return;

        if (!IsCooldownOver()) return;
        if (!CanShoot()) return;

        var direction = PlayerInput.Instance.ShootDirection;
        StartCoroutine(ShootRoutine(direction));
    }

    private bool IsCooldownOver()
    {
        return Time.time >= _lastShootTime + _shootCooldown;
    }

    private bool CanShoot()
    {
        var isParrying = PlayerParrying.Instance.IsParrying;
        var isChargingAttack = PlayerChargeAttack.Instance.IsCharging;
        var isClawAttacking = PiercingClaw.Instance.IsAttacking;
        var hasEnoughMana = PlayerView.Instance.PlayerModel.Mana >= _manaCost;

        return !isParrying && !isChargingAttack && !isClawAttacking && hasEnoughMana;
    }

    private void SpawnProjectile(Vector2 direction)
    {
        var instantiatePos = (Vector2)_shootPoint.position + direction * _shootPointOffset;
        var projectile = Instantiate(_projectilePrefab, instantiatePos, Quaternion.identity);

        var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        projectile.transform.rotation = Quaternion.Euler(0, 0, angle);

        var script = projectile.GetComponent<SoulProjectile>();
        script.SetVariables(direction, _normalSpeed, _slowSpeed, _lifeTime, _hitAmount, _hitInterval, _damage);
    }

    private IEnumerator ShootRoutine(Vector2 direction)
    {
        _lastShootTime = Time.time;
        PlayerMove.Instance.CanMove = false;
        _playerAnimation.SetBoolSoulRelease(true);
        _playerMana.SpendMana("SoulRelease", _manaCost);
        yield return StartCoroutine(SmoothStopRoutine(0.2f));

        SpawnProjectile(direction);

        yield return new WaitForSeconds(0.3f);
        _playerAnimation.SetBoolSoulRelease(false);
        yield return new WaitForSeconds(0.25f);
        _isStop = false;
        PlayerMove.Instance.CanMove = true;
    }

    private IEnumerator SmoothStopRoutine(float duration)
    {
        if (PlayerMove.Instance.IsJumping)
        {
            duration = 0;
        }

        var velocityX = _rigidbody.linearVelocity.x;
        var currentVelocity = 0f;
        
        var elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            velocityX = Mathf.SmoothDamp(velocityX, 0, ref currentVelocity, duration);
            _rigidbody.linearVelocity = new Vector2(velocityX, _rigidbody.linearVelocity.y);
            yield return null;
        }

        _isStop = true;
        _rigidbody.linearVelocity = new Vector2(0, 0);
        _rigidbody.gravityScale = 0;

    }

    #endregion
}
