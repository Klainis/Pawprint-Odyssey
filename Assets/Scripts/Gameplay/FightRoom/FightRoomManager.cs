using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FightRoomManager : MonoBehaviour
{
    #region SerializeFields

    [Header("Player")]
    [SerializeField] private float _freezePlayerTime = 1.5f;

    [Header("Particle")]
    [SerializeField] private float _particleLifetime = 2.5f;
    [SerializeField] private GameObject _particle;

    [Header("Spawn Positions")]
    [SerializeField] private Transform[] _groundEnemyPositions;
    [SerializeField] private Transform[] _airEnemyPositions;

    [Header("Waves")]
    [SerializeField] private List<Wave> _waves;

    #endregion

    #region Variables

    private int _currentWaveIndex = 0;
    private string _roomName;
    private readonly List<IEnemy> _aliveEnemies = new();
    private FightDoor _fightDoor;

    #endregion

    #region Common Methods

    private void Start()
    {
        _roomName = SceneManager.GetActiveScene().name;
        _fightDoor = FindAnyObjectByType<FightDoor>();

        if (!PlayerView.Instance.PlayerModel.CompletedFightRooms.Contains(_roomName))
        {
            StartFight();
        }
    }

    #endregion

    private void StartFight()
    {
        StartCoroutine(StartFightRoutine());
    }

    private void StartWave()
    {
        if (_currentWaveIndex >= _waves.Count)
        {
            CompleteRoom();
            return;
        }

        var wave = _waves[_currentWaveIndex];

        _aliveEnemies.Clear();

        SpawnGroundEnemies(wave);
        SpawnAirEnemies(wave);
    }

    private void CompleteRoom()
    {
        PlayerView.Instance.PlayerModel.CompletedFightRooms.Add(_roomName);
        OpenDoors();
    }

    private void HandleEnemyDeath(IEnemy enemy)
    {
        enemy.OnDeath -= HandleEnemyDeath;

        _aliveEnemies.Remove(enemy);

        if (_aliveEnemies.Count == 0)
        {
            _currentWaveIndex++;
            StartWave();
        }
    }

    #region Spawn Enemies

    private void SpawnGroundEnemies(Wave wave)
    {
        for (int i = 0; i < _groundEnemyPositions.Length; i++)
        {
            if (wave.groundEnemies.Count == 0) return;

            var prefab = wave.groundEnemies[UnityEngine.Random.Range(0, wave.groundEnemies.Count)];
            var pos = (Vector2)_groundEnemyPositions[i].position;

            StartCoroutine(SpawnEnemyRoutine(prefab, pos));
        }
    }

    private void SpawnAirEnemies(Wave wave)
    {
        for (int i = 0; i < _airEnemyPositions.Length; i++)
        {
            if (wave.airEnemies.Count == 0) return;

            var prefab = wave.airEnemies[UnityEngine.Random.Range(0, wave.airEnemies.Count)];
            var pos = (Vector2)_airEnemyPositions[i].position;

            StartCoroutine(SpawnEnemyRoutine(prefab, pos));
        }
    }

    private void SpawnEnemy(GameObject prefab, Vector2 position)
    {
        var enemyObject = Instantiate(prefab, position, Quaternion.identity);
        var enemy = enemyObject.GetComponent<IEnemy>();

        _aliveEnemies.Add(enemy);

        enemy.OnDeath += HandleEnemyDeath;
    }

    #endregion

    #region Doors

    private void CloseDoors()
    {
        //_fightDoor.CloseDoor(true);
    }

    private void OpenDoors()
    {
        //_fightDoor.CloseDoor(false);
    }

    #endregion

    #region IEnumerators

    private IEnumerator StartFightRoutine()
    {
        while (!PlayerMove.Instance.IsGrounded)
            yield return null;

        CloseDoors();
        PlayerView.Instance.StopPlayer();
        PlayerView.Instance.FreezePlayer(true);

        yield return new WaitForSeconds(_freezePlayerTime);

        PlayerView.Instance.FreezePlayer(false);
        StartWave();
    }

    private IEnumerator SpawnEnemyRoutine(GameObject prefab, Vector2 position)
    {
        var particle = Instantiate(_particle, position, Quaternion.identity);

        yield return new WaitForSeconds(_particleLifetime);

        Destroy(particle);

        SpawnEnemy(prefab, position);
    }

    #endregion
}

[Serializable]
public class Wave
{
    public int groundEnemiesCount;
    public List<GameObject> groundEnemies;

    public int airEnemiesCount;
    public List<GameObject> airEnemies;
}
