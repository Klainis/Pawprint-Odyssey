using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System;
using static PlayerModel;

public class CharacterController2D : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private PlayerData Data;

    [Header("Forces")]
    [SerializeField] private float jumpForce = 15f;
    [SerializeField] private float doubleJumpForce = 13f;
    [SerializeField] private Vector2 wallJumpForce = new Vector2(800f, 700f);
    [SerializeField] private float dashForce = 25f;

    [Header("Assists")]
    [SerializeField] [Range(0.01f, 0.5f)] private float coyoteTime;
    [SerializeField] [Range(0.01f, 10f)] private float jumpInputBufferTime;

    [Header("Actions")]
    [SerializeField] private InputActionReference jumpAction;

    [Header("")]
    [SerializeField] [Range(0, 0.3f)] private float m_MovementSmoothing = 0.05f;
    [SerializeField] private bool m_AirControl = false;
    [SerializeField] private bool doubleJump = false;
    [SerializeField] private LayerMask m_WhatIsGround;

    [Header("Checks")]
    [SerializeField] private Transform m_GroundCheck;                           
    [SerializeField] private Transform m_WallCheck;

    private float lastOnGroundTime;
    private float lastPressedJumpTime;

    const float k_GroundeDistance = 0.2f; // Radius of the overlap circle to determine if grounded
    public bool m_Grounded { get; private set; }            // Whether or not the player is grounded.
    private Rigidbody2D m_Rigidbody2D;
    public bool m_FacingRight { get; private set; } = true;  // For determining which way the player is currently facing.
    public int turnCoefficient { get; private set; } = 1;
    private Vector3 velocity = Vector3.zero;
    private float limitFallSpeed = 25f; // Limit fall moveX
    private int dashCounter = 0;

    private bool canDoubleJump = false; //If player can double jump
    private bool canDash = true;
    private bool isDashing = false; //If player is dashing
    private bool m_IsWall = false; //If there is a wall in front of the player
    private bool isWallSliding = false; //If player is sliding in a wall
    private bool oldWallSlidding = false; //If player is sliding in a wall in the previous frame
    private bool isWallRunning;
    private bool oldWallRunning;
    private float prevVelocityX = 0f;
    private bool canCheck = false; //For check if player is wallsliding
    private bool isInvincible = false;
    private bool canMove = true;
    private bool isJumping;


    private Gamepad gamepad;
    private Heart heart;

    private Animator animator;

    [Header("Particles")]
    [SerializeField] private ParticleSystem particleJumpUp; //Trail particles
    [SerializeField] private ParticleSystem particleJumpDown; //Explosion particles

    private float jumpWallStartX = 0;
    private float jumpWallDistX = 0; //Distance between player and wall
    private bool limitVelOnWallJump = false; //For limit wall jump distance with low fps

    [Header("Events")]
    [Space]

    [SerializeField] private UnityEvent OnFallEvent;
    [SerializeField] private UnityEvent OnLandEvent;


    [System.Serializable]
    public class BoolEvent : UnityEvent<bool> { }

    private void Awake()
    {
        gamepad = Gamepad.current;

        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        heart = GetComponent<Heart>();

        if (OnFallEvent == null)
            OnFallEvent = new UnityEvent();

        if (OnLandEvent == null)
            OnLandEvent = new UnityEvent();
    }
    private void OnDrawGizmos()
    {
        if (m_GroundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(m_GroundCheck.position, m_GroundCheck.position + Vector3.down * k_GroundeDistance);
        }
        if (m_WallCheck != null)
        {
            Gizmos.color = Color.red;
            Vector3 dir = m_FacingRight ? Vector3.right : Vector3.left;
            Gizmos.DrawLine(m_WallCheck.position, m_WallCheck.position + dir * k_GroundeDistance);
        }
    }

    private void Update()
    {

        lastOnGroundTime -= Time.deltaTime;
        lastPressedJumpTime -= Time.deltaTime;

        bool wasGrounded = m_Grounded;
        m_Grounded = false;

        if (Physics2D.Raycast(m_GroundCheck.position, Vector2.down, k_GroundeDistance, m_WhatIsGround))
        {
            m_Grounded = true;
            dashCounter = 0;
            isJumping = false;
            lastOnGroundTime = coyoteTime;

            if (!wasGrounded)
            {
                OnLandEvent.Invoke();
                if (!m_IsWall && !isDashing)
                    particleJumpDown.Play();

                canDoubleJump = true;

                if (m_Rigidbody2D.linearVelocity.y < 0f)
                    limitVelOnWallJump = false;
            }
        }

        m_IsWall = false;

        if (!m_Grounded)
        {
            OnFallEvent.Invoke();

            bool leftHit = Physics2D.Raycast(m_WallCheck.position, Vector2.left, k_GroundeDistance, m_WhatIsGround);
            bool rightHit = Physics2D.Raycast(m_WallCheck.position, Vector2.right, k_GroundeDistance, m_WhatIsGround);
            m_IsWall = leftHit || rightHit;

            if (m_IsWall)
            {
                dashCounter = 0;
                isDashing = false;
                isJumping = false;
            }

            prevVelocityX = m_Rigidbody2D.linearVelocity.x;
        }

        if (limitVelOnWallJump)
        {
            if (m_Rigidbody2D.linearVelocity.y < -0.5f)
                limitVelOnWallJump = false;
            jumpWallDistX = (jumpWallStartX - transform.position.x) * turnCoefficient;
            if (jumpWallDistX < -0.5f && jumpWallDistX > -1f)
            {
                canMove = true;
            }
            else if (jumpWallDistX < -1f && jumpWallDistX >= -2f)
            {
                canMove = true;
                m_Rigidbody2D.linearVelocity = new Vector2(10f * turnCoefficient, m_Rigidbody2D.linearVelocity.y);
            }
            else if (jumpWallDistX < -2f)
            {
                limitVelOnWallJump = false;
                m_Rigidbody2D.linearVelocity = new Vector2(0, m_Rigidbody2D.linearVelocity.y);
            }
            else if (jumpWallDistX > 0)
            {
                limitVelOnWallJump = false;
                m_Rigidbody2D.linearVelocity = new Vector2(0, m_Rigidbody2D.linearVelocity.y);
            }
        }

        ScaleJump();
    }

    public void Move(float moveY, float moveX, bool jump, bool dash, bool grab)
    {
        if (!canMove) return;

        //Дэш
        if (dash && canDash && !isWallSliding && m_Grounded)
        {
            StartCoroutine(DashCooldown());
        }
        else if (dash && canDash && !isWallSliding && !m_Grounded && CanAirDash())
        {
            StartCoroutine(DashCooldown());
        }
        if (!isDashing)
            MoveHorizontal(moveX);

        //Dash();

        // Прыжок
        if (lastPressedJumpTime > 0 && CanJump())
        {
            Jump();
        }
        else if (lastPressedJumpTime > 0 && canDoubleJump && doubleJump)
        {
            DoubleJump();
        }

        //Взбирание по стене
        if (grab)
        {
            WallRunnig(moveY, moveX, jump, dash);
        }
        else if (isWallRunning && !grab)
        {
            isWallRunning = false;
            oldWallRunning = false;
            HandleWallSliding(moveY, moveX, false, false);
            animator.SetBool("IsWallRunning", false);
        }

        // Скольжение по стене и дэш от нее
        if (m_IsWall && !m_Grounded && !grab)
        {
            HandleWallSliding(moveY, moveX, jump, dash);
        }
        else if (isWallSliding && !m_IsWall && canCheck)
        {
            isWallSliding = false;
            animator.SetBool("IsWallSliding", false);
            oldWallSlidding = false;
            m_WallCheck.localPosition = new Vector3(Mathf.Abs(m_WallCheck.localPosition.x), m_WallCheck.localPosition.y, 0);
            canDoubleJump = true;
        }
    }

    private void HandleWallSliding(float moveY, float moveX, bool jump, bool dash)
    {
        if (!oldWallSlidding && m_Rigidbody2D.linearVelocity.y < 0 && !isWallRunning || isDashing)
        {
            isWallSliding = true;
            m_WallCheck.localPosition = new Vector3(-m_WallCheck.localPosition.x, m_WallCheck.localPosition.y, 0);
            Turn();
            StartCoroutine(WaitToCheck(0.1f));
            canDoubleJump = true;
            animator.SetBool("IsJumping", false);
            animator.SetBool("IsWallRunning", false);
            animator.SetBool("IsWallSliding", true);
        }

        isDashing = false;

        if (isWallSliding)
        {
            if (moveX * turnCoefficient > 0.1f && oldWallSlidding)
            {
                StartCoroutine(WaitToEndSliding());
            }
            else
            {
                oldWallSlidding = true;
                m_Rigidbody2D.linearVelocity = new Vector2(-turnCoefficient * 2, -5);
            }
        }

        // Прыжок от стены
        if (jump && isWallSliding)
        {
            WallJump();
        }
        // Дэш от стены
        //else if (dash && canDash)
        //{
        //    isWallSliding = false;
        //    animator.SetBool("IsWallSliding", false);
        //    oldWallSlidding = false;
        //    m_WallCheck.localPosition = new Vector3(Mathf.Abs(m_WallCheck.localPosition.x), m_WallCheck.localPosition.y, 0);
        //    canDoubleJump = true;
        //    StartCoroutine(DashCooldown());

        //}
    }

    private void WallRunnig(float moveY, float moveX, bool jump, bool dash)
    {
        if (m_IsWall)
        {

            if (!oldWallRunning && moveY > 0)
            {
                isWallRunning = true;
                Turn();

                animator.SetBool("IsJumping", false);
                animator.SetBool("IsWallSliding", false);
                animator.SetBool("IsWallRunning", true);
            }

            if(isWallRunning)
            {
                m_Rigidbody2D.linearVelocity = new Vector2(m_Rigidbody2D.linearVelocity.x, moveY * 10f);
                oldWallRunning = true;

                if (jump)
                {
                    WallJump();
                }
            }
        }
    }

    public void MoveHorizontal(float move)
    {
        if (!(m_Grounded || m_AirControl)) return;

        if (m_Rigidbody2D.linearVelocity.y < -limitFallSpeed)
            m_Rigidbody2D.linearVelocity = new Vector2(m_Rigidbody2D.linearVelocity.x, -limitFallSpeed);

        Vector3 targetVelocity = new Vector2(move * 10f, m_Rigidbody2D.linearVelocity.y);
        m_Rigidbody2D.linearVelocity = Vector3.SmoothDamp(m_Rigidbody2D.linearVelocity, targetVelocity, ref velocity, m_MovementSmoothing);

        if (move > 0 && !m_FacingRight && !isWallSliding)
            Turn();
        else if (move < 0 && m_FacingRight && !isWallSliding)
            Turn();
    }

    private void ScaleJump()
    {
        if (m_Rigidbody2D.linearVelocity.y > 0)
        {
            if (!jumpAction.action.IsPressed() && !isWallRunning)
            {
                m_Rigidbody2D.linearVelocity = new Vector2(m_Rigidbody2D.linearVelocity.x, 0);
            }
        }
    }

    private bool CanJump()
    {
        return (lastOnGroundTime > 0 && !isJumping);
    }
    public void OnJumpInput()
    {
        lastPressedJumpTime = jumpInputBufferTime;
    }

    public void Jump()
    {
        Debug.Log("JUMP");
        lastOnGroundTime = 0;
        lastPressedJumpTime = 0;

        isJumping = true;
        animator.SetBool("IsJumping", true);
        animator.SetBool("JumpUp", true);
        m_Grounded = false;

        float force = jumpForce;
        if (m_Rigidbody2D.linearVelocity.y < 0)
            force -= m_Rigidbody2D.linearVelocity.y;

        m_Rigidbody2D.AddForce(Vector2.up * force, ForceMode2D.Impulse);

        canDoubleJump = true;
        particleJumpDown.Play();
        particleJumpUp.Play();
    }

    private void DoubleJump()
    {
        if (!canDoubleJump || isWallSliding) return;

        canDoubleJump = false;

        float force = doubleJumpForce;
        if (m_Rigidbody2D.linearVelocity.y < 0)
            force -= m_Rigidbody2D.linearVelocity.y;

        m_Rigidbody2D.AddForce(Vector2.up * force, ForceMode2D.Impulse);

        //m_Rigidbody2D.linearVelocity = new Vector2(m_Rigidbody2D.linearVelocity.x, 0);
        //m_Rigidbody2D.AddForce(new Vector2(0f, doubleJumpForce));
        animator.SetBool("IsDoubleJumping", true);
    }

    private void WallJump()
    {
        lastOnGroundTime = 0;
        lastPressedJumpTime = 0;

        animator.SetBool("IsJumping", true);
        animator.SetBool("JumpUp", true);
        m_Rigidbody2D.linearVelocity = new Vector2(0f, 0f);

        Vector2 force = new Vector2(turnCoefficient * wallJumpForce.x, wallJumpForce.y);

        //if (Mathf.Sign(m_Rigidbody2D.linearVelocity.x) != Mathf.Sign(force.x))
        //    force.x -= m_Rigidbody2D.linearVelocity.x;

        //if (m_Rigidbody2D.linearVelocity.y < 0)
        //    force.y -= m_Rigidbody2D.linearVelocity.y;

        //m_Rigidbody2D.AddForce(force, ForceMode2D.Impulse);
        //Debug.Log(m_Rigidbody2D.linearVelocity * m_Rigidbody2D.mass);

        m_Rigidbody2D.AddForce(force, ForceMode2D.Force);

        jumpWallStartX = transform.position.x;
        limitVelOnWallJump = true;
        canDoubleJump = true;

        isWallSliding = false;
        animator.SetBool("IsWallSliding", false);
        oldWallSlidding = false;
        m_WallCheck.localPosition = new Vector3(Mathf.Abs(m_WallCheck.localPosition.x), m_WallCheck.localPosition.y, 0);

        canMove = false;
        StartCoroutine(WaitToMove(0.15f));
    }

    private bool CanAirDash()
    {
        return dashCounter < 1;
    }
    //private void Dash()
    //{
    //    //if (isDashing)
    //    //{
    //    //    m_Rigidbody2D.linearVelocity = new Vector2(turnCoefficient * dashForce, 0);
    //    //}
    //}

    private void Turn()
    {
        if (m_FacingRight)
        {
            Vector3 rotator = new Vector3(transform.rotation.x, 180f, transform.rotation.z);
            transform.rotation = Quaternion.Euler(rotator);
            m_FacingRight = !m_FacingRight;
            turnCoefficient = -1;
        }
        else
        {
            Vector3 rotator = new Vector3(transform.rotation.x, 0f, transform.rotation.z);
            transform.rotation = Quaternion.Euler(rotator);
            m_FacingRight = !m_FacingRight;
            turnCoefficient = 1;
        }
    }

    public void ApplyDamage(int damage, Vector3 position)
    {
        if (isInvincible) return;

        animator.SetBool("Hit", true);
        heart.RemoveHearts(damage);
        var damageDir = Vector3.Normalize(transform.position - position) * 40f;
        m_Rigidbody2D.linearVelocity = Vector2.zero;
        m_Rigidbody2D.AddForce(damageDir * 15);
        if (Data.currentLife < 1)
        {
            StartCoroutine(WaitToDead());
        }
        else
        {
            //StartCoroutine(Stun(0.25f));
            StartCoroutine(MakeInvincible(1f));
        }
    }

    public void ApplyObjectDamage(int damage)
    {
        if (!isInvincible)
        {
            animator.SetBool("Hit", true);
            heart.RemoveHearts(damage);
            if (Data.currentLife < 1)
            {
                StartCoroutine(WaitToDead());
            }
            else
            {
                StartCoroutine(Stun(0.25f));
                StartCoroutine(MakeInvincible(1f));
            }
        }
    }

    IEnumerator DashCooldown()
    {
        if (!m_Grounded)
        {
            dashCounter++;
        }

        animator.SetBool("IsDashing", true);
        isDashing = true;
        canDash = false;

        if (isDashing)
        {
            m_Rigidbody2D.linearVelocity = new Vector2(turnCoefficient * dashForce, 0);
        }
        yield return new WaitForSeconds(0.1f); //0.1 
        isDashing = false;
        yield return new WaitForSeconds(0.3f); //0.25
        canDash = true;
    }

    IEnumerator Stun(float time)
    {
        canMove = false;
        yield return new WaitForSeconds(time);
        canMove = true;
    }
    IEnumerator MakeInvincible(float time)
    {
        isInvincible = true;
        yield return new WaitForSeconds(time);
        isInvincible = false;
    }
    IEnumerator WaitToMove(float time)
    {
        canMove = false;
        yield return new WaitForSeconds(time);
        canMove = true;
    }

    IEnumerator WaitToCheck(float time)
    {
        canCheck = false;
        yield return new WaitForSeconds(time);
        canCheck = true;
    }

    IEnumerator WaitToEndSliding()
    {
        yield return new WaitForSeconds(0.1f);
        canDoubleJump = true;
        isWallSliding = false;
        animator.SetBool("IsWallSliding", false);
        oldWallSlidding = false;
        m_WallCheck.localPosition = new Vector3(Mathf.Abs(m_WallCheck.localPosition.x), m_WallCheck.localPosition.y, 0);
    }

    IEnumerator WaitToDead()
    {
        animator.SetBool("IsDead", true);
        canMove = false;
        isInvincible = true;
        GetComponent<Attack>().enabled = false;
        yield return new WaitForSeconds(0.4f);
        m_Rigidbody2D.linearVelocity = new Vector2(0, m_Rigidbody2D.linearVelocity.y);
        yield return new WaitForSeconds(1.1f);
        //SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        SceneManager.LoadSceneAsync("F_Room_Tutorial");
    }
}
