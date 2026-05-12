using Unity.InferenceEngine;
using UnityEngine;

public class MnemirView : MonoBehaviour
{
    #region SerializeField

    [Header("Main params")]
    [SerializeField] private float _speed;
    [SerializeField] private LayerMask _turnLayerMask;

    #endregion

    #region Variables

    private MnemirMove _move;
    private MnemirAnimation _animation;

    #endregion

    #region Properties

    public bool FacingRight { get; private set; } = false;
    public bool IsBusy { get; private set; } = false;

    #endregion

    #region Common Methods

    private void Awake()
    {
        _move = GetComponent<MnemirMove>();
        _animation = GetComponent<MnemirAnimation>();

        _move.TurnLayerMask = _turnLayerMask;
    }

    private void FixedUpdate()
    {
        if (!IsBusy)
        {
            _move.Move(_speed, FacingRight);
            AnimationFlagsSwitcher(true);
        }
        else
            AnimationFlagsSwitcher(false);
    }

    #endregion

    private void AnimationFlagsSwitcher(bool isMoving)
    {
        _animation.SetBoolIdle(!isMoving);
        _animation.SetBoolMove(isMoving);
    }

    #region Events

    private void OnEnable()
    {
        _move.OnWallHit += HandleWallHit;
    }

    private void OnDisable()
    {
        _move.OnWallHit -= HandleWallHit;
    }

    private void HandleWallHit()
    {
        FacingRight = _move.Turn(FacingRight);
    }

    #endregion
}
