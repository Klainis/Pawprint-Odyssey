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


    #endregion

    #region Common Methods

    private void Awake()
    {
        SetObjectsActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            SetObjectsActive(true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
    }

    #endregion

    private void SetObjectsActive(bool enemyReveal)
    {
        _leavesPileObj.SetActive(!enemyReveal);
        _rootGuardianObj.SetActive(enemyReveal);
    }

    private void OnTriggerExitTimerStart()
    {

    }
}
