using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WallsExistence
{
    public HashSet<string> BrokenWalls { get; set; }

    private WallsExistence(HashSet<string> brokenWalls)
    {
        BrokenWalls = brokenWalls;
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

    //public class TransitionRoomWall
    //{
    //    public string SceneName;

    //    public WallItem[] WallItems;
    //}
    //public class WallItem
    //{
    //    public string WallName;
    //    public bool isOpen;
    //}

    //public static TransitionRoomWall[] TransitionWalls = new TransitionRoomWall[8]
    //{
    //    new TransitionRoomWall
    //    {
    //        SceneName = "F_Room_9",
    //        WallItems = new WallItem[1]
    //        {
    //            new WallItem
    //            {
    //                WallName = "9-12",
    //                isOpen = false
    //            }

    //        }
    //    },
    //    new TransitionRoomWall
    //    {
    //        SceneName = "F_Room_12",
    //        WallItems = new WallItem[2]
    //        {
    //            new WallItem
    //            {
    //                WallName = "9-12",
    //                isOpen = false
    //            },
    //            new WallItem
    //            {
    //                WallName = "12-14",
    //                isOpen = false
    //            }

    //        }
    //    },
    //    new TransitionRoomWall
    //    {
    //        SceneName = "F_Room_14",
    //        WallItems = new WallItem[1]
    //        {
    //            new WallItem
    //            {
    //                WallName = "12-14",
    //                isOpen = false
    //            }

    //        }
    //    },
    //    new TransitionRoomWall
    //    {
    //        SceneName = "F_Room_4",
    //        WallItems = new WallItem[1]
    //        {
    //            new WallItem
    //            {
    //                WallName = "4-16",
    //                isOpen = false
    //            }

    //        }
    //    },
    //    new TransitionRoomWall
    //    {
    //        SceneName = "F_Room_16",
    //        WallItems = new WallItem[2]
    //        {
    //            new WallItem
    //            {
    //                WallName = "4-16",
    //                isOpen = false
    //            },
    //            new WallItem
    //            {
    //                WallName = "18-16",
    //                isOpen = false
    //            }
    //        }
    //    },
    //    new TransitionRoomWall
    //    {
    //        SceneName = "F_Room_18",
    //        WallItems = new WallItem[1]
    //        {
    //            new WallItem
    //            {
    //                WallName = "18-16",
    //                isOpen = false
    //            }

    //        }
    //    },
    //    new TransitionRoomWall
    //    {
    //        SceneName = "F_Room_20",
    //        WallItems = new WallItem[1]
    //        {
    //            new WallItem
    //            {
    //                WallName = "20-21",
    //                isOpen = false
    //            }

    //        }
    //    },
    //    new TransitionRoomWall
    //    {
    //        SceneName = "F_Room_21",
    //        WallItems = new WallItem[1]
    //        {
    //            new WallItem
    //            {
    //                WallName = "20-21",
    //                isOpen = false
    //            }

    //        }
    //    },
    //};
}

[System.Serializable]
public struct WallSaveData
{
    public List<string> BrokenWalls;
}
