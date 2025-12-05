using UnityEngine;

public class ReceivingClaw : MonoBehaviour
{
    private PlayerMana playerMana;
    private PiercingClaw piercingClaw;
    private GameObject manaBar;

    private readonly string clawGetItemName = "ClawGetItem";
    private GameObject clawGetItem;

    private void Awake()
    {
        playerMana = GetComponent<PlayerMana>();
        piercingClaw = GetComponent<PiercingClaw>();

        clawGetItem = GameObject.Find(clawGetItemName);
    }

    private void Start()
    {
        if (PlayerView.Instance.PlayerModel.HasClaw)
            Destroy(clawGetItem);

        manaBar = InitializeManager._instance.manaBar;
        Debug.Log(manaBar != null);
        SetActiveManaBar();
    }

    public void SetActiveManaBar()
    {
        if (PlayerView.Instance.PlayerModel.HasClaw)
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
        PlayerView.Instance.PlayerModel.SetHasClaw();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(clawGetItemName))
        {
            Destroy(collision.gameObject);
            EnableClaw();
        }
    }
}
