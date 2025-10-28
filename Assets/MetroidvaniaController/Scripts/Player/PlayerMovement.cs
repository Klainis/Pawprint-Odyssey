using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour {

	[SerializeField] private CharacterController2D controller;
	[SerializeField] private Animator animator;
	[SerializeField] private InputActionReference moveAction;

	[SerializeField] private float runSpeed = 40f;

	private float horizontalMove = 0f;
	//float wallUpMove = 0f;
	public bool jump = false;
	private bool dash = false;

	Gamepad gamepad;
    private bool grab = false;
    private float verticalMove = 0f;

    //Rigidbody2D rb;
    //   public float velY;

    //bool dashAxis = false;

    // Update is called once per frame
    void Start()
	{
        gamepad = Gamepad.current;
    }
	void Update () 
	{

		if (moveAction != null && moveAction.action != null)
        {
            Vector2 move = moveAction.action.ReadValue<Vector2>();

            Run(move);
            WallGrab(move);
        }

        animator.SetFloat("Speed", Mathf.Abs(horizontalMove));

        if (Input.GetKeyDown(KeyCode.Z) || (gamepad != null && gamepad.aButton.wasPressedThisFrame))
		{
			jump = true;
		}

		if (Input.GetKeyDown(KeyCode.C) || (gamepad != null && gamepad.rightTrigger.wasPressedThisFrame))
		{
			dash = true;
		}
	}

    private void WallGrab(Vector2 move)
    {
        if (move.y > 0 && (gamepad != null && gamepad.rightTrigger.isPressed))
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
		jump = false;
		dash = false;
	}
}
