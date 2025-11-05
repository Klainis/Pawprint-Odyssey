using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour {

	[SerializeField] private CharacterController2D controller;
	[SerializeField] private Animator animator;
	[SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference jumpAction;
    [SerializeField] private InputActionReference dashAction;
    [SerializeField] private float runSpeed = 40f;

	private float horizontalMove = 0f;
	private bool jump = false;
	private bool dash = false;
    private float lastJumpTime;

	Gamepad gamepad;
    CharacterController2D characterController;
    [SerializeField] private UnityEvent jumpPressed;
    private bool grab = false;
    private float verticalMove = 0f;

    void Start()
	{
        gamepad = Gamepad.current;
        characterController = GetComponent<CharacterController2D>();
    }
	void Update () 
	{
        if (moveAction != null && moveAction.action != null)
        {
            Vector2 move = moveAction.action.ReadValue<Vector2>();

            Run(move);
            WallRun(move);
        }

        if (jumpAction != null && jumpAction.action != null)
        {
            if (jumpAction.action.WasPressedThisFrame())
            {
                jump = true;
                //jumpPressed.Invoke();
            }
        }

        if (dashAction != null && dashAction.action != null)
        {
            if (dashAction.action.WasPressedThisFrame())
                dash = true;
        }

        animator.SetFloat("Speed", Mathf.Abs(horizontalMove));
    }

    private void WallRun(Vector2 move)
    {
        if (move.y > 0 && dashAction.action.IsPressed())
        {
            grab = true;
            verticalMove = runSpeed;
        }
        else
        {
            grab = false;
            verticalMove = 0f;
        }
    }

    private void Run(Vector2 move)
    {
        if (move.x > 0.7f)
        {
            horizontalMove = runSpeed;
        }
        else if (move.x < -0.7f)
        {
            horizontalMove = -runSpeed;
        }
        else
        {
            horizontalMove = 0f;
        }
    }

    public void OnFall()
	{
		animator.SetBool("IsJumping", true);
	}

	public void OnLanding()
	{
		animator.SetBool("IsJumping", false);
	}

	void FixedUpdate ()
	{
		// Move our character
		controller.Move(verticalMove * Time.fixedDeltaTime, horizontalMove * Time.fixedDeltaTime, jump, dash, grab);
		
        if (jump)
            jumpPressed.Invoke();

        jump = false;
		dash = false;
	}
}
