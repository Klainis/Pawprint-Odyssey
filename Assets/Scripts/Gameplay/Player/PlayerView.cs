using GlobalEnums;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerView : MonoBehaviour
{
    #region SerializeFields

    [Header("Layers")]
    [SerializeField] private LayerMask whatIsGround;

    [Header("Parameters")]
    [SerializeField] private float _invisibleTime = 1.3f;
    [SerializeField] private float _damageFlashSpeed = 5.5f;

    [Header("Events")]
    [Space]
    [SerializeField] private UnityEvent OnFallEvent;
    [SerializeField] private UnityEvent OnLandEvent;

    #endregion

    #region Variables

    private Rigidbody2D rigidBody;
    private SpriteRenderer _spriteRenderer;
    private PlayerAnimation playerAnimation;
    private PlayerAttack playerAttack;
    private PlayerMove playerMove;
    private PlayerInput playerInput;
    private PlayerHeart playerHeart;
    private PlayerMana playerMana;
    private Interact playerInteract;
    private BoxCollider2D _playerCollider;

    private static PlayerView instance;
    private GameObject _manaBar;

    private bool isInvincible = false;

    #endregion

    #region Properties

    public static PlayerView Instance { get { return instance; } }
    public PlayerModel PlayerModel { get; set; }
    public PlayerAnimation PlayerAnimation { get { return playerAnimation; } }

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

        rigidBody = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _playerCollider = GetComponent<BoxCollider2D>();
        playerAnimation = GetComponent<PlayerAnimation>();
        playerAttack = GetComponent<PlayerAttack>();
        playerMove = GetComponent<PlayerMove>();
        playerInput = GetComponent<PlayerInput>();
        playerHeart = GetComponent<PlayerHeart>();
        playerInteract = GetComponent<Interact>();
        playerMana = GetComponent<PlayerMana>();

        if (OnFallEvent == null) OnFallEvent = new UnityEvent();
        if (OnLandEvent == null) OnLandEvent = new UnityEvent();
    }

    private void Update()
    {
        if (playerInput.AttackPressed)
            playerAttack.Attack();

        playerMove.LastOnGroundTime -= Time.deltaTime;
        playerMove.LastPressedJumpTime -= Time.deltaTime;

        var wasGrounded = playerMove.IsGrounded;
        playerMove.IsGrounded = false;

        if (Physics2D.Raycast(playerMove.GroundCheck.position, Vector2.down, PlayerMove.groundCheckRadius, whatIsGround))
        {
            playerMove.IsGrounded = true;
            //playerMove.SetGroundedAirState();
            playerMove.ResetDashCounter();
            if (playerMove.IsJumping && playerMove.PlayerRigidbody.linearVelocity.y <= 0)
            {
                playerMove.IsJumping = false;
            }
            playerMove.LastOnGroundTime = playerMove.CoyoteTime;

            if (!wasGrounded)
            {
                OnLandEvent.Invoke();

                if (!playerMove.IsWall && !playerMove.IsDashing)
                    playerMove.PlayParticleJumpDown();

                playerMove.CanDoubleJump = true;

                if (rigidBody.linearVelocity.y < 0f)
                    playerMove.LimitVelOnWallJump = false;
            }
        }

        playerMove.WallHit = false;

        if (playerMove.IsSpeedRunning)
        {
            var leftHit = Physics2D.Raycast(playerMove.WallCheck.position, Vector2.left, PlayerMove.groundCheckRadius, whatIsGround);
            var rightHit = Physics2D.Raycast(playerMove.WallCheck.position, Vector2.right, PlayerMove.groundCheckRadius, whatIsGround);
            playerMove.WallHit = leftHit || rightHit;

            if (playerMove.WallHit)
            {
                playerMove.StopRunAfterHit();
            }
        }

        playerMove.IsWall = false;

        if (!playerMove.IsGrounded)
        {
            OnFallEvent.Invoke();

            var leftHit = Physics2D.Raycast(playerMove.WallCheck.position, Vector2.left, PlayerMove.groundCheckRadius, whatIsGround);
            var rightHit = Physics2D.Raycast(playerMove.WallCheck.position, Vector2.right, PlayerMove.groundCheckRadius, whatIsGround);
            playerMove.IsWall = leftHit || rightHit;

            if (playerMove.IsWall)
            {
                playerMove.ResetDashCounter();
                playerMove.IsDashing = false;
            }

            //playerMove.PrevVelocityX = rigidBody.linearVelocity.x;
        }

        //if (playerMove.LimitVelOnWallJump)
        //{
        //    if (rigidBody.linearVelocity.y < -0.5f)
        //        playerMove.LimitVelOnWallJump = false;

        //    playerMove.JumpWallDistX = (playerMove.JumpWallDistX - transform.position.x) * playerMove.TurnCoefficient;

        //    if (playerMove.JumpWallDistX < -0.5f && playerMove.JumpWallDistX > -1f)
        //        playerMove.CanMove = true;
        //    else if (playerMove.JumpWallDistX < -1f && playerMove.JumpWallDistX >= -2f)
        //    {
        //        playerMove.CanMove = true;
        //        rigidBody.linearVelocity = new Vector2(10f * playerMove.TurnCoefficient, rigidBody.linearVelocity.y);
        //    }
        //    else if (playerMove.JumpWallDistX < -2f)
        //    {
        //        playerMove.LimitVelOnWallJump = false;
        //        rigidBody.linearVelocity = new Vector2(0, rigidBody.linearVelocity.y);
        //    }
        //    else if (playerMove.JumpWallDistX > 0)
        //    {
        //        playerMove.LimitVelOnWallJump = false;
        //        rigidBody.linearVelocity = new Vector2(0, rigidBody.linearVelocity.y);
        //    }
        //}

        playerMove.ScaleJump();
    }

    #endregion

    #region Heal & CheckPoint

    public void FullHeal()
    {
        //SetCheckPoint();
        MapManager.Instance.ShowAllOpenedRoomsAndWalls();
        PlayerModel.FullHeal();
        playerHeart.AddHearts();
        playerMana.FullMana();

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
        if (isInvincible) return;

        playerAnimation.SetBoolHit(true);
        PlayerModel.TakeDamage(damage);
        playerHeart.RemoveHearts();

        var damageDir = Vector3.Normalize(transform.position - position) * 40f;
        rigidBody.linearVelocity = Vector2.zero;
        rigidBody.AddForce(damageDir * 15);

        if (PlayerModel.IsDead)
            StartCoroutine(WaitToDead());
        else
        {
            StartCoroutine(Stun(0.25f));
            StartCoroutine(MakeInvincible(_invisibleTime));
        }
    }

    public void ApplyObjectDamage(int damage)
    {
        if (isInvincible) return;

        playerAnimation.SetBoolHit(true);
        PlayerModel.TakeDamage(damage);
        playerHeart.RemoveHearts();

        if (PlayerModel.IsDead)
            StartCoroutine(WaitToDead());
        else
        {
            StartCoroutine(Stun(0.25f));
            StartCoroutine(MakeInvincible(_invisibleTime));
        }
    }

    private void SetFlashAmount(float flashAmount)
    {
        _spriteRenderer.material.SetFloat("_FlashAmount", flashAmount); // когда появится нормальный арт героя, скоррекировать
    }
    #endregion

    #region IEnumerators

    private IEnumerator Stun(float time)
    {
        playerMove.CanMove = false;
        yield return new WaitForSeconds(time);
        playerMove.CanMove = true;
    }

    private IEnumerator MakeInvincible(float time)
    {
        isInvincible = true;
        gameObject.layer = LayerMask.NameToLayer("Invincible");
        StartCoroutine(FlashWhileInvicible(_damageFlashSpeed, time));
        yield return new WaitForSeconds(time);
        gameObject.layer = LayerMask.NameToLayer("Player");
        isInvincible = false;
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
        playerAnimation.SetBoolIsDead(true);
        playerMove.CanMove = false;
        isInvincible = true;
        playerAttack.enabled = false;
        yield return new WaitForSeconds(0.4f);
        rigidBody.linearVelocity = /*new Vector2(0, _rigidBody.linearVelocity.y);*/ Vector2.zero;
        yield return new WaitForSeconds(1.1f);
        SaveSystem.AutoSaveBeforePlayerDeath();
        GameManager.Instance.SetGameState(GameState.DEAD);
        GameManager.Instance.RevivalPlayer();
    }

    #endregion
}
