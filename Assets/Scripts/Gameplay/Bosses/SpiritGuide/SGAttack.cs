using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public class SGAttack : MonoBehaviour
{
    [Header("Main params")]
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private Transform checkTransfom;
    [SerializeField] private Transform pivotTop;
    [SerializeField] private Transform pivotBottom;

    [Header("Attack 'Ram'")]
    [SerializeField] private float acceleratedSpeed = 10f;
    [SerializeField] private float ramTelegraphTime = 0.25f;
    [SerializeField] private float ramPauseBetweenSeries = 3f;

    [Header("Attack 'Light Attack'")]
    [SerializeField] private GameObject lightAttackObject;
    [SerializeField] private float lightZoneCooldown = 3f;
    [SerializeField] private float lightZoneChance = 0.8f;
    [SerializeField] private float lightZoneTime = 1f;
    [SerializeField] private int lightObjectAmount = 7;
    [SerializeField] private float instantiateTimeStep = 0.5f;
    [SerializeField] private float lightAttackObjectOffsetStep = 1f;
    //[SerializeField] private float lightZoneTelegraphTime = 0.5f;

    //public event Action OnPlayerDetected;

    private SpiritGuideView sgView;
    private SGAnimation sgAnimation;

    private const int maxRamSeriesCount = 3;
    private int ramSeriesCount = 0;
    private float nextLightZoneTime = 0f;
    private bool inLightZone = false;

    private RigidbodyConstraints2D normalConstraints;
    private BoxCollider2D collider;

    public float AcceleratedSpeed { get { return acceleratedSpeed; } }

    private Coroutine lightAttackCoroutine;

    //private void OnDrawGizmos()
    //{
    //    Gizmos.DrawWireCube((Vector2)checkTransfom.position + (new Vector2(_distance / 2, 0) * _direction), new Vector2(_distance, 3));
    //}

    private void Awake()
    {
        sgView = GetComponent<SpiritGuideView>();
        sgAnimation = GetComponent<SGAnimation>();

        collider = GetComponent<BoxCollider2D>();
    }

    private void Start()
    {
        normalConstraints = sgView.RigidBody.constraints;
    }

    public void RandomAttack(bool facingRight)
    {
        var wallHits = GetWallHits(6);
        var playerHitsSecondStage = GetPlayerHits(10, facingRight);
        if (wallHits[0].collider == null && wallHits[1].collider == null && Time.time >= nextLightZoneTime)
        {
            bool isPlayerHits = false;
            for (var i = 0; i < playerHitsSecondStage.Count; i++)
            {
                if (playerHitsSecondStage[i].collider != null)
                {
                    Debug.Log($"Hace player collider {playerHitsSecondStage[i]}");
                    isPlayerHits = true;

                    if (playerHitsSecondStage[i].collider.transform.position.x < transform.position.x && !sgView.FacingRight)
                    {
                        sgView.TurnToPlayer();
                    }
                    else if (playerHitsSecondStage[i].collider.transform.position.x > transform.position.x && sgView.FacingRight)
                    {
                        sgView.TurnToPlayer();
                    }
                }

            }

            if (UnityEngine.Random.value <= lightZoneChance && isPlayerHits)
            {
                LightAttackTelegraph();
                nextLightZoneTime = Time.time + lightZoneCooldown;
                isPlayerHits = false;
            }
            else if (!inLightZone && isPlayerHits)
            {
                RamTelegraph();
                sgView.IsAccelerated = true;
                ramSeriesCount += 1;
                isPlayerHits = false;
            }
        }
    }

    public void RamAttack(List<RaycastHit2D> playerHits)
    {
        for (var i = 0; i < playerHits.Count; i++)
        {
            if (playerHits[i].collider != null)
            {
                if (playerHits[i].collider.transform.position.x < transform.position.x && !sgView.FacingRight)
                {
                    sgView.TurnToPlayer();
                }
                else if (playerHits[i].collider.transform.position.x > transform.position.x && sgView.FacingRight)
                {
                    sgView.TurnToPlayer();
                }

                RamTelegraph();
                sgView.IsAccelerated = true;
                ramSeriesCount += 1;
                break;
            }
        }
    }

    public void CheckRamSeriesCountAndPause()
    {
        Debug.Log($"current{ramSeriesCount}, max{maxRamSeriesCount}");
        if (ramSeriesCount >= maxRamSeriesCount)
        {
            ramSeriesCount = 0;
            StartCoroutine(RamPause());
        }
    }

    public List<RaycastHit2D> GetPlayerHits(float distance, bool facingRight)
    {
        var direction = facingRight ? Vector2.left : Vector2.right;
        var playerHitTopRight = Physics2D.Raycast(pivotTop.position, direction, distance, playerLayer);
        var playerHitBottomRight = Physics2D.Raycast(pivotBottom.position, direction, distance, playerLayer);
        var playerHitTopLeft = Physics2D.Raycast(pivotTop.position, -direction, distance, playerLayer);
        var playerHitBottomLeft = Physics2D.Raycast(pivotBottom.position, -direction, distance, playerLayer);

        //var playerHit = Physics2D.OverlapBox((Vector2)checkTransfom.position + (new Vector2(distance / 2, 0) * direction), new Vector2(distance, 3), 0);

        return new List<RaycastHit2D> { playerHitTopRight, playerHitBottomRight, playerHitTopLeft, playerHitBottomLeft };
    }

    public List<RaycastHit2D> GetWallHits(float distance)
    {
        var wallHitLeft = Physics2D.Raycast(pivotBottom.position, Vector2.left, distance, wallLayer);
        var wallHitRight = Physics2D.Raycast(pivotBottom.position, Vector2.right, distance, wallLayer);
        return new List<RaycastHit2D> { wallHitLeft, wallHitRight };
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (!sgView.Model.IsDead)
            {
                var playerView = collision.gameObject.GetComponent<PlayerView>();
                playerView.ApplyDamage(sgView.Model.Damage, transform.position, gameObject);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (!sgView.Model.IsDead)
            {
                var playerView = collision.gameObject.GetComponent<PlayerView>();
                playerView.ApplyDamage(sgView.Model.Damage, transform.position, gameObject);
            }
        }
    }

    private void RamTelegraph()
    {
        sgAnimation.SetWalkAnimation(false);
        sgAnimation.SetRunTelegraphAnimation(true);
        sgView.MoveDisabled = true;
        sgView.RigidBody.constraints = RigidbodyConstraints2D.FreezePosition;
    }

    public void Run()
    {
        sgAnimation.SetRunTelegraphAnimation(false);
        sgView.RigidBody.constraints = normalConstraints;
        sgView.MoveDisabled = false;
    }

    private IEnumerator RamPause()
    {
        Debug.Log("Stop Ram!!");
        sgView.MoveDisabled = true;
        sgAnimation.SetRunAnimation(false);
        sgAnimation.SetWalkAnimation(false);

        sgView.RigidBody.constraints = RigidbodyConstraints2D.FreezePosition;
        yield return new WaitForSeconds(ramPauseBetweenSeries);
        sgView.RigidBody.constraints = normalConstraints;
        
        sgView.MoveDisabled = false;
    }

    private void LightAttackTelegraph()
    {
        sgAnimation.SetWalkAnimation(false);
        sgAnimation.SetRunAnimation(false);
        sgAnimation.SetLightAttackAnimation(true);

        inLightZone = true;
        sgView.MoveDisabled = true;
        sgView.RigidBody.constraints = RigidbodyConstraints2D.FreezePosition;
        //renderer.color = normalColor;
    }

    public void LightAttack()
    {
        if (lightAttackCoroutine != null)
        {
            StopCoroutine(lightAttackCoroutine);
        }
        int turnCoefficient = sgView.FacingRight? -1 : 1;
        lightAttackCoroutine = StartCoroutine(LightAttackRoutine(turnCoefficient));
    }

    private IEnumerator LightAttackRoutine(int direction)
    {
        var currentAmount = 0;
        float lightAttackObjectStep = 0;
        GameObject lightAttackObjectInstance;

        Quaternion rotator = Quaternion.identity;

        while (currentAmount < lightObjectAmount)
        {
            currentAmount++;
            if (direction < 1)
            {
                rotator = Quaternion.Euler(new Vector3(0, 0, 0));
            }
            else
            {
                rotator = Quaternion.Euler(new Vector3(0, 180, 0));
            }

            lightAttackObjectInstance = Instantiate(lightAttackObject, checkTransfom.position + new Vector3(lightAttackObjectStep, 0, 0) * direction, rotator);

            lightAttackObjectStep += lightAttackObjectOffsetStep;

            yield return new WaitForSeconds(instantiateTimeStep);
            yield return null;
        }

        sgAnimation.SetLightAttackAnimation(false);

        lightAttackObjectStep = 0;

        sgView.MoveDisabled = false;
        sgView.RigidBody.constraints = normalConstraints;
        inLightZone = false;
    }
}
