
using System;

public abstract class VillagerAssignedSubTask : VillagerSubTask
{
    public readonly BuildingTask buildingTask;
    public readonly BuildingTaskData taskData;
    public Action<Villager, BuildingTaskData> canceledCallback;
    public Action<Villager, BuildingTaskData> finishedCallback;

    public VillagerAssignedSubTask(Villager villager, BuildingTask buildingTask, BuildingTaskData taskData)
        : base(villager)
    {
        this.buildingTask = buildingTask;
        this.taskData = taskData;
    }

    public override void StartTask()
    {
        taskData.BindDependencies();
        taskData.onCanceled += OnEvent_TaskCanceled;
        buildingTask.assignedVillagers++;
        villager.currentTaskDescription.value = taskData.taskDescription;
    }

    private void OnEvent_TaskCanceled()
    {
        villager.CancelTasks();
    }

    public override void InterruptTask()
    {
        CloseTask();

        if (canceledCallback != null)
            canceledCallback(villager, taskData);
    }

    public override void EndTask()
    {
        CloseTask();

        if (finishedCallback != null)
            finishedCallback(villager, taskData);
    }


    protected virtual void CloseTask()
    {
        taskData.UnbindDependencies();
        taskData.onCanceled -= OnEvent_TaskCanceled;
        buildingTask.assignedVillagers--;
    }
}