using UnityEngine;
using GlobalEnums;
using Unity.VisualScripting;
using System.Collections;

public class TransitionPoint : MonoBehaviour
{
    [Header("Distanation Scene")]
    [SerializeField] private string targetScene;
    [SerializeField] private string entryGate;

    private PlayerInput playerInput;
    private CharacterController2D characterController;
    private GameManager gameManager;

    private Collider2D collider;

    private bool activated;
    private bool endMoveToGate;

    private float transitionSpeed = 7f;

    private void Awake()
    {
        gameManager = GameManager._instance;
        playerInput = GameObject.FindAnyObjectByType<PlayerInput>(); // Надо будет заменить на Singleton
        if (playerInput != null)
            Debug.Log($"PlayerInput найден:{(bool)playerInput}");

        characterController = GameObject.FindAnyObjectByType<CharacterController2D>();

        collider = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D movigObj)
    {
        if (movigObj.CompareTag("Player"))
        {
            if (gameManager.GameState == GameState.PLAYING)
            {
                StartCoroutine(WalkIntoGate(movigObj, transitionSpeed));
            }
            else if (gameManager.GameState == GameState.EXITING_LEVEL)
            {
                StartCoroutine(WalkOutGate(movigObj, transitionSpeed));
            }
        }
    }

    //private void OnTriggerStay2D(Collider2D movigObj)
    //{
    //    if (!activated && movigObj.CompareTag("Player"))
    //    {
    //        TryDoTransition(movigObj);
    //    }
    //}

    private IEnumerator WalkIntoGate(Collider2D playerCollider, /*Vector2 targetPosotion,*/ float speed)
    {
        playerInput.enabled = false;
        // Так же разделить логику от всего остального CharacterController и отключать его
        characterController.enabled = false;

        GatePosition gatePosition = GetGatePosition();

        if (gatePosition == GatePosition.right || gatePosition == GatePosition.left)
        {
            Bounds gateBounds = collider.bounds;
            Bounds playerBounds = playerCollider.bounds;

            Vector2 playerPosition = playerCollider.transform.position;
            Vector2 targetPosition = (gatePosition == GatePosition.right) ? 
                new Vector2(gateBounds.max.x, playerPosition.y) : 
                new Vector2(gateBounds.min.x, playerPosition.y);

            while (Vector2.Distance(playerPosition, targetPosition) > 0f)
            {
                //Отделить анимации игрока и добавить сюда анимацию бега к двери
                playerPosition = Vector2.MoveTowards(playerPosition, targetPosition, speed * Time.deltaTime);
                playerCollider.transform.position = playerPosition;
                yield return null;
            }

            activated = true;
        }
        else if (gatePosition == GatePosition.top || gatePosition == GatePosition.bottom)
        {
            Bounds gateBounds = collider.bounds;
            Bounds playerBounds = playerCollider.bounds;

            Vector2 playerPosition = playerCollider.transform.position;
            //Vector2 targetPosition = (gatePosition == GatePosition.top) ?
            //    new Vector2(playerPosition.x, gateBounds.max.y) :
            //    new Vector2(playerPosition.x, gateBounds.max.y);
            if (gatePosition == GatePosition.top)
            {
                Vector2 targetPosition = new Vector2(gateBounds.extents.x, gateBounds.max.y);

                while (Vector2.Distance(playerPosition, targetPosition) > 0f)
                {
                    //Отделить анимации игрока и добавить сюда анимацию бега к двери
                    playerPosition = Vector2.MoveTowards(playerPosition, targetPosition, speed * Time.deltaTime);
                    playerCollider.transform.position = playerPosition;
                    yield return null;
                }
            }
            else if (gatePosition == GatePosition.bottom)
            {
                if (Vector2.Distance(playerPosition, new Vector2(playerPosition.x, gateBounds.min.y)) > 0f)
                {
                    yield return null;
                }
            }

            activated = true;
        }

        if (activated)
        {
            TryDoTransition(playerCollider);
        }
    }

    private IEnumerator WalkOutGate(Collider2D playerCollider, /*Vector2 targetPosotion,*/ float speed)
    {
        GatePosition gatePosition = GetGatePosition();

        Rigidbody2D playerRB1 = playerCollider.gameObject.GetComponent<Rigidbody2D>();

        if (gatePosition == GatePosition.right || gatePosition == GatePosition.left)
        {
            if (playerRB1)
            {
                playerRB1.linearVelocity = Vector2.zero;
            }

            Bounds gateBounds = collider.bounds;
            Bounds playerBounds = playerCollider.bounds;

            BoxCollider2D playerBox = playerCollider.GetComponent<BoxCollider2D>();

            Vector2 playerPosition = playerCollider.transform.position;
            Vector2 targetPosition = (gatePosition == GatePosition.right) ?
                new Vector2(gateBounds.min.x - /*playerBounds.extents.x*/2f, playerPosition.y) :
                new Vector2(gateBounds.max.x + /*playerBounds.extents.x*/2f, playerPosition.y);

            while (Vector2.Distance(playerPosition, targetPosition) > /*-playerBox.size.x*/0f)
            {
                //Отделить анимации игрока и добавить сюда анимацию бега к двери
                playerPosition = Vector2.MoveTowards(playerPosition, targetPosition, speed * Time.deltaTime);
                playerCollider.transform.position = playerPosition;
                yield return null;
            }

        }
        else if (gatePosition == GatePosition.top || gatePosition == GatePosition.bottom)
        {
            Bounds gateBounds = collider.bounds;
            Bounds playerBounds = playerCollider.bounds;

            Vector2 playerPosition = playerCollider.transform.position;
            Vector2 targetPosition = Vector2.zero;

            if (gatePosition == GatePosition.bottom)
            {
                targetPosition = new Vector2(gateBounds.extents.x, gateBounds.max.y + playerBounds.extents.y);
            }

            while (Vector2.Distance(playerPosition, targetPosition) > 0f)
            {
                //Отделить анимации игрока и добавить сюда анимацию бега к двери
                playerPosition = Vector2.MoveTowards(playerPosition, targetPosition, speed * Time.deltaTime);
                playerCollider.transform.position = playerPosition;
                yield return null;
            }
        }
        //Основа для перехода вверх
        //playerRB1.AddForce(Vector2.Lerp(playerRB1.position, new Vector2(-9f, 10f), transitionSpeed * Time.deltaTime));
        //playerRB1.AddForce(new Vector2(200f, 300f));
        //PushOutFromGate(playerCollider);

        playerInput.enabled = true;
        characterController.enabled = true;
        gameManager.SetGameState(GameState.PLAYING);

    }

    public void TryDoTransition(Collider2D playerCollider)
    {
        if (gameManager == null)
        {
            return;
        }
        Debug.Log(gameManager.GameState);

        if (gameManager.GameState == GameState.ENTERING_LEVEL)
        {
            return;
        }

        if (targetScene != null && entryGate != null)
        {
            activated = true;
            gameManager.BeginSceneTransition(targetScene, entryGate);
        }
    }

    public GatePosition GetGatePosition()
    {
        if (entryGate.Contains("top"))
        {
            return GatePosition.top;
        }

        if (entryGate.Contains("right"))
        {
            return GatePosition.right;
        }

        if (entryGate.Contains("left"))
        {
            return GatePosition.left;
        }

        if (entryGate.Contains("bot"))
        {
            return GatePosition.bottom;
        }

        //if (entryGate.Contains("door") || isADoor)
        //{
        //    return GatePosition.door;
        //}

        return GatePosition.unknown;
    }
}
