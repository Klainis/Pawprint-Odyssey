using UnityEngine;

public class SoulCrystal : MonoBehaviour
{
    [Header("Data")] 
    [SerializeField] private EnvironmentData environmentData;
    [Space(5)]
    private ShakeObjectAfterDamage shakeObjectAfterDamage;

    [Header("Particles")]
    [SerializeField] private ParticleSystem _playerWeaponParticle;

    private SoulCrystalCounter _crystalCounter;
    private ParticleSystem _playerWeaponParticleInstance;
    private DestroyBrokenCrystals _destroyBrokenCrystals;
    private int life;

    private void Awake()
    {
        _crystalCounter = PlayerView.Instance.GetComponent<SoulCrystalCounter>();
        _destroyBrokenCrystals = GetComponent<DestroyBrokenCrystals>();
        shakeObjectAfterDamage = GetComponent<ShakeObjectAfterDamage>();
    }

    private void Start()
    {
        life = environmentData.crystalLife;
    }

    private void Update()
    {
        if (life <= 0)
        {
            shakeObjectAfterDamage.Shake();
            _crystalCounter.CountCrystal();
            Destroy(gameObject);
            _destroyBrokenCrystals.AddInDestroyCrystalList();
        }
        else if (shakeObjectAfterDamage.shakeDuration > 0)
        {
            shakeObjectAfterDamage.Shake();
        }
    }

    public void ApplyDamage(int damage)
    {
        var direction = damage / Mathf.Abs(damage);
        SpawnDamageParticles(direction);
        life -= 1;
        shakeObjectAfterDamage.shakeDuration = environmentData.shakeDuration;
    }

    private void SpawnDamageParticles(int direction)
    {
        var vectorDirection = new Vector2(direction, 0);
        var spawnPlayerAttackRotation = Quaternion.FromToRotation(Vector2.right, -vectorDirection);
        _playerWeaponParticleInstance = Instantiate(_playerWeaponParticle, transform.position, spawnPlayerAttackRotation);
    }
}
