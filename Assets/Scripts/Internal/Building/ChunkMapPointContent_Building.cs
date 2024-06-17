using SheetCodes;

public abstract class ChunkMapPointContent_Building : ChunkMapPointContent_BuildingBase
{
    public override ContentTaskLocation contentTaskLocation => ContentTaskLocation.Inside;
    public override ChunkMapPointContentType contentType => ChunkMapPointContentType.Building;
    public readonly EventList<Villager> assignedVillagers;
    public readonly EventVariable<ChunkMapPointContent_Building, int> assignedVillagersCount;
    public virtual bool canBeDestroyed { get { return true; } }

    public ChunkMapPointContent_Building(Map map, Point pivotPoint, CardinalDirection direction, BuildingRecord record, Player owner, ChunkMapPointContent_BuildingConstruction construction)
        : this(map, pivotPoint, direction, record, owner) 
    {
        construction.inventory.CopyInventoryTo(inventory);
        
        foreach (ResourceCostRecord resourceCost in record.ResourceCosts)
        {
            Resource resource;
            if (!inventory.resources.TryGetValue(resourceCost.Resource.Identifier, out resource))
                continue;

            resource.amount.value -= resourceCost.Amount;
        }
    }

    public ChunkMapPointContent_Building(Map map, Point pivotPoint, CardinalDirection direction, BuildingRecord record, Player owner)
        : base(map, pivotPoint, direction, record, owner)
    {
        assignedVillagers = new EventList<Villager>();
        assignedVillagersCount = new EventVariable<ChunkMapPointContent_Building, int>(this, 0);
        assignedVillagers.onAdd += OnAdd_AssignedVillagers;
        assignedVillagers.onRemove += OnRemove_AssignedVillagers;
    }

    private void OnAdd_AssignedVillagers(Villager villager)
    {
        assignedVillagersCount.value++;
    }

    private void OnRemove_AssignedVillagers(Villager villager)
    {
        assignedVillagersCount.value--;
    }

    public void Deconstruct()
    {
        ChunkMapPointContent_BuildingDeconstruction deconstruction = new ChunkMapPointContent_BuildingDeconstruction(map, pivotPoint, direction, record, owner, this);
        RemoveFromMap(deconstruction);
        deconstruction.PlaceOnMap();
    }

    public override void PlaceOnMap()
    {
        base.PlaceOnMap();
        foreach (VillagerTaskPoint enterExitTaskPoint in villagerTaskPoints)
        {
            ChunkMapPoint mapPoint = map.GetChunkMapPoint(enterExitTaskPoint.point);
            mapPoint.enterExitTilePosition = enterExitTaskPoint.tilePosition;
        }
        owner.TryAssignMaxVillagersToBuilding(this);
    }

    public override void RemoveFromMap(ChunkMapPointContent replacement = null)
    {
        base.RemoveFromMap(replacement);
        foreach (VillagerTaskPoint enterExitTaskPoint in villagerTaskPoints)
        {
            ChunkMapPoint mapPoint = map.GetChunkMapPoint(enterExitTaskPoint.point);
            mapPoint.enterExitTilePosition = TilePosition.Center;
        }

        owner.UnassignAllVillagersFromBuilding(this);
    }

    public abstract bool TryAssignVillagerTasks(Villager villager);
}