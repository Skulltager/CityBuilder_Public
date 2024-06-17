using UnityEngine;

public class VillagerSubTask_MoveToLocation : VillagerAssignedSubTask
{
    private TilePosition previousTilePosition;
    private ChunkMapPoint previousChunkMapPoint;

    private int pathIndex;
    private Vector2 nextPosition;
    public readonly new BuildingTaskData_MoveToLocation taskData;

    public VillagerSubTask_MoveToLocation(Villager villager, BuildingTask buildingTask, BuildingTaskData_MoveToLocation taskData)
        : base(villager, buildingTask, taskData) 
    {
        this.taskData = taskData;
    }

    public override void StartTask()
    {
        taskData.CalculatePathData(villager);

        base.StartTask();

        pathIndex = 0;
        previousTilePosition = villager.GetTilePosition();
        previousChunkMapPoint = villager.owner.playerRegion.map.GetChunkMapPoint(villager.point.value);

        nextPosition = villager.CalculateVector(taskData.pathData.pathPoints[pathIndex]);
        villager.currentAnimation.value = AnimationType_Villager.Walk;

        if (taskData.reserveTaskLocation)
            taskData.pathData.taskPoint.enabled.value = false;
    }

    private void CalculateNextPosition()
    {
        PathPoint previousPathPoint = taskData.pathData.pathPoints[pathIndex - 1];
        previousChunkMapPoint = previousPathPoint.mapPoint;
        previousTilePosition = previousPathPoint.tilePosition;

        PathPoint nextPathPoint = taskData.pathData.pathPoints[pathIndex];
        //Only recalculate the path if the villager walks into a wall, DO NOT CANCEL IF A BUILDING IS PLACED ON TOP OF THE VILLAGER!
        if (nextPathPoint.doViableCheck && !nextPathPoint.mapPoint.tileTraversalDirection.value.IsViablePosition(nextPathPoint.tilePosition) && previousChunkMapPoint.tileTraversalDirection.value.IsViablePosition(previousTilePosition))
        {
            taskData.RecalculatePathData(villager);
            pathIndex = 0;
        }

        nextPosition = villager.CalculateVector(taskData.pathData.pathPoints[pathIndex]);
    }

    protected override void CloseTask()
    {
        base.CloseTask();
        if (taskData.reserveTaskLocation)
            taskData.pathData.taskPoint.enabled.value = true;
    }

    public override bool Update(float deltaTime)
    {
        Vector2 difference = nextPosition - villager.position.value;
        Vector2 direction = difference.normalized;
        float currentMoveSpeed = villager.movementSpeed / villager.chunkMapPoint.value.movementSlowFactor;
        float moveAmount = deltaTime * currentMoveSpeed;
        villager.currentMovementSpeed.value = currentMoveSpeed;

        RotateTowardsDirection(deltaTime, direction);
        while (moveAmount > difference.magnitude)
        {
            villager.position.value = nextPosition;
            moveAmount -= difference.magnitude;

            //Reached the end
            pathIndex++;
            if (pathIndex == taskData.pathData.pathPoints.Count)
                return true;

            CalculateNextPosition();
            difference = nextPosition - villager.position.value;
            direction = difference.normalized;
        }

        villager.position.value += direction * moveAmount;
        return false;
    }
}