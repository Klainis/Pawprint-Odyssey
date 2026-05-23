using System.Collections.Generic;
using UnityEngine;

public class MoneyObjectExistence
{
    public HashSet<string> BrokenMoneyObject { get; set; }

    private MoneyObjectExistence(HashSet<string> brokenMoneyObject)
    {
        BrokenMoneyObject = brokenMoneyObject;
    }

    public static MoneyObjectExistence CreateEmpty()
    {
        return new MoneyObjectExistence(new HashSet<string>());
    }

    public static MoneyObjectExistence CreateMoneyObjectExistence(ref MoneyObjectSaveData data)
    {
        HashSet<string> moneyObjectSet;
        if (data.BrokenMoneyObjects == null)
            moneyObjectSet = new HashSet<string>();
        else
            moneyObjectSet = new HashSet<string>(data.BrokenMoneyObjects);
        return new MoneyObjectExistence(moneyObjectSet);
    }

    public bool IsMoneyObjectBroken(string moneyObjectID)
    {
        return BrokenMoneyObject.Contains(moneyObjectID);
    }

    public void BreakMoneyObject(string moneyObjectID)
    {
        BrokenMoneyObject.Add(moneyObjectID);
    }

    public void Save(ref MoneyObjectSaveData data)
    {
        data.BrokenMoneyObjects = new List<string>(BrokenMoneyObject);
    }
}

[System.Serializable]
public struct MoneyObjectSaveData
{
    public List<string> BrokenMoneyObjects;
}
