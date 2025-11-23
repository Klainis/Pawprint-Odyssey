using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour {
	[SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference jumpAction;
    [SerializeField] private InputActionReference dashAction;
    [SerializeField] private InputActionReference attackAction;
    [SerializeField] private InputActionReference saveAction;
    [SerializeField] private InputActionReference loadAction;
    [SerializeField] private float runSpeed = 40f;
    [SerializeField] private UnityEvent jumpPressed;

    private PlayerAnimation playerAnimation;
    private PlayerMove playerMove;
    private PlayerHeart playerHeart;

	private float horizontalMove = 0f;
    private float verticalMove = 0f;
    private bool attackPressed;
	private bool jump = false;
	private bool dash = false;
    private bool grab = false;

    public bool AttackPressed { get { return attackPressed; } private set { attackPressed = value; } }

    private void Awake()
    {
        playerAnimation = GetComponent<PlayerAnimation>();
        playerMove = GetComponent<PlayerMove>();
        playerHeart = GetComponent<PlayerHeart>();
    }

    private void Update () 
	{
        if (attackAction != null && attackAction.action != null)
            attackPressed = attackAction.action.WasPressedThisFrame();

        if (moveAction != null && moveAction.action != null)
        {
            var move = moveAction.action.ReadValue<Vector2>();

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

        if (saveAction != null && saveAction.action != null)
        {
            if (saveAction.action.WasPressedThisFrame())
                SaveSystem.Save();
        }
        if (loadAction != null && loadAction.action != null)
        {
            if (loadAction.action.WasPressedThisFrame())
            {
                SaveSystem.TryLoad();
                playerHeart.RemoveHearts();
            }
        }

        playerAnimation.SetFloatSpeed(Mathf.Abs(horizontalMove));
    }

    private void FixedUpdate()
    {
        // Move our character
        playerMove.Move(verticalMove * Time.fixedDeltaTime, horizontalMove * Time.fixedDeltaTime, jump, dash, grab);

        if (jump)
            jumpPressed.Invoke();

        jump = false;
        dash = false;
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
            horizontalMove = runSpeed;
        else if (move.x < -0.7f)
            horizontalMove = -runSpeed;
        else
            horizontalMove = 0f;
    }

    public void OnFall()
	{
        playerAnimation.SetBoolIsJumping(true);
    }

    public void OnLanding()
	{
        playerAnimation.SetBoolIsJumping(false);
    }
}
