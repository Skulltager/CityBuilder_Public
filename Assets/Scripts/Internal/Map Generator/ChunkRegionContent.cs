
public abstract class ChunkRegionContent
{
    public readonly ChunkRegion chunkRegion;

    public ChunkRegionContent(ChunkRegion chunkRegion)
    {
        this.chunkRegion = chunkRegion;
    }

    public abstract void GenerateContent(Map map);
}