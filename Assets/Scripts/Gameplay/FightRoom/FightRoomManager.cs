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
    private FightDoor[] _fightDoors;

    #endregion

    #region Common Methods

    private void Start()
    {
        _roomName = SceneManager.GetActiveScene().name;
        _fightDoors = FindObjectsByType<FightDoor>(FindObjectsSortMode.None);

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
        SwitchDoorsState(false);
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
        if (wave.groundEnemies.Count == 0) return;

        var availablePositions = new List<Transform>(_groundEnemyPositions);
        var spawnCount = Mathf.Min(wave.groundEnemiesCount, availablePositions.Count);

        for (int i = 0; i < spawnCount; i++)
        {
            var positionIndex = UnityEngine.Random.Range(0, availablePositions.Count);
            var position = (Vector2)availablePositions[positionIndex].position;

            availablePositions.RemoveAt(positionIndex);

            var prefab = wave.groundEnemies[UnityEngine.Random.Range(0, wave.groundEnemies.Count)];
            StartCoroutine(SpawnEnemyRoutine(prefab, position));
        }
    }

    private void SpawnAirEnemies(Wave wave)
    {
        if (wave.airEnemies.Count == 0) return;

        var availablePositions = new List<Transform>(_airEnemyPositions);
        var spawnCount = Mathf.Min(wave.airEnemiesCount, availablePositions.Count);

        for (int i = 0; i < spawnCount; i++)
        {
            var positionIndex = UnityEngine.Random.Range(0, availablePositions.Count);
            var position = (Vector2)availablePositions[positionIndex].position;

            availablePositions.RemoveAt(positionIndex);

            var prefab = wave.airEnemies[UnityEngine.Random.Range(0, wave.airEnemies.Count)];
            StartCoroutine(SpawnEnemyRoutine(prefab, position));
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

    private void SwitchDoorsState(bool toCloseState)
    {
        foreach (var door in _fightDoors)
        {
            door.CloseDoor(toCloseState);
        }
    }

    #endregion

    #region IEnumerators

    private IEnumerator StartFightRoutine()
    {
        while (!PlayerMove.Instance.IsGrounded)
            yield return null;

        SwitchDoorsState(true);
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
