using System.Collections;
using UnityEngine;

public class ShowEducation : MonoBehaviour
{
    [Header("What education")]
    [SerializeField] private bool isMove;
    [SerializeField] private bool isWallJump;
    [SerializeField] private bool isAttack;
    [SerializeField] private bool isInteract;
    [SerializeField] private bool isClaw;
    [SerializeField] private bool isDoubleJump;

    [Header("If Claw Education")]
    [SerializeField] private InstantiateClawWallForEducation _clawWallForEd;

    [Header("Duration")]
    [SerializeField] private float _fadeDuration = 0.5f;
    [SerializeField] private float _waitBeforeShow = 1.5f;

    private CanvasGroup _canvasGroup;

    private Coroutine _fadeCoroutine;

    public bool HasClawOld { get; set; } = false;
    public bool HasDoubleJumpOld { get; set; } = false;
    public bool IsMove => isMove;
    public bool IsWallJump => isWallJump;
    public bool IsAttack => isAttack;

    private bool _isFading = false;
    private bool _pendingFadeOut = false;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _canvasGroup.alpha = 0f;
    }

    private void Update()
    {
        if(isMove)
        {
            if(PlayerInput.Instance.PlayerMovingEd)
            {
                FadeOut();
                PlayerView.Instance.PlayerModel.SetMoveEducation(true);
            }
        }
        if (isWallJump)
        {
            if (PlayerMove.Instance.IsWallJumping)
            {
                FadeOut();
                PlayerView.Instance.PlayerModel.SetWallJumpEducation(true);
            }
        }
        if (isAttack)
        {
            if (PlayerInput.Instance.PlayerAttackingEd)
            {
                FadeOut();
                PlayerView.Instance.PlayerModel.SetAttackEducation(true);
            }
        }
        if (isInteract)
        {
            if (PlayerInput.Instance.InteractPressed)
            {
                FadeOut();
            }
        }
        if (isClaw)
        {
            if (PlayerView.Instance.PlayerModel.HasClaw && !HasClawOld)
            {
                Debug.Log("Получили коготь");
                HasClawOld = true;
                _clawWallForEd.EnableClawWallEd();
                gameObject.SetActive(true);
                FadeIn();
            }
            else if (PlayerInput.Instance.PlayerClawEd)
            {
                FadeOut();
            }
        }
        if (isDoubleJump)
        {
            if (PlayerView.Instance.PlayerModel.HasDoubleJump && !HasDoubleJumpOld)
            {
                HasDoubleJumpOld = true;
                FadeIn();
            }
            else if (PlayerMove.Instance.PlayerDoubleJumpEd)
            {
                FadeOut();
            }
        }
    }

    public void FadeIn()
    {
        if (_isFading || _fadeCoroutine != null) return;

        _pendingFadeOut = false;
        StartCoroutine(WaitBeforeFadeIn());
    }

    public void FadeOut()
    {
        if (_canvasGroup.alpha == 0f && !_isFading) return;

        if (_isFading && _canvasGroup.alpha < 1f)
        {
            _pendingFadeOut = true;
            return;
        }

        if (!_isFading)
        {
            StartFade(0f);
        }
    }

    private void StartFade(float targetAlpha)
    {
        //if (_fadeCoroutine != null)
        //    StopCoroutine(_fadeCoroutine);

        _fadeCoroutine = StartCoroutine(FadeRoutine(targetAlpha));
    }

    private IEnumerator WaitBeforeFadeIn()
    {
        yield return new WaitForSeconds(_waitBeforeShow);
        StartFade(1f);
    }

    private IEnumerator FadeRoutine(float targetAlpha)
    {
        _isFading = true;

        float startAlpha = _canvasGroup.alpha;
        float time = 0f;

        while (time < _fadeDuration)
        {
            time += Time.unscaledDeltaTime;
            _canvasGroup.alpha = Mathf.MoveTowards(startAlpha, targetAlpha, time / _fadeDuration);
            yield return null;
        }

        _canvasGroup.alpha = targetAlpha;

        _isFading = false;

        if (targetAlpha == 1f)
        {
            if (_pendingFadeOut)
            {
                _pendingFadeOut = false;
                StartFade(0f);
            }
        }
        else if (targetAlpha == 0)
        {
            gameObject.SetActive(false);
        }
    }
}
