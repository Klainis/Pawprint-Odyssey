using UnityEngine;
using GlobalEnums;
using Cinemachine;
using System.Collections;
using UnityEngine.UIElements;

public class EnterBossFight : MonoBehaviour
{
    [SerializeField] private BossHealth _health;
    [SerializeField] private GameObject _spiritGuide;
    [SerializeField] private GameObject _guardianOwl;
    [SerializeField] private FightDoor _door;
    [SerializeField] private CinemachineVirtualCamera _bossCamera;
    [SerializeField] private float _showBossTime = 3f;

    private SpiritGuideView _guideView;
    private GuardianOwlView _guardianOwlView;

    private int _initialBossCameraPrioriry = 0;

    //private CinemachineVirtualCamera _followCamera;
    //private Transform _initialFollowTarget;

    //private void Awake()
    //{
    //    _followCamera = GameObject.FindGameObjectWithTag("CinemachineFollowCamera").GetComponent<CinemachineVirtualCamera>();
    //}

    private void Start()
    {
        var guideIsDeadOrMissing = CheckAndInitSpiritGuide();
        var owlIsDeadOrMissing = CheckAndInitGuardianOwl();

        if (guideIsDeadOrMissing && owlIsDeadOrMissing)
            Destroy(gameObject);

        _initialBossCameraPrioriry = _bossCamera.Priority;
    }

    #region Trigger Enter & Exit

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && GameManager.Instance.GameState == GameState.PLAYING)
        {
            if (_door != null && _spiritGuide != null) // Boss 1
            {
                _spiritGuide.SetActive(true);
                _guideView = _spiritGuide.GetComponent<SpiritGuideView>();
                _guideView.enabled = true;
                _guideView.MoveDisabled = true;

                if (!_guideView.Model.IsDead)
                {
                    StartCoroutine(ShowBoss());
                }
            }
            else
            {
                Debug.Log($"Door is {_door != null}, GuideView is {_guideView != null}");
            }



            if (_guardianOwl != null) // Boss 2
            {
                _guardianOwl.SetActive(true);
                _guardianOwlView = _guardianOwl.GetComponent<GuardianOwlView>();
                _guardianOwlView.enabled = true;
                _guardianOwlView.StartFight = false;

                if (!_guardianOwlView.Model.IsDead)
                {
                    StartCoroutine(ShowBoss());
                }
            }
            else
            {
                Debug.Log($"GuardianView is {_guardianOwlView != null}");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && GameManager.Instance.GameState != GameState.IN_FIGHT_ROOM)
        {
            _health.DestroyBossHealthSlider();
        }
    }

    #endregion

    private IEnumerator ShowBoss()
    {
        GameManager.Instance.SetGameState(GameState.CUTSCENE);

        PlayerView.Instance.StopPlayer();
        yield return new WaitForSeconds(0.5f);
        PlayerView.Instance.FreezePlayerWithDisableMove(true);
        PlayerAnimation.Instance.ResetAnimatorParameters();
        if (PlayerView.Instance.PlayerModel.FacingRight)
        {
            PlayerMove.Instance.CallTurn();
        }

        yield return new WaitForSeconds(1f);

        _bossCamera.Priority = 100;

        //Диалог с боссом 1
        //Диалог с боссом 2

        yield return new WaitForSeconds(_showBossTime);

        GameManager.Instance.SetGameState(GameState.IN_FIGHT_ROOM);

        _bossCamera.Priority = _initialBossCameraPrioriry;
        _health.InstantiateBossHealth();
        
        if (_door != null) _door.CloseDoor(true);

        yield return new WaitForSeconds(0.5f);

        PlayerView.Instance.FreezePlayerWithDisableMove(false);
        if (_guideView != null)
        {
            _guideView.MoveDisabled = false;
        }
        if (_guardianOwlView != null)
        {
            _guardianOwlView.StartFight = true;
        }
    }

    private bool CheckAndInitSpiritGuide()
    {
        if (_spiritGuide == null) return true;

        if (PlayerView.Instance.PlayerModel.SpiritGuideKilled)
        {
            Destroy(_spiritGuide);
            return true;
        }
        else
        {
            _spiritGuide.SetActive(false);
            return false;
        }
    }

    private bool CheckAndInitGuardianOwl()
    {
        if (_guardianOwl == null) return true;

        if (PlayerView.Instance.PlayerModel.GuardianOwlKilled)
        {
            Destroy(_guardianOwl);
            return true;
        }
        else
        {
            _guardianOwl.SetActive(false);
            return false;
        }
    }
}
