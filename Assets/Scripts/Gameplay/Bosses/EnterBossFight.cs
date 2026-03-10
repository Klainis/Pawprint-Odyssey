using UnityEngine;
using GlobalEnums;
using Cinemachine;
using System.Collections;

public class EnterBossFight : MonoBehaviour
{
    [SerializeField] private BossHealth _health;
    [SerializeField] private GameObject _spiritGuide;
    [SerializeField] private GameObject _guardianOwl;
    [SerializeField] private FightDoor _door;

    private SpiritGuideView _guideView;
    private GuardianOwlView _guardianOwlView;

    private CinemachineVirtualCamera _followCamera;
    private Transform _initialFollowTarget;

    private void Awake()
    {
        _followCamera = GameObject.FindGameObjectWithTag("CinemachineFollowCamera").GetComponent<CinemachineVirtualCamera>();
    }

    private void Start()
    {
        var guideIsDeadOrMissing = CheckAndInitSpiritGuide();
        var owlIsDeadOrMissing = CheckAndInitGuardianOwl();

        if (guideIsDeadOrMissing && owlIsDeadOrMissing)
            Destroy(gameObject);
    }

    #region Trigger Enter & Exit

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            //_initialFollowTarget = _followCamera.Follow;

            if (_door != null && _spiritGuide != null)
            {
                _spiritGuide.SetActive(true);
                _guideView = _spiritGuide.GetComponent<SpiritGuideView>();
                _guideView.enabled = true;
                _guideView.MoveDisabled = true;

                if (!_guideView.Model.IsDead && GameManager.Instance.GameState == GameState.PLAYING)
                {
                    PlayerMove.Instance.CanMove = false;
                    //_followCamera.Follow = _spiritGuide.transform;
                    _followCamera.Priority = 0;
                    GameManager.Instance.SetGameState(GameState.IN_FIGHT_ROOM);
                    _health.InstantiateBossHealth();
                    _door.CloseDoor(true);
                }

                //StartCoroutine(WaitForShowBoss(1f));
                PlayerMove.Instance.CanMove = true;
                _guideView.MoveDisabled = false;
                //Debug.Log(_followCamera.Follow);
            }
            else
            {
                Debug.Log($"Door is {_door != null}, GuideView is {_guideView != null}");
            }

            if (_guardianOwl != null)
            {
                _guardianOwl.SetActive(true);
                _guardianOwlView = _guardianOwl.GetComponent<GuardianOwlView>();
                _guardianOwlView.enabled = true;

                if (!_guardianOwlView.Model.IsDead && GameManager.Instance.GameState == GameState.PLAYING)
                {
                    PlayerMove.Instance.CanMove = false;
                    _followCamera.Follow = _guardianOwl.transform;
                    GameManager.Instance.SetGameState(GameState.IN_FIGHT_ROOM);
                    _health.InstantiateBossHealth();
                }

                StartCoroutine(WaitForShowBoss(1.5f));
                PlayerMove.Instance.CanMove = true;
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
            _followCamera.Priority = 10;
        }
    }

    #endregion

    private IEnumerator WaitForShowBoss(float time)
    {
        yield return new WaitForSeconds(time);
        //_followCamera.Follow = _initialFollowTarget;
        _followCamera.Priority = 10;
    }

    //private IEnumerator SetFollowTarget(Transform initial, Transform target)
    //{
    //    var flexTarget = target;

    //    while (flexTarget != target)
    //    {
    //        flexTarget = Vector3.Lerp(initial, target, 2f);
    //        yield return;
    //    }
    //}

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
