using SheetCodes;
using System.Collections.Generic;
using UnityEngine;

public class Villager
{
    private const float EDGE_WALK_DISTANCE = 0.1f;

    public readonly Player owner;
    public readonly VillagerRecord record;
    public readonly EventVariable<Villager, Point> point;
    public readonly EventVariable<Villager, ChunkMapPoint> chunkMapPoint;
    public readonly EventVariable<Villager, Vector2> position;
    public readonly EventVariable<Villager, float> currentRotation;
    public readonly EventVariable<Villager, AnimationType_Villager> currentAnimation;
    public readonly EventVariable<Villager, ChunkMapPointContent_Building> assignedBuilding;
    public readonly EventVariable<Villager, float> currentMovementSpeed;
    public readonly List<VillagerSubTask> queuedTasks;
    public readonly EventVariable<Villager, ResourceCount> inventory;
    public readonly EventVariable<Villager, string> currentTaskDescription;
    public readonly string name;
    public VillagerSubTask currentTask;

    public float baseRotateSpeed { private set; get; }
    public float rotateDifferenceSpeedFactor { private set; get; }
    public float movementSpeed { private set; get; }
    public float harvestSpeed { private set; get; }
    public int carryCapacity { private set; get; }

    public Villager(Player owner, VillagerRecord record, ChunkMapPoint chunkMapPoint, string name)
    {
        this.name = name;
        this.owner = owner;
        this.record = record;
        movementSpeed = 1f;
        baseRotateSpeed = 90;
        rotateDifferenceSpeedFactor = 2f;
        carryCapacity = 4;
        harvestSpeed = 1;

        point = new EventVariable<Villager, Point>(this, chunkMapPoint.globalPoint);
        position = new EventVariable<Villager, Vector2>(this, new Vector2(chunkMapPoint.globalPoint.xIndex + 0.5f, chunkMapPoint.globalPoint.yIndex + 0.5f));
        currentRotation = new EventVariable<Villager, float>(this, 0);
        currentAnimation = new EventVariable<Villager, AnimationType_Villager>(this, default);
        assignedBuilding = new EventVariable<Villager, ChunkMapPointContent_Building>(this, default);
        this.chunkMapPoint = new EventVariable<Villager, ChunkMapPoint>(this, chunkMapPoint);
        currentMovementSpeed = new EventVariable<Villager, float>(this, 1);
        queuedTasks = new List<VillagerSubTask>();
        inventory = new EventVariable<Villager, ResourceCount>(this, null);
        currentTaskDescription = new EventVariable<Villager, string>(this, string.Empty);

        assignedBuilding.onValueChange += OnValueChanged_AssignedBuilding;
        position.onValueChange += OnValueChanged_Position;
        point.onValueChange += OnValueChanged_Point;
        currentTask = new VillagerSubTask_Idle(this, 1);
    }

    private void OnValueChanged_Point(Point oldValue, Point newValue)
    {
        chunkMapPoint.value = owner.playerRegion.map.GetChunkMapPoint(newValue);
    }

    private void OnValueChanged_AssignedBuilding(ChunkMapPointContent_Building oldValue, ChunkMapPointContent_Building newValue)
    {
        if (oldValue != null)
            oldValue.assignedVillagers.Remove(this);

        if (newValue != null)
            newValue.assignedVillagers.Add(this);

        CancelTasks();
    }

    private void OnValueChanged_Position(Vector2 oldValue, Vector2 newValue)
    {
        int xGridPosition = Mathf.FloorToInt(newValue.x);
        int yGridPosition = Mathf.FloorToInt(newValue.y);

        point.value = new Point(xGridPosition, yGridPosition);
    }

    public TilePosition GetTilePosition()
    {
        Vector2 position = this.position.value;
        float xRemainder = position.x < 0 ? 1 - (Mathf.Abs(position.x) % 1) : position.x % 1;
        float yRemainder = position.y < 0 ? 1 - (Mathf.Abs(position.y) % 1) : position.y % 1;

        if (xRemainder < EDGE_WALK_DISTANCE * 2)
        {
            if (yRemainder < EDGE_WALK_DISTANCE * 2)
                return TilePosition.TopRight;

            if (yRemainder > 1 - EDGE_WALK_DISTANCE * 2)
                return TilePosition.TopLeft;

            return TilePosition.CenterLeft;
        }

        if (xRemainder > 1 - EDGE_WALK_DISTANCE * 2)
        {
            if (yRemainder < EDGE_WALK_DISTANCE * 2)
                return TilePosition.BottomRight;

            if (yRemainder > 1 - EDGE_WALK_DISTANCE * 2)
                return TilePosition.TopRight;

            return TilePosition.CenterRight;
        }

        if (yRemainder < EDGE_WALK_DISTANCE * 2)
            return TilePosition.BottomCenter;

        if (yRemainder > 1 - EDGE_WALK_DISTANCE * 2)
            return TilePosition.TopCenter;

        return TilePosition.Center;
    }

    public void NextTask()
    {
        if (currentTask != null)
            currentTask.EndTask();

        if (queuedTasks.Count == 0)
        {
            if (inventory.value != null)
                queuedTasks.Add(new VillagerSubTask_DropOffResources(this));
            else if (!assignedBuilding.value.TryAssignVillagerTasks(this))
                queuedTasks.Add(new VillagerSubTask_Idle(this, 1));
        }

        currentTask = queuedTasks[0];
        queuedTasks.RemoveAt(0);
        currentTask.StartTask();
    }

    public void CancelTasks()
    {
        if (currentTask == null)
            return;

        currentTask.InterruptTask();
        currentTask = null;

        queuedTasks.Clear();
        NextTask();
    }

    public void FixedUpdate()
    {
        while (currentTask == null || currentTask.Update(Time.fixedDeltaTime) )
            NextTask();
    }

    public void Show()
    {
        VillagerVisualizer.instance.Show(this);
    }

    public void Hide()
    {
        VillagerVisualizer.instance.Hide(this);
    }

    public Vector2 CalculateVector(PathPoint pathPoint)
    {
        return CalculateVector(pathPoint.mapPoint.globalPoint, pathPoint.tilePosition);
    }

    public Vector2 CalculateVector(Point currentPoint, TilePosition currentTilePosition)
    {
        switch (currentTilePosition)
        {
            case TilePosition.Center:
                return new Vector2(currentPoint.xIndex + 0.5f, currentPoint.yIndex + 0.5f);
            case TilePosition.BottomLeft:
                return new Vector2(currentPoint.xIndex + EDGE_WALK_DISTANCE, currentPoint.yIndex + EDGE_WALK_DISTANCE);
            case TilePosition.BottomCenter:
                return new Vector2(currentPoint.xIndex + 0.5f, currentPoint.yIndex + EDGE_WALK_DISTANCE);
            case TilePosition.BottomRight:
                return new Vector2(currentPoint.xIndex + 1 - EDGE_WALK_DISTANCE, currentPoint.yIndex + EDGE_WALK_DISTANCE);
            case TilePosition.CenterLeft:
                return new Vector2(currentPoint.xIndex + EDGE_WALK_DISTANCE, currentPoint.yIndex + 0.5f);
            case TilePosition.CenterRight:
                return new Vector2(currentPoint.xIndex + 1 - EDGE_WALK_DISTANCE, currentPoint.yIndex + 0.5f);
            case TilePosition.TopLeft:
                return new Vector2(currentPoint.xIndex + EDGE_WALK_DISTANCE, currentPoint.yIndex + 1 - EDGE_WALK_DISTANCE);
            case TilePosition.TopCenter:
                return new Vector2(currentPoint.xIndex + 0.5f, currentPoint.yIndex + 1 - EDGE_WALK_DISTANCE);
            case TilePosition.TopRight:
                return new Vector2(currentPoint.xIndex + 1 - EDGE_WALK_DISTANCE, currentPoint.yIndex + 1 - EDGE_WALK_DISTANCE);
        }

        throw new System.Exception(string.Format("CalculateVector is missing implementation for {0}", currentTilePosition));
    }
}