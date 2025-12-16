using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
public class DestructibleWall : MonoBehaviour
{
    //public WallsExistence wallsExistence { get; private set; }

    [Header("Data")]
    [SerializeField] private EnvironmentData _environmentData;

    [Header("Particles")]
    [SerializeField] private ParticleSystem _playerWeaponParticle;

    [Space(5)]
    [SerializeField] private GameObject _lightDoorGameObject;
    [SerializeField] private string entryGate;

    private ParticleSystem _playerWeaponParticleInstance;
    private GameObject _lightDoorGameObjectInstance;
    //private string _currentScene;
    //[SerializeField] private string _currentWall;
    private DestroyBrokenWalls _destroyBrokenWalls;
    private ShakeObjectAfterDamage _shakeObjectAfterDamage;

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
        if (_shakeObjectAfterDamage.shakeDuration > 0)
        {
            _shakeObjectAfterDamage.Shake();
        }

        if (life <= 0)
        {
            Destroy(gameObject);
            InstantiateDoorLight();
            _destroyBrokenWalls.AddInDestroyWallList();
            SaveSystem.AutoSave();
        }
    }

    private void InstantiateDoorLight()
    {
        Quaternion rotaion = Quaternion.identity;
        Vector3 positionOffset = new Vector3(0.4f, 0, 0);
        Vector3 position = Vector3.zero;

        if (entryGate.Contains("right"))
        {
            position = transform.position + positionOffset;
            rotaion = Quaternion.Euler(0, 0, 90);
        }
        else if (entryGate.Contains("left"))
        {
            position = transform.position - positionOffset;
            rotaion = Quaternion.Euler(0, 0, -90);
        }

        _lightDoorGameObjectInstance = Instantiate(_lightDoorGameObject, position, rotaion);
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
