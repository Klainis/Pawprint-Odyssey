using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using GlobalEnums;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference jumpAction;
    [SerializeField] private InputActionReference dashAction;
    [SerializeField] private InputActionReference attackAction;
    [SerializeField] private InputActionReference pauseMenuAction;
    [SerializeField] private InputActionReference pauseMenuActionUI;
    [SerializeField] private InputActionReference gameMenuAction;
    [SerializeField] private InputActionReference gameMenuActionUI;
    [SerializeField] private InputActionReference closeWindowButton;
    [SerializeField] private InputActionReference swapToRightGameMenuWindow;
    [SerializeField] private InputActionReference swapToLeftGameMenuWindow;
    [SerializeField] private InputActionReference buyAbility;
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
    private bool run = false;

    public bool AttackPressed { get { return attackPressed; } private set { attackPressed = value; } }

    #region Common Methods

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

            //if (IsValidAction(mapAction))
            //{
            //    if (mapAction.action.WasPressedThisFrame() && playerMove.IsGrounded)
            //    {
            //        GameManager.Instance.OpenMap();
            //        return;
            //    }
            //}
            if (IsValidAction(gameMenuAction))
            {
                if (gameMenuAction.action.WasPressedThisFrame() && playerMove.IsGrounded)
                {
                    GameManager.Instance.OpenGameMenu();
                    return;
                }
            }
        }
        else
        {
            if (IsValidAction(closeWindowButton))
            {
                if (closeWindowButton.action.WasPressedThisFrame())
                {
                    if (GameManager.Instance.GameState == GameState.GAME_MENU)
                    {
                        GameManager.Instance.CloseGameMenu();
                        return;
                    }
                    if (GameManager.Instance.GameState == GameState.PAUSE_MENU)
                    {
                        GameManager.Instance.ClosePauseMenu();
                        return;
                    }
                }
            }

            if (IsValidAction(pauseMenuActionUI))
            {
                if (pauseMenuActionUI.action.WasPressedThisFrame())
                {
                    GameManager.Instance.ClosePauseMenu();
                    return;
                }
            }

            if (IsValidAction(gameMenuActionUI))
            {
                if (gameMenuActionUI.action.WasPressedThisFrame())
                {
                    GameManager.Instance.CloseGameMenu();
                    return;
                }
            }

            if (IsValidAction(swapToRightGameMenuWindow))
            {
                if (swapToRightGameMenuWindow.action.WasPressedThisFrame())
                {
                    GameMenuUI.Instance.SwapToRightWindow();
                    return;
                }
            }

            if (IsValidAction(swapToLeftGameMenuWindow))
            {
                if (swapToLeftGameMenuWindow.action.WasPressedThisFrame())
                {
                    GameMenuUI.Instance.SwapToLeftWindow();
                    return;
                }
            }

            if (IsValidAction(buyAbility))
            {
                if (buyAbility.action.WasPressedThisFrame())
                {
                    AbilitiesTreeUIManager.Instance.BuyNode();
                }
            }

            //if (IsValidAction(mapActionUI) || IsValidAction(closeWindowButton))
            //{
            //    if (mapActionUI.action.WasPressedThisFrame() || closeWindowButton.action.WasPressedThisFrame())
            //    {
            //        GameManager.Instance.CloseMap();
            //        return;
            //    }
            //}
        }

        if (IsValidAction(attackAction))
            attackPressed = attackAction.action.WasPressedThisFrame();

        if (IsValidAction(moveAction))
        {
            var move = moveAction.action.ReadValue<Vector2>();

            Run(move);
            WallRun(move);
        }

        if (IsValidAction(dashAction))
        {
            if(dashAction.action.IsPressed())
            {
                run = true;
            }
            else
            {
                run = false;
            }
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

        //playerAnimation.SetFloatSpeed(Mathf.Abs(horizontalMove));
    }

    private void FixedUpdate()
    {
        playerMove.Move(verticalMove * Time.fixedDeltaTime, horizontalMove * Time.fixedDeltaTime, jump, dash, grab, run);

        if (jump)
            jumpPressed.Invoke();

        jump = false;
        dash = false;
    }

    #endregion

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
