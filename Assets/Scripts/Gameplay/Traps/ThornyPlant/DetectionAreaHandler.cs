using System.Collections;
using UnityEngine;

public class DetectionAreaHandler : MonoBehaviour
{
    [SerializeField] private float _maxStayTime = 3f;
    [SerializeField] private float _hideCooldown = 4f;

    private ThornyPlant thornyPlant;
    private float _stayTimer = 0f;
    private bool _isCooldownActive = false;

    private void Awake()
    {
        thornyPlant = transform.parent.GetComponent<ThornyPlant>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_isCooldownActive) return;

        var player = collision.gameObject.CompareTag("Player");
        if (player && thornyPlant.IsHidden)
        {
            thornyPlant.ChangeForm(false);
            StartCoroutine(WaitAfterShowUp());
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        var player = collision.gameObject.CompareTag("Player");
        if (!player || _isCooldownActive) return;

        if (thornyPlant.IsHidden)
        {
            thornyPlant.ChangeForm(false);
            _stayTimer = 0f;
        }
        else
        {
            _stayTimer += Time.deltaTime;

            if (_stayTimer >= _maxStayTime)
                StartCoroutine(ForceHideCooldownRoutine());
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        var player = collision.gameObject.CompareTag("Player");
        if (player)
        {
            _stayTimer = 0f;

            if (!_isCooldownActive && !thornyPlant.IsHidden)
                StartCoroutine(WaitAndChangeForm());
        }
    }

    IEnumerator WaitAndChangeForm()
    {
        while (thornyPlant.IsShooting)
            yield return null;
        thornyPlant.ChangeForm(true);
    }

    IEnumerator WaitAfterShowUp()
    {
        yield return new WaitForSeconds(0.7f);
    }

    private IEnumerator ForceHideCooldownRoutine()
    {
        _isCooldownActive = true;
        _stayTimer = 0f;

        while (thornyPlant.IsShooting)
            yield return null;

        thornyPlant.ChangeForm(true);

        yield return new WaitForSeconds(_hideCooldown);

        _isCooldownActive = false;
    }
}
