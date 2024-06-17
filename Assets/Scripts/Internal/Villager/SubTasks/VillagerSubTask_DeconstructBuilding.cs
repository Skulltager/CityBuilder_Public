public class VillagerSubTask_DeconstructBuilding : VillagerAssignedSubTask
{
    public readonly new BuildingTaskData_DeconstructBuilding taskData;

    public VillagerSubTask_DeconstructBuilding(Villager villager, BuildingTask buildingTask, BuildingTaskData_DeconstructBuilding taskData)
        : base(villager, buildingTask, taskData)
    {
        this.taskData = taskData;
    }

    public override void StartTask()
    {
        base.StartTask();
        villager.currentAnimation.value = AnimationType_Villager.Building;
        taskData.taskPoint.enabled.value = false;
    }

    protected override void CloseTask()
    {
        base.CloseTask();
        taskData.taskPoint.enabled.value = true;
    }

    public override bool Update(float deltaTime)
    {
        RotateTowardsTilePositionDirection(deltaTime, taskData.taskPoint.tilePosition);
        taskData.buildingDeconstruction.deconstructionProgress.value += deltaTime / taskData.buildingDeconstruction.record.ConstructionDuration;
        if (taskData.buildingDeconstruction.deconstructionProgress.value >= 1)
            return true;

        return false;
    }
}