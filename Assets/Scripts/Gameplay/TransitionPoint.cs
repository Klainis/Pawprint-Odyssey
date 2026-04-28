using UnityEngine;
using GlobalEnums;
using System.Collections;

public class TransitionPoint : MonoBehaviour
{
    [Header("Distanation Scene")]
    [SerializeField] private string targetScene;
    [SerializeField] private string entryGate;

    private PlayerInput playerInput;
    private PlayerView playerView;
    private GameManager gameManager;

    private Transform saveGround;

    private Collider2D col;

    private bool activated;
    private bool _isTransitioning = false;

    private float transitionSpeed = 7f;

    private void Awake()
    {
        gameManager = GameManager.Instance;
        playerInput = GameObject.FindAnyObjectByType<PlayerInput>(); // Надо будет заменить на Singleton
        //if (playerInput != null)
        //    Debug.Log($"PlayerInput найден:{(bool)playerInput}");

        playerView = GameObject.FindAnyObjectByType<PlayerView>();
        col = GetComponent<Collider2D>();
    }

    private void OnTriggerStay2D(Collider2D movigObj)
    {
        if (movigObj.CompareTag("Player") && !_isTransitioning)
        {
            _isTransitioning = true;

            if (gameManager.GameState == GameState.PLAYING)
            {
                StartCoroutine(WalkIntoGate(movigObj, transitionSpeed));
            }
            else if (gameManager.GameState == GameState.EXITING_LEVEL)
            {
                StartCoroutine(WalkOutGate(movigObj, transitionSpeed));
            }
            else
            {
                _isTransitioning = false;
            }
        }
    }

    private void OnEnable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnTransitionComplete += ResetTransitionLock;
        }
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnTransitionComplete -= ResetTransitionLock;
        }
    }

    private void ResetTransitionLock()
    {
        _isTransitioning = false;
        activated = false;
    }

    private IEnumerator WalkIntoGate(Collider2D playerCollider, /*Vector2 targetPosotion,*/ float speed)
    {
        Debug.Log($"[Transition] WalkIntoGate START | State: {gameManager.GameState}");

        playerInput.enabled = false;
        //playerView.enabled = false;
        Rigidbody2D playerRB1 = playerCollider.gameObject.GetComponent<Rigidbody2D>();

        GatePosition gatePosition = GetGatePosition();

        if (gatePosition == GatePosition.right || gatePosition == GatePosition.left)
        {
            Bounds gateBounds = col.bounds;
            Bounds playerBounds = playerCollider.bounds;

            Vector2 playerPosition = playerCollider.transform.position;
            Vector2 targetPosition = (gatePosition == GatePosition.right) ? 
                new Vector2(gateBounds.max.x, playerPosition.y) : 
                new Vector2(gateBounds.min.x, playerPosition.y);

            while (Vector2.Distance(playerPosition, targetPosition) > 0.01f)
            {
                //Отделить анимации игрока и добавить сюда анимацию бега к двери
                playerPosition = Vector2.MoveTowards(playerPosition, targetPosition, speed * Time.deltaTime);
                playerCollider.transform.position = playerPosition;
                yield return null;
            }

            //playerCollider.transform.position = playerPosition;
            //playerRB1.position = playerPosition;

            PlayerMove.Instance.CanMove = false;
            playerRB1.linearVelocity = Vector2.zero;

            activated = true;
        }
        else if (gatePosition == GatePosition.top || gatePosition == GatePosition.bottom)
        {
            Bounds gateBounds = col.bounds;
            Bounds playerBounds = playerCollider.bounds;

            Vector2 playerPosition = playerCollider.transform.position;

            if (gatePosition == GatePosition.top)
            {
                Vector2 targetPosition = new Vector2(playerPosition.x, gateBounds.max.y/*playerPosition.y + 200*/);

                while (Vector2.Distance(playerPosition, targetPosition) > 0.01f)
                {
                    //Отделить анимации игрока и добавить сюда анимацию бега к двери
                    playerPosition = Vector2.MoveTowards(playerPosition, targetPosition, speed * Time.deltaTime);
                    playerCollider.transform.position = playerPosition;
                    yield return null;
                }

                //playerCollider.transform.position = playerPosition;
                //playerRB1.position = playerPosition;

                PlayerMove.Instance.CanMove = false;
                playerRB1.linearVelocity = Vector2.zero;
                activated = true;
            }
            else if (gatePosition == GatePosition.bottom)
            {
                activated = true;
            }
        }

        Debug.Log($"[Transition] WalkIntoGate END | activated: {activated}");

        if (activated)
        {
            Debug.Log($"[Transition] Calling TryDoTransition");
            TryDoTransition(playerCollider);
        }

        yield return new WaitForSeconds(0.2f);
        _isTransitioning = false;
        Debug.Log($"[Transition] Lock released");
    }

    private IEnumerator WalkOutGate(Collider2D playerCollider, float speed)
    {
        Debug.Log($"[Transition] WalkOutGate START | State: {gameManager.GameState}");

        GatePosition gatePosition = GetGatePosition();

        Rigidbody2D playerRB1 = playerCollider.gameObject.GetComponent<Rigidbody2D>();

        if (gatePosition == GatePosition.right || gatePosition == GatePosition.left)
        {
            //playerView.enabled = true;
            //if (playerRB1)
            //    playerRB1.linearVelocity = Vector2.zero;

            Bounds gateBounds = col.bounds;
            Bounds playerBounds = playerCollider.bounds;

            BoxCollider2D playerBox = playerCollider.GetComponent<BoxCollider2D>();

            Vector2 playerPosition = playerCollider.transform.position;
            Vector2 targetPosition = (gatePosition == GatePosition.right) ?
                new Vector2(gateBounds.min.x - 2f, playerPosition.y) :
                new Vector2(gateBounds.max.x + 2f, playerPosition.y);

            while (Vector2.Distance(playerPosition, targetPosition) > 0.01f)
            {
                //Отделить анимации игрока и добавить сюда анимацию бега к двери
                playerPosition = Vector2.MoveTowards(playerPosition, targetPosition, speed * Time.deltaTime);
                playerCollider.transform.position = playerPosition;
                yield return null;
            }

        }
        else if (gatePosition == GatePosition.top || gatePosition == GatePosition.bottom)
        {
            //playerView.enabled = false;
            Bounds gateBounds = col.bounds;
            Bounds playerBounds = playerCollider.bounds;

            Vector2 playerPosition = playerCollider.transform.position;

            if (gatePosition == GatePosition.bottom)
            {
                saveGround = GameObject.FindGameObjectWithTag("SaveGround").transform;

                Vector3 saveGrounsPosition= new Vector3(saveGround.position.x, saveGround.position.y, 0);
                playerCollider.transform.SetPositionAndRotation(saveGrounsPosition, playerCollider.transform.rotation);
            }

        }

        Debug.Log($"[Transition] WalkOutGate END | activated: {activated}");

        playerView.enabled = true;
        playerInput.enabled = true;
        PlayerMove.Instance.CanMove = true;
        SafeGroundSaver.Instance.SetNewSafeGroundLocation();

        gameManager.SetGameState(GameState.PLAYING);

        yield return new WaitForSeconds(0.2f);
        _isTransitioning = false;
        Debug.Log($"[Transition] Lock released");
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
            activated = false;
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
