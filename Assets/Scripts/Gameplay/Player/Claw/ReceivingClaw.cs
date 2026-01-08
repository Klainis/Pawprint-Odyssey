using UnityEngine;

public class ReceivingClaw : MonoBehaviour
{
    private PiercingClaw piercingClaw;
    private GameObject manaBar;

    private readonly string clawGetItemTag = "ClawGetItem";
    private GameObject clawGetItem;

    private void Awake()
    {
        piercingClaw = GetComponent<PiercingClaw>();
    }

    private void Start()
    {
        manaBar = InitializeManager.Instance.manaBar;
        //Debug.Log(manaBar != null);
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
            PlayerMana.Instance.enabled = true;
            piercingClaw.enabled = true;
            manaBar.SetActive(true);
        }
    }

    public void EnableClaw()
    {
        PlayerMana.Instance.enabled = true;
        piercingClaw.enabled = true;
        manaBar.SetActive(true);
        PlayerView.Instance.PlayerModel.SetHasClaw();
        SaveSystem.AutoSave();
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
