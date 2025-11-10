using System;

public class PlayerModel
{
    public int Life { get; private set; }
    public int Mana { get; private set; }
    public int Damage { get; private set; }
    public int SoulCrystalsCollected { get; private set; }
    public string CurrentScene { get; private set; }
    public bool HasClaw { get; private set; }
    public bool IsDead { get { return Life <= 0; } }

    public PlayerModel(int life, int mana, int damage, int soulCrystalsCollected, string currentScene, bool hasClaw)
    {
        Life = Math.Max(1, life);
        Mana = Math.Max(0, mana);
        Damage = Math.Max(1, damage);
        SoulCrystalsCollected = Math.Max(0, soulCrystalsCollected);
        CurrentScene = currentScene;
        HasClaw = hasClaw;
    }

    //public PlayerModel InitializePlayerModel(string)
    //{
    //    var data = GetDataFromJsonSave("json");
    //    return new PlayerModel(data);
    //}

    //public string GetJsonStringForSave()
    //{
    //    // Создать json строку(файл создавать сразу думаю не надо - его создавать уже из полученных отсюда данных)
    //    return "json с данными игрока";
    //}

    //private string GetDataFromJsonSave(string jsonString)
    //{
    //    return "json с данными игрока";
    //}

    public bool TakeDamage(int damage)
    {
        if (damage <= 0)
            return false;
        Life = Math.Max(0, Life - damage);
        return true;
    }

    public bool ChangeAmountOfMana(int value)
    {
        Mana = value;
        return true;
    }

    public bool IncreaseDamage(int value)
    {
        if (value <= 0)
            return false;
        Damage += value;
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
