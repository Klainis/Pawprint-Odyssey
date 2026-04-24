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

    [Header("Wall Run - Ledge Grab")]
    [SerializeField] private float ledgeCheckDistance = 0.3f;
    [SerializeField] private float ledgeHeadCheckHeight = 1f;
    [SerializeField] private float ledgeGrabSpeed = 5f;

    [Header("Particles")]
    [SerializeField] private ParticleSystem particleJumpUp;
    [SerializeField] private ParticleSystem particleJumpDown;
    [SerializeField] private GameObject particleDoubleJump;
    [SerializeField] private Transform particlePositionTransform;

    [Header("")]
    [SerializeField][Range(0, 1f)] private float movementSmoothing;
    [SerializeField] private float timeToWaitAfterWallJump = 1;

    [Header("Ground check")]
    [SerializeField] private Transform groundCheck1;
    [SerializeField] private Transform groundCheck2;
    [SerializeField] private Transform groundCheck3;

    [Header("Wall check")]
    [SerializeField] private Transform wallCheckLeft;
    [SerializeField] private Transform wallCheckRight;

    #endregion

    #region Variables

    public const float groundCheckRadius = 0.2f;
    
    private Rigidbody2D rb;
    //private PlayerView playerView;
    private PlayerAnimation playerAnimation;
    private PlayerSoulRelease playerSoulRelease;

    //private Transform wallCheck;

    private Vector3 velocity = Vector3.zero;
    private float lastOnGroundTime;
    private float lastPressedJumpTime;
    private float prevVelocityX = 0f;
    private float jumpWallDistX = 0;
    private float _initialGravityScale;
    private int dashCounter = 0;
    private int turnCoefficient = 1;
    private int _dashDirection = 1;

    private bool isWall = false;
    private bool isGrounded;
    private bool isJumping;
    private bool isDoubleJumping;
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
    private bool didJumpThisFrame = false;
    private bool canCheck = false;
    private bool canDash = true;
    private bool limitVelocityOnWallJump = false;
    private bool canJump = true;
    private bool wallHit = false;
    private bool isKnockBack = false;
    private bool isTurnOld = false;
    private bool canWallRun = true;
    private bool _isGrabbingLedge = false;

    //private float _fallSpeedYDampingChangeThreshold;

    #endregion

    #region Properties

    private bool CanJump { get { return (lastOnGroundTime > 0 && !isJumping); } }
    private bool CanAirDash { get { return dashCounter < 1; } }

    public bool IsWall { get { return isWall; } private set { } }

    public int TurnCoefficient { get { return turnCoefficient; } set { turnCoefficient = value; } }
    public bool CanMove { get { return canMove; } set { canMove = value; } }

    public bool IsGrounded { get { return isGrounded; } set { isGrounded = value; } }
    public bool IsJumping { get { return isJumping; } set { isJumping = value; } }
    public bool IsDashing { get { return isDashing; } set { isDashing = value; } }

    public bool PlayerDoubleJumpEd { get; set; } = false;
    public bool IsWallJumping { get { return isWallJumping; } }
    public bool IsWallRunJumping { get { return isWallRunJumping; } }

    #endregion

    private void OnDrawGizmos()
    {
        // Точка начала рейкаста
        Vector2 originLow = turnCoefficient == 1 ? wallCheckRight.position : wallCheckLeft.position;
        Vector2 originHight = turnCoefficient == 1 ? wallCheckRight.position + new Vector3(0, 1f) : wallCheckLeft.position + new Vector3(0, 1f);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(originLow, originLow + Vector2.right * turnCoefficient * ledgeCheckDistance);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(originHight, originHight + Vector2.right * turnCoefficient * ledgeCheckDistance);
        //Gizmos.DrawWireSphere(originLow + Vector2.down * ledgeCheckDistance, 0.05f);

        // Если есть хит — покажем нормаль
        //RaycastHit2D hit = Physics2D.Raycast(originLow, Vector2.down, ledgeCheckDistance, whatIsGround);
        //if (hit.collider != null)
        //{
        //    Gizmos.color = Color.green;
        //    Gizmos.DrawSphere(hit.point, 0.05f);
        //    // Рисуем нормаль (зелёная стрелка)
        //    Gizmos.DrawLine(hit.point, hit.point + hit.normal * 0.3f);
        //}
    }

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
        playerSoulRelease = GetComponent<PlayerSoulRelease>();

        //wallCheck = transform.Find("WallCheck");

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

        didJumpThisFrame = false;

        //Debug.Log($"Air State: {_airState}");
        //Debug.Log($"Is WallRunning: {isWallRunning}");
        //Debug.Log($"Is Wall: {isWall}");

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
                playerAnimation.SetBoolIsFall(false);

                //if (!isWall && !isDashing)
                //    PlayParticleJumpDown();

                canDoubleJump = true;

                if (rb.linearVelocity.y < 0f)
                    limitVelocityOnWallJump = false;
            }
        }

        wallHit = false;

        if (isSpeedRunning)
        {
            var leftHit = Physics2D.Raycast(wallCheckLeft.position, Vector2.left, PlayerMove.groundCheckRadius, whatIsGround);
            var rightHit = Physics2D.Raycast(wallCheckRight.position, Vector2.right, PlayerMove.groundCheckRadius, whatIsGround);
            wallHit = leftHit || rightHit;

            if (wallHit)
            {
                StopRunAfterHit();
            }
        }

        ScaleJump();
    }

    private void FixedUpdate()
    {
        isWall = false;

        if (!isGrounded)
        {
            playerAnimation.SetBoolIsJumping(true);
            if (rb.linearVelocity.y < 0)
            {
                playerAnimation.SetBoolIsFall(true);
            }

            var leftHit = Physics2D.Raycast(wallCheckLeft.position, Vector2.left, PlayerMove.groundCheckRadius, whatIsGround);
            var rightHit = Physics2D.Raycast(wallCheckRight.position, Vector2.right, PlayerMove.groundCheckRadius, whatIsGround);
            isWall = leftHit || rightHit;

            if (isWall)
            {
                ResetDashCounter();
                isDashing = false;
            }
        }
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
    #endregion

    #region Movement
    public void Movement(float moveY, float moveX, bool jump, bool dash, bool grab, bool speedRun) //�������������� � FixedUpdate � PlayerInput
    {
        if (!canMove) return;

        if (Mathf.Abs(moveX) > 0.1f)
            _dashDirection = (int)Mathf.Sign(moveX);
        else
            _dashDirection = PlayerView.Instance.PlayerModel.FacingRight ? 1 : -1;

        if (!isWallSliding && !isWallRunning)
        {
            if (isTurnOld)
                Turn();
            else if (moveX > 0 && !PlayerView.Instance.PlayerModel.FacingRight)
                Turn();
            else if (moveX < 0 && PlayerView.Instance.PlayerModel.FacingRight)
                Turn();
        }

        // --- DASH ---
        DoDash(dash);

        // --- RUN ---
        //DoRun(speedRun, moveX);

        // --- MOVE ---
        DoMove(moveX);

        // --- JUMP ---
        DoJump(jump);

        //// --- WALL RUN ---
        if (canWallRun)
        {
            //Debug.Log($"Is Wall: {isWall}");
            DoWallRun(grab, moveX, moveY);
        }

        //// --- WALL SLIDING ---
        DoWallSlide(grab, moveX);

        // --- FALLING STATE ---
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
            isDoubleJumping = false;
            isWallJumping = false;
            isWallRunJumping = false;
            isWallSliding = false;
            isWallRunning = false;
            rb.gravityScale = _initialGravityScale;
        }

        // --- Limit Fall Speed ---
        if (rb.linearVelocity.y < -limitFallSpeed)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, -limitFallSpeed);

        //--- Camera Y Damping ---
        if (rb.linearVelocity.y < CameraManager.Instance.FallSpeedYDampingChangeTreshold && !CameraManager.Instance.IsLerpingYDamping && !CameraManager.Instance.LerpedFromPlayerFalling)
        {
            CameraManager.Instance.LerpYDamping(true);
        }
        if(rb.linearVelocity.y >= 0f && !CameraManager.Instance.IsLerpingYDamping && CameraManager.Instance.LerpedFromPlayerFalling)
        {
            CameraManager.Instance.LerpedFromPlayerFalling = false;
            
            CameraManager.Instance.LerpYDamping(false);
        }
    }

    #region Movement Methods
    private void DoDash(bool dash)
    {
        if (PlayerView.Instance.PlayerModel.HasDash)
        {
            if (dash && canDash && !isWallSliding && (isGrounded || (!isGrounded && CanAirDash)))
            {
                if (limitVelocityOnWallJump)
                {
                    limitVelocityOnWallJump = false;
                }
                StartCoroutine(DashCooldown());
            }
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
        if (didJumpThisFrame) return;

        switch (_airState)
        {
            case AirState.WallSliding:
                if (jump && isWallSliding && isWall)
                {
                    WallJump();
                    didJumpThisFrame = true;
                }
                break;

            case AirState.WallRunning:
                if (jump && isWallRunning && isWall)
                {
                    WallRunningJump();
                    didJumpThisFrame = true;
                }
                break;

            case AirState.Grounded:
                if (lastPressedJumpTime > 0 && CanJump && canJump) //LastPressedTime ����� � ���������� ������� ������ ��������� ���� ����� jump
                {
                    //Debug.Log("Grounded Jump");
                    Jump();
                    didJumpThisFrame = true;
                }
                break;

            case AirState.Falling:
                if (lastOnGroundTime > 0 && jump && CanJump && canJump)
                {
                    Jump();
                    didJumpThisFrame = true;
                }
                else if (jump && canDoubleJump && canJump && PlayerView.Instance.PlayerModel.HasDoubleJump)
                {
                    DoubleJump();
                    didJumpThisFrame = true;
                }
                break;
        }
    }

    private void DoWallRun(bool grab, float moveX, float moveY)
    {
        if (PlayerView.Instance.PlayerModel.HasWallRun)
        {
            if (isWallRunJumping) return;

            if (isWall && !isGrounded && grab)
                WallRunning(moveY, moveX);
            else if (isWallRunning && !grab)
            {
                isWallRunning = false;
                oldWallRunning = false;
                //HandleWallSliding(moveX);
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
            //wallCheck.localPosition = new Vector3(Mathf.Abs(wallCheck.localPosition.x), wallCheck.localPosition.y, 0);
        }
    }
    #endregion

    private void MoveHorizontal(float move)
    {
        if (isKnockBack || isWallRunning || limitVelocityOnWallJump) return;

        playerAnimation.SetFloatSpeed(Mathf.Abs(move));

        if ((isWallJumping || isWallRunJumping) && move == 0)
        {
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
        CameraFollowObject.Instance.CallCameraTurn();
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
        if (isWallRunning && isDashing) return;

        if (!oldWallSliding && rb.linearVelocity.y < 0/* && (!isWallRunning || isDashing)*/)
        {
            SetPlayerAirState(AirState.WallSliding);

            isWallSliding = true;
            isJumping = false;
            isDoubleJumping = false;
            isWallJumping = false;
            isWallRunJumping = false;
            //wallCheck.localPosition = new Vector3(-wallCheck.localPosition.x, wallCheck.localPosition.y, 0);
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

    private void WallRunning(float moveY, float moveX)
    {
        if (isWallRunJumping) return;

        bool currentWallCheck = false;
        if (!isGrounded)
        {
            var leftHit = Physics2D.Raycast(wallCheckLeft.position, Vector2.left, groundCheckRadius, whatIsGround);
            var rightHit = Physics2D.Raycast(wallCheckRight.position, Vector2.right, groundCheckRadius, whatIsGround);
            currentWallCheck = leftHit || rightHit;
        }

        if (moveY > 0.1f)
        {
            if (TryGrabLedge())
            {
                Debug.Log("Забрались на уступ");
                return;
            }
        }

        if (!currentWallCheck)
        {
            isWallRunning = false;
            oldWallSliding = false;
            //SetPlayerAirState(AirState.Falling);
            playerAnimation.SetBoolIsWallSliding(false);
            playerAnimation.SetBoolIsWallRunning(false);
            return;
        }

        //if (moveY > 0.1f)
        //{
        //    if (TryGrabLedge())
        //    {
        //        return;
        //    }
        //}

        if (!oldWallRunning && moveY > 0)
        {
            isWallSliding = false;
            oldWallSliding = false;
            isWallRunning = true;
            isWallRunJumping = false;
            isWallJumping = false;
            SetPlayerAirState(AirState.WallRunning);
            Turn();

            playerAnimation.SetBoolIsJumping(false);
            playerAnimation.SetBoolIsWallSliding(false);
            playerAnimation.SetBoolIsWallRunning(true);
        }

        if (isWallRunning)
        {
            isJumping = false;
            isDoubleJumping = false;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, moveY * 13f);
            oldWallRunning = true;
        }
    }

    #endregion

    #region Climbing

    private bool TryGrabLedge()
    {
        if (_isGrabbingLedge) return false;

        Vector2 ledgeCheckOriginLow = wallCheckRight.position;
        Vector2 ledgeCheckOriginHight = wallCheckRight.position + new Vector3(0, 1f);
        if (turnCoefficient == 1)
        {
            ledgeCheckOriginLow = wallCheckRight.position;
            ledgeCheckOriginHight = wallCheckRight.position + new Vector3(0, 1f);
        }
        else
        {
            ledgeCheckOriginLow = wallCheckLeft.position;
            ledgeCheckOriginHight = wallCheckLeft.position + new Vector3(0, 0.5f);
        }
        //Debug.Log(ledgeCheckOriginLow);
        bool hitWallLow = Physics2D.Raycast(ledgeCheckOriginLow, Vector2.right * turnCoefficient, ledgeCheckDistance, whatIsGround);
        bool hitWallHight = Physics2D.Raycast(ledgeCheckOriginHight, Vector2.right * turnCoefficient, ledgeCheckDistance, whatIsGround);

        Debug.Log($"Hight  {hitWallHight}");
        Debug.Log($"Low  { hitWallLow}");

        if (hitWallLow && !hitWallHight)
        {
            //if (Mathf.Abs(hitWallLow.normal.x) > 0.7f)
            //{
            //    Debug.Log("Вертикальная поверхность");
            //    return false;
            //}

            //if (hitWallLow.normal.y > 0.7f)
            //{
            Debug.Log("Горизонтальная поверхность");
            Vector2 headCheckStart = new Vector2(ledgeCheckOriginLow.x + turnCoefficient * 0.1f, ledgeCheckOriginLow.y * 0.4f);

            if (Physics2D.Raycast(headCheckStart, Vector2.up, ledgeHeadCheckHeight, whatIsGround)) //не помещаемсся
            {
                Debug.Log("Игрок не взалет");
                return false;
            }

            StartCoroutine(GrabLedgeCoroutine(turnCoefficient, headCheckStart.y));
            return true;
            //}
        }

        return false;
    }

    private IEnumerator GrabLedgeCoroutine(float directionX, float targetY)
    {
        _isGrabbingLedge = true;

        Debug.Log("Зашли в корутину залазанья");

        float savedGravity = rb.gravityScale;
        rb.gravityScale = 0f;

        canMove = false;
        isWallRunning = false;
        playerAnimation.SetBoolIsWallRunning(false);

        Vector3 startPos = transform.position;
        Vector3 targetPos = new Vector3(
            startPos.x + directionX * 0.5f,
            targetY - 0.9f,
            startPos.z
        );

        float elapsed = 0f;
        float duration = 0.3f; // Время анимации подтягивания

        //playerAnimation.SetBoolIsClimbing(true);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            transform.position = Vector3.Lerp(startPos, targetPos, elapsed / duration);
            yield return null;
        }

        transform.position = targetPos;
        //playerAnimation.SetBoolIsClimbing(false);

        //isGrounded = true;
        //lastOnGroundTime = coyoteTime;
        //SetPlayerAirState(AirState.Grounded);

        _isGrabbingLedge = false;
        rb.gravityScale = savedGravity;

        rb.linearVelocity = Vector2.zero;
        canMove = true;

        StartCoroutine(WaitAfterWallRun());
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
        playerAnimation.SetBoolIsFall(false);
        isGrounded = false;

        //var force = jumpForce;
        //if (rb.linearVelocity.y < 0)
        //    force -= rb.linearVelocity.y;

        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        }

        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

        //canDoubleJump = true;
        StartCoroutine(WaitAfterJumpForDoubleJump(0.05f));
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
        isDoubleJumping = true;
        lastPressedJumpTime = 0;
        SetPlayerAirState(AirState.Jumping);

        canDoubleJump = false;

        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        }

        rb.AddForce(Vector2.up * doubleJumpForce, ForceMode2D.Impulse);

        PlatParticleDoubleJump();

        playerAnimation.SetBoolIsDoubleJumping(true);
        playerAnimation.SetBoolIsFall(false);
    }

    private void WallJump()
    {
        lastOnGroundTime = 0;
        lastPressedJumpTime = -1;
        SetPlayerAirState(AirState.Jumping);

        isWallSliding = false;
        playerAnimation.SetBoolIsWallSliding(false);
        oldWallSliding = false;
        //wallCheck.localPosition = new Vector3(Mathf.Abs(wallCheck.localPosition.x), wallCheck.localPosition.y, 0);

        isWallJumping = true;

        playerAnimation.SetBoolIsJumping(true);
        playerAnimation.SetBoolJumpUp(true);
        playerAnimation.SetBoolIsFall(false);

        rb.linearVelocity = Vector2.zero;

        var force = new Vector2(turnCoefficient * wallJumpForce.x, wallJumpForce.y);

        if (Mathf.Sign(rb.linearVelocity.x) != Mathf.Sign(force.x))
            force.x -= rb.linearVelocity.x;

        rb.AddForce(force, ForceMode2D.Impulse);

        limitVelocityOnWallJump = true;
        StartCoroutine(WaitToMoveAfterWallJump(timeToWaitAfterWallJump));
        canDoubleJump = true;

        PlayParticleJumpUp();
    }

    private void WallRunningJump()
    {
        lastOnGroundTime = 0;
        lastPressedJumpTime = -1;
        SetPlayerAirState(AirState.Jumping);

        isWallRunning = false;
        playerAnimation.SetBoolIsWallRunning(false);
        oldWallRunning = false;
        //wallCheck.localPosition = new Vector3(Mathf.Abs(wallCheck.localPosition.x), wallCheck.localPosition.y, 0);

        canWallRun = false;
        StartCoroutine(WaitAfterWallRun());

        isWallRunJumping = true;

        playerAnimation.SetBoolIsJumping(true);
        playerAnimation.SetBoolJumpUp(true);
        playerAnimation.SetBoolIsFall(false);

        rb.linearVelocity = Vector2.zero;

        //напрачление прыжка от стены 
        float jumpDirX = -turnCoefficient;
        if (Physics2D.Raycast(wallCheckLeft.position, Vector2.left, groundCheckRadius, whatIsGround))
            jumpDirX = 1f;
        else if (Physics2D.Raycast(wallCheckRight.position, Vector2.right, groundCheckRadius, whatIsGround))
            jumpDirX = -1f;

        var force = new Vector2(jumpDirX * wallJumpForce.x, wallJumpForce.y * 1.05f);

        if (Mathf.Sign(rb.linearVelocity.x) != Mathf.Sign(force.x))
            force.x -= rb.linearVelocity.x;

        rb.AddForce(force, ForceMode2D.Impulse);

        Turn();

        limitVelocityOnWallJump = true;
        StartCoroutine(WaitToMoveAfterWallJump(timeToWaitAfterWallJump));
        canDoubleJump = true;

        PlayParticleJumpUp();
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

    public void PlatParticleDoubleJump()
    {
        Instantiate(particleDoubleJump, particlePositionTransform.position, particleDoubleJump.transform.rotation);
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
            rb.linearVelocity = new Vector2(_dashDirection * dashForce, 0);
        }

        yield return new WaitForSeconds(0.15f); //0.1 
        rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, Vector3.zero, 1);
        PlayerAttack.Instance.CanAttack = true;
        rb.gravityScale = _initialGravityScale;
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

    private IEnumerator WaitToMoveAfterWallJump(float time)
    {
        yield return new WaitForSeconds(time);
        limitVelocityOnWallJump = false;
    }

    private IEnumerator WaitAfterWallRun()
    {
        yield return new WaitForSeconds(0.05f);
        canWallRun = true;
    }

    private IEnumerator WaitAfterJumpForDoubleJump(float time)
    {
        yield return new WaitForSeconds(time);
        canDoubleJump = true;
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
        //wallCheck.localPosition = new Vector3(Mathf.Abs(wallCheck.localPosition.x), wallCheck.localPosition.y, 0);
    }

    #endregion
}
