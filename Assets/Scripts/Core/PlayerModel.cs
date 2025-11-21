using System;

public class PlayerModel
{
    public int Life { get; private set; }
    public int MaxLife { get; private set; }
    public int Mana { get; private set; }
    public int MaxMana { get; private set; }
    public int ManaAfterDeath { get; private set; }
    public int Damage { get; private set; }
    public int ClawDamage { get; private set; }
    public int SoulCrystalsCollected { get; private set; }
    public string CurrentScene { get; private set; }
    public bool HasClaw { get; private set; }
    public bool FacingRight { get; private set; }
    public bool IsDead { get { return Life <= 0; } }

    private PlayerModel(int life, int maxLife, int mana, int maxMana, int manaAfterDeath, int damage, int clawDamage, int soulCrystalsCollected, string currentScene, bool hasClaw, bool facingRight)
    {
        MaxLife = Math.Max(1, maxLife);
        Life = Math.Max(1, Math.Min(life, MaxLife));

        MaxMana = Math.Max(0, maxMana);
        Mana = Math.Max(0, Math.Min(mana, MaxMana));
        ManaAfterDeath = Math.Max(0, Math.Min(manaAfterDeath, MaxMana));

        Damage = Math.Max(1, damage);
        ClawDamage = Math.Max(1, clawDamage);

        SoulCrystalsCollected = Math.Max(0, soulCrystalsCollected);

        CurrentScene = currentScene;

        HasClaw = hasClaw;
        FacingRight = facingRight;
    }

    public static PlayerModel CreateFromPlayerData(PlayerData playerData)
    {
        return new PlayerModel(
            playerData.life,
            playerData.maxLife,
            playerData.mana,
            playerData.maxMana,
            playerData.manaAfterDeath,
            playerData.damage,
            playerData.clawDamage,
            playerData.soulCrystalsCollected,
            playerData.currentScene,
            playerData.hasClaw,
            playerData.facingRight
        );
    }

    //public string GetJsonStringForSave()
    //{
    //    // Создать json строку(файл создавать сразу думаю не надо - его создавать уже из полученных отсюда данных)
    //    return "json с данными игрока";
    //}

    //private string GetDataFromJsonSave(string jsonString)
    //{
    //    return "json с данными игрока";
    //}

    public bool SetFacingRight(bool facingRight)
    {
        FacingRight = facingRight;
        return true;
    }

    public bool AddLife(int value)
    {
        if (value <= 0)
            return false;
        Life = Math.Min(Life += value, MaxLife);
        return true;
    }

    public bool TakeDamage(int damage)
    {
        if (damage <= 0)
            return false;
        Life = Math.Max(0, Life - damage);
        return true;
    }

    public bool SetMana(int value)
    {
        Mana = Math.Max(0, value);
        if (Mana > MaxMana)
            Mana = MaxMana;
        return true;
    }

    public bool SetDamage(int value)
    {
        Damage = value;
        return true;
    }

    public bool SetClawDamage(int value)
    {
        ClawDamage = value;
        return true;
    }

    public bool AddSoulCrystal()
    {
        SoulCrystalsCollected += 1;
        return true;
    }

    public bool SetCurrentScene(string sceneName)
    {
        CurrentScene = sceneName;
        return true;
    }

    public bool SetHasClaw()
    {
        HasClaw = true;
        return HasClaw;
    }
}
