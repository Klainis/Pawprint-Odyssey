using System.Collections;
using UnityEngine;

public class ShowEducation : MonoBehaviour
{
    [Header("What education")]
    [SerializeField] private bool isMove;
    [SerializeField] private bool isAttack;
    [SerializeField] private bool isInteract;
    [SerializeField] private bool isClaw;
    [SerializeField] private bool isDoubleJump;

    [Header("Duration")]
    [SerializeField] private float _fadeDuration = 0.5f;
    [SerializeField] private float _waitBeforeShow = 1.5f;

    private CanvasGroup _canvasGroup;

    private Coroutine _fadeCoroutine;

    public bool HasClawOld { get; set; } = false;
    public bool HasDoubleJumpOld { get; set; } = false;

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
            }
        }
        if (isAttack)
        {
            if (PlayerInput.Instance.PlayerAttackingEd)
            {
                FadeOut();
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
                HasClawOld = true;
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
        StartCoroutine(WaitBeforeFadeIn());
    }

    public void FadeOut()
    {
        StartFade(0f);
    }

    private void StartFade(float targetAlpha)
    {
        if (_fadeCoroutine != null)
            StopCoroutine(_fadeCoroutine);

        _fadeCoroutine = StartCoroutine(FadeRoutine(targetAlpha));
    }

    private IEnumerator WaitBeforeFadeIn()
    {
        yield return new WaitForSeconds(_waitBeforeShow);
        StartFade(1f);
    }

    private IEnumerator FadeRoutine(float targetAlpha)
    {
        Debug.Log("FadeRoutine");
        float startAlpha = _canvasGroup.alpha;
        float time = 0f;

        while (time < _fadeDuration)
        {
            time += Time.unscaledDeltaTime;
            _canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / _fadeDuration);
            yield return null;
        }

        if (targetAlpha == 0)
        {
            gameObject.SetActive(false);
        }

        _canvasGroup.alpha = targetAlpha;
    }
}
