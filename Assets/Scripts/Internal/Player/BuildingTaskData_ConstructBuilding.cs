
using System.Collections.Generic;

public class BuildingTaskData_ConstructBuilding : BuildingTaskData
{
    public readonly ChunkMapPointContent_BuildingConstruction buildingConstruction;
    public readonly VillagerTaskPoint taskPoint;

    public BuildingTaskData_ConstructBuilding(List<ChunkMapPointContent> dependencies, ChunkMapPointContent_BuildingConstruction buildingConstruction, VillagerTaskPoint taskPoint, string taskDescription, object extraData = null) 
        : base(dependencies, taskDescription, extraData)
    {
        this.buildingConstruction = buildingConstruction;
        this.taskPoint = taskPoint;
    }
}