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
    [SerializeField] private Slider slider;

    private void Awake()
    {
        piercingClaw = GetComponent<PiercingClaw>();
        attack = GetComponent<Attack>();

        slider.maxValue = maxMana;
    }

    private void Update()
    {
        if (mana > maxMana)
            mana = maxMana;
        if (mana <= 0)
            mana = 0;

        manaForReading = mana;

        ManaBar();
    }

    public void SpendMana()
    {
        mana -= 25;
    }

    public void GetMana()
    {
        mana += 4;
    }

    public void ManaBar()
    {
        var manaValue = mana;
        slider.value = manaValue;
    }
}
