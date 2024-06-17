public class VillagerSubTask_DeconstructRoad : VillagerAssignedSubTask
{
    public readonly new BuildingTaskData_DeconstructRoad taskData;

    public VillagerSubTask_DeconstructRoad(Villager villager, BuildingTask buildingTask, BuildingTaskData_DeconstructRoad taskData)
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
        taskData.roadDeconstruction.deconstructionProgress.value += deltaTime / taskData.roadDeconstruction.roadRecord.ConstructionTime;
        if (taskData.roadDeconstruction.deconstructionProgress.value >= 1)
            return true;

        return false;
    }
}