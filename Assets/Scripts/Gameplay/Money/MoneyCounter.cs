using TMPro;
using UnityEngine;

public class MoneyCounter : MonoBehaviour
{
    private int _initialAmountOfMoney = 000;
    private TMP_Text _counterText;
    private int _moneyCount;

    private void Start()
    {
        _counterText = InitializeManager.Instance.moneyText;
        _moneyCount = PlayerView.Instance.PlayerModel.MoneyCollected;

        _counterText.text = $"{(_moneyCount + _initialAmountOfMoney)}";
    }

    public void CountMoney(int reward)
    {
        _moneyCount = PlayerView.Instance.PlayerModel.MoneyCollected;
        _moneyCount += 2;
        PlayerView.Instance.PlayerModel.AddMoney(reward);

        _counterText.text = $"{(_moneyCount + _initialAmountOfMoney)}";
    }
}
