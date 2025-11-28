using UnityEngine;
using GlobalEnums;

public class EnterBossFight : MonoBehaviour
{
    [SerializeField] private BossHealth _health;
    [SerializeField] private SpiritGuideView _guideView;
    [SerializeField] private GuardianOwlView _guardianOwlView;
    [SerializeField] private FightDoor _door;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (_door != null && _guideView != null)
            {

                if (_guideView.Model.Life > 0 && GameManager._instance.GameState == GameState.PLAYING)
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

            if (_guardianOwlView != null)
            {
                if (_guardianOwlView.Model.Life > 0 && GameManager._instance.GameState == GameState.PLAYING)
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
}
