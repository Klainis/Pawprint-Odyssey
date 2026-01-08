using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameMenuUI : MonoBehaviour
{
    private static GameMenuUI instance;
    public static GameMenuUI Instance { get { return instance; } }

    [SerializeField] private Toggle _mapButton;
    [SerializeField] private Toggle _abilityButton;

    [SerializeField] private Color _selectedColor;
    [SerializeField] private Color _normalColor;

    //[Header("Currency Text")]
    //[SerializeField] private TMP_Text _abilityTreeMoneyText;
    //[SerializeField] private TMP_Text _abilityTreeCrystalText;
    //[SerializeField] private TMP_Text _mapCrystalText;

    private TMP_Text _mapCrystalText;

    private List<Toggle> _windowButtons = new ();

    private int _windowNumber = 0;

    //public TMP_Text AbilityTreeMoneyText { get { return _abilityTreeMoneyText; } }
    //public TMP_Text AbilityTreeCrystalText { get { return _abilityTreeCrystalText; } }
    //public TMP_Text MapCrystalText { get { return _mapCrystalText; } }

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        if (_mapButton == null)
        {
            Debug.Log("Map Button is NULL!");
            return;
        }
        if (_abilityButton == null)
        {
            Debug.Log("Ability Button is NULL!");
            return;
        }

        //var mapObject = GameManager.Instance.MapOb;
        //_mapCrystalText = mapObject.GetComponentInChildren<TMP_Text>();

        _mapButton.onValueChanged.AddListener((value) =>
        {
            if (_mapButton.isOn)
            {
                if (GameManager.Instance != null)
                    GameManager.Instance.OpenMap();
            }
        });
        _abilityButton.onValueChanged.AddListener((value) =>
        {
            if (_abilityButton.isOn)
            {
                if (GameManager.Instance != null)
                    GameManager.Instance.OpenAbilitiesTree();
            }
        });
    }

    private void Start()
    {
        _windowButtons.Add(_mapButton);
        _windowButtons.Add(_abilityButton);
    }

    public void SwapToRightWindow()
    {
        _windowNumber++;

        if (_windowNumber >= _windowButtons.Count)
        {
            _windowNumber = 0;
        }

        EventSystem.current.SetSelectedGameObject(_windowButtons[_windowNumber].gameObject);

        _windowButtons[_windowNumber].isOn = true;
    }

    public void SwapToLeftWindow()
    {
        _windowNumber--;

        if (_windowNumber < 0)
        {
            _windowNumber = _windowButtons.Count - 1;
        }

        EventSystem.current.SetSelectedGameObject(_windowButtons[_windowNumber].gameObject);

        _windowButtons[_windowNumber].isOn = true;
    }

    void OnEnable()
    {
        if (_mapButton == null || _abilityButton == null)
        {
            Debug.Log("Map or AbilityTree Toggle is NULL!");
            return;
        }

        //var money = PlayerView.Instance.PlayerModel.MoneyCollected;
        //var crystal = PlayerView.Instance.PlayerModel.SoulCrystalsCollected;
        ////_abilityTreeMoneyText.text = money.ToString();
        ////_abilityTreeCrystalText.text = crystal.ToString();
        //_mapCrystalText.text = crystal.ToString();
        //Debug.Log(_mapCrystalText.text);
        _mapButton.isOn = true;
        _abilityButton.isOn = false;
        EventSystem.current.SetSelectedGameObject(_mapButton.gameObject);
        GameManager.Instance.OpenMap();
    }

    //private void OnDisable()
    //{
    //    if (PlayerView.Instance.PlayerModel.HasDamageDash)
    //    {
    //        var _manaBar = InitializeManager.Instance.manaBar;
    //        PlayerMana.Instance.enabled = true;
    //        _manaBar.gameObject.SetActive(true);
    //    }
    //}
}
