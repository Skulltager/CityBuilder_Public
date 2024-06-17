
using System.Collections.Generic;

public class BuildingTaskData_DeconstructRoad : BuildingTaskData
{
    public readonly ChunkMapPointContent_RoadDeconstruction roadDeconstruction;
    public readonly VillagerTaskPoint taskPoint;

    public BuildingTaskData_DeconstructRoad(List<ChunkMapPointContent> dependencies, ChunkMapPointContent_RoadDeconstruction roadDeconstruction, VillagerTaskPoint taskPoint, string taskDescription, object extraData = null)
        : base(dependencies, taskDescription, extraData)
    {
        this.roadDeconstruction = roadDeconstruction;
        this.taskPoint = taskPoint;
    }
}