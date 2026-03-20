using GlobalEnums;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI;

public class PlayerView : MonoBehaviour
{
    private static PlayerView instance;
    public static PlayerView Instance { get { return instance; } }

    #region SerializeFields

    //[Header("Layers")]
    //[SerializeField] private LayerMask whatIsGround;

    [Header("Parameters")]
    [SerializeField] private float _invisibleTime = 1.3f;
    [SerializeField] private float _damageFlashSpeed = 5.5f;
    [SerializeField] private float _knockbackForce = 10f;
    [SerializeField] private float _vignetteSpeed = 0.8f;
    [SerializeField] private float _vignetteMinValue = 0f;
    [SerializeField] private float _vignetteMaxValue = 0.7f;

    //[Header("Events")]
    //[Space]
    //[SerializeField] private UnityEvent OnFallEvent;
    //[SerializeField] private UnityEvent OnLandEvent;

    #endregion

    #region Variables

    private Rigidbody2D _rigidBody;
    private SpriteRenderer _spriteRenderer;
    private BoxCollider2D _playerCollider;
    private Vignette _vignette;
    private GameObject _globalVolumeInstance;
    private GameObject _manaBar;
    private Coroutine _vignetteCoroutine;

    private PlayerAnimation _playerAnimation;
    private PlayerAttack _playerAttack;
    private PlayerMove _playerMove;
    private PlayerInput _playerInput;
    private PlayerHeart _playerHeart;
    private PlayerMana _playerMana;
    private Interact _playerInteract;

    private bool _isInvincible = false;

    #endregion

    #region Properties
    public PlayerModel PlayerModel { get; set; }
    public PlayerAnimation PlayerAnimation { get { return _playerAnimation; } }

    #endregion

    #region Common Methods

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        _manaBar = InitializeManager.Instance.manaBar;

        _rigidBody = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _playerCollider = GetComponent<BoxCollider2D>();
        _playerAnimation = GetComponent<PlayerAnimation>();
        _playerAttack = GetComponent<PlayerAttack>();
        _playerMove = GetComponent<PlayerMove>();
        _playerInput = GetComponent<PlayerInput>();
        _playerHeart = GetComponent<PlayerHeart>();
        _playerInteract = GetComponent<Interact>();
        _playerMana = GetComponent<PlayerMana>();
    }

    private void Update()
    {
        if (_playerInput.AttackPressed)
            _playerAttack.Attack();
    }

    #endregion

    #region Heal & CheckPoint

    public void FullHeal()
    {
        //SetCheckPoint();
        MapManager.Instance.ShowAllOpenedRoomsAndWalls();
        PlayerModel.FullHeal();
        _playerHeart.AddHearts();
        _playerMana.FullMana();

        SaveSystem.Save();
    }

    public void SetCheckPoint(Vector3 checkPointPos)
    {
        //var curPos = SafeGroundSaver.Instance.SafeGroundLocation;
        //if (curPos == Vector3.zero)
        //    curPos = gameObject.transform.position;

        PlayerModel.SetCurrentScene(GameManager.Instance.CurrentScene);
        PlayerModel.SetCurrentPosition(checkPointPos.x, checkPointPos.y);
        
        PlayerModel.SetCheckPointScene(PlayerModel.CurrentScene);
        PlayerModel.SetCheckPointPosition(PlayerModel.CurPosX, PlayerModel.CurPosY);
    }

    #endregion

    #region ApplyDamage

    public void ApplyDamage(int damage, Vector3 position)
    {
        if (_isInvincible) return;

        _playerAnimation.SetBoolHit(true);
        PlayerModel.TakeDamage(damage);
        _playerHeart.RemoveHearts();

        var damageDir = Vector3.Normalize(transform.position - position) * 40f;
        _rigidBody.linearVelocity = Vector2.zero;
        _rigidBody.AddForce(damageDir * _knockbackForce);

        if (PlayerModel.IsDead)
            StartCoroutine(WaitToDead());
        else
        {
            ChangeVignette();
            StartCoroutine(Stun(0.25f));
            StartCoroutine(MakeInvincible(_invisibleTime));
        }
    }

    public void ApplyObjectDamage(int damage)
    {
        if (_isInvincible) return;

        _playerAnimation.SetBoolHit(true);
        PlayerModel.TakeDamage(damage);
        _playerHeart.RemoveHearts();

        if (PlayerModel.IsDead)
            StartCoroutine(WaitToDead());
        else
        {
            ChangeVignette();
            StartCoroutine(Stun(0.25f));
            StartCoroutine(MakeInvincible(_invisibleTime));
        }
    }

    private void SetFlashAmount(float flashAmount)
    {
        _spriteRenderer.material.SetFloat("_FlashAmount", flashAmount); // когда появится нормальный арт героя, скоррекировать
    }

    private void ChangeVignette()
    {
        if (_vignette == null)
        {
            var globalVolume = EntryPoint.Instance.GlobalVolumeInstance.GetComponent<Volume>();
            if (globalVolume != null)
            {
                globalVolume.profile.TryGet<Vignette>(out _vignette);
            }
        }
        if (_vignette == null) return;

        if (_vignetteCoroutine != null)
            StopCoroutine(_vignetteCoroutine);

        _vignetteCoroutine = StartCoroutine(ChangeVignetteRoutine());
    }

    #endregion

    #region IEnumerators

    private IEnumerator ChangeVignetteRoutine()
    {
        _vignette.active = true;

        var elapsedTime = 0f;
        while (elapsedTime < _vignetteSpeed)
        {
            elapsedTime += Time.deltaTime;
            _vignette.intensity.value = Mathf.Lerp(_vignetteMinValue, _vignetteMaxValue, elapsedTime / _vignetteSpeed);
            yield return null;
        }

        elapsedTime = 0f;
        while (elapsedTime < _vignetteSpeed)
        {
            elapsedTime += Time.deltaTime;
            _vignette.intensity.value = Mathf.Lerp(_vignetteMaxValue, _vignetteMinValue, elapsedTime / _vignetteSpeed);
            yield return null;
        }

        _vignette.intensity.value = 0f;
        _vignette.active = false;
        _vignetteCoroutine = null;
    }

    private IEnumerator Stun(float time)
    {
        _playerMove.CanMove = false;
        yield return new WaitForSeconds(time);
        _playerMove.CanMove = true;
    }

    private IEnumerator MakeInvincible(float time)
    {
        _isInvincible = true;
        gameObject.layer = LayerMask.NameToLayer("Invincible");
        StartCoroutine(FlashWhileInvicible(_damageFlashSpeed, time));
        yield return new WaitForSeconds(time);
        gameObject.layer = LayerMask.NameToLayer("Player");
        _isInvincible = false;
    }

    public IEnumerator FlashWhileInvicible(float flashSpeed, float flashTime)
    {
        float currentFlashAmount = 0f;
        float elapsedTime = 0f;

        while (elapsedTime < flashTime)
        {
            elapsedTime += Time.deltaTime;
            //Debug.Log($"В цикле, осталось {flashTime}");
            //_spriteRenderer.material.color = Color.Lerp(Color.white, Color.black, Mathf.PingPong(Time.time * flashSpeed, 1f));
            currentFlashAmount = Mathf.Lerp(0.03f, 1f, Mathf.PingPong(Time.time*flashSpeed, 1f));
            SetFlashAmount(currentFlashAmount);
            yield return null;
        }
        SetFlashAmount(1f);///////
        //_spriteRenderer.material.color = Color.white;
    }

    private IEnumerator WaitToDead()
    {
        _playerAnimation.SetBoolIsDead(true);
        _playerMove.CanMove = false;
        _isInvincible = true;
        _playerAttack.enabled = false;
        yield return new WaitForSeconds(0.4f);
        _rigidBody.linearVelocity = /*new Vector2(0, _rigidBody.linearVelocity.y);*/ Vector2.zero;
        yield return new WaitForSeconds(1.1f);
        SaveSystem.AutoSaveBeforePlayerDeath();
        GameManager.Instance.SetGameState(GameState.DEAD);
        GameManager.Instance.RevivalPlayer();
    }

    #endregion
}
