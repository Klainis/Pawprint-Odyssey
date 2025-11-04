using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Mana : MonoBehaviour
{
    [SerializeField] private int maxMana = 50;
    [SerializeField] private int mana = 50;
    public int manaForReading { get; private set; }

    private PiercingClaw piercingClaw;
    private Attack attack;
    [SerializeField] private Image manaBar;

    private void Awake()
    {
        piercingClaw = GetComponent<PiercingClaw>();
        attack = GetComponent<Attack>();

        //slider.maxValue = maxMana;
    }

    private void Update()
    {
        if (mana > maxMana)
            mana = maxMana;
        if (mana <= 0)
            mana = 0;

        manaForReading = mana;
    }

    public void SpendMana()
    {
        mana -= 25;
        manaBar.fillAmount = (float)mana / (float)maxMana;
    }

    public void GetMana()
    {
        mana += 4;
        manaBar.fillAmount = (float)mana / (float)maxMana;
    }
}
