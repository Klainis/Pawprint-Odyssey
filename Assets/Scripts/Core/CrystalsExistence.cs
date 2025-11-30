using System.Collections.Generic;
using UnityEngine;

public class CrystalsExistence
{
    public HashSet<string> BrokenCrystals { get; set; }

    private CrystalsExistence(HashSet<string> brokenCrystals)
    {
        BrokenCrystals = brokenCrystals;
    }

    public static CrystalsExistence CreateEmpty()
    {
        return new CrystalsExistence(new HashSet<string>());
    }

    public static CrystalsExistence CreateCrystalsExistence(ref CrystalSaveData data)
    {
        HashSet<string> crystalsSet;
        if (data.BrokenCrystals == null)
            crystalsSet = new HashSet<string>();
        else
            crystalsSet = new HashSet<string>(data.BrokenCrystals);
        return new CrystalsExistence(crystalsSet);
    }

    public bool IsCrystalBroken(string crystalID)
    {
        return BrokenCrystals.Contains(crystalID);
    }

    public void BreakCrystal(string crystalID)
    {
        BrokenCrystals.Add(crystalID);
    }

    public void Save(ref CrystalSaveData data)
    {
        data.BrokenCrystals = new List<string>(BrokenCrystals);
    }
}

[System.Serializable]
public struct CrystalSaveData
{
    public List<string> BrokenCrystals;
}
