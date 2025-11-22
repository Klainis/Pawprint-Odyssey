using Unity.VisualScripting;
using UnityEngine;

public static class WallsExistence
{
    private static WallSaveData _wallData = new WallSaveData();

    public static bool IsWallBroken(string wallID)
    {
        return _wallData.brokenWalls.Contains(wallID);
    }

    public static void BreakWall(string wallID)
    {
        _wallData.brokenWalls.Add(wallID);
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
