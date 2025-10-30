using UnityEngine;

public class Mana : MonoBehaviour
{
    [SerializeField] private int maxMana = 50;
    [SerializeField] private int mana = 50;
    public int manaForReading { get; private set; }

    private PiercingClaw piercingClaw;
    private Attack attack;

    private void Awake()
    {
        piercingClaw = GetComponent<PiercingClaw>();
        attack = GetComponent<Attack>();
    }

    private void Update()
    {
        if (mana > maxMana)
            mana = maxMana;

        manaForReading = mana;
    }

    public void SpendMana()
    {
        mana -= 25;
    }

    public void GetMana()
    {
        mana += 5;
    }
}
