using SheetCodes;

public class VillagerSubTask_CraftItem : VillagerAssignedSubTask
{
    private const string TASK_DESCRIPTION_FORMAT = "Crafting {0}";
    public readonly new BuildingTaskData_CraftItem taskData;

    public VillagerSubTask_CraftItem(Villager villager, BuildingTask villagerTask, BuildingTaskData_CraftItem taskData)
        : base(villager, villagerTask, taskData)
    {
        this.taskData = taskData;
    }

    public override bool Update(float deltaTime)
    {
        if (taskData.craftingSlot.AddProgress(deltaTime))
            return true;

        return false;
    }
}