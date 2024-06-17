
public class ChunkMapTileDirectionPoint
{
    public readonly ChunkMapPoint chunkMapPoint;
    public readonly TileTraversalEdgeDirection tileEdgeDirection;

    public ChunkMapTileDirectionPoint(ChunkMapPoint chunkMapPoint, TileTraversalEdgeDirection tileEdgeDirection)
    {
        this.chunkMapPoint = chunkMapPoint;
        this.tileEdgeDirection = tileEdgeDirection;
    }
}