using Unity.InferenceEngine;
using UnityEngine;

public class MoneyObject : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private EnvironmentData environmentData;
    [Space(5)]
    private ShakeObject shakeObjectAfterDamage;

    [Header("Particles")]
    [SerializeField] private ParticleSystem _playerWeaponParticle;

    private InstantiateMoney _money;
    private ParticleSystem _playerWeaponParticleInstance;
    private DestroyBrokenMoneyObject _destroyBrokenMoneyObject;
    private int life;

    private void Awake()
    {
        _destroyBrokenMoneyObject = GetComponent<DestroyBrokenMoneyObject>();
        shakeObjectAfterDamage = GetComponent<ShakeObject>();
        _money = GetComponent<InstantiateMoney>();
    }

    private void Start()
    {
        life = environmentData.moneyObjectLife;
    }

    private void Update()
    {
        if (life <= 0)
        {
            shakeObjectAfterDamage.Shake();
            Destroy(gameObject);

            //_money.SetReward(environmentData.moneyObjectReward);
            _money.InstantiateMon(transform.position, 5/*environmentData.moneyObjectReward*/);

            _destroyBrokenMoneyObject.AddInDestroyMoneyObjectList();
            SaveSystem.MoneySave();
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
