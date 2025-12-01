using UnityEngine;

public class ReceivingClaw : MonoBehaviour
{
    private PlayerMana playerMana;
    private PiercingClaw piercingClaw;
    private GameObject manaBar;

    private PlayerView playerView;

    private void Awake()
    {
        playerView = GetComponent<PlayerView>();
        playerMana = GetComponent<PlayerMana>();
        piercingClaw = GetComponent<PiercingClaw>();
    }

    private void Start()
    {
        manaBar = InitializeManager._instance.manaBar;
        Debug.Log(manaBar != null);
        SetActiveManaBar();
    }

    public void SetActiveManaBar()
    {
        if (playerView.PlayerModel.HasClaw)
        {
            playerMana.enabled = true;
            piercingClaw.enabled = true;
            manaBar.SetActive(true);
        }
    }

    public void EnableClaw()
    {
        playerMana.enabled = true;
        piercingClaw.enabled = true;
        manaBar.SetActive(true);
        playerView.PlayerModel.SetHasClaw();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "ClawGetItem")
        {
            Destroy(collision.gameObject);
            EnableClaw();
        }
    }
}
