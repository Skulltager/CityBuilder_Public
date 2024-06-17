public class ChunkRegionPointContent_BuildingPoint : ChunkRegionPointContent
{
    public readonly ChunkMapPointContent_Building building;
    public readonly bool isCenterPoint;

    public ChunkRegionPointContent_BuildingPoint(Map map, Point point, bool isCenterPoint, ChunkMapPointContent_Building building)
        : base(map, point)
    {
        this.building = building;
        this.isCenterPoint = isCenterPoint;
    }

    public override void SetChunkMapPointData(ChunkMapPoint chunkMapPoint)
    {
        chunkMapPoint.content.value = building;

        if (isCenterPoint)
            building.PlaceOnMap();
    }
}