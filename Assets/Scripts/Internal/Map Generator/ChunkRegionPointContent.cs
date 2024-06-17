
public abstract class ChunkRegionPointContent
{
    public readonly Map map;
    public readonly Point point;

    public ChunkRegionPointContent(Map map, Point point)
    {
        this.map = map;
        this.point = point;
    }

    public abstract void SetChunkMapPointData(ChunkMapPoint chunkMapPoint);
}