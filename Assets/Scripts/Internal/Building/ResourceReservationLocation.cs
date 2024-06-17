
using SheetCodes;

public class ResourceReservationLocation
{
    public readonly ChunkMapPointContent_BuildingBase building;
    public readonly ResourcesRecord record;
    public readonly int amount;

    public ResourceReservationLocation(ChunkMapPointContent_BuildingBase building, ResourcesRecord record, int amount)
    {
        this.building = building;
        this.record = record;
        this.amount = amount;
    }
}