using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class RootGuardianTargetZoneHandler : MonoBehaviour
{
    #region SerializeFields

    [Header("Parameters")]
    [SerializeField] private GameObject _leavesPileObj;
    [SerializeField] private GameObject _rootGuardianObj;

    #endregion

    #region Variables

    private RootGuardianView _view;
    private RootGuardianAnimation _animation;

    private Rigidbody2D _rb;

    private RigidbodyConstraints2D _defaultConstraints;

    private bool _isRevealed = false;

    #endregion

    #region Common Methods

    private void Awake()
    {
        SetObjectsActive(false);

        _view = _rootGuardianObj.GetComponent<RootGuardianView>();
        _animation = _rootGuardianObj.GetComponent<RootGuardianAnimation>();

        _rb = _view.gameObject.GetComponent<Rigidbody2D>();

        _defaultConstraints = _rb.constraints;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !_isRevealed)
        {
            SetObjectsActive(true);
            _rb.constraints = RigidbodyConstraints2D.FreezePosition | RigidbodyConstraints2D.FreezeRotation;

            var isPlayerToTheRight = collision.transform.position.x > transform.position.x;
            _view.FacePlayerOnSpawn(isPlayerToTheRight);

            StartCoroutine(RevealRoutine());
            _view.StopRetreatSequence();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && _isRevealed)
            _view.StartRetreatSequence(transform.position);
    }

    #endregion

    private void SetObjectsActive(bool enemyReveal)
    {
        _isRevealed = enemyReveal;
        _leavesPileObj.SetActive(!enemyReveal);
        _rootGuardianObj.SetActive(enemyReveal);
    }

    public void HideEnemy()
    {
        SetObjectsActive(false);
    }

    private IEnumerator RevealRoutine()
    {
        _animation.SetBoolRevealing(true);
        yield return new WaitForSeconds(0.2f);
        _rb.constraints = _defaultConstraints;
        _animation.SetBoolRevealing(false);
    }
}
