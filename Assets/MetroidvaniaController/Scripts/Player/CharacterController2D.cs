using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System;

public class CharacterController2D : MonoBehaviour
{
    [SerializeField] private float m_JumpForce = 400f;
    [SerializeField] private float life = 10f;
    [Range(0, .3f)][SerializeField] private float m_MovementSmoothing = .05f;   // How much to smooth out the movement
    [SerializeField] private bool m_AirControl = false;                         // Whether or not a player can steer while jumping;
    [SerializeField] private LayerMask m_WhatIsGround;                          // A mask determining what is ground to the character
    [SerializeField] private Transform m_GroundCheck;                           // A position marking where to check if the player is grounded.
    [SerializeField] private Transform m_WallCheck;                             // Posicion que controla si el personaje toca una pared

    const float k_GroundedRadius = 0.2f; // Radius of the overlap circle to determine if grounded
    public bool m_Grounded { get; private set; }            // Whether or not the player is grounded.
    private Rigidbody2D m_Rigidbody2D;
    private bool m_FacingRight = true;  // For determining which way the player is currently facing.
    private int turnCoefficient;
    private Vector3 velocity = Vector3.zero;
    private float limitFallSpeed = 25f; // Limit fall moveX

    public bool canDoubleJump = true; //If player can double jump
    [SerializeField] private float m_DashForce = 25f;
    private bool canDash = true;
    private bool isDashing = false; //If player is dashing
    private bool m_IsWall = false; //If there is a wall in front of the player
    private bool isWallSliding = false; //If player is sliding in a wall
    private bool oldWallSlidding = false; //If player is sliding in a wall in the previous frame
    private bool isWallRunning;
    private bool oldWallRunning;
    private float prevVelocityX = 0f;
    private bool canCheck = false; //For check if player is wallsliding

    private bool invincible = false; //If player can die
    private bool canMove = true; //If player can moveX

    private Animator animator;
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
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

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
            Gizmos.DrawLine(m_GroundCheck.position, m_GroundCheck.position + Vector3.down * k_GroundedRadius);
        }
        if (m_WallCheck != null)
        {
            Gizmos.color = Color.red;
            Vector3 dir = m_FacingRight ? Vector3.right : Vector3.left;
            Gizmos.DrawLine(m_WallCheck.position, m_WallCheck.position + dir * k_GroundedRadius);
        }
    }



    private void FixedUpdate()
    {
        bool wasGrounded = m_Grounded;
        m_Grounded = false;

        RaycastHit2D groundHit = Physics2D.Raycast(m_GroundCheck.position, Vector2.down, k_GroundedRadius, m_WhatIsGround);
        if (groundHit.collider != null)
        {
            m_Grounded = true;
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

            bool leftHit = Physics2D.Raycast(m_WallCheck.position, Vector2.left, k_GroundedRadius, m_WhatIsGround);
            bool rightHit = Physics2D.Raycast(m_WallCheck.position, Vector2.right, k_GroundedRadius, m_WhatIsGround);
            m_IsWall = leftHit || rightHit;

            if (m_IsWall)
                isDashing = false;

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

    private void ScaleJump()
    {
        if (m_Rigidbody2D.linearVelocity.y > 0)
        {
            if (!Input.GetKey(KeyCode.Z) && (Gamepad.current == null || !Gamepad.current.aButton.isPressed) && !isWallRunning)
            {
                m_Rigidbody2D.linearVelocity = new Vector2(m_Rigidbody2D.linearVelocity.x, 0);
            }
        }
    }

    public void Move(float moveY, float moveX, bool jump, bool dash, bool grab)
    {
        if (!canMove) return;

        //Дэш
        if (dash && canDash && !isWallSliding)
        {
            StartCoroutine(DashCooldown());
        }

        Dash(moveX);

        // Прыжок
        if (m_Grounded && jump)
        {
            Jump();
        }
        else if (!m_Grounded && jump)
        {
            DoubleJump(jump);
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

    private void MoveHorizontal(float move)
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

    private void Jump()
    {
        animator.SetBool("IsJumping", true);
        animator.SetBool("JumpUp", true);
        m_Grounded = false;
        m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
        canDoubleJump = true;
        particleJumpDown.Play();
        particleJumpUp.Play();
    }

    private void DoubleJump(bool jumpPressed)
    {
        if (!canDoubleJump || isWallSliding) return;

        canDoubleJump = false;
        m_Rigidbody2D.linearVelocity = new Vector2(m_Rigidbody2D.linearVelocity.x, 0);
        m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce * 0.9f));
        animator.SetBool("IsDoubleJumping", true);
    }

    private void WallJump()
    {
        animator.SetBool("IsJumping", true);
        animator.SetBool("JumpUp", true);
        m_Rigidbody2D.linearVelocity = new Vector2(0f, 0f);

        m_Rigidbody2D.AddForce(new Vector2(turnCoefficient * m_JumpForce * 1.2f, m_JumpForce));
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

    private void Dash(float move)
    {
        if (isDashing)
        {
            m_Rigidbody2D.linearVelocity = new Vector2(turnCoefficient * m_DashForce, 0);
        }
        else
        {
            MoveHorizontal(move);
        }
    }

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

    public void ApplyDamage(float damage, Vector3 position)
    {
        if (!invincible)
        {
            animator.SetBool("Hit", true);
            life -= damage;
            Vector2 damageDir = Vector3.Normalize(transform.position - position) * 40f;
            m_Rigidbody2D.linearVelocity = Vector2.zero;
            m_Rigidbody2D.AddForce(damageDir * 15);
            if (life <= 0)
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
        animator.SetBool("IsDashing", true);
        isDashing = true;
        canDash = false;
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
        invincible = true;
        yield return new WaitForSeconds(time);
        invincible = false;
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
        invincible = true;
        GetComponent<Attack>().enabled = false;
        yield return new WaitForSeconds(0.4f);
        m_Rigidbody2D.linearVelocity = new Vector2(0, m_Rigidbody2D.linearVelocity.y);
        yield return new WaitForSeconds(1.1f);
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
    }
}
