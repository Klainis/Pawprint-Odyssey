using GlobalEnums;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

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
    [SerializeField] private float _vignetteMaxValue = 0.7f;
    [SerializeField] private float _muffleSoundSpeed = 1.0f;
    [SerializeField] private float _muffleSoundMinValue = 0.3f;

    [Header("Particles")]
    [SerializeField] private ParticleSystem _damageParticle;

    //[Header("Events")]
    //[Space]
    //[SerializeField] private UnityEvent OnFallEvent;
    //[SerializeField] private UnityEvent OnLandEvent;

    #endregion

    #region Variables

    private Rigidbody2D _rigidBody;
    private SpriteRenderer _spriteRenderer;
    private BoxCollider2D _playerCollider;
    private GameObject _globalVolumeInstance;
    private GameObject _manaBar;
    
    private Vignette _vignette;
    private Coroutine _vignetteCoroutine;

    private AudioSource _music;
    private Coroutine _muffleSoundCoroutine;

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

        SpawnDamageParticles(damageDir.x);

        if (PlayerModel.IsDead)
            StartCoroutine(WaitToDead());
        else
        {
            IndicateApplyDamage();
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
            IndicateApplyDamage();
            StartCoroutine(Stun(0.25f));
            StartCoroutine(MakeInvincible(_invisibleTime));
        }
    }

    private void SetFlashAmount(float flashAmount)
    {
        _spriteRenderer.material.SetFloat("_FlashAmount", flashAmount); // когда появится нормальный арт героя, скоррекировать
    }

    #endregion

    #region Particles

    private void SpawnDamageParticles(float direction)
    {
        var vectorDirection = Vector2.zero;
        vectorDirection.x = direction >= 0 ? 1 : -1;
        
        var spawnRotation = Quaternion.FromToRotation(Vector2.right, vectorDirection);

        Instantiate(_damageParticle, transform.position, spawnRotation);
    }

    #endregion

    #region IndicateApplyDamage

    private void IndicateApplyDamage()
    {
        ChangeVignette();
        // stop frame
        MuffleSound();
        // particles
    }

    private void ChangeVignette()
    {
        if (_vignette == null)
        {
            if (EntryPoint.Instance.GlobalVolumeInstance.TryGetComponent<Volume>(out var globalVolume))
            {
                globalVolume.profile.TryGet<Vignette>(out _vignette);
            }
        }
        if (_vignette == null) return;

        if (_vignetteCoroutine != null)
            StopCoroutine(_vignetteCoroutine);

        _vignetteCoroutine = StartCoroutine(VignettePulseWrapper());
    }

    private void MuffleSound() // Приглушение звука
    {
        if (_music == null)
        {
            EntryPoint.Instance.MusicHandlerInstance.TryGetComponent<AudioSource>(out _music);
        }
        if (_music == null) return;

        if (_muffleSoundCoroutine != null)
            StopCoroutine(_muffleSoundCoroutine);

        _muffleSoundCoroutine = StartCoroutine(PulseValueRoutine(
            _muffleSoundSpeed, 1, _muffleSoundMinValue, 1, (v) => _music.volume = v)
        );
    }

    #endregion

    #region IEnumerators

    private IEnumerator VignettePulseWrapper()
    {
        _vignette.active = true;

        yield return StartCoroutine(PulseValueRoutine(
            _vignetteSpeed, 0, _vignetteMaxValue, 0, (v) => _vignette.intensity.value = v)
        );

        _vignette.active = false;
        _vignetteCoroutine = null;
    }

    private IEnumerator PulseValueRoutine(
        float speed, float minValue, float maxValue, float finishValue, System.Action<float> applyValue)
    // "Пульсация" значения
    {
        var elapsed = 0f;
        while (elapsed < speed)
        {
            elapsed += Time.deltaTime;
            applyValue(Mathf.Lerp(minValue, maxValue, elapsed / speed));
            yield return null;
        }

        elapsed = 0f;
        while (elapsed < speed)
        {
            elapsed += Time.deltaTime;
            applyValue(Mathf.Lerp(maxValue, minValue, elapsed / speed));
            yield return null;
        }

        applyValue(finishValue);
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
