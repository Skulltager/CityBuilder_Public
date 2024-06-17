
using System.Collections.Generic;

public class BuildingTaskData_ConstructRoad : BuildingTaskData
{
    public readonly ChunkMapPointContent_RoadConstruction roadConstruction;
    public readonly VillagerTaskPoint taskPoint;

    public BuildingTaskData_ConstructRoad(List<ChunkMapPointContent> dependencies, ChunkMapPointContent_RoadConstruction buildingConstruction, VillagerTaskPoint taskPoint, string taskDescription, object extraData = null)
        : base(dependencies, taskDescription, extraData)
    {
        this.roadConstruction = buildingConstruction;
        this.taskPoint = taskPoint;
    }
}