using TMPro;
using UnityEngine;

public class SoulCrystalCounter : MonoBehaviour
{
    private int _initialAmountOfCrystal = 000;
    private TMP_Text counterText;
    private int crystalCount;

    private void Start()
    {
        counterText = InitializeManager.Instance.soulCrystalText;
        crystalCount = PlayerView.Instance.PlayerModel.SoulCrystalsCollected;

        //var amountCr = _initialAmountOfCrystal != null? _initialAmountOfCrystal : 000;
        counterText.text = $"{(crystalCount + _initialAmountOfCrystal)}";
    } 

    public void CountCrystal()
    {
        crystalCount = PlayerView.Instance.PlayerModel.SoulCrystalsCollected;
        crystalCount++;
        PlayerView.Instance.PlayerModel.AddSoulCrystal();

        counterText.text = $"{(crystalCount + _initialAmountOfCrystal)}";
    }
}
