
using System;
using System.Collections.Generic;

public class BuildingTaskData_MoveToLocation : BuildingTaskData
{
    public readonly List<VillagerTaskPoint> villagerTaskPoints;
    public readonly ContentTaskLocation contentTaskLocation;
    public readonly bool reserveTaskLocation;
    public Action<Villager, BuildingTaskData_MoveToLocation> pathCalculatedCallback;

    public PathData pathData;

    public BuildingTaskData_MoveToLocation(List<ChunkMapPointContent> dependencies, List<VillagerTaskPoint> taskLocations, ContentTaskLocation contentTaskLocation, bool reserveTaskLocation, string taskDescription, object extraData = null)
        : base(dependencies, taskDescription, extraData)
    {
        this.villagerTaskPoints = taskLocations;
        this.contentTaskLocation = contentTaskLocation;
        this.reserveTaskLocation = reserveTaskLocation;
    }

    public void CalculatePathData(Villager villager)
    {
        pathData = villager.owner.playerRegion.CalculatePath(villager.point.value, villager.GetTilePosition(), villagerTaskPoints, contentTaskLocation);
        if (!dependencies.Contains(pathData.taskPoint.owner))
            AddDependency(pathData.taskPoint.owner);

        if (pathCalculatedCallback != null)
            pathCalculatedCallback(villager, this);
    }

    public void RecalculatePathData(Villager villager)
    {
        pathData = villager.owner.playerRegion.RecalculatePath(villager.point.value, villager.GetTilePosition(), pathData.taskPoint, contentTaskLocation);
    }
}