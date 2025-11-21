using TMPro;
using UnityEngine;

public class SoulCrystalCounter : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private PlayerData Data;
    [Space(5)]
    private TMP_Text counterText;
    private int crystalCount;

    private void Start()
    {
        counterText = InitializeManager._instance.soulCrystalText;
        crystalCount = Data.soulCrystalsCollected;
        counterText.text = crystalCount.ToString();
    }

    public void CrystalCounter()
    {
        crystalCount++;
        Data.soulCrystalsCollected = crystalCount;
        counterText.text = crystalCount.ToString();
    }
}
