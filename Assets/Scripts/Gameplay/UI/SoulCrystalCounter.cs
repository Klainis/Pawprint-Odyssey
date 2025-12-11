using TMPro;
using UnityEngine;

public class SoulCrystalCounter : MonoBehaviour
{
    private TMP_Text counterText;
    private int crystalCount;

    private void Start()
    {
        counterText = InitializeManager.Instance.soulCrystalText;
        crystalCount = PlayerView.Instance.PlayerModel.SoulCrystalsCollected;
        counterText.text = $"{crystalCount.ToString()} / 12";
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
