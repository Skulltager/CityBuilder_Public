
using System.Collections.Generic;

public class BuildingTaskData_DeconstructBuilding : BuildingTaskData
{
    public readonly ChunkMapPointContent_BuildingDeconstruction buildingDeconstruction;
    public readonly VillagerTaskPoint taskPoint;

    public BuildingTaskData_DeconstructBuilding(List<ChunkMapPointContent> dependencies, ChunkMapPointContent_BuildingDeconstruction buildingDeconstruction, VillagerTaskPoint taskPoint, string taskDescription, object extraData = null)
        : base(dependencies, taskDescription, extraData)
    {
        this.buildingDeconstruction = buildingDeconstruction;
        this.taskPoint = taskPoint;
    }
}