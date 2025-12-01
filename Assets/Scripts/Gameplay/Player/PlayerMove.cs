using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    [Header("Forces")]
    [SerializeField] private float jumpForce = 15f;
    [SerializeField] private float doubleJumpForce = 13f;
    [SerializeField] private float dashForce = 25f;
    [SerializeField] private Vector2 wallJumpForce = new Vector2(800f, 700f);

    [Header("Actions")]
    [SerializeField] private InputActionReference jumpAction;

    [Header("Assists")]
    [SerializeField][Range(0.01f, 0.5f)] private float coyoteTime;
    [SerializeField][Range(0.01f, 10f)] private float jumpInputBufferTime;
    
    [Header("Particles")]
    [SerializeField] private ParticleSystem particleJumpUp;
    [SerializeField] private ParticleSystem particleJumpDown;

    [Header("")]
    [SerializeField][Range(0, 0.3f)] private float movementSmoothing = 0.05f;
    [SerializeField] private bool airControl = false;

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

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        playerView = GetComponent<PlayerView>();
        playerAnimation = GetComponent<PlayerAnimation>();

        groundCheck = transform.Find("GroundCheck");
        wallCheck = transform.Find("WallCheck");
    }

    public void Move(float moveY, float moveX, bool jump, bool dash, bool grab)
    {
        if (!canMove) return;

        // Дэш
        if (dash && canDash && !isWallSliding && isGrounded)
            StartCoroutine(DashCooldown());
        else if (dash && canDash && !isWallSliding && !isGrounded && CanAirDash)
            StartCoroutine(DashCooldown());
        if (!isDashing)
            MoveHorizontal(moveX);

        // Прыжок
        if (lastPressedJumpTime > 0 && CanJump)
            Jump();
        else if (lastPressedJumpTime > 0 && canDoubleJump && playerView.PlayerModel.HasDoubleJump)
            DoubleJump();

        // Взбирание по стене
        if (grab)
            WallRunnig(moveY, moveX, jump, dash);
        else if (isWallRunning && !grab)
        {
            isWallRunning = false;
            oldWallRunning = false;
            HandleWallSliding(moveY, moveX, false, false);
            playerAnimation.SetBoolIsWallRunning(false);
        }

        // Скольжение по стене и дэш от нее
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

        // Дэш от стены
        //else if (dash && canDash)
        //{
        //    isWallSliding = false;
        //    _animator.SetBool("IsWallSliding", false);
        //    oldWallSlidding = false;
        //    m_WallCheck.localPosition = new Vector3(Mathf.Abs(m_WallCheck.localPosition.x), m_WallCheck.localPosition.y, 0);
        //    canDoubleJump = true;
        //    StartCoroutine(DashCooldown());

        //}
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

            if (jump)
                WallJump();
        }
    }

    private void MoveHorizontal(float move)
    {
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

    private void DoubleJump()
    {
        if (!canDoubleJump || isWallSliding) return;

        canDoubleJump = false;

        var force = doubleJumpForce;
        if (rigidBody.linearVelocity.y < 0)
            force -= rigidBody.linearVelocity.y;

        rigidBody.AddForce(Vector2.up * force, ForceMode2D.Impulse);

        //m_Rigidbody2D.linearVelocity = new Vector2(m_Rigidbody2D.linearVelocity.x, 0);
        //m_Rigidbody2D.AddForce(new Vector2(0f, doubleJumpForce));
        playerAnimation.SetBoolIsDoubleJumping(true);
    }

    private void WallJump()
    {
        lastOnGroundTime = 0;
        lastPressedJumpTime = 0;

        playerAnimation.SetBoolIsJumping(true);
        playerAnimation.SetBoolJumpUp(true);
        rigidBody.linearVelocity = new Vector2(0f, 0f);

        var force = new Vector2(turnCoefficient * wallJumpForce.x, wallJumpForce.y);

        //if (Mathf.Sign(m_Rigidbody2D.linearVelocity.x) != Mathf.Sign(force.x))
        //    force.x -= m_Rigidbody2D.linearVelocity.x;

        //if (m_Rigidbody2D.linearVelocity.y < 0)
        //    force.y -= m_Rigidbody2D.linearVelocity.y;

        //m_Rigidbody2D.AddForce(force, ForceMode2D.Impulse);
        //Debug.Log(m_Rigidbody2D.linearVelocity * m_Rigidbody2D.mass);

        rigidBody.AddForce(force, ForceMode2D.Force);

        limitVelOnWallJump = true;
        canDoubleJump = true;

        isWallSliding = false;
        playerAnimation.SetBoolIsWallSliding(false);
        oldWallSliding = false;
        wallCheck.localPosition = new Vector3(Mathf.Abs(wallCheck.localPosition.x), wallCheck.localPosition.y, 0);

        canMove = false;
        StartCoroutine(WaitToMove(0.15f));
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

    public void PlayParticleJumpUp()
    {
        particleJumpUp.Play();
    }

    public void PlayParticleJumpDown()
    {
        particleJumpDown.Play();
    }

    public void ResetDashCounter()
    {
        dashCounter = 0;
    }

    private IEnumerator DashCooldown()
    {
        if (!isGrounded)
            dashCounter++;

        playerAnimation.SetBoolIsDashing(true);
        isDashing = true;
        canDash = false;

        if (isDashing)
            rigidBody.linearVelocity = new Vector2(turnCoefficient * dashForce, 0);

        yield return new WaitForSeconds(0.1f); //0.1 
        isDashing = false;
        yield return new WaitForSeconds(0.3f); //0.25
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
}
