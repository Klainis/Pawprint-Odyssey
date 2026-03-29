using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class CostAbilitiesCheck : MonoBehaviour, ISelectHandler
{
    [SerializeField] private TMP_Text _moneyCost;
    [SerializeField] private TMP_Text _crystalCost;

    private PlayerModel _playerModel;

    private bool _moneyTrue;
    private bool _crystalTrue;

    public bool canBuy {  get; private set; } = false;
    public int MoneyCost { get { return GetCostfromText(_moneyCost); } }
    public int CrystalCost { get { return GetCostfromText(_crystalCost); } }

    private void Awake()
    {
        _playerModel = PlayerView.Instance.PlayerModel;
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (_playerModel.SoulCrystalsCollected < GetCostfromText(_crystalCost))
        {
            _crystalCost.color = Color.red;
            _crystalTrue = false;
            //Debug.Log($"Не хватает кристаллов{GetCostfromText(_crystalCost) - _playerModel.SoulCrystalsCollected}");
        }
        else
        {
            _crystalCost.color = Color.white;
            _crystalTrue = true;
        }

        if (_playerModel.MoneyCollected < GetCostfromText(_moneyCost))
        {
            _moneyCost.color = Color.red;
            _moneyTrue = false;
            //Debug.Log($"Не хватает денег{GetCostfromText(_moneyCost) - _playerModel.MoneyCollected}");
        }
        else
        {
            _moneyCost.color = Color.white;
            _moneyTrue = true;
        }

        if (_moneyTrue && _crystalTrue)
        {
            canBuy = true;
        }
        else
        {
            canBuy = false;
        }
    }

    private int GetCostfromText(TMP_Text text)
    {
        return int.Parse(text.text);
    }

    public void HideCost()
    {
        _moneyCost.gameObject.SetActive(false);
        _crystalCost.gameObject.SetActive(false);
    }
}
