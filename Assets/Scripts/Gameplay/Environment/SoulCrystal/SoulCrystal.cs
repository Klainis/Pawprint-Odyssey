using UnityEngine;
using UnityEngine.Events;

public class SoulCrystal : MonoBehaviour
{
    [Header("Data")] 
    [SerializeField] private EnvironmentData environmentData;
    [Space(5)]
    private ShakeObjectAfterDamage shakeObjectAfterDamage;

    [Header("Particles")]
    [SerializeField] private ParticleSystem _playerWeaponParticle;

    private ParticleSystem _playerWeaponParticleInstance;

    private int life;

    [SerializeField] private UnityEvent crystalCountEvent;

    private void Awake()
    {
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
            crystalCountEvent.Invoke();
            Destroy(gameObject);
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
        Vector2 vectorDirection = new Vector2(direction, 0);
        Quaternion spawnPlayerAttackRotation = Quaternion.FromToRotation(Vector2.right, -vectorDirection);
        _playerWeaponParticleInstance = Instantiate(_playerWeaponParticle, transform.position, spawnPlayerAttackRotation);
    }

}
