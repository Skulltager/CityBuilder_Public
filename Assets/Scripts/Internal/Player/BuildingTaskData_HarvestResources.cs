using System.Collections.Generic;

public class BuildingTaskData_HarvestResources : BuildingTaskData
{
    public readonly ChunkMapPointContent_WorldResource worldResource;
    public readonly VillagerTaskPoint villagerTaskPoint;
    public readonly CardinalDirection harvestDirection;

    public BuildingTaskData_HarvestResources(List<ChunkMapPointContent> dependencies, ChunkMapPointContent_WorldResource worldResource, VillagerTaskPoint villagerTaskPoint, CardinalDirection harvestDirection, string taskDescription, object extraData = null) 
        : base(dependencies, taskDescription, extraData)
    {
        this.worldResource = worldResource;
        this.villagerTaskPoint = villagerTaskPoint;
        this.harvestDirection = harvestDirection;
    }
}