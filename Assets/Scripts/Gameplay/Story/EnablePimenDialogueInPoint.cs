using UnityEngine;

public class EnablePimenDialogueInPoint : MonoBehaviour
{
    [SerializeField] private bool _beforeFirstBoss = false;
    [SerializeField] private bool _beforeTakeClaw = false;
    [SerializeField] private bool _beforeTakeMnemirItem = false;
    [SerializeField] private bool _beforeFirstFightRoom = false;
    [SerializeField] private bool _afterFirstFightRoom = false;
    [SerializeField] private bool _beforeLastBoss = false;
    [SerializeField] private bool _lastRoom = false;

    [SerializeField] private BoxCollider2D _collider;

    private void Awake()
    {
        if (_collider == null)
            _collider = GetComponent<BoxCollider2D>();
    }

    private void Start()
    {
        if (!PlayerView.Instance.PlayerModel.HasQuestMnemir && _beforeTakeMnemirItem)
        {
            _collider.enabled = false;
        }
        else
        {
            _collider.enabled = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        var playerModel = PlayerView.Instance.PlayerModel;
        PimenTalk talk = PimenView.Instance.gameObject.GetComponent<PimenTalk>();

        if (_beforeFirstBoss && !playerModel.BeforeFirstBossDialogue)
        {
            talk.BeforeFirstBoss();
            playerModel.SetBeforeFirstBoss(true);
            return;
        }

        if (_beforeTakeClaw && !playerModel.SawClawDialogue)
        {
            talk.SawClaw();
            playerModel.SetSawClaw(true);
            return;
        }

        if (_beforeTakeMnemirItem && !playerModel.SawMnemirItemDialogue)
        {
            talk.SawMnemirItem();
            playerModel.SetSawMnemirItem(true);
            return;
        }

        if (_beforeFirstFightRoom && !playerModel.BeforeFirstFightRoomDialogue)
        {
            talk.BeforeFirstFightRoom();
            playerModel.SetBeforeFirstFightRoom(true);
            return;
        }

        if (_afterFirstFightRoom && !playerModel.AfterFirstFightRoomDialogue)
        {
            talk.AfterFirstFightRoom();
            playerModel.SetAfterFirstFightRoom(true);
            return;
        }

        if (_beforeLastBoss && !playerModel.BeforeFinalBossDialogue)
        {
            talk.BeforeFinalBoss();
            playerModel.SetBeforeFinalBoss(true);
            return;
        }

        if (_lastRoom)
        {
            if (PlayerView.Instance.PlayerModel.ArtefactCollected < 3)
            {
                talk.DontHaveAllArtifact();
            }
            else if (PlayerView.Instance.PlayerModel.ArtefactCollected >= 3 && !playerModel.LastRoomDialogue)
            {
                talk.LastRoom();
                playerModel.SetLastRoom(true);
                gameObject.SetActive(false);
            }
            return;
        }
    }
}
