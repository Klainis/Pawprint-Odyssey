using UnityEngine;

public class EnablePimenDialogueInPoint : MonoBehaviour
{
    [SerializeField] private bool _beforeFirstBoss = false;
    [SerializeField] private bool _beforeTakeClaw = false;
    [SerializeField] private bool _beforeFirstFightRoom = false;
    [SerializeField] private bool _afterFirstFightRoom = false;
    [SerializeField] private bool _beforeLastBoss = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PimenTalk wr = PimenView.Instance.gameObject.GetComponent<PimenTalk>();

            if (_beforeFirstBoss)
            {
                wr.BeforeFirstBoss();
                return;
            }
            if (_beforeTakeClaw)
            {
                wr.SawClaw();
                return;
            }
            if (_beforeFirstFightRoom)
            {
                wr.BeforeFirstFightRoom();
                return;
            }
            if (_afterFirstFightRoom)
            {
                wr.AfterFirstFightRoom();
                return;
            }
            if (_beforeLastBoss)
            {
                wr.BeforeFinalBoss();
                return;
            }
        }
    }
}
