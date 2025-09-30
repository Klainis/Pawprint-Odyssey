using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour {

	public CharacterController2D controller;
	public Animator animator;

	public float runSpeed = 40f;

	float horizontalMove = 0f;
	bool jump = false;
	bool dash = false;

	//bool dashAxis = false;
	
	// Update is called once per frame
	void Update () {

		horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;

		animator.SetFloat("Speed", Mathf.Abs(horizontalMove));

		var gamepad = Gamepad.current;
		if (gamepad != null)
		{
			if (Input.GetKeyDown(KeyCode.Z) || gamepad.aButton.wasPressedThisFrame)
			{
				jump = true;
			}

			if (Input.GetKeyDown(KeyCode.C) || gamepad.rightTrigger.wasPressedThisFrame)
			{
				dash = true;
			}
		}
		/*if (Input.GetAxisRaw("Dash") == 1 || Input.GetAxisRaw("Dash") == -1) //RT in Unity 2017 = -1, RT in Unity 2019 = 1
		{
			if (dashAxis == false)
			{
				dashAxis = true;
				dash = true;
			}
		}
		else
		{
			dashAxis = false;
		}
		*/

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
		controller.Move(horizontalMove * Time.fixedDeltaTime, jump, dash);
		jump = false;
		dash = false;
	}
}
