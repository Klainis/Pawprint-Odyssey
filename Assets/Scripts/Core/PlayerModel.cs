using System;

public class PlayerModel
{
    #region Properties

    public int Life { get; private set; }
    public int MaxLife { get; private set; }
    public int Mana { get; private set; }
    public int MaxMana { get; private set; }
    public int ManaAfterDeath { get; private set; }
    public int Damage { get; private set; }
    public int ClawDamage { get; private set; }
    public int SoulCrystalsCollected { get; private set; }
    public int MoneyCollected { get; private set; }
    public string CurrentScene { get; private set; }
    public float CurPosX { get; private set; }
    public float CurPosY { get; private set; }
    public string CheckPointScene { get; private set; }
    public float CheckPointPosX { get; private set; }
    public float CheckPointPosY { get; private set; }
    public bool HasClaw { get; private set; }
    public bool HasDoubleJump { get; private set; }
    public bool HasDash { get; private set; }
    public bool HasWallRun { get; private set; }
    public bool HasRun { get; private set; }
    public bool HasDamageDash { get; private set; }
    public bool FacingRight { get; private set; }
    public bool IsDead { get { return Life <= 0; } }
    public bool SpiritGuideKilled { get; private set; }
    public bool GuardianOwlKilled { get; private set; }

    #endregion

    #region Init New Instance

    private PlayerModel(int life, int maxLife,
                        int mana, int maxMana, int manaAfterDeath,
                        int damage, int clawDamage,
                        int soulCrystalsCollected,
                        int moneyCollected,
                        string currentScene, float curPosX, float curPosY,
                        string checkPointScene, float checkPointPosX, float checkPointPosY,
                        bool hasClaw, bool hasDoubleJump, bool hasDash, bool hasWallRun, bool hasRun, bool hasDamageDash,
                        bool facingRight,
                        bool spiritGuideKilled, bool guardianOwlKilled)
    {
        MaxLife = Math.Max(1, maxLife);
        Life = Math.Max(1, Math.Min(life, MaxLife));

        MaxMana = Math.Max(0, maxMana);
        Mana = Math.Max(0, Math.Min(mana, MaxMana));
        ManaAfterDeath = Math.Max(0, Math.Min(manaAfterDeath, MaxMana));

        Damage = Math.Max(1, damage);
        ClawDamage = Math.Max(1, clawDamage);

        SoulCrystalsCollected = Math.Max(0, soulCrystalsCollected);
        MoneyCollected = Math.Max (0, moneyCollected);

        CurrentScene = currentScene;
        CurPosX = curPosX;
        CurPosY = curPosY;

        CheckPointScene = checkPointScene;
        CheckPointPosX = checkPointPosX;
        CheckPointPosY = checkPointPosY;

        HasClaw = hasClaw;
        HasDoubleJump = hasDoubleJump;
        HasDash = hasDash;
        HasWallRun = hasWallRun;
        HasRun = hasRun;
        HasDamageDash = hasDamageDash;
        FacingRight = facingRight;
        SpiritGuideKilled = spiritGuideKilled;
        GuardianOwlKilled = guardianOwlKilled;
    }

    public static PlayerModel CreateFromSave(ref PlayerSaveData data)
    {
        return new PlayerModel(
            data.Life,
            data.MaxLife,
            data.Mana,
            data.MaxMana,
            data.ManaAfterDeath,
            data.Damage,
            data.ClawDamage,
            data.SoulCrystalsCollected,
            data.MoneyCollected,
            data.CurrentScene,
            data.CurPosX,
            data.CurPosY,
            data.CheckPointScene,
            data.CheckPointPosX,
            data.CheckPointPosY,
            data.HasClaw,
            data.HasDoubleJump,
            data.HasDash,
            data.HasWallRun,
            data.HasRun,
            data.HasDamageDash,
            data.FacingRight,
            data.SpiritGuideKilled,
            data.GuardianOwlKilled
        );
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
            playerData.moneyCollected,
            playerData.currentScene,
            playerData.curPosX,
            playerData.curPosY,
            playerData.checkPointScene,
            playerData.checkPointPosX,
            playerData.checkPointPosY,
            playerData.hasClaw,
            playerData.hasDoubleJump,
            playerData.hasDash,
            playerData.hasWallRun,
            playerData.hasRun,
            playerData.hasDamageDash,
            playerData.facingRight,
            playerData.spiritGuideKilled,
            playerData.guardianOwlKilled
        );
    }

    #endregion

    #region Change Properties

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

    public bool FullHeal()
    {
        Life = MaxLife;
        return true;
    }

    public bool FullMana()
    {
        Mana = MaxMana;
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

    public bool AddMoney(int reward)
    {
        MoneyCollected += reward;
        return true;
    }

    public bool SpendCrystal(int spend)
    {
        SoulCrystalsCollected -= spend;
        return true;
    }

    public bool SpendMoney(int spend)
    {
        MoneyCollected -= spend;
        return true;
    }

    public bool SetCurrentScene(string sceneName)
    {
        CurrentScene = sceneName;
        return true;
    }

    public bool SetCurrentPosition(float posX, float posY)
    {
        CurPosX = posX;
        CurPosY = posY;
        return true;
    }

    public bool SetCheckPointScene(string sceneName)
    {
        CheckPointScene = sceneName;
        return true;
    }

    public bool SetCheckPointPosition(float posX, float posY)
    {
        CheckPointPosX = posX;
        CheckPointPosY = posY;
        return true;
    }

    public bool SetHasClaw()
    {
        HasClaw = true;
        return HasClaw;
    }

    public bool SetHasDoubleJump()
    {
        HasDoubleJump = true;
        return HasDoubleJump;
    }

    public bool SetHasDash()
    {
        HasDash = true;
        return HasDash;
    }

    public bool SetHasWallRun()
    {
        HasWallRun = true;
        return HasWallRun;
    }

    public bool SetHasRun()
    {
        HasRun = true;
        return HasRun;
    }

    public bool SetHasDamageDash()
    {
        HasDamageDash = true;
        return HasDamageDash;
    }

    public bool SetSpiritGuideKilled()
    {
        SpiritGuideKilled = true;
        return SpiritGuideKilled;
    }

    public bool SetGuardianOwlKilled()
    {
        GuardianOwlKilled = true;
        return GuardianOwlKilled;
    }

    #endregion

    #region Save & Load

    public void Save(ref PlayerSaveData data)
    {
        data.Life = Life;
        data.MaxLife = MaxLife;
        data.Mana = Mana;
        data.MaxMana = MaxMana;
        data.ManaAfterDeath = ManaAfterDeath;
        data.Damage = Damage;
        data.ClawDamage = ClawDamage;
        data.SoulCrystalsCollected = SoulCrystalsCollected;
        data.MoneyCollected = MoneyCollected;
        data.CurrentScene = CurrentScene;
        data.CurPosX = CurPosX;
        data.CurPosY = CurPosY;
        data.CheckPointScene = CheckPointScene;
        data.CheckPointPosX = CheckPointPosX;
        data.CheckPointPosY = CheckPointPosY;
        data.HasClaw = HasClaw;
        data.HasDoubleJump = HasDoubleJump;
        data.HasDash = HasDash;
        data.HasWallRun = HasWallRun;
        data.HasRun = HasRun;
        data.HasDamageDash = HasDamageDash;
        data.FacingRight = true;
        data.SpiritGuideKilled = SpiritGuideKilled;
        data.GuardianOwlKilled = GuardianOwlKilled;
    }

    public void Load(PlayerSaveData data)
    {
        Life = data.Life;
        MaxLife = data.MaxLife;
        Mana = data.Mana;
        MaxMana = data.MaxMana;
        ManaAfterDeath = data.ManaAfterDeath;
        Damage = data.Damage;
        ClawDamage = data.ClawDamage;
        SoulCrystalsCollected = data.SoulCrystalsCollected;
        MoneyCollected = data.MoneyCollected;
        CurrentScene = data.CurrentScene;
        CurPosX = data.CurPosX;
        CurPosY = data.CurPosY;
        CheckPointScene = data.CheckPointScene;
        CheckPointPosX = data.CheckPointPosX;
        CheckPointPosY = data.CheckPointPosY;
        HasClaw = data.HasClaw;
        HasDoubleJump = data.HasDoubleJump;
        HasDash = data.HasDash;
        HasWallRun = data.HasWallRun;
        HasRun = data.HasRun;
        HasDamageDash = data.HasDamageDash;
        FacingRight = data.FacingRight;
        SpiritGuideKilled = data.SpiritGuideKilled;
        GuardianOwlKilled = data.GuardianOwlKilled;
    }

    #endregion
}

[System.Serializable]
public struct PlayerSaveData
{
    public int Life;
    public int MaxLife;
    public int Mana;
    public int MaxMana;
    public int ManaAfterDeath;
    public int Damage;
    public int ClawDamage;
    public int SoulCrystalsCollected;
    public int MoneyCollected;
    public string CurrentScene;
    public float CurPosX;
    public float CurPosY;
    public string CheckPointScene;
    public float CheckPointPosX;
    public float CheckPointPosY;
    public bool HasClaw;
    public bool HasDoubleJump;
    public bool HasDash;
    public bool HasWallRun;
    public bool HasRun;
    public bool HasDamageDash;
    public bool FacingRight;
    public bool SpiritGuideKilled;
    public bool GuardianOwlKilled;
}
