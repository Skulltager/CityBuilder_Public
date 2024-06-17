using SheetCodes;
using System.Collections.Generic;

public abstract class ChunkMapPointContent_BuildingBase : ChunkMapPointContent
{
    public override string name => record.Name;
    public virtual bool allowResourcePickup => true;
    public virtual bool allowAnyResourceDumping => false;
    public readonly BuildingRecord record;
    public readonly Player owner;
    public readonly List<ChunkMapPoint> outsideEnterExitPoints;
    public readonly List<ChunkMapPoint> insideEnterExitPoints;

    public readonly Inventory inventory;

    private int showCounter;

    public ChunkMapPointContent_BuildingBase(Map map, Point pivotPoint, CardinalDirection direction, BuildingRecord record, Player owner)
        : base(map, pivotPoint, direction, record.BuildingGridData.size)
    {
        this.record = record;
        this.owner = owner;

        outsideEnterExitPoints = new List<ChunkMapPoint>();
        insideEnterExitPoints = new List<ChunkMapPoint>();

        if (record.InfiniteStorage)
            inventory = new Inventory_Unlimited(record.AdjustableStorageDemand);
        else
            inventory = new Inventory_Limited(record.InventorySize, record.AdjustableStorageDemand);
    }

    public ChunkMapPointContent_BuildingBase(Map map, Point pivotPoint, CardinalDirection direction, BuildingRecord record, Player owner, Inventory inventory)
        : base(map, pivotPoint, direction, record.BuildingGridData.size)
    {
        this.inventory = inventory;
        this.record = record;
        this.owner = owner;

        outsideEnterExitPoints = new List<ChunkMapPoint>();
        insideEnterExitPoints = new List<ChunkMapPoint>();
    }

    protected override void InitializeSub()
    {
        base.InitializeSub();
        foreach (BuildingEnterExitGridPoint gridPoint in record.BuildingGridData.enterExitPoints)
        {
            CardinalDirection enterExitDirection = gridPoint.direction.RotateByDirection(direction);
            Point enterExitPoint = gridPoint.centerOffset.RotateByDirection(direction) + pivotPoint;
            ChunkMapPoint insideChunkMapPoint = map.GetChunkMapPoint(enterExitPoint);
            ChunkMapPoint outsideChunkMapPoint = map.GetChunkMapPoint(enterExitPoint.AddDirection(enterExitDirection));

            insideEnterExitPoints.Add(insideChunkMapPoint);
            outsideEnterExitPoints.Add(outsideChunkMapPoint);
        }

        if (contentTaskLocation == ContentTaskLocation.Inside)
            SetVillagerTaskPoints_InsideBuilding();
        else
            SetVillagerTaskPoints_AroundEdge();
    }

    public override void PlaceOnMap()
    {
        base.PlaceOnMap();
        owner.buildings.Add(this);

        foreach (ChunkMapPoint enterExitPoint in outsideEnterExitPoints)
            enterExitPoint.blockPreventionCounter.value++;
    }

    public override void RemoveFromMap(ChunkMapPointContent replacement = null)
    {
        base.RemoveFromMap(replacement);
        owner.buildings.Remove(this);

        foreach (ChunkMapPoint enterExitPoint in outsideEnterExitPoints)
            enterExitPoint.blockPreventionCounter.value--;
    }

    public override void Hide()
    {
        showCounter--;

        if (showCounter == 0)
            HideSub();
    }

    public override void Show()
    {
        showCounter++;

        if (showCounter == 1)
            ShowSub();
    }

    protected void SetVillagerTaskPoints_InsideBuilding()
    {
        foreach (BuildingEnterExitGridPoint gridPoint in record.BuildingGridData.enterExitPoints)
        {
            CardinalDirection enterExitDirection = gridPoint.direction.RotateByDirection(direction);
            Point enterExitPoint = gridPoint.centerOffset.RotateByDirection(direction) + pivotPoint;

            TilePosition tilePosition = default;

            switch (enterExitDirection)
            {
                case CardinalDirection.Right:
                    tilePosition = TilePosition.CenterRight;
                    break;

                case CardinalDirection.Bottom:
                    tilePosition = TilePosition.BottomCenter;
                    break;

                case CardinalDirection.Left:
                    tilePosition = TilePosition.CenterLeft;
                    break;

                case CardinalDirection.Top:
                    tilePosition = TilePosition.TopCenter;
                    break;
            }

            villagerTaskPoints.Add(new VillagerTaskPoint(this, enterExitPoint, tilePosition));
        }
    }

    protected virtual void ShowSub()
    {
        BuildingVisualizer.instance.Show(this);
        BuildingWorldUIManager.instance.Show(this);
    }

    protected virtual void HideSub()
    {
        BuildingVisualizer.instance.Hide(this);
        BuildingWorldUIManager.instance.Hide(this);
    }
}