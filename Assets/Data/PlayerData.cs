using UnityEngine;

[CreateAssetMenu(menuName = "Player Data")]
public class PlayerData : ScriptableObject
{
    [Header("Player Health")]
    public int life;
    public int maxLife;

    [Header("Player Mana")]
    public int mana;
    public int maxMana;
    public int manaAfterDeath;

    [Header("Player Damage")]
    public int damage;
    public int clawDamage;

    [Header("Soul Crystal")]
    public int soulCrystalsCollected;

    [Header("Current Scene")]
    public string currentScene;

    [Header("Received Abilities")]
    public bool hasClaw;
    public bool hasDoubleJump;

    [Header("Player State")]
    public bool facingRight;
    public bool isDead;

    [Header("Killed Bosses")]
    public bool spiritGuideKilled;
    public bool guardianOwlKilled;

    public void Reset()
    {
        life = maxLife;
        mana = manaAfterDeath;
        isDead = false;
    }
}
