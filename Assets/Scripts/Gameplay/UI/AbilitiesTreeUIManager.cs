using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using TMPro;

public class AbilitiesTreeUIManager : MonoBehaviour
{
    private static AbilitiesTreeUIManager instance;
    public static AbilitiesTreeUIManager Instance {  get { return instance; } }

    [Header("Tree Nodes")]
    [SerializeField] private Button _dashButton;
    [SerializeField] private Button _wallRunButton;
    [SerializeField] private Button _runButton;
    [SerializeField] private Button _damageDashButton;
    [SerializeField] private Button _dashBuyButton;
    [SerializeField] private Button _wallRunBuyButton;
    [SerializeField] private Button _runBuyButton;
    [SerializeField] private Button _damageDashBuyButton;

    [Header("Nodes Frames")]
    [SerializeField] private Image _dashNodeFrameImage;
    [SerializeField] private Image _wallRunNodeFrameImage;
    [SerializeField] private Image _runNodeFrameImage;
    [SerializeField] private Image _damageDashNodeFrameImage;

    [Header("Nodes Images")]
    [SerializeField] private Image _dashAbilityImage;
    [SerializeField] private Image _wallRunAbilityImage;
    [SerializeField] private Image _runAbilityImage;
    [SerializeField] private Image _damageDashAbilityImage;

    [Header("Color Of Node State")]
    [SerializeField] private Color _getColor;
    [SerializeField] private Color _canGetColor;
    [SerializeField] private Color _canNotGetColor;
    [SerializeField] private Color _getFrameColor;
    [SerializeField] private Color _canGetFrameColor;
    [SerializeField] private Color _canNotGetFrameColor;

    [Header("Ability Card")]
    [SerializeField] private GameObject _dashAbilityText;
    [SerializeField] private GameObject _wallRunAbilityText;
    [SerializeField] private GameObject _runAbilityText;
    [SerializeField] private GameObject _damageDashAbilityText;

    [Header("Currency Text")]
    [SerializeField] private TMP_Text _moneyText;
    [SerializeField] private TMP_Text _crystalText;

    [Header("Errors text")]
    [SerializeField] private GameObject _areaErrorText;

    private MoneyCounter _moneyCounter;
    private SoulCrystalCounter _soulCrystalCounter;
    private Interact _interact;
    private InstantiateParticles _instantiateParticles;

    private CostAbilitiesCheck _dashCostCheck;
    private CostAbilitiesCheck _wallRunCostCheck;
    private CostAbilitiesCheck _runCostCheck;
    private CostAbilitiesCheck _damageDashCostCheck;

    private EventSystem _eventSystem;

    private PlayerModel _playerModel;

    private GameObject _manaBar;

    private bool _isTreeActive;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        _eventSystem = EventSystem.current;
        _playerModel = PlayerView.Instance.PlayerModel;
        _instantiateParticles = GetComponent<InstantiateParticles>();

        _moneyCounter = GameObject.FindAnyObjectByType<MoneyCounter>();
        _soulCrystalCounter = GameObject.FindAnyObjectByType<SoulCrystalCounter>();
        _interact = GameObject.FindAnyObjectByType<Interact>();

        _dashCostCheck = _dashButton.GetComponent<CostAbilitiesCheck>();
        _wallRunCostCheck = _wallRunButton.GetComponent<CostAbilitiesCheck>();
        _runCostCheck = _runButton.GetComponent<CostAbilitiesCheck>();
        _damageDashCostCheck = _damageDashButton.GetComponent<CostAbilitiesCheck>();


        //_dashAbilityImage = (Image)_dashButton.targetGraphic;
        //_wallRunAbilityImage = (Image)_wallRunButton.targetGraphic;
        //_runAbilityImage = (Image)_runButton.targetGraphic;
        //_damageDashAbilityImage = (Image)_damageDashButton.targetGraphic;

        InitializeNodes();

        _dashBuyButton.onClick.AddListener(() =>
        {
            if (_interact.AbilitiesTree)
            {
                if (_dashCostCheck.canBuy && !_playerModel.HasDash)
                {
                    BuyDash();
                }
            }
        });
        _wallRunBuyButton.onClick.AddListener(() =>
        {
            if (_interact.AbilitiesTree)
            {
                if (_wallRunCostCheck.canBuy && !_playerModel.HasWallRun
                                        && _playerModel.HasDash)
                {
                    BuyWallRun();
                }
            }
        });
        _runBuyButton.onClick.AddListener(() =>
        {
            if (_interact.AbilitiesTree)
            {
                if (_runCostCheck.canBuy && !_playerModel.HasRun
                                    && _playerModel.HasWallRun)
                {
                    BuyRun();
                }
            }
        });
        _damageDashBuyButton.onClick.AddListener(() =>
        {
            if (_interact.AbilitiesTree)
            {
                if (_damageDashCostCheck.canBuy && !_playerModel.HasDamageDash
                                            && _playerModel.HasRun)
                {
                    BuyDamageDash();
                }
            }
        });
    }

    private void Update()
    {
        EnableAreaErrorText(!_interact.AbilitiesTree);

        SetActiveAbilityText(_dashAbilityText, IsSelectedDash());
        SetActiveAbilityText(_wallRunAbilityText, IsSelectedWallRun());
        SetActiveAbilityText(_runAbilityText, IsSelectedRun());
        SetActiveAbilityText(_damageDashAbilityText, IsSelectedDamageDash());
    }

    public void BuyNode()
    {
        if (IsSelectedDash())
        {
            _dashBuyButton.onClick.Invoke();
        }
        else if (IsSelectedWallRun())
        {
            _wallRunBuyButton.onClick.Invoke();
        }
        else if (IsSelectedRun())
        {
            _runBuyButton.onClick.Invoke();
        }
        else if (IsSelectedDamageDash())
        {
            _damageDashBuyButton.onClick.Invoke();
        }
    }

    #region Is Selected Node
    private bool IsSelectedDash()
    {
        return IsSelected(_dashButton) || IsSelected(_dashBuyButton);
    }
    private bool IsSelectedWallRun()
    {
        return IsSelected(_wallRunButton) || IsSelected(_wallRunBuyButton);
    }
    private bool IsSelectedRun()
    {
        return IsSelected(_runButton) || IsSelected(_runBuyButton);
    }
    private bool IsSelectedDamageDash()
    {
        return IsSelected(_damageDashButton) || IsSelected(_damageDashBuyButton);
    }
    #endregion

    private void SetActiveAbilityText(GameObject abilityTextOb, bool visible)
    {
        var canvasGroup = abilityTextOb.GetComponent<CanvasGroup>();
        canvasGroup.alpha = visible ? 1 : 0;
    }

    private void InitializeNodes()
    {
        if (_playerModel.HasDash)
        {
            GetDashTreeState();
        }
        //else
        //{
        //    DontGetDashTreeState();
        //}
        if (_playerModel.HasWallRun)
        { 
            GetWallRunTreeState();
        }
        //else
        //{
        //    DontGetWallRunTreeState();
        //}
        if (_playerModel.HasRun)
        {
            GetRunTreeState();
        }
        //else
        //{
        //    DontGetRunTreeState();
        //}
        if( _playerModel.HasDamageDash)
        {
            GetDamageDashTreeState();
        }
        //else
        //{
        //    DontGetDamageDashTreeState();
        //}
    }

    public void EnableAreaErrorText(bool enabled)
    {
        _areaErrorText.SetActive(enabled);
    }

    #region Buy and Save Logic

    private void BuyDash()
    {
        _playerModel.SetHasDash();
        _soulCrystalCounter.SpendCrystal(_dashCostCheck.CrystalCost);
        _moneyCounter.SpendMoney(_dashCostCheck.MoneyCost);
        UpdateCurrencyText();

        GetDashTreeState();
        _instantiateParticles.InstantiateNodePollen(_dashButton.transform.position);
        _eventSystem.SetSelectedGameObject(_wallRunButton.gameObject);

        SaveSystem.CrystalSave();
        SaveSystem.MoneySave();

    }

    private void GetDashTreeState()
    {
        _dashCostCheck.HideCost();
        _dashBuyButton.gameObject.SetActive(false);

        _dashAbilityImage.color = _getColor;
        _wallRunAbilityImage.color = _canGetColor;

        _dashNodeFrameImage.color = _getFrameColor;
        _wallRunNodeFrameImage.color = _canGetFrameColor;
    }

    //private void DontGetDashTreeState()
    //{
    //    _dashAbilityImage.color = _canGetColor;
    //    _wallRunAbilityImage.color = _canNotGetColor;

    //    _dashNodeFrameImage.color = _canGetFrameColor;
    //    _wallRunNodeFrameImage.color = _canNotGetFrameColor;
    //}

    private void BuyWallRun()
    {
        _playerModel.SetHasWallRun();
        _soulCrystalCounter.SpendCrystal(_wallRunCostCheck.CrystalCost);
        _moneyCounter.SpendMoney(_wallRunCostCheck.MoneyCost);
        UpdateCurrencyText();

        GetWallRunTreeState();
        _instantiateParticles.InstantiateNodePollen(_wallRunButton.transform.position);
        _eventSystem.SetSelectedGameObject(_runButton.gameObject);

        SaveSystem.CrystalSave();
        SaveSystem.MoneySave();
    }

    private void GetWallRunTreeState()
    {
        _wallRunCostCheck.HideCost();
        _wallRunBuyButton.gameObject.SetActive(false);

        _wallRunAbilityImage.color = _getColor;
        _runAbilityImage.color = _canGetColor;

        _wallRunNodeFrameImage.color = _getFrameColor;
        _runNodeFrameImage.color = _canGetFrameColor;
    }

    //private void DontGetWallRunTreeState()
    //{
    //    _wallRunCostCheck.HideCost();

    //    _wallRunAbilityImage.color = _getColor;
    //    _runAbilityImage.color = _canGetColor;

    //    _wallRunNodeFrameImage.color = _getFrameColor;
    //    _runNodeFrameImage.color = _canGetFrameColor;
    //}

    private void BuyRun()
    {
        _playerModel.SetHasRun();
        _soulCrystalCounter.SpendCrystal(_runCostCheck.CrystalCost);
        _moneyCounter.SpendMoney(_runCostCheck.MoneyCost);
        UpdateCurrencyText();

        GetRunTreeState();
        _instantiateParticles.InstantiateNodePollen(_runButton.transform.position);
        _eventSystem.SetSelectedGameObject(_damageDashButton.gameObject);

        SaveSystem.CrystalSave();
        SaveSystem.MoneySave();
    }

    private void GetRunTreeState()
    {
        _runCostCheck.HideCost();
        _runBuyButton.gameObject.SetActive(false);

        _runAbilityImage.color = _getColor;
        _damageDashAbilityImage.color = _canGetColor;

        _runNodeFrameImage.color= _getFrameColor;
        _damageDashNodeFrameImage.color = _canGetFrameColor;
    }

    //private void DontGetRunTreeState()
    //{
    //    _runCostCheck.HideCost();

    //    _runAbilityImage.color = _getColor;
    //    _damageDashAbilityImage.color = _canGetColor;

    //    _runNodeFrameImage.color = _getFrameColor;
    //    _damageDashNodeFrameImage.color = _canGetFrameColor;
    //}

    private void BuyDamageDash()
    {
        _manaBar = InitializeManager.Instance.manaBar;
        Debug.Log($"Abilities Tree {_manaBar}");
        PlayerMana.Instance.enabled = true;
        _manaBar.gameObject.SetActive(true);

        _playerModel.SetHasDamageDash();
        _soulCrystalCounter.SpendCrystal(_damageDashCostCheck.CrystalCost);
        _moneyCounter.SpendMoney(_damageDashCostCheck.MoneyCost);
        UpdateCurrencyText();

        GetDamageDashTreeState();
        _instantiateParticles.InstantiateNodePollen(_damageDashButton.transform.position);
        SaveSystem.CrystalSave();
        SaveSystem.MoneySave();
    }

    private void GetDamageDashTreeState()
    {
        _damageDashCostCheck.HideCost();
        _damageDashBuyButton.gameObject.SetActive(false);

        _damageDashAbilityImage.color = _getColor;
        //_wallRunAbilityImage.color = _canGetColor;

        _damageDashNodeFrameImage.color = _getFrameColor;
    }

    //private void DontGetDamageDashTreeState()
    //{
    //    _damageDashCostCheck.HideCost();

    //    _damageDashAbilityImage.color = _getColor;
    //    //_wallRunAbilityImage.color = _canGetColor;

    //    _damageDashNodeFrameImage.color = _getFrameColor;
    //}
    #endregion

    public void SetSelectedAfterOpenWindow()
    {
        _eventSystem.SetSelectedGameObject(_dashButton.gameObject);
    }

    private bool IsSelected(Button bt)
    {
        if (_eventSystem.currentSelectedGameObject == bt.gameObject)
        {
            return true;
        }
        return false;
    }

    private void UpdateCurrencyText()
    {
        _moneyText.text = PlayerView.Instance.PlayerModel.MoneyCollected.ToString();
        _crystalText.text = PlayerView.Instance.PlayerModel.SoulCrystalsCollected.ToString();
    }

    private void OnEnable()
    {
        UpdateCurrencyText();
    }
}
