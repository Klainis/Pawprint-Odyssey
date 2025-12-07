using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
public class DestructibleWall : MonoBehaviour
{
    //public WallsExistence wallsExistence { get; private set; }

    [Header("Data")]
    [SerializeField] private EnvironmentData _environmentData;

    [Space(5)]
    private ShakeObjectAfterDamage _shakeObjectAfterDamage;

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
        _shakeObjectAfterDamage = GetComponent<ShakeObjectAfterDamage>();
        _destroyBrokenWalls = GetComponent<DestroyBrokenWalls>();
    }

    private void Start()
    {
        life = _environmentData.wallLife;
    }

    private void Update()
    {
        if (life <= 0)
        {
            _shakeObjectAfterDamage.Shake();
            Destroy(gameObject);
            _destroyBrokenWalls.AddInDestroyWallList();
        }
        else if (_shakeObjectAfterDamage.shakeDuration > 0)
        {
            _shakeObjectAfterDamage.Shake();
        }
    }

    public void ApplyDamage(object[] message)
    {
        var isClaw = (bool)message[0];
        var damage = (int)message[1];

        var direction = damage / Mathf.Abs(damage);

        if (isClaw)
        {
            Debug.Log("Ударили по стене когтем");
            _shakeObjectAfterDamage.shakeDuration = _environmentData.shakeDuration;
            life -= 9999;
        }
        else
        {
            _shakeObjectAfterDamage.shakeDuration = _environmentData.shakeDuration;
            life -= 1;
            SpawnDamageParticles(direction);
        };
    }

    private void SpawnDamageParticles(int direction)
    {
        Vector2 vectorDirection = new Vector2(direction, 0);
        Quaternion spawnPlayerAttackRotation = Quaternion.FromToRotation(Vector2.right, -vectorDirection);
        _playerWeaponParticleInstance = Instantiate(_playerWeaponParticle, transform.position, spawnPlayerAttackRotation);
    }
}
