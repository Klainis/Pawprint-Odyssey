using TMPro;
using UnityEngine;

public class SoulCrystalCounter : MonoBehaviour
{
    [SerializeField] private string _amountOfCrystal = "12";
    private TMP_Text counterText;
    private int crystalCount;

    private void Start()
    {
        counterText = InitializeManager.Instance.soulCrystalText;
        crystalCount = PlayerView.Instance.PlayerModel.SoulCrystalsCollected;

        var amountCr = _amountOfCrystal != null? _amountOfCrystal : "0";
        counterText.text = $"{crystalCount.ToString()} / {amountCr}";
    }

    //private void Update()
    //{
    //    if (counterText == null)
    //        counterText = InitializeManager._instance.soulCrystalText;
    //}

    public void CountCrystal()
    {
        crystalCount = PlayerView.Instance.PlayerModel.SoulCrystalsCollected;
        crystalCount++;
        PlayerView.Instance.PlayerModel.AddSoulCrystal();

        counterText.text = $"{crystalCount.ToString()} / 12";
    }
}
