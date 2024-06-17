using SheetCodes;

public class ChunkMapPointContent_Building_House : ChunkMapPointContent_Building
{
    public ChunkMapPointContent_Building_House(Map map, Point centerPoint, CardinalDirection direction, Player owner, ChunkMapPointContent_BuildingConstruction construction)
        : base(map, centerPoint, direction, BuildingIdentifier.House.GetRecord(), owner, construction)
    {
    }

    public override bool TryAssignVillagerTasks(Villager villager)
    {
        return false;
    }
}