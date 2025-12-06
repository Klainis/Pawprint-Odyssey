using TMPro;
using UnityEngine;

public class SoulCrystalCounter : MonoBehaviour
{
    private TMP_Text counterText;
    private int crystalCount;

    private void Start()
    {
        counterText = InitializeManager._instance.soulCrystalText;
        crystalCount = PlayerView.Instance.PlayerModel.SoulCrystalsCollected;
        counterText.text = crystalCount.ToString();
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

        counterText.text = crystalCount.ToString();
    }
}
