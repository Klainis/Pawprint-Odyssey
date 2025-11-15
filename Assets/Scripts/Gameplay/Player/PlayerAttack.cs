using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class PlayerAttack : MonoBehaviour
{
    [Header("Main Attack Params")]
    [SerializeField] private float attackSeriesTimeout = 0.9f;
    [SerializeField] private int maxAttackSeriesCount = 3;

    [SerializeField] private UnityEvent<GameObject> getMana;

    const float attackCheckRadius = 1.1f;

    private PlayerView playerView;
    private PlayerAnimation playerAnimation;

    private Transform attackCheck;

    private float lastAttackTime;
    private int attackSeriesCount = 0;

    private bool isAttacking = false;
    private bool canAttack = true;

    public int AttackSeriesCount { get { return attackSeriesCount; } private set { attackSeriesCount = value; } }

    private void Awake()
    {
        playerView = GetComponent<PlayerView>();
        playerAnimation = GetComponent<PlayerAnimation>();

        attackCheck = transform.Find("AttackCheck");
    }

    private void Update()
    {
        playerAnimation.ApplyRootMotion(false);

        if (attackSeriesCount > 0 && (Time.time - lastAttackTime > attackSeriesTimeout))
            ResetCombo();
    }

    public void Attack()
    {
        if (isAttacking || !canAttack) return;

        lastAttackTime = Time.time;
        attackSeriesCount++;

        playerAnimation.SetBoolIsAttacking(true);

        if (attackSeriesCount == 1)
        {
            playerView.PlayerModel.ChangeDamage(1);
            playerAnimation.SetTriggerAttack(1);
        }
        else if (attackSeriesCount == 2)
        {
            playerView.PlayerModel.ChangeDamage(1);
            playerAnimation.SetTriggerAttack(2);
        }
        else if (attackSeriesCount == 3)
        {
            playerView.PlayerModel.ChangeDamage(3);
            playerAnimation.SetTriggerAttack(3);
        }

        isAttacking = true;
    }

    public void OnAttackAnimationEnd()
    {
        isAttacking = false;
        canAttack = false;

        if (attackSeriesCount >= maxAttackSeriesCount)
        {
            StartCoroutine(AttackCooldown(0.3f));
            Debug.Log("Серия завершена");
            ResetCombo();
        }
        else
            StartCoroutine(AttackCooldown(0.03f));
    }

    private void ResetCombo()
    {
        attackSeriesCount = 0;
        isAttacking = false;
        playerAnimation.ResetTriggerAttack(1);
        playerAnimation.ResetTriggerAttack(2);
        playerAnimation.ResetTriggerAttack(3);
    }

    public void AttackDamage()
    {
        var collidersEnemies = Physics2D.OverlapCircleAll(attackCheck.position, attackCheckRadius);
        for (var i = 0; i < collidersEnemies.Length; i++)
        {
            var enemy = collidersEnemies[i].gameObject;
            var objectEnvironment = collidersEnemies[i].gameObject;

            if (enemy.CompareTag("Enemy"))
            {
                var damageToApply = playerView.PlayerModel.Damage;

                if (collidersEnemies[i].transform.position.x - transform.position.x < 0)
                    damageToApply = -damageToApply;

                collidersEnemies[i].gameObject.SendMessage("ApplyDamage", damageToApply);

                if (getMana != null && !enemy.CompareTag("isDead") && !objectEnvironment.CompareTag("Object"))
                    getMana.Invoke(enemy);
            }

            if (objectEnvironment.CompareTag("Object"))
                collidersEnemies[i].gameObject.SendMessage("ApplyDamage", false);
        }
    }

    private IEnumerator AttackCooldown(float durationAfterSeries)
    {
        yield return new WaitForSeconds(durationAfterSeries);
        canAttack = true;
    }
}
