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
    [Header("Layers")]
    [SerializeField] private LayerMask whatIsGround;

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
    [SerializeField] private float _jumpFallVelocityModifier = 1f;
    [SerializeField] private float _gravityOnJumpApexModifier = 0.7f;
    [SerializeField] private float _velocityModifierForJumpScale = 0.3f;
    [SerializeField] private float _gravityFallJumpModifier = 1f;
    [SerializeField] private float _gravityUpJumpModifier = 0.8f;
    [SerializeField] private float _jumpHorizantalVelocityModifier = 0.5f;
    [Space(5)]
    [SerializeField] private float limitFallSpeed = 25f;

    [Header("Particles")]
    [SerializeField] private ParticleSystem particleJumpUp;
    [SerializeField] private ParticleSystem particleJumpDown;

    [Header("")]
    [SerializeField][Range(0, 1f)] private float movementSmoothing;
    [SerializeField] private float timeToWaitAfterWallJump = 1;

    [Header("Check")]
    [SerializeField] private Transform groundCheck1;
    [SerializeField] private Transform groundCheck2;
    [SerializeField] private Transform groundCheck3;

    #endregion

    #region Variables

    public const float groundCheckRadius = 0.2f;
    
    private Rigidbody2D rb;
    //private PlayerView playerView;
    private PlayerAnimation playerAnimation;

    private Transform wallCheck;

    private Vector3 velocity = Vector3.zero;
    private float lastOnGroundTime;
    private float lastPressedJumpTime;
    private float prevVelocityX = 0f;
    private float jumpWallDistX = 0;
    private float _initialGravityScale;
    private int dashCounter = 0;
    private int turnCoefficient = 1;

    private bool isWall = false;
    private bool isGrounded;
    private bool isJumping;
    private bool isWallJumping = false;
    private bool isWallRunJumping = false;
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
    private bool canWallRun = true;

    //private float _fallSpeedYDampingChangeThreshold;

    #endregion

    #region Properties

    private bool CanJump { get { return (lastOnGroundTime > 0 && !isJumping); } }
    private bool CanAirDash { get { return dashCounter < 1; } }

    //public Transform GroundCheck { get { return groundCheck; } }
    public Transform WallCheck { get { return wallCheck; } }

    public int TurnCoefficient { get { return turnCoefficient; } set { turnCoefficient = value; } }
    public bool CanMove { get { return canMove; } set { canMove = value; } }

    public bool IsGrounded { get { return isGrounded; } set { isGrounded = value; } }
    public bool IsJumping { get { return isJumping; } set { isJumping = value; } }
    public bool IsDashing { get { return isDashing; } set { isDashing = value; } }

    public bool PlayerDoubleJumpEd { get; set; } = false;
    public bool IsWallJumping { get { return isWallJumping; } }

    #endregion

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        rb = GetComponent<Rigidbody2D>();
        playerAnimation = GetComponent<PlayerAnimation>();

        wallCheck = transform.Find("WallCheck");

        //_fallSpeedYDampingChangeThreshold = CameraManager.Instance.FallSpeedYDampingChangeTreshold;
    }

    private void Start()
    {
        _initialGravityScale = rb.gravityScale;
    }

    private void Update()
    {
        lastOnGroundTime -= Time.deltaTime;
        lastPressedJumpTime -= Time.deltaTime;

        var wasGrounded = isGrounded;
        isGrounded = false;

        if (Physics2D.Raycast(groundCheck1.position, Vector2.down, PlayerMove.groundCheckRadius, whatIsGround) ||
            Physics2D.Raycast(groundCheck2.position, Vector2.down, PlayerMove.groundCheckRadius, whatIsGround) ||
            Physics2D.Raycast(groundCheck3.position, Vector2.down, PlayerMove.groundCheckRadius, whatIsGround))
        {
            isGrounded = true;
            lastOnGroundTime = coyoteTime;

            ResetDashCounter();

            if (isJumping && rb.linearVelocity.y <= 0)
            {
                isJumping = false;
            }

            if (!wasGrounded)
            {
                playerAnimation.SetBoolIsJumping(false);//

                if (!isWall && !isDashing)
                    PlayParticleJumpDown();

                canDoubleJump = true;

                if (rb.linearVelocity.y < 0f)
                    limitVelocityOnWallJump = false;
            }
        }

        wallHit = false;

        if (isSpeedRunning)
        {
            var leftHit = Physics2D.Raycast(wallCheck.position, Vector2.left, PlayerMove.groundCheckRadius, whatIsGround);
            var rightHit = Physics2D.Raycast(wallCheck.position, Vector2.right, PlayerMove.groundCheckRadius, whatIsGround);
            wallHit = leftHit || rightHit;

            if (wallHit)
            {
                StopRunAfterHit();
            }
        }

        isWall = false;

        if (!isGrounded)
        {
            playerAnimation.SetBoolIsJumping(true);

            var leftHit = Physics2D.Raycast(wallCheck.position, Vector2.left, PlayerMove.groundCheckRadius, whatIsGround);
            var rightHit = Physics2D.Raycast(wallCheck.position, Vector2.right, PlayerMove.groundCheckRadius, whatIsGround);
            isWall = leftHit || rightHit;

            if (isWall)
            {
                ResetDashCounter();
                isDashing = false;
            }
        }

        ScaleJump();
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
    public void Movement(float moveY, float moveX, bool jump, bool dash, bool grab, bool speedRun) //�������������� � FixedUpdate � PlayerInput
    {
        if (!canMove) return;

        // --- DASH ---
        DoDash(dash);

        // --- RUN ---
        DoRun(speedRun, moveX);

        // --- MOVE ---
        DoMove(moveX);

        // --- JUMP ---
        DoJump(jump);
        //Debug.Log(lastPressedJumpTime);

        //Debug.Log(rb.gravityScale);
        if (!isGrounded &&
            !isWallSliding &&
            !isWallRunning)
        {
            SetPlayerAirState(AirState.Falling);

            if (isDashing)
            {
                return;
            }

            if ((isJumping || isWallJumping || isWallRunJumping))
            {
                if (rb.linearVelocity.y < 0)
                {
                    rb.gravityScale = _initialGravityScale * _gravityFallJumpModifier;
                }
                if (Mathf.Abs(rb.linearVelocity.y) <= 0.3)
                {
                    rb.gravityScale = _initialGravityScale * _gravityOnJumpApexModifier;
                }
                //else if (isJumping && Mathf.Abs(rb.linearVelocity.y) > 0.3)
                //{
                //    //rb.gravityScale = _initialGravityScale;
                //    rb.gravityScale = _initialGravityScale * _gravityJumpModifier;
                //}
                else if (rb.linearVelocity.y > 0.3)
                {
                    rb.gravityScale = _initialGravityScale * _gravityUpJumpModifier;
                }

                if (!isWallJumping && !isWallRunJumping && !isDashing)
                {
                    rb.linearVelocity = new Vector2(
                                                Mathf.Lerp(rb.linearVelocity.x, rb.linearVelocity.x * _jumpHorizantalVelocityModifier, Time.fixedDeltaTime * 10f),
                                                rb.linearVelocity.y);
                }
            }
        }
        else if (isWallSliding || isWallRunning)
        {
            rb.gravityScale = _initialGravityScale;
        }
        else if (isGrounded)
        {
            SetPlayerAirState(AirState.Grounded);
            isJumping = false;
            isWallJumping = false;
            isWallRunJumping = false;
            rb.gravityScale = _initialGravityScale;
        }

        if (rb.linearVelocity.y < -limitFallSpeed)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, -limitFallSpeed);

        //--- Camera Y Damping ---
        if (rb.linearVelocity.y < CameraManager.Instance.FallSpeedYDampingChangeTreshold && !CameraManager.Instance.IsLerpingYDamping && !CameraManager.Instance.LerpedFromPlayerFalling)
        {
            CameraManager.Instance.LerpYDamping(true);
        }
        if(rb.linearVelocity.y >= -0.01f && !CameraManager.Instance.IsLerpingYDamping && CameraManager.Instance.LerpedFromPlayerFalling)
        {
            CameraManager.Instance.LerpedFromPlayerFalling = false;
            
            CameraManager.Instance.LerpYDamping(false);
        }

        // --- WALL RUN ---
        if (canWallRun)
        {
            DoWallRun(grab, moveX, moveY);
        }

        // --- WALL SLIDING ---
        DoWallSlide(grab, moveX);
    }

    #region Movement Methods
    private void DoDash(bool dash)
    {
        if (PlayerView.Instance.PlayerModel.HasDash)
        {
            if (dash && canDash && !isWallSliding && isGrounded)
                StartCoroutine(DashCooldown());
            else if (dash && canDash && !isWallSliding && !isGrounded && CanAirDash)
                StartCoroutine(DashCooldown());
        }
    }

    private void DoRun(bool speedRun, float moveX)
    {
        if (PlayerView.Instance.PlayerModel.HasRun)
        {
            if (!isDashing && isGrounded && speedRun && canMove)
            {
                isSpeedRunning = true;
            }
            else
            {
                isSpeedRunning = false;
            }

            if (isSpeedRunning)
            {
                MoveHorizontal(moveX * speedRunModifier);
                //var targetRunVelocity = new Vector2(turnCoefficient * 18 * speedRunModifier, rb.linearVelocity.y);
                //rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, targetRunVelocity, movementSmoothing);

                //playerAnimation.SetFloatSpeed(Mathf.Abs(targetRunVelocity.x));
            }
        }
    }

    private void DoMove(float moveX)
    {
        if (!isDashing && canMove)
            MoveHorizontal(moveX);
    }

    private void DoJump(bool jump)
    {
        switch (_airState)
        {
            case AirState.WallSliding:
                if (jump && isWallSliding && isWall)
                {
                    WallJump();
                }
                break;

            case AirState.WallRunning:
                if (jump && isWallRunning && isWall)
                {
                    WallRunningJump();
                }
                break;

            case AirState.Grounded:
                if (lastPressedJumpTime > 0 && CanJump && canJump) //LastPressedTime ����� � ���������� ������� ������ ��������� ���� ����� jump
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
                else if (jump && canDoubleJump && canJump && PlayerView.Instance.PlayerModel.HasDoubleJump)
                {
                    DoubleJump();
                }
                break;
        }
    }

    private void DoWallRun(bool grab, float moveX, float moveY)
    {
        if (PlayerView.Instance.PlayerModel.HasWallRun)
        {
            if (grab)
                WallRunnig(moveY, moveX);
            else if (isWallRunning && !grab)
            {
                isWallRunning = false;
                oldWallRunning = false;
                HandleWallSliding(moveX);
                playerAnimation.SetBoolIsWallRunning(false);
            }
        }
    }

    private void DoWallSlide(bool grab, float moveX)
    {
        if (isWall && !isGrounded && !grab)
            HandleWallSliding(moveX);
        else if (isWallSliding && !isWall && canCheck)
        {
            isWallSliding = false;
            playerAnimation.SetBoolIsWallSliding(false);
            oldWallSliding = false;
            wallCheck.localPosition = new Vector3(Mathf.Abs(wallCheck.localPosition.x), wallCheck.localPosition.y, 0);
        }
    }
    #endregion

    private void MoveHorizontal(float move)
    {
        if (isKnockBack) return;

        if (limitVelocityOnWallJump) return;

        playerAnimation.SetFloatSpeed(Mathf.Abs(move));

        if ((isWallJumping || isWallRunJumping) && move == 0)
        {
            //var targetWallJumpVelocity = new Vector2(turnCoefficient * wallJumpForce.x * Time.fixedDeltaTime, rb.linearVelocity.y);

            //rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, targetWallJumpVelocity, 0.35f);
            return;
        }

        var targetVelocity = new Vector2(move * 10f, rb.linearVelocity.y);
        if (_airState == AirState.Grounded)
        {
            rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, targetVelocity, 1);
        }
        else if (_airState == AirState.Falling)
        {
            rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, targetVelocity, 0.35f);
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
    public void StopRunAfterHit()
    {
        Debug.Log("����� �����");
        isSpeedRunning = false;
        //isKnockBack = true;

        var damageDir = Vector2.right * (-turnCoefficient);
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(damageDir * 15, ForceMode2D.Impulse);
        StartCoroutine(WaintAndResetForce(0.05f));
        StartCoroutine(WaitToMove(0.8f));
        //isKnockBack = false;
    }

    private IEnumerator WaintAndResetForce(float time)
    {
        yield return new WaitForSeconds(time);
        rb.linearVelocity = Vector2.zero;
    }
    #endregion

    #region Wall Sliding & Runnig

    private void HandleWallSliding(float moveX)
    {
        SetPlayerAirState(AirState.WallSliding);
        if (!oldWallSliding && rb.linearVelocity.y < 0 && (!isWallRunning || isDashing))
        {
            isWallSliding = true;
            isJumping = false;
            isWallJumping = false;
            wallCheck.localPosition = new Vector3(-wallCheck.localPosition.x, wallCheck.localPosition.y, 0);
            Turn();
            StartCoroutine(WaitToCheck(0.1f));

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
                rb.linearVelocity = new Vector2(-turnCoefficient * 2, -5);
            }
        }
    }

    private void WallRunnig(float moveY, float moveX)
    {
        if (!isWall)
        {
            SetPlayerAirState(AirState.Falling);
            return;
        }

        if (!oldWallRunning && moveY > 0)
        {
            isWallRunning = true;
            isWallRunJumping = false;
            SetPlayerAirState(AirState.WallRunning);
            Turn();

            playerAnimation.SetBoolIsJumping(false);
            playerAnimation.SetBoolIsWallSliding(false);
            playerAnimation.SetBoolIsWallRunning(true);
        }

        if (isWallRunning)
        {
            isJumping = false;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, moveY * 10f);
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

        isJumping = true;
        playerAnimation.SetBoolIsJumping(true);
        playerAnimation.SetBoolJumpUp(true);
        isGrounded = false;

        var force = jumpForce;
        if (rb.linearVelocity.y < 0)
            force -= rb.linearVelocity.y;

        rb.AddForce(Vector2.up * force, ForceMode2D.Impulse);
        //rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

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
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        rb.AddForce(Vector2.up * doubleJumpForce, ForceMode2D.Impulse);
        playerAnimation.SetBoolIsDoubleJumping(true);
    }

    private void WallJump()
    {
        lastOnGroundTime = 0;
        lastPressedJumpTime = -1;
        SetPlayerAirState(AirState.Jumping);

        isWallSliding = false;
        playerAnimation.SetBoolIsWallSliding(false);
        oldWallSliding = false;
        wallCheck.localPosition = new Vector3(Mathf.Abs(wallCheck.localPosition.x), wallCheck.localPosition.y, 0);

        isWallJumping = true;


        playerAnimation.SetBoolIsJumping(true);
        playerAnimation.SetBoolJumpUp(true);

        var force = new Vector2(turnCoefficient * wallJumpForce.x, wallJumpForce.y);

        if (Mathf.Sign(rb.linearVelocity.x) != Mathf.Sign(force.x))
            force.x -= rb.linearVelocity.x;

        if (rb.linearVelocity.y < 0)
            force.y -= rb.linearVelocity.y;

        rb.AddForce(force, ForceMode2D.Impulse);

        limitVelocityOnWallJump = true;
        StartCoroutine(WaitToMoveAfterWallJump(timeToWaitAfterWallJump));
        canDoubleJump = true;
    }

    private void WallRunningJump()
    {
        lastOnGroundTime = 0;
        lastPressedJumpTime = 0;
        SetPlayerAirState(AirState.Jumping);

        isWallRunning = false;
        playerAnimation.SetBoolIsWallRunning(false);
        oldWallRunning = false;
        wallCheck.localPosition = new Vector3(Mathf.Abs(wallCheck.localPosition.x), wallCheck.localPosition.y, 0);

        canWallRun = false;
        StartCoroutine(WaitAfterWallRun());

        isWallRunJumping = true;

        playerAnimation.SetBoolIsJumping(true);
        playerAnimation.SetBoolJumpUp(true);

        var force = new Vector2(-turnCoefficient * wallJumpForce.x, wallJumpForce.y * 1.3f);

        if (Mathf.Sign(rb.linearVelocity.x) != Mathf.Sign(force.x))
            force.x -= rb.linearVelocity.x;

        if (rb.linearVelocity.y < 0)
            force.y -= rb.linearVelocity.y;

        rb.AddForce(force, ForceMode2D.Impulse);

        limitVelocityOnWallJump = true;
        StartCoroutine(WaitToMoveAfterWallJump(timeToWaitAfterWallJump));
        canDoubleJump = true;   
    }

    public void ScaleJump()
    {
        if (rb.linearVelocity.y > 0)
        {
            if (!jumpAction.action.IsPressed() && !isWallRunning)
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * _velocityModifierForJumpScale);
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
        {
            rb.gravityScale = 0f;
            rb.linearVelocity = new Vector2(turnCoefficient * dashForce, 0);
        }

        yield return new WaitForSeconds(0.15f); //0.1 
        PlayerAttack.Instance.CanAttack = true;
        isDashing = false;
        rb.gravityScale = _initialGravityScale;
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

    private IEnumerator WaitToMoveAfterWallJump(float time)
    {
        yield return new WaitForSeconds(time);
        limitVelocityOnWallJump = false;
    }

    private IEnumerator WaitAfterWallRun()
    {
        yield return new WaitForSeconds(0.1f);
        canWallRun = true;
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
        isWallSliding = false;
        playerAnimation.SetBoolIsWallSliding(false);
        oldWallSliding = false;
        wallCheck.localPosition = new Vector3(Mathf.Abs(wallCheck.localPosition.x), wallCheck.localPosition.y, 0);
    }

    #endregion
}
