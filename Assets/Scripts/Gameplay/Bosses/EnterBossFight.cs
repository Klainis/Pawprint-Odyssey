using UnityEngine;
using GlobalEnums;

public class EnterBossFight : MonoBehaviour
{
    [SerializeField] private BossHealth _health;
    [SerializeField] private SpiritGuideView _guideView;
    [SerializeField] private FightDoor _door;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (_guideView.Model.Life > 0 && GameManager._instance.GameState == GameState.PLAYING)
            {
                GameManager._instance.SetGameState(GameState.IN_FIGHT_ROOM);
                _health.InstantiateBossHealth();
                _door.CloseDoor(true);
            }
            //else if проверка на других боссов
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
