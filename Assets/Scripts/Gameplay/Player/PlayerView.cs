using GlobalEnums;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerView : MonoBehaviour
{
    private static PlayerView instance;
    public static PlayerView Instance { get { return instance; } }

    public PlayerModel PlayerModel { get; set; }

    [Header("")]
    [SerializeField] private LayerMask whatIsGround;

    [Header("Events")]
    [Space]
    [SerializeField] private UnityEvent OnFallEvent;
    [SerializeField] private UnityEvent OnLandEvent;

    private Rigidbody2D rigidBody;
    private PlayerAnimation playerAnimation;
    private PlayerAttack playerAttack;
    private PlayerMove playerMove;
    private PlayerInput playerInput;
    private PlayerHeart playerHeart;
    private PlayerMana playerMana;
    private Interact playerInteract;

    private bool isInvincible = false;

    public PlayerAnimation PlayerAnimation { get { return playerAnimation; } }

    #region Common Methods

    private void Awake()
    {
        instance = this;

        rigidBody = GetComponent<Rigidbody2D>();
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
            playerMove.ResetDashCounter();
            playerMove.IsJumping = false;
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
                playerMove.IsJumping = false;
            }

            playerMove.PrevVelocityX = rigidBody.linearVelocity.x;
        }

        if (playerMove.LimitVelOnWallJump)
        {
            if (rigidBody.linearVelocity.y < -0.5f)
                playerMove.LimitVelOnWallJump = false;

            playerMove.JumpWallDistX = (playerMove.JumpWallDistX - transform.position.x) * playerMove.TurnCoefficient;

            if (playerMove.JumpWallDistX < -0.5f && playerMove.JumpWallDistX > -1f)
                playerMove.CanMove = true;
            else if (playerMove.JumpWallDistX < -1f && playerMove.JumpWallDistX >= -2f)
            {
                playerMove.CanMove = true;
                rigidBody.linearVelocity = new Vector2(10f * playerMove.TurnCoefficient, rigidBody.linearVelocity.y);
            }
            else if (playerMove.JumpWallDistX < -2f)
            {
                playerMove.LimitVelOnWallJump = false;
                rigidBody.linearVelocity = new Vector2(0, rigidBody.linearVelocity.y);
            }
            else if (playerMove.JumpWallDistX > 0)
            {
                playerMove.LimitVelOnWallJump = false;
                rigidBody.linearVelocity = new Vector2(0, rigidBody.linearVelocity.y);
            }
        }

        playerMove.ScaleJump();
    }

    //private void OnEnable()
    //{
    //    playerInteract.
    //}

    #endregion

    #region Heal

    public void Heal(int life)
    {
        // FullHeal игрока на конкретное количество HP
    }

    public void FullHeal()
    {
        PlayerModel.FullHeal();
        playerHeart.AddHearts();
        playerMana.FullMana();

        SaveSystem.Save();
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
            StartCoroutine(MakeInvincible(1f));
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
            StartCoroutine(MakeInvincible(1f));
        }
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
        yield return new WaitForSeconds(time);
        isInvincible = false;
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
        //SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        GameManager.Instance.SetGameState(GameState.DEAD);
        GameManager.Instance.RevivalPlayer();
    }

    #endregion
}
