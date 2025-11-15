using System;

public class PlayerModel
{
    public int Life { get; private set; }
    public int MaxLife { get; private set; }
    public int Mana { get; private set; }
    public int MaxMana { get; private set; }
    public int Damage { get; private set; }
    public int SoulCrystalsCollected { get; private set; }
    public string CurrentScene { get; private set; }
    public bool HasClaw { get; private set; }
    public bool FacingRight { get; private set; }
    public bool IsDead { get { return Life <= 0; } }

    private PlayerModel(int life, int maxLife, int mana, int maxMana, int damage, int soulCrystalsCollected, string currentScene, bool hasClaw, bool facingRight)
    {
        Life = Math.Max(1, life);
        MaxLife = Math.Max(1, maxLife);
        Mana = Math.Max(0, mana);
        MaxMana = Math.Max(0, maxMana);
        Damage = Math.Max(1, damage);
        SoulCrystalsCollected = Math.Max(0, soulCrystalsCollected);
        CurrentScene = currentScene;
        HasClaw = hasClaw;
        FacingRight = facingRight;
    }

    public static PlayerModel CreateFromPlayerData(PlayerData playerData)
    {
        return new PlayerModel(
            playerData.currentLife,
            playerData.maxLife,
            playerData.currentMana,
            playerData.maxMana,
            playerData.damage,
            playerData.soulCrystalCount,
            playerData.currentScene,
            playerData.clawIsReceived,
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

    public bool ChangeLife(int life)
    {
        if (life <= 0)
            return false;
        Life = life;
        return true;
    }

    public bool TakeDamage(int damage)
    {
        if (damage <= 0)
            return false;
        Life = Math.Max(0, Life - damage);
        return true;
    }

    public bool ChangeAmountOfMana(int value)
    {
        Mana = Math.Max(0, value);
        if (Mana > MaxMana)
            Mana = MaxMana;
        return true;
    }

    public bool ChangeDamage(int value)
    {
        Damage = value;
        return true;
    }

    public bool AddSoulCrystal()
    {
        SoulCrystalsCollected += 1;
        return true;
    }

    public bool ChangeCurrentScene(string sceneName)
    {
        CurrentScene = sceneName;
        return true;
    }

    public bool GetClaw()
    {
        HasClaw = true;
        return HasClaw;
    }
}
