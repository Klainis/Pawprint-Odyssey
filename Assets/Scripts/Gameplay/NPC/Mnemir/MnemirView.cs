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

    private float _timer;

    #endregion

    #region Properties

    public bool FacingRight { get; private set; } = false;
    public bool IsSpeaking { get; private set; } = false;  /* Use when speak with player */
    public bool IsLocked { get; private set; } = false;

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
        _timer -= Time.deltaTime;
        if (_timer <= 0) SwitchState();

        if (!IsSpeaking && !IsLocked)
        {
            _move.Move(_speed, FacingRight);
            _animation.SwitchMoving(true);
        }
        else
        {
            _move.StopMove();
            _animation.SwitchMoving(false);
        }
    }

    #endregion

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

    private void SwitchState()
    {
        _timer = UnityEngine.Random.Range(3f, 7f);
        IsLocked = !IsLocked;
    }
}
