
using System.Collections.Generic;
using UnityEngine;

public class BuildingTaskData_CraftItem : BuildingTaskData
{
    public readonly ChunkMapPointContent_Building_CraftingStation craftingStation;
    public readonly CraftingSlot craftingSlot;

    public BuildingTaskData_CraftItem(List<ChunkMapPointContent> dependencies, ChunkMapPointContent_Building_CraftingStation craftingStation, CraftingSlot craftingSlot, string taskDescription, object extraData = null)
        : base(dependencies, taskDescription, extraData)
    {
        this.craftingStation = craftingStation;
        this.craftingSlot = craftingSlot;
    }
}