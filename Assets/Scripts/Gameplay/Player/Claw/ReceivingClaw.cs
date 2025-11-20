using UnityEngine;

public class ReceivingClaw : MonoBehaviour
{
    [SerializeField] private PlayerData Data;

    [SerializeField] private Mana manaScript;
    [SerializeField] private PiercingClaw piercingClaw;
    public GameObject manaBar;

    private PlayerView playerView;

    private void Awake()
    {
        playerView = GetComponent<PlayerView>();
        manaScript = GetComponent<Mana>();
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
            manaScript.enabled = true;
            piercingClaw.enabled = true;
            manaBar.SetActive(true);
        }
    }

    public void EnableClaw()
    {
        manaScript.enabled = true;
        piercingClaw.enabled = true;
        manaBar.SetActive(true);
        Data.clawIsReceived = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == "ClawGetItem")
        {
            Destroy(collision.gameObject);
            EnableClaw();
        }
    }
}
