using UnityEngine;

public class ReceivingClaw : MonoBehaviour
{
    private Mana mana;
    private PiercingClaw piercingClaw;
    [SerializeField] private GameObject manaBar;
    private bool isHaveClaw;
    void Start()
    {
        mana = GetComponent<Mana>();
        piercingClaw = GetComponent<PiercingClaw>();
    }

    void Update()
    {
        
    }

    public void EnableClaw()
    {
        mana.enabled = true;
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
