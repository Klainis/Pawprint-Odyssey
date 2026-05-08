using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using GlobalEnums;

public class AttackPimenManager : MonoBehaviour
{
    private static AttackPimenManager _instance;
    public static AttackPimenManager Instance { get { return _instance; } }

    [Header("Enemies")]
    [SerializeField] private int _maxEnemyAmount = 2;

    [Header("Close walls")]
    [SerializeField] private List<GameObject> _walls;

    [Header("Pimen Settings")]
    [SerializeField] private Transform _pimenFirstMeetTopPoint;

    private PimenMove _pimenMove;
    private PimenAnimation _pimenAnimation;
    private Wraith _pimenWraith;
    private StunAudioController _stunAudioController;

    private GameObject _pimenObject;

    private int _currentDeadEnemyAmount = 0;
    private bool _inDialogue = true;

    private Coroutine _victoryOverEnemiesCoroutine;
    private Coroutine _pimenDialogueCoroutine;

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
        _pimenAnimation = _pimenObject.GetComponent<PimenAnimation>();
        _pimenWraith = _pimenObject.GetComponent<Wraith>();

        if (!PlayerView.Instance.PlayerModel.MeetPimen)
        {
            _pimenMove.enabled = false;
            _pimenAnimation.enabled = false;
            _pimenAnimation.animator.enabled = false;
        }

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
            StartVictoryOverEnemiesCoroutine();
        }
    }

    private void StartVictoryOverEnemiesCoroutine()
    {
        if (_victoryOverEnemiesCoroutine != null)
        {
            StopCoroutine(_victoryOverEnemiesCoroutine);
        }
        _victoryOverEnemiesCoroutine = StartCoroutine(VictoryOverEnemies());
    }

    private IEnumerator VictoryOverEnemies()
    {
        _stunAudioController.TriggerStun();

        Time.timeScale = 0.4f;

        yield return new WaitForSecondsRealtime(1.5f);

        Time.timeScale = 1;

        yield return new WaitForSecondsRealtime(1.5f);

        float time = 1f;
        float elapsedTime = 0;
        while (elapsedTime < time)
        {
            elapsedTime += Time.deltaTime;
            _pimenObject.transform.position = Vector3.MoveTowards(_pimenObject.transform.position, _pimenFirstMeetTopPoint.position, elapsedTime / time);
            yield return null;
        }

        _pimenAnimation.enabled = true;
        _pimenAnimation.animator.enabled = true;
        _pimenWraith.FirstTalkWithPimen();


        while (GameManager.Instance.GameState == GameState.DIALOGUE)
        {
            yield return null;
        }

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
