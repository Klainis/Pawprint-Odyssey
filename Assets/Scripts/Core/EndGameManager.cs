using GlobalEnums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;

public class EndGameManager : MonoBehaviour
{
    private static EndGameManager _instance;
    public static EndGameManager Instance {  get { return _instance; } }

    
    public GameObject endGameScreenObject;
    public Volume volume;
    public GameObject finalTransition;

    [Header("Artifact Settings")]
    public GameObject artifactObject;
    public Transform endPoint;
    public float duration = 2.0f;
    public float arcHeight = 2.0f;
    public float _moveUpPointY = 0.8f;
    public float _moveUpSpeedY = 1f;

    [Header("Light Door Settings")]
    public Light2D _lightDoor;
    public float _lightDoorFlashTime = 0.3f;
    public float _lightDoorFlashAmount = 17f;

    private Vignette _vignette;

    private GameObject _artifactObjectInstance;

    private GameObject _player;
    private PlayerMove _playerMove;

    private bool _arcDirection = true;
    private int _direction = 1;
    private float _initialLightDoorIntensity;

    private Coroutine _disableVignetteCoroutine;
    private Coroutine _moveUpCoroutine;
    private Coroutine _artifactCreaterCoroutine;
    private Coroutine _lightFlashCoroutine;
    private Coroutine _lastDialogueCoroutne;

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
        }
        _instance = this;

        volume = FindAnyObjectByType<Volume>();

        if (volume.profile.TryGet(out Vignette vignette))
        {
            _vignette = vignette;
        }

        _player = GameObject.FindGameObjectWithTag("Player");
        _playerMove = PlayerMove.Instance;
    }

    private void Start()
    {
        if (endGameScreenObject != null)
        {
            endGameScreenObject.SetActive(false);
        }
        else
        {
            Debug.LogError("EndGameScreen Object is NULL");
        }

        finalTransition.SetActive(false);
        _initialLightDoorIntensity = _lightDoor.intensity;
    }

    public void GiveArtifactScene()
    {
        if (PlayerView.Instance.PlayerModel.ArtefactCollected < 3)
        {
            Debug.Log("Не хватает артефактов");
            return;
        }

        Debug.Log("Начали сцену с Артефактами");
        GameManager.Instance.SetGameState(GameState.CUTSCENE);

        PlayerView.Instance.FreezePlayerWithDisableMove(true);

        StartMoveUp();

        finalTransition.SetActive(true);
    }

    public void EnableEndGameScreen()
    {
        if (endGameScreenObject == null)
        {
            Debug.LogError("EndGameScreen Object is NULL");
            return;
        }

        endGameScreenObject.SetActive(true);

        StartDisableVignette();
        MusicHandler.Instance.AudioFadeOut();
    }

    private void MoveArtifact(GameObject artifactObject, Vector3 startPoint)
    {
        var endPos = endPoint.transform.position;
        
        if (_arcDirection)
        {
            _direction = 1;
            _arcDirection = !_arcDirection;
        }
        else if (!_arcDirection)
        {
            _direction = -1;
            _arcDirection = !_arcDirection;
        }
        Vector3 peak = Vector3.Lerp(startPoint, endPos, 0.5f) + Vector3.up * (_direction * arcHeight);

        Vector3[] path = new Vector3[] {
            startPoint,
            startPoint,
            peak,
            endPos,
            endPos
        };

        artifactObject.transform.position = startPoint;
        LeanTween.moveSpline(artifactObject, path, duration).setEase(LeanTweenType.easeInOutSine).setOnComplete(() =>
        {
            StartLightFlash();
            Destroy(artifactObject);
        });
    }

    #region Start Coroutine

    private void StartDisableVignette()
    {
        if (_disableVignetteCoroutine != null)
        {
            StopCoroutine(_disableVignetteCoroutine);
        }
        StartCoroutine(DisableVignette(1f, 0));
    }

    private void StartMoveUp()
    {
        if (_moveUpCoroutine != null)
        {
            StopCoroutine(_moveUpCoroutine);
        }
        StartCoroutine(MoveUp(_player.transform.position));
    }

    private void StartCreateArtifact()
    {
        if (_artifactCreaterCoroutine != null)
        {
            StopCoroutine(_artifactCreaterCoroutine);
        }
        StartCoroutine(CreateArtifact());
    }

    private void StartLightFlash()
    {
        //if (_lightFlashCoroutine != null)
        //{
        //    StopCoroutine(_lightFlashCoroutine);
        //}
        StartCoroutine(LightDoorFlash());
    }

    private void StartLastDialogue()
    {
        if (_lastDialogueCoroutne != null)
        {
            StopCoroutine(_lastDialogueCoroutne);
        }
        StartCoroutine(WaitLastDialogue());
    }

    #endregion

    #region Coroutines

    private IEnumerator DisableVignette(float duration, float finishValue)
    {
        var elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            _vignette.intensity.value = Mathf.Lerp(0.181f, 0, elapsed / duration);
            yield return null;
        }

        _vignette.intensity.value = 0;
    }

    private IEnumerator MoveUp(Vector3 initialPlayerPosition)
    {
        while (!PlayerMove.Instance.IsGrounded)
        {
            yield return null;
        }

        yield return new WaitForSeconds(1.5f);

        PlayerAnimation.Instance.SetBoolIsGiveArtifact(true);
        //_playerMove.enabled = false;

        float startY = initialPlayerPosition.y;
        float targetY = transform.position.y + _moveUpPointY;
        float currentY = startY;


        float distance = Mathf.Abs(startY - targetY);
        float time = _moveUpSpeedY > 0 ? distance / _moveUpSpeedY : 0.1f;
        float elapsedTime = 0f;

        while (elapsedTime < time && Mathf.Abs(startY - targetY) > 0.1)
        {
            elapsedTime += Time.deltaTime;
            currentY = Mathf.Lerp(startY, targetY, elapsedTime / time);
            _player.transform.position = new Vector3(_player.transform.position.x, currentY, _player.transform.position.z);
            yield return null;
        }

        StartCreateArtifact();
    }

    private IEnumerator CreateArtifact()
    {
        Vector3 startPoint = _player.transform.position;
        yield return new WaitForSeconds(1f);
        int artifactCreated = 0;

        while (artifactCreated < PlayerView.Instance.PlayerModel.ArtefactCollected)
        {
            artifactCreated++;

            _artifactObjectInstance = Instantiate(artifactObject, startPoint, Quaternion.identity);
            MoveArtifact(_artifactObjectInstance, startPoint);

            yield return new WaitForSeconds(0.5f);
        }

        yield return new WaitForSeconds(1.5f);

        GameManager.Instance.SetGameState(GameState.PLAYING);
        //_playerMove.enabled = true;
        PlayerAnimation.Instance.ResetAnimatorParameters();
        PlayerAnimation.Instance.SetBoolIsFall(true);
        PlayerView.Instance.FreezePlayerWithDisableMove(false);

        StartLastDialogue();
    }

    private IEnumerator LightDoorFlash()
    {
        _lightDoor.intensity = _lightDoorFlashAmount;

        float currentFlashAmount = _initialLightDoorIntensity;
        float elapsedTime = 0f;

        while (elapsedTime < _lightDoorFlashTime)
        {
            elapsedTime += Time.deltaTime;

            currentFlashAmount = Mathf.Lerp(1f, _initialLightDoorIntensity, (elapsedTime / _lightDoorFlashTime));
            _lightDoor.intensity = currentFlashAmount;

            yield return null;
        }
    }

    private IEnumerator WaitLastDialogue()
    {
        yield return new WaitForSeconds(0.2f);
        while (!PlayerMove.Instance.IsGrounded)
        {
            yield return null;
        }

        PlayerView.Instance.FreezePlayerWithDisableMove(true);

        yield return new WaitForSeconds(1.5f);

        PimenTalk talk = PimenView.Instance.gameObject.GetComponent<PimenTalk>();
        talk.Ending();
    }

    #endregion
}
