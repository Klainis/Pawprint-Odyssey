using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
public class DestructibleWall : MonoBehaviour
{
    //public WallsExistence wallsExistence { get; private set; }

    [Header("Data")]
    [SerializeField] private EnvironmentData environmentData;

    [Space(5)]
    private ShakeObjectAfterDamage shakeObjectAfterDamage;

    [Space(5)]
    private DestroyBrokenWalls _destroyBrokenWalls;

    [Header("Particles")]
    [SerializeField] private ParticleSystem _playerWeaponParticle;

    private ParticleSystem _playerWeaponParticleInstance;
    //private string _currentScene;
    //[SerializeField] private string _currentWall;

    private int life;

    private void Awake()
    {
        shakeObjectAfterDamage = GetComponent<ShakeObjectAfterDamage>();
        _destroyBrokenWalls = GetComponent<DestroyBrokenWalls>();
    }

    private void Start()
    {
        life = environmentData.wallLife;
    }

    private void Update()
    {
        if (life <= 0)
        {
            shakeObjectAfterDamage.Shake();
            Destroy(gameObject);
            _destroyBrokenWalls.AddInDestroyWallList();
        }
        else if (shakeObjectAfterDamage.shakeDuration > 0)
        {
            shakeObjectAfterDamage.Shake();
        }
    }

    public void ApplyDamage(bool isClaw, int damage)
    {
        var direction = damage / Mathf.Abs(damage);

        if (isClaw)
        {
            life -= 9999;
            shakeObjectAfterDamage.shakeDuration = environmentData.shakeDuration;
        }
        else
        {
            life -= 1;
            SpawnDamageParticles(direction);
            shakeObjectAfterDamage.shakeDuration = environmentData.shakeDuration;
        };
    }

    private void SpawnDamageParticles(int direction)
    {
        Vector2 vectorDirection = new Vector2(direction, 0);
        Quaternion spawnPlayerAttackRotation = Quaternion.FromToRotation(Vector2.right, -vectorDirection);
        _playerWeaponParticleInstance = Instantiate(_playerWeaponParticle, transform.position, spawnPlayerAttackRotation);
    }
}
