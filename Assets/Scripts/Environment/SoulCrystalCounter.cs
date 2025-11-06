using TMPro;
using UnityEngine;

public class SoulCrystalCounter : MonoBehaviour
{
    [SerializeField] private PlayerData Data;
    [SerializeField] private TMP_Text counterText;
    private int crystalCount;

    private void Start()
    {
        crystalCount = Data.soulCrystalCount;
        counterText.text = crystalCount.ToString();
    }

    public void CrystalCounter()
    {
        crystalCount++;
        Data.soulCrystalCount = crystalCount;
        counterText.text = crystalCount.ToString();
    }
}
