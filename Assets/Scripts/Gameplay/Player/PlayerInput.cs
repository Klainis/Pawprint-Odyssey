using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference jumpAction;
    [SerializeField] private InputActionReference dashAction;
    [SerializeField] private InputActionReference attackAction;
    [SerializeField] private InputActionReference pauseMenuAction;
    [SerializeField] private InputActionReference pauseMenuActionUI;
    [SerializeField] private InputActionReference mapAction;
    [SerializeField] private InputActionReference mapActionUI;
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

    private void Update()
    {
        var isGameActive = Time.timeScale > 0;
        if (isGameActive)
        {
            if (IsValidAction(pauseMenuAction))
            {
                if (pauseMenuAction.action.WasPressedThisFrame())
                {
                    GameManager.Instance.OpenPauseMenu();
                    return;
                }
            }

            if (IsValidAction(mapAction))
            {
                if (mapAction.action.WasPressedThisFrame())
                    GameManager.Instance.OpenMap();
            }
        }
        else
        {
            if (IsValidAction(pauseMenuActionUI))
            {
                if (pauseMenuActionUI.action.WasPressedThisFrame())
                {
                    GameManager.Instance.ClosePauseMenu();
                    return;
                }
            }

            if (IsValidAction(mapActionUI))
            {
                if (mapActionUI.action.WasPressedThisFrame())
                    GameManager.Instance.CloseMap();
            }
        }

        if (IsValidAction(attackAction))
            attackPressed = attackAction.action.WasPressedThisFrame();

        if (IsValidAction(moveAction))
        {
            var move = moveAction.action.ReadValue<Vector2>();

            Run(move);
            WallRun(move);
        }

        if (IsValidAction(jumpAction))
        {
            if (jumpAction.action.WasPressedThisFrame())
            {
                jump = true;
                //jumpPressed.Invoke();
            }
        }

        if (IsValidAction(dashAction))
        {
            if (dashAction.action.WasPressedThisFrame())
                dash = true;
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

    private bool IsValidAction(InputActionReference actionReference)
    {
        return actionReference != null && actionReference.action != null;
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
