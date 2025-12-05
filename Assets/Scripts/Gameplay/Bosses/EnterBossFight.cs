using UnityEngine;
using GlobalEnums;

public class EnterBossFight : MonoBehaviour
{
    [SerializeField] private BossHealth _health;
    [SerializeField] private GameObject _spiritGuide;
    [SerializeField] private GameObject _guardianOwl;
    [SerializeField] private FightDoor _door;

    private SpiritGuideView _guideView;
    private GuardianOwlView _guardianOwlView;

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
            if (_door != null && _spiritGuide != null)
            {
                _spiritGuide.SetActive(true);
                _guideView = _spiritGuide.GetComponent<SpiritGuideView>();
                _guideView.enabled = true;

                if (!_guideView.Model.IsDead && GameManager._instance.GameState == GameState.PLAYING)
                {
                    GameManager._instance.SetGameState(GameState.IN_FIGHT_ROOM);
                    _health.InstantiateBossHealth();
                    _door.CloseDoor(true);
                }
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

                if (!_guardianOwlView.Model.IsDead && GameManager._instance.GameState == GameState.PLAYING)
                {
                    GameManager._instance.SetGameState(GameState.IN_FIGHT_ROOM);
                    _health.InstantiateBossHealth();
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
        if (collision.gameObject.CompareTag("Player") && GameManager._instance.GameState != GameState.IN_FIGHT_ROOM)
        {
            _health.DestroyBossHealthSlider();
        }
    }

    #endregion

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
