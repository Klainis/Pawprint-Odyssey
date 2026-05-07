using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackPimenManager : MonoBehaviour
{
    private static AttackPimenManager _instance;
    public static AttackPimenManager Instance {  get { return _instance; } }

    [Header("Enemies")]
    [SerializeField] private int _maxEnemyAmount = 2;

    [Header("Close walls")]
    [SerializeField] private List<GameObject> _walls;

    private PimenMove _pimenMove;
    private StunAudioController _stunAudioController;

    private GameObject _pimenObject;

    private int _currentDeadEnemyAmount = 0;

    private Coroutine _victoryOverEnemies;

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
        }
        _instance = this;

        if (PlayerView.Instance != null && PlayerView.Instance.PlayerModel.MeetPimen)
        {
            gameObject.SetActive(false);
        }

        _pimenObject = GameObject.FindGameObjectWithTag("Pimen");

        _pimenMove = _pimenObject.GetComponent<PimenMove>();

        _stunAudioController = GetComponent<StunAudioController>();
    }

    private void Start()
    {
        _currentDeadEnemyAmount = 0;
        Debug.Log(_currentDeadEnemyAmount);
    }

    public void CountDeadEnemy()
    {
        _currentDeadEnemyAmount += 1;
        Debug.Log(_currentDeadEnemyAmount);

        if (_currentDeadEnemyAmount >= _maxEnemyAmount)
        {
            if (_victoryOverEnemies != null)
            {
                StopCoroutine(_victoryOverEnemies);
            }
            _victoryOverEnemies = StartCoroutine(VictoryOverEnemies());
        }
    }

    private IEnumerator VictoryOverEnemies()
    {
        _stunAudioController.TriggerStun();

        Time.timeScale = 0.4f;

        yield return new WaitForSecondsRealtime(1.5f);

        Time.timeScale = 1;

        _pimenMove.enabled = true;

        PlayerView.Instance.PlayerModel.SetMeetPimen();
        SaveSystem.Save();
        SaveSystem.AutoSave();

        yield return new WaitForSeconds(1.5f);

        foreach (var wall in _walls)
        {
            var closedGr = wall.GetComponent<ClosedGround>();
            closedGr.StartDestroyer();
        }
    }
}
