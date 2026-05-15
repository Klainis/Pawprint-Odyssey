using Unity.InferenceEngine;
using UnityEngine;

public class MnemirView : MonoBehaviour
{
    #region SerializeField

    [Header("Main params")]
    [SerializeField] private float _speed;
    [SerializeField] private LayerMask _turnLayerMask;
    [SerializeField] private GameObject _artefactObject;

    #endregion

    #region Variables

    //private MnemirMove _move;
    private MnemirAnimation _animation;
    private Interact interact;

    private GameObject _artefactInstance;

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
        //_move = GetComponent<MnemirMove>();
        _animation = GetComponent<MnemirAnimation>();
        interact = FindAnyObjectByType<Interact>();

        //_move.TurnLayerMask = _turnLayerMask;
    }

    private void FixedUpdate()
    {
        _timer -= Time.deltaTime;
        if (_timer <= 0) SwitchState();

        //if (!IsSpeaking && !IsLocked)
        //{
        //    _move.Move(_speed, FacingRight);
        //    _animation.SwitchMoving(true);
        //}
        //else
        //{
        //    _move.StopMove();
        //    _animation.SwitchMoving(false);
        //}
    }

    #endregion

    #region Events

    private void OnEnable()
    {
        //_move.OnWallHit += HandleWallHit;
        interact.OnCompleteMnemirQuest += HandleCompleteMnemirQuest;
    }

    private void OnDisable()
    {
        //_move.OnWallHit -= HandleWallHit;
        interact.OnCompleteMnemirQuest -= HandleCompleteMnemirQuest;
    }

    //private void HandleWallHit()
    //{
    //    FacingRight = _move.Turn(FacingRight);
    //}

    private void HandleCompleteMnemirQuest()
    {
        //var pos = transform.position;
        //var rewardPos = new Vector3(pos.x - 1, pos.y + 0.5f, pos.z);
        //Instantiate(_mnemirQuestReward, rewardPos, Quaternion.identity);

        _artefactInstance = Instantiate(_artefactObject, transform.position, Quaternion.identity);
        var artefactRB = _artefactInstance.GetComponent<Rigidbody2D>();
        artefactRB.AddForce(new Vector2(-50f, 50f));
    }

    #endregion

    private void SwitchState()
    {
        _timer = UnityEngine.Random.Range(3f, 7f);
        IsLocked = !IsLocked;
    }
}
