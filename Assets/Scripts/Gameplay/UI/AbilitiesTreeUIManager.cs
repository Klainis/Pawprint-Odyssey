using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using TMPro;

public class AbilitiesTreeUIManager : MonoBehaviour
{
    private static AbilitiesTreeUIManager instance;
    public static AbilitiesTreeUIManager Instance {  get { return instance; } }

    [Header("Tree Nodes Buttons")]
    [SerializeField] private Button _dashButton;
    [SerializeField] private Button _wallRunButton;
    //[SerializeField] private Button _runButton;
    [SerializeField] private Button _damageDashButton;
    [SerializeField] private Button _chargedAttackButton;
    [SerializeField] private Button _fourPawsButton;
    [SerializeField] private Button _soulReleaseButton;
    [SerializeField] private Button _parryingButton;
    [SerializeField] private Button _piercingClawButton;
    [SerializeField] private Button _doubleJumpButton;

    [Header("Buy Buttons")]
    [SerializeField] private Button _dashBuyButton;
    [SerializeField] private Button _wallRunBuyButton;
    //[SerializeField] private Button _runBuyButton;
    [SerializeField] private Button _damageDashBuyButton;
    [SerializeField] private Button _chargedAttackBuyButton;
    [SerializeField] private Button _fourPawsBuyButton;
    [SerializeField] private Button _soulReleaseBuyButton;
    [SerializeField] private Button _parryingBuyButton;

    [Header("Nodes Frames")]
    [SerializeField] private Image _dashNodeFrameImage;
    [SerializeField] private Image _wallRunNodeFrameImage;
    [SerializeField] private Image _runNodeFrameImage;
    [SerializeField] private Image _damageDashNodeFrameImage;
    [SerializeField] private Image _chargedAttackNodeFrameImage;
    [SerializeField] private Image _fourPawsNodeFrameImage;
    [SerializeField] private Image _soulReleaseNodeFrameImage;
    [SerializeField] private Image _parryingNodeFrameImage;
    [SerializeField] private Image _piercingClawNodeFrameImage;
    [SerializeField] private Image _doubleJumpNodeFrameImage;

    [Header("Nodes Images")]
    [SerializeField] private Image _dashAbilityImage;
    [SerializeField] private Image _wallRunAbilityImage;
    [SerializeField] private Image _runAbilityImage;
    [SerializeField] private Image _damageDashAbilityImage;
    [SerializeField] private Image _chargedAttackAbilityImage;
    [SerializeField] private Image _fourPawsAbilityImage;
    [SerializeField] private Image _soulReleaseAbilityImage;
    [SerializeField] private Image _parryingAbilityImage;
    [SerializeField] private Image _piercingClawAbilityImage;
    [SerializeField] private Image _doubleJumpAbilityImage;

    [Header("Color Of Node State")]
    [SerializeField] private Color _getColor;
    [SerializeField] private Color _canGetColor;
    [SerializeField] private Color _canNotGetColor;
    [SerializeField] private Color _getFrameColor;
    [SerializeField] private Color _canGetFrameColor;
    [SerializeField] private Color _canNotGetFrameColor;

    [Header("Ability Card")]
    [SerializeField] private GameObject _dashAbilityTextCard;
    [SerializeField] private GameObject _wallRunAbilityTextCard;
    [SerializeField] private GameObject _runAbilityTextCard;
    [SerializeField] private GameObject _damageDashAbilityTextCard;
    [SerializeField] private GameObject _chargedAttackAbilityTextCard;
    [SerializeField] private GameObject _fourPawsAbilityTextCard;
    [SerializeField] private GameObject _soulReleaseAbilityTextCard;
    [SerializeField] private GameObject _parryingAbilityTextCard;
    [SerializeField] private GameObject _piercingClawAbilityTextCard;
    [SerializeField] private GameObject _doubleJumpAbilityTextCard;

    [Header("Currency Text")]
    [SerializeField] private TMP_Text _moneyText;
    [SerializeField] private TMP_Text _crystalText;

    [Header("Errors text")]
    [SerializeField] private GameObject _areaErrorText;

    private MoneyCounter _moneyCounter;
    private SoulCrystalCounter _soulCrystalCounter;
    private Interact _canInteract;
    private InstantiateParticles _instantiateParticles;

    private CostAbilitiesCheck _dashCostCheck;
    private CostAbilitiesCheck _wallRunCostCheck;
    private CostAbilitiesCheck _runCostCheck;
    private CostAbilitiesCheck _damageDashCostCheck;

    private EventSystem _eventSystem;

    private PlayerModel _playerModel;
    private PlayerMana _playerMana;

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

        _playerMana = FindAnyObjectByType<PlayerMana>();

        _eventSystem = EventSystem.current;
        _playerModel = PlayerView.Instance.PlayerModel;
        _instantiateParticles = GetComponent<InstantiateParticles>();

        _moneyCounter = GameObject.FindAnyObjectByType<MoneyCounter>();
        _soulCrystalCounter = GameObject.FindAnyObjectByType<SoulCrystalCounter>();
        _canInteract = GameObject.FindAnyObjectByType<Interact>();

        _dashCostCheck = _dashButton.GetComponent<CostAbilitiesCheck>();
        _wallRunCostCheck = _wallRunButton.GetComponent<CostAbilitiesCheck>();
        _damageDashCostCheck = _damageDashButton.GetComponent<CostAbilitiesCheck>();


        //_dashAbilityImage = (Image)_dashButton.targetGraphic;
        //_wallRunAbilityImage = (Image)_wallRunButton.targetGraphic;
        //_runAbilityImage = (Image)_runButton.targetGraphic;
        //_damageDashAbilityImage = (Image)_damageDashButton.targetGraphic;

        InitializeNodes();

        _dashBuyButton.onClick.AddListener(() =>
        {
            if (_canInteract.AbilitiesTree)
            {
                if (_dashCostCheck.canBuy && !_playerModel.HasDash)
                {
                    BuyDash();
                }
            }
        });
        _wallRunBuyButton.onClick.AddListener(() =>
        {
            if (_canInteract.AbilitiesTree)
            {
                if (_wallRunCostCheck.canBuy && !_playerModel.HasWallRun
                                        && _playerModel.HasDash)
                {
                    BuyWallRun();
                }
            }
        });

        _damageDashBuyButton.onClick.AddListener(() =>
        {
            if (_canInteract.AbilitiesTree)
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
        EnableAreaErrorText(!_canInteract.AbilitiesTree);

        SetActiveAbilityText(_dashAbilityTextCard, IsSelected(_dashButton));
        SetActiveAbilityText(_wallRunAbilityTextCard, IsSelected(_wallRunButton));
        SetActiveAbilityText(_damageDashAbilityTextCard, IsSelected(_damageDashButton));
        SetActiveAbilityText(_chargedAttackAbilityTextCard, IsSelected(_chargedAttackButton));
        SetActiveAbilityText(_fourPawsAbilityTextCard, IsSelected(_fourPawsButton));
        SetActiveAbilityText(_soulReleaseAbilityTextCard, IsSelected(_soulReleaseButton));
        SetActiveAbilityText(_parryingAbilityTextCard, IsSelected(_parryingButton));
        SetActiveAbilityText(_piercingClawAbilityTextCard, IsSelected(_piercingClawButton));
        SetActiveAbilityText(_doubleJumpAbilityTextCard, IsSelected(_doubleJumpButton));

    }

    public void BuyNode()
    {
        if (IsSelected(_dashButton))
        {
            _dashBuyButton.onClick.Invoke();
        }
        else if (IsSelected(_wallRunButton))
        {
            _wallRunBuyButton.onClick.Invoke();
        }
        else if (IsSelected(_damageDashButton))
        {
            _damageDashBuyButton.onClick.Invoke();
        }
    }

    #region Is Selected Node
    //private bool IsSelectedDash()
    //{
    //    return IsSelected(_dashButton) || IsSelected(_dashBuyButton);
    //}
    //private bool IsSelectedWallRun()
    //{
    //    return IsSelected(_wallRunButton) || IsSelected(_wallRunBuyButton);
    //}
    //private bool IsSelectedDamageDash()
    //{
    //    return IsSelected(_damageDashButton) || IsSelected(_damageDashBuyButton);
    //}
    //private bool IsSelectedChargedAttack()
    //{
    //    return IsSelected(_chargedAttackButton) || IsSelected(_chargedAttackBuyButton);
    //}
    //private bool IsSelectedFourPaws()
    //{
    //    return IsSelected(_fourPawsButton) || IsSelected(_fourPawsBuyButton);
    //}
    //private bool IsSelectedSoulReleased()
    //{
    //    return IsSelected(_soulReleaseButton) || IsSelected(_soulReleaseBuyButton);
    //}
    //private bool IsSelectedParrying()
    //{
    //    return IsSelected(_parryingButton) || IsSelected(_parryingBuyButton);
    //}
    //private bool IsSelectedPiercingClaw()
    //{
    //    return IsSelected(_piercingClawButton) || IsSelected(_damageDashBuyButton);
    //}
    //private bool IsSelectedDamageDash()
    //{
    //    return IsSelected(_damageDashButton) || IsSelected(_damageDashBuyButton);
    //}
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
        //if (_playerModel.HasRun)
        //{
        //    //GetRunTreeState();
        //}
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
        //_soulCrystalCounter.SpendCrystal(_dashCostCheck.CrystalCost);
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
        //_soulCrystalCounter.SpendCrystal(_wallRunCostCheck.CrystalCost);
        _moneyCounter.SpendMoney(_wallRunCostCheck.MoneyCost);
        UpdateCurrencyText();

        GetWallRunTreeState();
        _instantiateParticles.InstantiateNodePollen(_wallRunButton.transform.position);
        //_eventSystem.SetSelectedGameObject(_runButton.gameObject);

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

    //private void BuyRun()
    //{
    //    _playerModel.SetHasRun();
    //    //_soulCrystalCounter.SpendCrystal(_runCostCheck.CrystalCost);
    //    _moneyCounter.SpendMoney(_runCostCheck.MoneyCost);
    //    UpdateCurrencyText();

    //    GetRunTreeState();
    //    _instantiateParticles.InstantiateNodePollen(_runButton.transform.position);
    //    _eventSystem.SetSelectedGameObject(_damageDashButton.gameObject);

    //    SaveSystem.CrystalSave();
    //    SaveSystem.MoneySave();
    //}

    //private void GetRunTreeState()
    //{
    //    _runCostCheck.HideCost();
    //    _runBuyButton.gameObject.SetActive(false);

    //    _runAbilityImage.color = _getColor;
    //    _damageDashAbilityImage.color = _canGetColor;

    //    _runNodeFrameImage.color= _getFrameColor;
    //    _damageDashNodeFrameImage.color = _canGetFrameColor;
    //}

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
        if (_playerMana.isActiveAndEnabled)
        {
            _manaBar = InitializeManager.Instance.manaBar;
            Debug.Log($"Abilities Tree {_manaBar}");
            _playerMana.enabled = true;
            _manaBar.gameObject.SetActive(true);
        }

        _playerModel.SetHasDamageDash();
        //_soulCrystalCounter.SpendCrystal(_damageDashCostCheck.CrystalCost);
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
