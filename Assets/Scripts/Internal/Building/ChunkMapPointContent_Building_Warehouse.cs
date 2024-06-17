using SheetCodes;

public class ChunkMapPointContent_Building_Warehouse : ChunkMapPointContent_Building
{
    public override bool allowAnyResourceDumping => true;
    private BuildingTask_Warehouse buildingTask;
    public new Inventory_Limited inventory => base.inventory as Inventory_Limited;

    public ChunkMapPointContent_Building_Warehouse(Map map, Point centerPoint, CardinalDirection direction, Player owner, ChunkMapPointContent_BuildingConstruction construction)
        : base(map, centerPoint, direction, BuildingIdentifier.Warehouse.GetRecord(), owner, construction)
    {
        buildingTask = new BuildingTask_Warehouse(this, 1, 1);
    }

    public override bool TryAssignVillagerTasks(Villager villager)
    {
        if (!buildingTask.TryReserveVillagerTaskData(villager))
            return false;

        return true;
    }
}