using System;
using System.Collections.Generic;

public abstract class BuildingTaskData
{
    public readonly List<ChunkMapPointContent> dependencies;
    public readonly object extraData;
    public event Action onCanceled;
    public string taskDescription;
    private bool boundToDependencies;

    public BuildingTaskData(List<ChunkMapPointContent> dependencies, string taskDescription, object extraData)
    {
        this.dependencies = dependencies;
        this.taskDescription = taskDescription;
        this.extraData = extraData;
    }

    public void BindDependencies()
    {
        boundToDependencies = true;
        foreach (ChunkMapPointContent dependency in dependencies)
        {
            dependency.onRemovedFromMap += OnEvent_Dependency_RemovedFromMap;
        }
    }

    public void UnbindDependencies()
    {
        boundToDependencies = false;
        foreach (ChunkMapPointContent dependency in dependencies)
        {
            dependency.onRemovedFromMap -= OnEvent_Dependency_RemovedFromMap;
        }
    }

    protected void AddDependency(ChunkMapPointContent dependency)
    {
        if (!boundToDependencies)
            return;

        dependency.onRemovedFromMap += OnEvent_Dependency_RemovedFromMap;
        dependencies.Add(dependency);
    }

    private void OnEvent_Dependency_RemovedFromMap(ChunkMapPointContent replacement)
    {
        if (onCanceled != null)
            onCanceled();
    }
}