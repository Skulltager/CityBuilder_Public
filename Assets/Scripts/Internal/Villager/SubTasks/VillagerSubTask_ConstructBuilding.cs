public class VillagerSubTask_ConstructBuilding : VillagerAssignedSubTask
{

    public readonly new BuildingTaskData_ConstructBuilding taskData;

    public VillagerSubTask_ConstructBuilding(Villager villager, BuildingTask buildingTask, BuildingTaskData_ConstructBuilding taskData)
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
        taskData.buildingConstruction.constructionProgress.value += deltaTime / taskData.buildingConstruction.record.ConstructionDuration;
        return false;
    }
}