using System.Collections.Generic;
using UnityEngine;

public class WallsExistence
{
    public HashSet<string> BrokenWalls { get; set; }

    private WallsExistence(HashSet<string> brokenWalls)
    {
        BrokenWalls = brokenWalls;
    }

    public static WallsExistence CreateEmpty()
    {
        return new WallsExistence(new HashSet<string>());
    }

    public static WallsExistence CreateWallsExistence(ref WallSaveData data)
    {
        HashSet<string> wallsSet;
        if (data.BrokenWalls == null)
            wallsSet = new HashSet<string>();
        else
            wallsSet = new HashSet<string>(data.BrokenWalls);
        return new WallsExistence(wallsSet);
    }

    public bool IsWallBroken(string wallID)
    {
        return BrokenWalls.Contains(wallID);
    }

    public void BreakWall(string wallID)
    {
        BrokenWalls.Add(wallID);
    }

    public void Save(ref WallSaveData data)
    {
        data.BrokenWalls = new List<string>(BrokenWalls);
    }
}

[System.Serializable]
public struct WallSaveData
{
    public List<string> BrokenWalls;
}
