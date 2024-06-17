
using SheetCodes;

public class ChunkRegionPointContent_WorldResource : ChunkRegionPointContent
{
    private readonly WorldResourceRecord record;

    public ChunkRegionPointContent_WorldResource(Map map, Point point, WorldResourceRecord record)
        : base (map, point)
    {
        this.record = record;
    }

    public override void SetChunkMapPointData(ChunkMapPoint chunkMapPoint)
    {
        ChunkMapPointContent_WorldResource worldResource = new ChunkMapPointContent_WorldResource(map, chunkMapPoint, record);
        chunkMapPoint.content.value = worldResource;
        chunkMapPoint.tileTraversalDirection.value = TileTraversalEdgeDirection.AllEdges;

        worldResource.PlaceOnMap();
    }
}