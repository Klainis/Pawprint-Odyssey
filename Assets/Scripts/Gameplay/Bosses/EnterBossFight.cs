using UnityEngine;

public class EnterBossFight : MonoBehaviour
{
    [SerializeField] private BossHealth _health;
    [SerializeField] private SpiritGuideView _guideView;
    [SerializeField] private FightDoor _door;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            _health.InstantiateBossHealth();

            if (_guideView.Model.Life > 0)
            {
                _door.CloseDoor(true);
            }
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            _health.DestroyBossHealthSlider();
        }
    }
}
