public class ChunkRegionPointContent_Blocker : ChunkRegionPointContent
{
    public ChunkRegionPointContent_Blocker(Map map, Point point)
        : base(map, point) { }

    public override void SetChunkMapPointData(ChunkMapPoint chunkMapPoint)
    {
        chunkMapPoint.content.value = new ChunkMapPointContent_Blocker(map, point);
        chunkMapPoint.tileTraversalDirection.value = TileTraversalEdgeDirection.AllEdges;
    }
}