using UnityEngine;

public class VillagerSubTask_DropOffResources : VillagerSubTask
{
    private PathData pathData;
    private int pathIndex;
    private Vector2 nextPosition;
    private ChunkMapPointContent_BuildingBase building;
    private ResourceDelivery delivery;
    private TilePosition previousTilePosition;
    private ChunkMapPoint previousChunkMapPoint;

    public VillagerSubTask_DropOffResources(Villager villager)
        : base(villager) { }

    public override void StartTask()
    {
        base.StartTask();

        villager.currentAnimation.value = AnimationType_Villager.Walk;

        previousTilePosition = villager.GetTilePosition();
        previousChunkMapPoint = villager.owner.playerRegion.map.GetChunkMapPoint(villager.point.value);

        building = villager.owner.GetResourceDumpingLocation(villager);
        building.onRemovedFromMap += OnEvent_BuildingRemovedFromMap;
        pathData = villager.owner.playerRegion.CalculatePath(villager.point.value, previousTilePosition, building.villagerTaskPoints, building.contentTaskLocation);

        pathIndex = 0;
        nextPosition = villager.CalculateVector(pathData.pathPoints[pathIndex]);

        delivery = new ResourceDelivery(villager, building.inventory, villager.inventory.value.record, villager.inventory.value.amount.value);
        delivery.StartDelivery();
        villager.currentTaskDescription.value = string.Format("Delivering {0} {1} to {2}", delivery.amount, delivery.record.Name, building.name);
    }

    private void OnEvent_BuildingRemovedFromMap(ChunkMapPointContent replacement)
    {
        villager.CancelTasks();
    }

    private void CalculateNextPosition()
    {
        PathPoint previousPathPoint = pathData.pathPoints[pathIndex - 1];
        previousChunkMapPoint = previousPathPoint.mapPoint;
        previousTilePosition = previousPathPoint.tilePosition;

        PathPoint nextPathPoint = pathData.pathPoints[pathIndex];
        //Only recalculate the path if the villager walks into a wall, DO NOT CANCEL IF A BUILDING IS PLACED ON TOP OF THE VILLAGER!
        if (nextPathPoint.doViableCheck && !nextPathPoint.mapPoint.tileTraversalDirection.value.IsViablePosition(nextPathPoint.tilePosition) && previousChunkMapPoint.tileTraversalDirection.value.IsViablePosition(previousTilePosition))
        {
            pathData = villager.owner.playerRegion.RecalculatePath(villager.point.value, villager.GetTilePosition(), pathData.taskPoint, building.contentTaskLocation);
            pathIndex = 0;
        }

        nextPosition = villager.CalculateVector(pathData.pathPoints[pathIndex]);
    }

    public override void InterruptTask()
    {
        base.InterruptTask();
        building.onRemovedFromMap -= OnEvent_BuildingRemovedFromMap;
        delivery.CancelDelivery();
    }

    public override void EndTask()
    {
        base.EndTask();
        building.onRemovedFromMap -= OnEvent_BuildingRemovedFromMap;
        delivery.DeliverDelivery();
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
            if (pathIndex == pathData.pathPoints.Count)
                return true;

            CalculateNextPosition();
            difference = nextPosition - villager.position.value;
            direction = difference.normalized;
        }

        villager.position.value += direction * moveAmount;
        return false;
    }
}