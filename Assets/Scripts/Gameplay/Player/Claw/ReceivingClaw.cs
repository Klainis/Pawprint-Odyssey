using UnityEngine;

public class ReceivingClaw : MonoBehaviour
{
    private PlayerMana playerMana;
    private PiercingClaw piercingClaw;
    private GameObject manaBar;

    private readonly string clawGetItemTag = "ClawGetItem";
    private GameObject clawGetItem;

    private void Awake()
    {
        playerMana = GetComponent<PlayerMana>();
        piercingClaw = GetComponent<PiercingClaw>();
    }

    private void Start()
    {
        manaBar = InitializeManager.Instance.manaBar;
        Debug.Log(manaBar != null);
        SetActiveManaBar();
    }

    private void FixedUpdate()
    {
        if (clawGetItem == null)
            clawGetItem = GameObject.Find(clawGetItemTag);
        if (clawGetItem != null)
            if (PlayerView.Instance.PlayerModel.HasClaw)
                Destroy(clawGetItem);
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
        SaveSystem.Save();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(clawGetItemTag))
        {
            Destroy(collision.gameObject);
            EnableClaw();
        }
    }
}
