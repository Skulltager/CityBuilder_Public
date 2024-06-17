
public class ChunkRegionContent_Borders : ChunkRegionContent
{
    public ChunkRegionContent_Borders(ChunkRegion chunkRegion)
        : base(chunkRegion) { }

    public override void GenerateContent(Map map)
    {
        for (int i = 0; i < chunkRegion.points.Count; i++)
        {
            Point point = chunkRegion.points[i];
            map.SetChunkRegionPointContent(point.xIndex, point.yIndex, new ChunkRegionPointContent_Blocker(map, point));
        }
    }
}