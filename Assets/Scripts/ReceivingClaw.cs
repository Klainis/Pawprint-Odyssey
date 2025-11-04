using UnityEngine;

public class ReceivingClaw : MonoBehaviour
{
    private Mana manaScript;
    private PiercingClaw piercingClaw;
    [SerializeField] private GameObject manaBar;
    private bool isHaveClaw;
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
