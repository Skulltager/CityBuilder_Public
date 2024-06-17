using SheetCodes;

public class VillagerSubTask_HarvestResource : VillagerAssignedSubTask
{
    public readonly new BuildingTaskData_HarvestResources taskData;

    private ChunkMapPointContent_WorldResource worldResource;
    private float harvestTimeRemaining;
    private bool cancelImmedietly;

    public VillagerSubTask_HarvestResource(Villager villager, BuildingTask villagerTask, BuildingTaskData_HarvestResources taskData)
        : base(villager, villagerTask, taskData)
    {
        this.taskData = taskData;
    }

    public override void StartTask()
    {
        base.StartTask();
        cancelImmedietly = false;
        worldResource = taskData.villagerTaskPoint.owner as ChunkMapPointContent_WorldResource;
        worldResource.onRemovedFromMap += OnEvent_WorldResourceRemovedFromMap;
        switch (worldResource.record.Identifier)
        {
            case WorldResourceIdentifier.IronOre:
                villager.currentAnimation.value = AnimationType_Villager.Mining;
                break;
            case WorldResourceIdentifier.Tree:
                villager.currentAnimation.value = AnimationType_Villager.Woodcutting;
                break;
            case WorldResourceIdentifier.BerryBush:
                villager.currentAnimation.value = AnimationType_Villager.Gathering;
                break;
        }
        harvestTimeRemaining = worldResource.record.HarvestDuration;
        taskData.villagerTaskPoint.enabled.value = false;
    }

    protected override void CloseTask()
    {
        base.CloseTask();
        taskData.villagerTaskPoint.enabled.value = true;
        worldResource.onRemovedFromMap -= OnEvent_WorldResourceRemovedFromMap;
    }

    private void OnEvent_WorldResourceRemovedFromMap(ChunkMapPointContent replacement)
    {
        cancelImmedietly = true;
    }

    public override bool Update(float deltaTime)
    {
        if (cancelImmedietly)
            return true;

        harvestTimeRemaining -= deltaTime * villager.harvestSpeed;
        RotateTowardsDirection(deltaTime, taskData.harvestDirection);
        if (harvestTimeRemaining <= 0)
        {
            if (villager.inventory.value == null)
                villager.inventory.value = new ResourceCount(worldResource.record.ResourceDrops, 1);
            else
                villager.inventory.value.amount.value++;

            worldResource.timesHarvestableRemaining.value--;
            harvestTimeRemaining = worldResource.record.HarvestDuration;
            if (villager.inventory.value.amount.value == villager.carryCapacity)
                return true;
        }

        return false;
    }
}