using TMPro;
using UnityEngine;

public class FogShadowMove : MonoBehaviour
{
    #region Variables

    private FogShadowAttack _attack;
    private FogShadowAnimation _animation;

    private Vector2 _targetPosition;
    private Vector2 _startPosition;
    private bool _isChasing = false;

    #endregion

    #region Properties

    public float Speed { get; set; }
    public float ChaseSpeed { get { return Speed + 0.5f; } }
    public Vector2 PatrolRange { get; set; }
    public Vector2 TargetPosition { get { return _targetPosition; } }

    #endregion

    #region Common Methods

    private void Start()
    {
        _attack = GetComponent<FogShadowAttack>();
        _animation = GetComponent<FogShadowAnimation>();

        _startPosition = transform.position;
        SetNewTarget();
    }

    private void FixedUpdate()
    {
        
    }

    #endregion

    public void Chase()
    {
        _animation.SetBoolPatrol(false);
        _animation.SetBoolMove(true);

        transform.position = Vector2.MoveTowards(transform.position, _targetPosition, ChaseSpeed * Time.deltaTime);
        if (Vector2.Distance(transform.position, _targetPosition) < 0.2f)
            SetNewTarget();
    }

    public void Patrol()
    {
        _animation.SetBoolMove(false);
        _animation.SetBoolPatrol(true);

        transform.position = Vector2.MoveTowards(transform.position, _targetPosition, Speed * Time.deltaTime);
        if (Vector2.Distance(transform.position, _targetPosition) < 0.2f)
            SetNewTarget();
    }

    private void SetNewTarget()
    {
        var randomX = Random.Range(_startPosition.x - PatrolRange.x, _startPosition.x + PatrolRange.x);
        var randomY = Random.Range(_startPosition.y - PatrolRange.y, _startPosition.y + PatrolRange.y);

        _targetPosition = new Vector2(randomX, randomY);
    }

    public bool Turn(bool facingRight)
    {
        if (_attack.IsAttacking)
            return facingRight;

        Vector3 rotator;
        if (facingRight)
            rotator = new Vector3(transform.rotation.x, 0, transform.rotation.z);
        else
            rotator = new Vector3(transform.rotation.x, 180f, transform.rotation.z);
        transform.rotation = Quaternion.Euler(rotator);
        return !facingRight;
    }
}
