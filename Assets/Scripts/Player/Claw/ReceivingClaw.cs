using UnityEngine;

public class ReceivingClaw : MonoBehaviour
{
    [SerializeField] private PlayerData Data;

    [SerializeField] private Mana manaScript;
    [SerializeField] private PiercingClaw piercingClaw;
    [SerializeField] private GameObject manaBar;

    private void Awake()
    {
        if (Data.clawIsReceived)
        {
            manaScript.enabled = true;
            piercingClaw.enabled = true;
            manaBar.SetActive(true);
        }
    }

    void Start()
    {
        manaScript = GetComponent<Mana>();
        piercingClaw = GetComponent<PiercingClaw>();
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
