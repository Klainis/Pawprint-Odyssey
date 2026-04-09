using UnityEngine;

public class PlayerSoulRelease : MonoBehaviour
{
    #region SerializeFields

    [Header("Parameters")]
    [SerializeField] private float _shootCooldown = 1.0f;
    [SerializeField] private int _manaCost = 5;
    [SerializeField] private Transform shootPoint;

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

    private PlayerMana _playerMana;
    private float _lastShootTime = 0f;

    #endregion

    #region Common Methods

    private void Awake()
    {
        _playerMana = GetComponent<PlayerMana>();
    }

    private void Update()
    {
        HandleShootInput();
    }

    #endregion

    #region Main Logic

    private bool IsCooldownOver()
    {
        return Time.time >= _lastShootTime + _shootCooldown;
    }

    private bool CanShoot()
    {
        var isParrying = PlayerParrying.Instance.IsParrying;
        var isChargingAttack = PlayerChargeAttack.Instance.IsCharging;
        var isClawAttacking = PiercingClaw.Instance.IsAttacking;
        if (isParrying || isChargingAttack || isClawAttacking)
            return false;
        return true;
    }

    private void HandleShootInput()
    {
        if (!IsCooldownOver()) return;
        if (!CanShoot()) return;

        var shootPressed = PlayerInput.Instance.ShootPressed;
        if (shootPressed)
        {
            var direction = PlayerInput.Instance.ShootDirection;
            Shoot(direction);
        }
    }

    private void Shoot(Vector2 direction)
    {
        _lastShootTime = Time.time;
        _playerMana.SpendMana("SoulRelease", _manaCost);

        var projectile = Instantiate(
            _projectilePrefab, shootPoint.position, Quaternion.identity);

        var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        projectile.transform.rotation = Quaternion.Euler(0, 0, angle);

        var script = projectile.GetComponent<SoulProjectile>();
        script.SetVariables(
            direction, _normalSpeed, _slowSpeed, _lifeTime, _hitAmount, _hitInterval, _damage);
    }

    #endregion
}
