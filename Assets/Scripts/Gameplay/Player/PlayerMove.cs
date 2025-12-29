using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    #region SerializeFields

    [Header("Forces")]
    [SerializeField] private float jumpForce;
    [SerializeField] private float doubleJumpForce;
    [SerializeField] private float dashForce;
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
    [SerializeField] private bool airControl = false;

    #endregion

    #region Variables

    public const float groundCheckRadius = 0.2f;
    
    private Rigidbody2D rigidBody;
    private PlayerView playerView;
    private PlayerAnimation playerAnimation;

    private Transform groundCheck;
    private Transform wallCheck;

    private Vector3 velocity = Vector3.zero;
    private float lastOnGroundTime;
    private float lastPressedJumpTime;
    private float prevVelocityX = 0f;
    private float jumpWallDistX = 0;
    private float limitFallSpeed = 25f;
    private int dashCounter = 0;
    private int turnCoefficient = 1;

    private bool isWall = false;
    private bool isGrounded;
    private bool isJumping;
    private bool isDashing = false;
    private bool isWallRunning;
    private bool oldWallRunning;
    private bool isWallSliding = false;
    private bool oldWallSliding = false;
    private bool canMove = true;
    private bool canDoubleJump = true;
    private bool canCheck = false;
    private bool canDash = true;
    private bool limitVelOnWallJump = false;
    private bool canAirJump = false;
    private bool canJump = true;

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
    public bool LimitVelOnWallJump { get { return limitVelOnWallJump; } set { limitVelOnWallJump = value; } }

    #endregion

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        playerView = GetComponent<PlayerView>();
        playerAnimation = GetComponent<PlayerAnimation>();

        groundCheck = transform.Find("GroundCheck");
        wallCheck = transform.Find("WallCheck");
    }

    public void ResetDashCounter()
    {
        dashCounter = 0;
    }

    #region Movement

    public void Move(float moveY, float moveX, bool jump, bool dash, bool grab)
    {
        //if (!canMove) return;

        // --- DASH ---
        if (playerView.PlayerModel.HasDash)
        {
            if (dash && canDash && !isWallSliding && isGrounded)
                StartCoroutine(DashCooldown());
            else if (dash && canDash && !isWallSliding && !isGrounded && CanAirDash)
                StartCoroutine(DashCooldown());
        }

        // --- MOVE ---
        if (!isDashing && canMove)
            MoveHorizontal(moveX);

        // --- JUMP ---
        if (lastPressedJumpTime > 0 && CanJump && canJump)
            Jump();
        else if (lastPressedJumpTime > 0 && canAirJump && canJump)
            AirJump();
        else if (lastPressedJumpTime > 0 && canDoubleJump && canJump && playerView.PlayerModel.HasDoubleJump)
            DoubleJump();

        // --- WALL RUN ---
        if (playerView.PlayerModel.HasWallRun)
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
        if (limitVelOnWallJump) return;

        if (!(isGrounded || airControl)) return;

        if (rigidBody.linearVelocity.y < -limitFallSpeed)
            rigidBody.linearVelocity = new Vector2(rigidBody.linearVelocity.x, -limitFallSpeed);

        var targetVelocity = new Vector2(move * 10f, rigidBody.linearVelocity.y);
        rigidBody.linearVelocity = Vector3.SmoothDamp(rigidBody.linearVelocity, targetVelocity, ref velocity, movementSmoothing);

        if (!isWallSliding)
        {
            if (move > 0 && !playerView.PlayerModel.FacingRight)
                Turn();
            else if (move < 0 && playerView.PlayerModel.FacingRight)
                Turn();
        }
    }

    private void Turn()
    {
        Vector3 rotator;
        if (playerView.PlayerModel.FacingRight)
        {
            rotator = new Vector3(transform.rotation.x, 180f, transform.rotation.z);
            turnCoefficient = -1;
        }
        else
        {
            rotator = new Vector3(transform.rotation.x, 0f, transform.rotation.z);
            turnCoefficient = 1;
        }
        playerView.PlayerModel.SetFacingRight(!playerView.PlayerModel.FacingRight);
        transform.rotation = Quaternion.Euler(rotator);
    }

    #endregion

    #region Wall Sliding & Runnig

    private void HandleWallSliding(float moveY, float moveX, bool jump, bool dash)
    {
        if (!oldWallSliding && rigidBody.linearVelocity.y < 0 && (!isWallRunning || isDashing))
        {
            isWallSliding = true;
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

        // Прыжок от стены
        if (jump && isWallSliding)
            WallJump();
    }

    private void WallRunnig(float moveY, float moveX, bool jump, bool dash)
    {
        if (!isWall) return;

        if (!oldWallRunning && moveY > 0)
        {
            isWallRunning = true;
            Turn();

            playerAnimation.SetBoolIsJumping(false);
            playerAnimation.SetBoolIsWallSliding(false);
            playerAnimation.SetBoolIsWallRunning(true);
        }

        if (isWallRunning)
        {
            rigidBody.linearVelocity = new Vector2(rigidBody.linearVelocity.x, moveY * 10f);
            oldWallRunning = true;
        }

        if (jump && isWallRunning)
        {
            WallRunningJump();
        }
    }

    #endregion

    #region Jump

    private void Jump()
    {
        lastOnGroundTime = 0;
        lastPressedJumpTime = 0;

        isJumping = true;
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

    private void AirJump()
    {
        if (!canAirJump || isWallSliding) return;

        canAirJump = false;
        canDoubleJump = true;

        rigidBody.linearVelocity = new Vector2(rigidBody.linearVelocity.x, 0f);
        rigidBody.AddForce(Vector2.up * doubleJumpForce, ForceMode2D.Impulse);

        playerAnimation.SetBoolIsDoubleJumping(true);
        lastPressedJumpTime = 0;
    }

    private void DoubleJump()
    {
        if (!canDoubleJump || isWallSliding) return;

        canDoubleJump = false;
        rigidBody.linearVelocity = new Vector2(rigidBody.linearVelocity.x, 0f);
        rigidBody.AddForce(Vector2.up * doubleJumpForce, ForceMode2D.Impulse);
        playerAnimation.SetBoolIsDoubleJumping(true);
    }

    private void WallJump()
    {
        lastOnGroundTime = 0;
        lastPressedJumpTime = 0;

        isJumping = true;

        playerAnimation.SetBoolIsJumping(true);
        playerAnimation.SetBoolJumpUp(true);

        rigidBody.linearVelocity = Vector2.zero;

        var force = new Vector2(turnCoefficient * wallJumpForce.x, wallJumpForce.y);

        rigidBody.AddForce(force, ForceMode2D.Impulse);

        limitVelOnWallJump = true;
        canAirJump = true;
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

        isJumping = true;

        playerAnimation.SetBoolIsJumping(true);
        playerAnimation.SetBoolJumpUp(true);

        rigidBody.linearVelocity = Vector2.zero;

        var force = new Vector2(turnCoefficient * wallJumpForce.x, wallJumpForce.y);

        rigidBody.AddForce(force, ForceMode2D.Impulse);

        limitVelOnWallJump = true;
        canAirJump = true;
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
        isDashing = true;
        canDash = false;
        canJump = false;

        if (isDashing)
            rigidBody.linearVelocity = new Vector2(turnCoefficient * dashForce, 0);

        yield return new WaitForSeconds(0.11f); //0.1 
        isDashing = false;
        canJump = true;
        gameObject.layer = LayerMask.NameToLayer("Player");
        rigidBody.linearVelocity = Vector3.Lerp(Vector3.zero, rigidBody.linearVelocity, 0.01f);
        yield return new WaitForSeconds(dashCoolDown); //0.25
        canDash = true;
    }

    private IEnumerator WaitToMove(float time)
    {
        canMove = false;
        yield return new WaitForSeconds(time);
        canMove = true;
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
