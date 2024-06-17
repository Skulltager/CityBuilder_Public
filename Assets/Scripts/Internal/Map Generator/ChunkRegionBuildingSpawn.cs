
using SheetCodes;

public class ChunkRegionBuildingSpawn
{
    public readonly BuildingRecord buildingRecord;
    public readonly Point spawnPoint;
    public readonly CardinalDirection direction;

    public ChunkRegionBuildingSpawn(BuildingRecord buildingRecord, Point spawnPoint, CardinalDirection direction)
    {
        this.buildingRecord = buildingRecord;
        this.spawnPoint = spawnPoint;
        this.direction = direction;
    }
}