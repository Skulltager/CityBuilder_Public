using SheetCodes;

public class ChunkDataMap
{
    public readonly WorldResourceRecord[,] resourceMap;
    public readonly BiomeType[,] biomeTypeMap;
    public readonly VisionState[,] fogOfWarMap;

    public readonly ChunkRegionMap regionMap;

    public int chunkWidth => regionMap.chunkWidth;
    public int chunkHeight => regionMap.chunkHeight;

    public int xChunkPosition => regionMap.xChunkPosition;
    public int yChunkPosition => regionMap.yChunkPosition;

    private bool regionContentGenerated;

    public ChunkDataMap(ChunkRegionMap regionMap)
    {
        this.regionMap = regionMap;
        resourceMap = new WorldResourceRecord[chunkWidth, chunkHeight];
        biomeTypeMap = new BiomeType[chunkWidth + 2, chunkHeight + 2];
        fogOfWarMap = new VisionState[chunkWidth + 2, chunkHeight + 2];
    }

    public void GenerateRegionContent()
    {
        if (regionContentGenerated)
            return;

        regionContentGenerated = true;
        regionMap.borderRegion.GenerateContent(regionMap.map);
        foreach (ChunkRegion region in regionMap.regions)
            region.GenerateContent(regionMap.map);
    }

    public void GenerateAdjacentChunkMapRegionContent()
    {
        regionMap.map.GetChunkDataMap(xChunkPosition - 1, yChunkPosition - 1).GenerateRegionContent();
        regionMap.map.GetChunkDataMap(xChunkPosition, yChunkPosition - 1).GenerateRegionContent();
        regionMap.map.GetChunkDataMap(xChunkPosition + 1, yChunkPosition - 1).GenerateRegionContent();

        regionMap.map.GetChunkDataMap(xChunkPosition - 1, yChunkPosition).GenerateRegionContent();
        regionMap.map.GetChunkDataMap(xChunkPosition + 1, yChunkPosition).GenerateRegionContent();

        regionMap.map.GetChunkDataMap(xChunkPosition - 1, yChunkPosition + 1).GenerateRegionContent();
        regionMap.map.GetChunkDataMap(xChunkPosition, yChunkPosition + 1).GenerateRegionContent();
        regionMap.map.GetChunkDataMap(xChunkPosition + 1, yChunkPosition + 1).GenerateRegionContent();
    }
}