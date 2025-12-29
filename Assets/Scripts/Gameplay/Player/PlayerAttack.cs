using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class PlayerAttack : MonoBehaviour
{
    private static PlayerAttack instance;
    public static PlayerAttack Instance { get { return instance; } }

    [Header("Main Attack Params")]
    [SerializeField] private float attackSeriesTimeout = 0.9f;
    [SerializeField] private int maxAttackSeriesCount = 3;

    //[Header("Particles")]
    //[SerializeField] private ParticleSystem _attackParticle;

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
        if (instance != null)
        {
            Destroy(instance);
            return;
        }
        instance = this;

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
            playerView.PlayerModel.SetDamage(1);
            playerAnimation.SetTriggerAttack(1);
        }
        else if (attackSeriesCount == 2)
        {
            playerView.PlayerModel.SetDamage(1);
            playerAnimation.SetTriggerAttack(2);
        }
        else if (attackSeriesCount == 3)
        {
            playerView.PlayerModel.SetDamage(3);
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
            //Debug.Log("Серия завершена");
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

    public void AttackDamage() //Вызывается в середние анимаци атаки
    {
        var collidersEnemies = Physics2D.OverlapCircleAll(attackCheck.position, attackCheckRadius);
        for (var i = 0; i < collidersEnemies.Length; i++)
        {
            var enemy = collidersEnemies[i].gameObject;
            var objectEnvironment = collidersEnemies[i].gameObject;

            var damageToApply = playerView.PlayerModel.Damage;

            if (collidersEnemies[i].transform.position.x - transform.position.x < 0)
                damageToApply = -damageToApply;

            if (enemy.CompareTag("Enemy"))
            {
                collidersEnemies[i].gameObject.SendMessage("ApplyDamage", damageToApply);

                if (getMana != null && !enemy.CompareTag("isDead") && !objectEnvironment.CompareTag("Object"))
                    getMana.Invoke(enemy);
            }

            if (objectEnvironment.CompareTag("Object"))
                collidersEnemies[i].gameObject.SendMessage("ApplyDamage", new object[2] { false, damageToApply });
        }
    }

    public void AttackDashDamage()
    {
        var collidersEnemies = Physics2D.OverlapCircleAll(transform.position, attackCheckRadius);
        for (var i = 0; i < collidersEnemies.Length; i++)
        {
            var enemy = collidersEnemies[i].gameObject;
            var objectEnvironment = collidersEnemies[i].gameObject;

            playerView.PlayerModel.SetDamage(1);
            var damageToApply = playerView.PlayerModel.Damage;

            if (collidersEnemies[i].transform.position.x - transform.position.x < 0)
                damageToApply = -damageToApply;

            if (enemy.CompareTag("Enemy"))
            {
                collidersEnemies[i].gameObject.SendMessage("ApplyDamage", damageToApply);
            }
        }

    }

    private IEnumerator AttackCooldown(float durationAfterSeries)
    {
        yield return new WaitForSeconds(durationAfterSeries);
        canAttack = true;
    }
}
