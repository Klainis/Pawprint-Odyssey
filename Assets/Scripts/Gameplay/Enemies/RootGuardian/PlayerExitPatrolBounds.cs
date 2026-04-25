using UnityEngine;

public class PlayerExitPatrolBounds : MonoBehaviour
{
    [SerializeField] private GameObject _rootGuardianObj;
    [SerializeField] private GameObject _zoneHandlerObj;

    private RootGuardianView _view;
    private RootGuardianTargetZoneHandler _targetZoneHandler;

    private void Awake()
    {
        _view = _rootGuardianObj.GetComponent<RootGuardianView>();
        _targetZoneHandler = _zoneHandlerObj.GetComponent<RootGuardianTargetZoneHandler>();
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && _targetZoneHandler.IsRevealed)
        {
            //Debug.Log("Игрок вышел из патрульной зоны");
            _view.StartRetreatSequence(_zoneHandlerObj.transform.position);
        }
    }
}
