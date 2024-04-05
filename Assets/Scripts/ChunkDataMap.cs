using SheetCodes;

public class ChunkDataMap
{
    public readonly WorldResourceRecord[,] resourceMap;
    public readonly BiomeType[,] biomeTypeMap;
    public readonly bool[,] fogOfWarMap;

    public readonly ChunkRegionMap regionMap;

    public int chunkWidth => regionMap.chunkWidth;
    public int chunkHeight => regionMap.chunkHeight;

    public int xChunkPosition => regionMap.xChunkPosition;
    public int yChunkPosition => regionMap.yChunkPosition;

    public ChunkDataMap(ChunkRegionMap regionMap)
    {
        this.regionMap = regionMap;
        resourceMap = new WorldResourceRecord[chunkWidth, chunkHeight];
        biomeTypeMap = new BiomeType[chunkWidth, chunkHeight];
        fogOfWarMap = new bool[chunkWidth, chunkHeight];
    }

    public void GenerateRegionContent()
    {
        regionMap.borderRegion.GenerateContent(regionMap.map);
        //foreach (ChunkRegion region in regionMap.regions)
        //{
        //    region.GenerateContent(regionMap.map);
        //}
    }
}