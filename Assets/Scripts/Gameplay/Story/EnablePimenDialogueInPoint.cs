using UnityEngine;

public class EnablePimenDialogueInPoint : MonoBehaviour
{
    [SerializeField] private bool _beforeFirstBoss = false;
    [SerializeField] private bool _beforeTakeClaw = false;
    [SerializeField] private bool _beforeFirstFightRoom = false;
    [SerializeField] private bool _beforeLastBoss = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            var playerModel = PlayerView.Instance.PlayerModel;
            PimenTalk wr = PimenView.Instance.gameObject.GetComponent<PimenTalk>();

            if (_beforeFirstBoss && !playerModel.BeforeFirstBoss)
            {
                wr.BeforeFirstBoss();
                playerModel.SetBeforeFirstBoss(true);
                return;
            }
            if (_beforeTakeClaw && !playerModel.SawClaw)
            {
                wr.SawClaw();
                playerModel.SetSawClaw(true);
                return;
            }
            if (_beforeFirstFightRoom && !playerModel.BeforeFirstFightRoom)
            {
                wr.BeforeFirstFightRoom();
                playerModel.SetBeforeFirstFightRoom(true);
                return;
            }
            if (_beforeLastBoss && !playerModel.BeforeFinalBoss)
            {
                wr.BeforeFinalBoss();
                playerModel.SetBeforeFinalBoss(true);
                return;
            }
        }
    }
}
