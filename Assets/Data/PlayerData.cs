using UnityEngine;

[CreateAssetMenu(menuName = "Player Data")]
public class PlayerData : ScriptableObject
{
    [Header("Player Health")]
    public int currentLife;
    public int maxLife;

    [Header("Player Mana")]
    public int currentMana;
    public int maxMana;

    [Header("Received Abilities")]
    public bool clawIsReceived;

    [Header("Soul Crystal")]
    public int soulCrystalCount;
}
