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

    #endregion

    #region Common Methods

    private void Awake()
    {
        SetObjectsActive(false);

        _view = _rootGuardianObj.GetComponent<RootGuardianView>();
        _animation = _rootGuardianObj.GetComponent<RootGuardianAnimation>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            SetObjectsActive(true);
            StartCoroutine(RevealRoutine());
            _view.StopRetreatSequence();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            _view.StartRetreatSequence(transform.position);
    }

    #endregion

    private void SetObjectsActive(bool enemyReveal)
    {
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
        _animation.SetBoolRevealing(false);
    }
}
