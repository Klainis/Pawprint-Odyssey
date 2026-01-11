using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerMove : MonoBehaviour
{
    private static PlayerMove instance;
    public static PlayerMove Instance {  get { return instance; } }

    enum AirState
    {
        Grounded,
        Falling,
        WallSliding,
        WallRunning,
        Jumping
    }
    private AirState _airState = AirState.Grounded;

    #region SerializeFields

    [Header("Forces")]
    [SerializeField] private float jumpForce;
    [SerializeField] private float doubleJumpForce;
    [SerializeField] private float dashForce;
    [SerializeField][Range(0.01f, 5f)] private float speedRunModifier;
    [SerializeField] private Vector2 wallJumpForce;

    [Header("Cooldowns")]
    [SerializeField] private float dashCoolDown = 0.3f;

    [Header("Actions")]
    [SerializeField] private InputActionReference jumpAction;

    [Header("Assists")]
    [SerializeField][Range(0.01f, 0.5f)] private float coyoteTime;
    [SerializeField][Range(0.01f, 10f)] private float jumpInputBufferTime;
    
    [Header("Particles")]
    [SerializeField] private ParticleSystem particleJumpUp;
    [SerializeField] private ParticleSystem particleJumpDown;

    [Header("")]
    [SerializeField][Range(0, 0.3f)] private float movementSmoothing;
    [SerializeField] private float gravityFallScale = 4;
    [SerializeField] private float timeToWaitAfterWallJump = 1;
    //[SerializeField] private bool airControl = true;

    #endregion

    #region Variables

    public const float groundCheckRadius = 0.2f;
    
    private Rigidbody2D rigidBody;
    //private PlayerView playerView;
    private PlayerAnimation playerAnimation;

    private Transform groundCheck;
    private Transform wallCheck;

    private Vector3 velocity = Vector3.zero;
    private float lastOnGroundTime;
    private float lastPressedJumpTime;
    private float prevVelocityX = 0f;
    private float airVelocityX = 30f;
    private float jumpWallDistX = 0;
    private float limitFallSpeed = 23f;
    private int dashCounter = 0;
    private int turnCoefficient = 1;

    private bool isWall = false;
    private bool isGrounded;
    private bool isJumping;
    private bool isWallJumping = false;
    private bool isSpeedRunning;
    private bool isDashing = false;
    private bool isWallRunning;
    private bool oldWallRunning;
    private bool isWallSliding = false;
    private bool oldWallSliding = false;
    private bool canMove = true;
    private bool canDoubleJump = true;
    private bool canCheck = false;
    private bool canDash = true;
    private bool limitVelocityOnWallJump = false;
    private bool canJump = true;
    private bool wallHit = false;
    private bool isKnockBack = false;
    private bool isTurnOld = false;

    #endregion

    #region Properties

    private bool CanJump { get { return (lastOnGroundTime > 0 && !isJumping); } }
    private bool CanAirDash { get { return dashCounter < 1; } }

    public Transform GroundCheck { get { return groundCheck; } }
    public Transform WallCheck { get { return wallCheck; } }
    public float CoyoteTime { get { return coyoteTime; } }
    public float LastOnGroundTime { get { return lastOnGroundTime; } set { lastOnGroundTime = value; } }
    public float LastPressedJumpTime { get { return lastPressedJumpTime; } set { lastPressedJumpTime = value; } }
    public float PrevVelocityX { get { return prevVelocityX; } set { prevVelocityX = value; } }
    public float JumpWallDistX { get { return jumpWallDistX; } set { jumpWallDistX = value; } }
    public int TurnCoefficient { get { return turnCoefficient; } set { turnCoefficient = value; } }
    public bool CanMove { get { return canMove; } set { canMove = value; } }
    public bool CanDoubleJump { get { return canDoubleJump; } set { canDoubleJump = value; } }
    public bool IsWall { get { return isWall; } set { isWall = value; } }
    public bool IsGrounded { get { return isGrounded; } set { isGrounded = value; } }
    public bool IsJumping { get { return isJumping; } set { isJumping = value; } }
    public bool IsDashing { get { return isDashing; } set { isDashing = value; } }
    public bool IsSpeedRunning { get { return isSpeedRunning; } set { isSpeedRunning = value; } }
    public bool WallHit { get { return wallHit; } set { wallHit = value; } }
    public bool LimitVelOnWallJump { get { return limitVelocityOnWallJump; } set { limitVelocityOnWallJump = value; } }
    public bool IsWallSliding { get; set; }
    public Rigidbody2D PlayerRigidbody { get { return rigidBody; } }
    public bool PlayerDoubleJumpEd { get; set; } = false;

    #endregion

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        rigidBody = GetComponent<Rigidbody2D>();
        //playerView = GetComponent<PlayerView>();
        playerAnimation = GetComponent<PlayerAnimation>();

        groundCheck = transform.Find("GroundCheck");
        wallCheck = transform.Find("WallCheck");
    }

    public void ResetDashCounter()
    {
        dashCounter = 0;
    }

    #region Player Air State
    private void SetPlayerAirState(AirState state)
    {
        _airState = state;
    }
    public void SetGroundedAirState()
    {
        _airState = AirState.Grounded;
    }
    public void SetFallingAirState()
    {
        _airState = AirState.Falling;
    }
    #endregion

    #region Movement
    public void Move(float moveY, float moveX, bool jump, bool dash, bool grab, bool speedRun) //Обрабатывается в Update в PlayerInput
    {
        if (!canMove) return;

        if (!isGrounded &&
            !isWallSliding &&
            !isWallRunning)
        {
            SetPlayerAirState(AirState.Falling);

            if (isJumping && rigidBody.linearVelocity.y < 0)
            {
                rigidBody.linearVelocityY -= 0.13f;
            }
            if (isJumping && Mathf.Abs(rigidBody.linearVelocity.y) < 0.3)
            {
                rigidBody.gravityScale = rigidBody.gravityScale * 0.7f;
            }
            else if (isJumping && Mathf.Abs(rigidBody.linearVelocity.y) >= 0.3)
            {
                rigidBody.gravityScale = 3f;
            }

            Debug.Log(rigidBody.gravityScale);
        }
        else if (isWallSliding || isWallRunning)
        {
            rigidBody.gravityScale = 3f;
        }
        else if (isGrounded)
        {
            SetPlayerAirState(AirState.Grounded);
            isWallJumping = false;
            rigidBody.gravityScale = 3f;
        }

        if (rigidBody.linearVelocity.y < -limitFallSpeed)
            rigidBody.linearVelocity = new Vector2(rigidBody.linearVelocity.x, -limitFallSpeed);


        // --- DASH ---
        if (PlayerView.Instance.PlayerModel.HasDash)
        {
            if (dash && canDash && !isWallSliding && isGrounded)
                StartCoroutine(DashCooldown());
            else if (dash && canDash && !isWallSliding && !isGrounded && CanAirDash)
                StartCoroutine(DashCooldown());
        }


        // --- RUN ---
        if (PlayerView.Instance.PlayerModel.HasRun)
        {
            //Отталкивание от стены во View
            if (!isDashing/* && isGrounded*/ && speedRun && canMove)
            {
                MoveHorizontal(moveX * speedRunModifier);
                isSpeedRunning = true;
            }
            else
            {
                isSpeedRunning= false;
            }
        }


        // --- MOVE ---
        if (!isDashing && canMove)
            MoveHorizontal(moveX);


        // --- JUMP ---
        switch (_airState)
        {
            case AirState.WallSliding:
                if (jump && isWallSliding)
                {
                    WallJump();
                }
                break;

            case AirState.WallRunning:
                if (jump && isWallRunning)
                {
                    WallRunningJump();
                }
                break;

            case AirState.Grounded:
                if (lastPressedJumpTime > 0 && CanJump && canJump) //LastPressedTime время с последнего нажатия прыжка выполняет роль флага jump
                {
                    //Debug.Log("Grounded Jump");
                    Jump();
                }
                break;

            case AirState.Falling:
                if (lastPressedJumpTime > 0 && CanJump && canJump)
                {
                    Jump();
                }
                else if (lastPressedJumpTime > 0 && canDoubleJump && canJump && PlayerView.Instance.PlayerModel.HasDoubleJump)
                {
                    DoubleJump();
                }
                break;
        }
        //Debug.Log(_airState);
        //Debug.Log(lastPressedJumpTime);

        // --- WALL RUN ---
        if (PlayerView.Instance.PlayerModel.HasWallRun)
        {
            if (grab)
                WallRunnig(moveY, moveX, jump, dash);
            else if (isWallRunning && !grab)
            {
                isWallRunning = false;
                oldWallRunning = false;
                HandleWallSliding(moveY, moveX, false, false);
                playerAnimation.SetBoolIsWallRunning(false);
            }
        }


        // --- WALL SLIDING ---
        if (isWall && !isGrounded && !grab)
            HandleWallSliding(moveY, moveX, jump, dash);
        else if (isWallSliding && !isWall && canCheck)
        {
            isWallSliding = false;
            playerAnimation.SetBoolIsWallSliding(false);
            oldWallSliding = false;
            wallCheck.localPosition = new Vector3(Mathf.Abs(wallCheck.localPosition.x), wallCheck.localPosition.y, 0);
            canDoubleJump = true;
        }
    }

    private void MoveHorizontal(float move)
    {
        if (isKnockBack) return;

        if (limitVelocityOnWallJump) return;

        playerAnimation.SetFloatSpeed(Mathf.Abs(move));

        if (isWallJumping && move == 0)
        {
            var targetWallJumpVelocity = new Vector2(turnCoefficient * wallJumpForce.x, rigidBody.linearVelocity.y);

            rigidBody.linearVelocity = Vector3.Lerp(rigidBody.linearVelocity, targetWallJumpVelocity, movementSmoothing);
        }

        var targetVelocity = new Vector2(move * 10f, rigidBody.linearVelocity.y);
        if (_airState == AirState.Grounded)
        {
            rigidBody.linearVelocity = Vector3.Lerp(rigidBody.linearVelocity, targetVelocity, movementSmoothing);
        }
        else if (_airState == AirState.Falling)
        {
            rigidBody.linearVelocity = Vector3.Lerp(rigidBody.linearVelocity, targetVelocity, movementSmoothing * 0.6f);
        }

        if (!isWallSliding)
        {
            if (isTurnOld)
                Turn();
            else if (move > 0 && !PlayerView.Instance.PlayerModel.FacingRight)
                Turn();
            else if (move < 0 && PlayerView.Instance.PlayerModel.FacingRight)
                Turn();
        }
    }

    private void Turn()
    {
        if (PlayerAttack.Instance.IsAttacking)
        {
            isTurnOld = true;
            return;
        }

        Vector3 rotator;
        if (PlayerView.Instance.PlayerModel.FacingRight)
        {
            rotator = new Vector3(transform.rotation.x, 180f, transform.rotation.z);
            turnCoefficient = -1;
        }
        else
        {
            rotator = new Vector3(transform.rotation.x, 0f, transform.rotation.z);
            turnCoefficient = 1;
        }
        isTurnOld = false;
        PlayerView.Instance.PlayerModel.SetFacingRight(!PlayerView.Instance.PlayerModel.FacingRight);
        transform.rotation = Quaternion.Euler(rotator);
    }

    #endregion

    #region Run
    //public void StopRunAfterHit()
    //{
    //    Debug.Log("Вызов стопа");
    //    isSpeedRunning = false;
    //    isKnockBack = true;

    //    var damageDir = Vector2.right * (-turnCoefficient);
    //    rigidBody.linearVelocity = Vector2.zero;
    //    rigidBody.AddForce(damageDir * 4, ForceMode2D.Impulse);
    //    StartCoroutine(WaitToMove(1f));
    //    //isKnockBack = false;
    //}
    #endregion

    #region Wall Sliding & Runnig

    private void HandleWallSliding(float moveY, float moveX, bool jump, bool dash)
    {
        SetPlayerAirState(AirState.WallSliding);
        if (!oldWallSliding && rigidBody.linearVelocity.y < 0 && (!isWallRunning || isDashing))
        {
            isWallSliding = true;
            isJumping = false;
            isWallJumping = false;
            wallCheck.localPosition = new Vector3(-wallCheck.localPosition.x, wallCheck.localPosition.y, 0);
            Turn();
            StartCoroutine(WaitToCheck(0.1f));
            canDoubleJump = true;

            playerAnimation.SetBoolIsJumping(false);
            playerAnimation.SetBoolIsWallRunning(false);
            playerAnimation.SetBoolIsWallSliding(true);
        }

        isDashing = false;

        if (isWallSliding)
        {
            if (moveX * turnCoefficient > 0.1f && oldWallSliding)
                StartCoroutine(WaitToEndSliding());
            else
            {
                oldWallSliding = true;
                rigidBody.linearVelocity = new Vector2(-turnCoefficient * 2, -5);
            }
        }
    }

    private void WallRunnig(float moveY, float moveX, bool jump, bool dash)
    {
        if (!isWall) return;

        if (!oldWallRunning && moveY > 0)
        {
            isWallRunning = true;
            SetPlayerAirState(AirState.WallRunning);
            Turn();

            playerAnimation.SetBoolIsJumping(false);
            playerAnimation.SetBoolIsWallSliding(false);
            playerAnimation.SetBoolIsWallRunning(true);
        }

        if (isWallRunning)
        {
            isJumping = false;
            rigidBody.linearVelocity = new Vector2(rigidBody.linearVelocity.x, moveY * 10f);
            oldWallRunning = true;
        }
    }

    #endregion

    #region Jump

    private void Jump()
    {
        lastOnGroundTime = 0;
        lastPressedJumpTime = 0;
        SetPlayerAirState(AirState.Jumping);

        IsJumping = true;
        playerAnimation.SetBoolIsJumping(true);
        playerAnimation.SetBoolJumpUp(true);
        isGrounded = false;

        var force = jumpForce;
        if (rigidBody.linearVelocity.y < 0)
            force -= rigidBody.linearVelocity.y;

        rigidBody.AddForce(Vector2.up * force, ForceMode2D.Impulse);

        canDoubleJump = true;
        PlayParticleJumpDown();
        PlayParticleJumpUp();
    }

    private void DoubleJump()
    {
        if (!canDoubleJump || isWallSliding) return;

        lastOnGroundTime = 0;
        lastPressedJumpTime = 0;

        PlayerDoubleJumpEd = true;

        IsJumping = true;
        lastPressedJumpTime = 0;
        SetPlayerAirState(AirState.Jumping);

        canDoubleJump = false;
        rigidBody.linearVelocity = new Vector2(rigidBody.linearVelocity.x, 0f);
        rigidBody.AddForce(Vector2.up * doubleJumpForce, ForceMode2D.Impulse);
        playerAnimation.SetBoolIsDoubleJumping(true);
    }

    private void WallJump()
    {
        lastOnGroundTime = 0;
        lastPressedJumpTime = -1;
        SetPlayerAirState(AirState.Jumping);

        isWallJumping = true;

        playerAnimation.SetBoolIsJumping(true);
        playerAnimation.SetBoolJumpUp(true);

        var force = new Vector2(turnCoefficient * wallJumpForce.x, wallJumpForce.y);

        if (Mathf.Sign(rigidBody.linearVelocity.x) != Mathf.Sign(force.x))
            force.x -= rigidBody.linearVelocity.x;

        if (rigidBody.linearVelocity.y < 0)
            force.y -= rigidBody.linearVelocity.y;

        rigidBody.AddForce(force, ForceMode2D.Impulse);

        limitVelocityOnWallJump = true;
        StartCoroutine(WaitAfterWallJump(timeToWaitAfterWallJump));
        canDoubleJump = false;

        isWallSliding = false;
        playerAnimation.SetBoolIsWallSliding(false);
        oldWallSliding = false;
        wallCheck.localPosition = new Vector3(Mathf.Abs(wallCheck.localPosition.x), wallCheck.localPosition.y, 0);
    }

    private void WallRunningJump()
    {
        lastOnGroundTime = 0;
        lastPressedJumpTime = 0;
        SetPlayerAirState(AirState.Jumping);

        IsJumping = true;

        playerAnimation.SetBoolIsJumping(true);
        playerAnimation.SetBoolJumpUp(true);

        rigidBody.linearVelocity = Vector2.zero;

        var force = new Vector2(-turnCoefficient * wallJumpForce.x, wallJumpForce.y);

        rigidBody.AddForce(force, ForceMode2D.Impulse);

        limitVelocityOnWallJump = true;
        canDoubleJump = false;

        isWallRunning = false;
        playerAnimation.SetBoolIsWallRunning(false);
        oldWallRunning = false;
        wallCheck.localPosition = new Vector3(Mathf.Abs(wallCheck.localPosition.x), wallCheck.localPosition.y, 0);
    }

    public void ScaleJump()
    {
        if (rigidBody.linearVelocity.y > 0)
        {
            if (!jumpAction.action.IsPressed() && !isWallRunning)
                rigidBody.linearVelocity = new Vector2(rigidBody.linearVelocity.x, 0);
        }
    }

    public void OnJumpInput()
    {
        lastPressedJumpTime = jumpInputBufferTime;
    }

    #endregion

    #region Particles

    public void PlayParticleJumpUp()
    {
        particleJumpUp.Play();
    }

    public void PlayParticleJumpDown()
    {
        particleJumpDown.Play();
    }

    #endregion

    #region IEnumerators

    private IEnumerator DashCooldown()
    {
        if (!isGrounded)
            dashCounter++;

        gameObject.layer = LayerMask.NameToLayer("PlayerDash");
        playerAnimation.SetBoolIsDashing(true);
        PlayerAttack.Instance.CanAttack = false;
        isDashing = true;
        canDash = false;
        canJump = false;

        if (isDashing)
            rigidBody.linearVelocity = new Vector2(turnCoefficient * dashForce, 0);

        yield return new WaitForSeconds(0.15f); //0.1 
        PlayerAttack.Instance.CanAttack = true;
        isDashing = false;
        PlayerAttack.Instance.SpendMana = true;
        canJump = true;
        gameObject.layer = LayerMask.NameToLayer("Player");
        yield return new WaitForSeconds(dashCoolDown); //0.25
        canDash = true;
    }

    private IEnumerator WaitToMove(float time)
    {
        canMove = false;
        yield return new WaitForSeconds(time);
        canMove = true;
    }

    private IEnumerator WaitAfterWallJump(float time)
    {
        yield return new WaitForSeconds(time);
        limitVelocityOnWallJump = false;
    }

    private IEnumerator WaitToCheck(float time)
    {
        canCheck = false;
        yield return new WaitForSeconds(time);
        canCheck = true;
    }

    private IEnumerator WaitToEndSliding()
    {
        yield return new WaitForSeconds(0.1f);
        canDoubleJump = true;
        isWallSliding = false;
        playerAnimation.SetBoolIsWallSliding(false);
        oldWallSliding = false;
        wallCheck.localPosition = new Vector3(Mathf.Abs(wallCheck.localPosition.x), wallCheck.localPosition.y, 0);
    }

    #endregion
}
