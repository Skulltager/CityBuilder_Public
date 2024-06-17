public class VillagerSubTask_ConstructRoad : VillagerAssignedSubTask
{
    public readonly new BuildingTaskData_ConstructRoad taskData;

    public VillagerSubTask_ConstructRoad(Villager villager, BuildingTask buildingTask, BuildingTaskData_ConstructRoad taskData)
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
        taskData.roadConstruction.constructionProgress.value += deltaTime / taskData.roadConstruction.roadRecord.ConstructionTime;
        return false;
    }
}