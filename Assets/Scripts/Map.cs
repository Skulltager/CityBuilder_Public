
using SheetCodes;
using UnityEngine;

public class Map
{
    public readonly int regionMinSize;
    public readonly int regionMaxSize;
    public readonly int chunkWidth;
    public readonly int chunkHeight;
    public readonly FourDirectionalList<ChunkRegionMap> chunkRegionMaps;
    public readonly FourDirectionalList<ChunkDataMap> chunkDataMaps;
    public readonly FourDirectionalList<ChunkMap> chunkMaps;
    public readonly EventList<ChunkMap> generatedChunkMaps;

    public Map(int chunkWidth, int chunkHeight, int regionMinSize, int regionMaxSize)
    {
        this.chunkWidth = chunkWidth;
        this.chunkHeight = chunkHeight;
        this.regionMinSize = regionMinSize;
        this.regionMaxSize = regionMaxSize;
        chunkRegionMaps = new FourDirectionalList<ChunkRegionMap>(null);
        chunkDataMaps = new FourDirectionalList<ChunkDataMap>(null);
        chunkMaps = new FourDirectionalList<ChunkMap>(null);
        generatedChunkMaps = new EventList<ChunkMap>();
    }

    public void GenerateChunkMapRegions(int xChunkPosition, int yChunkPosition)
    {
        ChunkRegionMap chunkMap = GetRegionMap(xChunkPosition, yChunkPosition);
        chunkMap.GenerateRegions();
    }

    public ChunkRegion GetRegion(int xGridGlobalPosition, int yGridGlobalPosition)
    {
        int xChunkPosition = Mathf.FloorToInt(xGridGlobalPosition / (float)chunkWidth);
        int yChunkPosition = Mathf.FloorToInt(yGridGlobalPosition / (float)chunkHeight);
        int xGridLocalPosition = xGridGlobalPosition - xChunkPosition * chunkWidth;
        int yGridLocalPosition = yGridGlobalPosition - yChunkPosition * chunkHeight;

        ChunkRegionMap chunkMap = GetRegionMap(xChunkPosition, yChunkPosition);
        return chunkMap.assignedRegions[xGridLocalPosition, yGridLocalPosition];
    }

    public void AddPointToBorderRegion(int xGridGlobalPosition, int yGridGlobalPosition)
    {
        int xChunkPosition = Mathf.FloorToInt(xGridGlobalPosition / (float)chunkWidth);
        int yChunkPosition = Mathf.FloorToInt(yGridGlobalPosition / (float)chunkHeight);
        int xGridLocalPosition = xGridGlobalPosition - xChunkPosition * chunkWidth;
        int yGridLocalPosition = yGridGlobalPosition - yChunkPosition * chunkHeight;

        ChunkRegionMap chunkMap = GetRegionMap(xChunkPosition, yChunkPosition);
        chunkMap.borderRegion.AddPoint(new Point(xGridGlobalPosition, yGridGlobalPosition));
        chunkMap.assignedRegions[xGridLocalPosition, yGridLocalPosition] = chunkMap.borderRegion;
    }

    public void ChangePointFromBorderRegion(int xGridGlobalPosition, int yGridGlobalPosition, ChunkRegion region)
    {
        int xChunkPosition = Mathf.FloorToInt(xGridGlobalPosition / (float)chunkWidth);
        int yChunkPosition = Mathf.FloorToInt(yGridGlobalPosition / (float)chunkHeight);
        int xGridLocalPosition = xGridGlobalPosition - xChunkPosition * chunkWidth;
        int yGridLocalPosition = yGridGlobalPosition - yChunkPosition * chunkHeight;

        ChunkRegionMap chunkMap = GetRegionMap(xChunkPosition, yChunkPosition);
        chunkMap.borderRegion.RemovePoint(new Point(xGridGlobalPosition, yGridGlobalPosition));
        region.AddPoint(new Point(xGridGlobalPosition, yGridGlobalPosition));
        chunkMap.assignedRegions[xGridLocalPosition, yGridLocalPosition] = region;
    }

    public void SetRegion(int xGridGlobalPosition, int yGridGlobalPosition, ChunkRegion region)
    {
        int xChunkPosition = Mathf.FloorToInt(xGridGlobalPosition / (float)chunkWidth);
        int yChunkPosition = Mathf.FloorToInt(yGridGlobalPosition / (float)chunkHeight);
        int xGridLocalPosition = xGridGlobalPosition - xChunkPosition * chunkWidth;
        int yGridLocalPosition = yGridGlobalPosition - yChunkPosition * chunkHeight;

        ChunkRegionMap chunkMap = GetRegionMap(xChunkPosition, yChunkPosition);
        region.AddPoint(new Point(xGridGlobalPosition, yGridGlobalPosition));
        chunkMap.assignedRegions[xGridLocalPosition, yGridLocalPosition] = region;
    }

    public void SetBiomeType(int xGridGlobalPosition, int yGridGlobalPosition, BiomeType biomeType)
    {
        int xChunkPosition = Mathf.FloorToInt(xGridGlobalPosition / (float)chunkWidth);
        int yChunkPosition = Mathf.FloorToInt(yGridGlobalPosition / (float)chunkHeight);
        int xGridLocalPosition = xGridGlobalPosition - xChunkPosition * chunkWidth;
        int yGridLocalPosition = yGridGlobalPosition - yChunkPosition * chunkHeight;

        ChunkDataMap chunkMap = GetChunkDataMap(xChunkPosition, yChunkPosition);
        chunkMap.biomeTypeMap[xGridLocalPosition, yGridLocalPosition] = biomeType;
    }

    public void SetFogState(int xGridGlobalPosition, int yGridGlobalPosition, bool fogState)
    {
        int xChunkPosition = Mathf.FloorToInt(xGridGlobalPosition / (float)chunkWidth);
        int yChunkPosition = Mathf.FloorToInt(yGridGlobalPosition / (float)chunkHeight);
        int xGridLocalPosition = xGridGlobalPosition - xChunkPosition * chunkWidth;
        int yGridLocalPosition = yGridGlobalPosition - yChunkPosition * chunkHeight;

        ChunkDataMap chunkMap = GetChunkDataMap(xChunkPosition, yChunkPosition);
        chunkMap.fogOfWarMap[xGridLocalPosition, yGridLocalPosition] = fogState;
    }

    public void SetWorldResource(int xGridGlobalPosition, int yGridGlobalPosition, WorldResourceRecord record)
    {
        int xChunkPosition = Mathf.FloorToInt(xGridGlobalPosition / (float)chunkWidth);
        int yChunkPosition = Mathf.FloorToInt(yGridGlobalPosition / (float)chunkHeight);
        int xGridLocalPosition = xGridGlobalPosition - xChunkPosition * chunkWidth;
        int yGridLocalPosition = yGridGlobalPosition - yChunkPosition * chunkHeight;

        ChunkDataMap chunkMap = GetChunkDataMap(xChunkPosition, yChunkPosition);
        chunkMap.resourceMap[xGridLocalPosition, yGridLocalPosition] = record;
    }

    public ChunkRegionMap GetRegionMap(int xChunkPosition, int yChunkPosition)
    {
        ChunkRegionMap chunkMap;
        if (!chunkRegionMaps.TryGetListItem(xChunkPosition, yChunkPosition, out chunkMap))
        {
            chunkMap = new ChunkRegionMap(this, xChunkPosition, yChunkPosition);
            chunkRegionMaps.AddItemToList(chunkMap, xChunkPosition, yChunkPosition);
        }
        return chunkMap;
    }

    public ChunkDataMap GetChunkDataMap(int xChunkPosition, int yChunkPosition)
    {
        ChunkDataMap chunkDataMap;
        if (!chunkDataMaps.TryGetListItem(xChunkPosition, yChunkPosition, out chunkDataMap))
        {
            ChunkRegionMap regionMap = GetRegionMap(xChunkPosition, yChunkPosition);
            regionMap.GenerateRegions();
            regionMap.GenerateAdjacentChunkMapRegions();

            chunkDataMap = new ChunkDataMap(regionMap);
            chunkDataMaps.AddItemToList(chunkDataMap, xChunkPosition, yChunkPosition);
        }
        return chunkDataMap;
    }

    public ChunkMap GetChunkMap(int xChunkPosition, int yChunkPosition)
    {
        ChunkMap chunkMap;
        if (!chunkMaps.TryGetListItem(xChunkPosition, yChunkPosition, out chunkMap))
        {
            ChunkDataMap chunkDataMap = GetChunkDataMap(xChunkPosition, yChunkPosition);
            chunkDataMap.GenerateRegionContent();

            chunkMap = new ChunkMap(chunkDataMap);
            chunkDataMaps.AddItemToList(chunkDataMap, xChunkPosition, yChunkPosition);
            generatedChunkMaps.Add(chunkMap);
        }

        return chunkMap;
    }

    public void GenerateChunkMap(int xChunkPosition, int yChunkPosition)
    {
        ChunkMap chunkMap;
        if (chunkMaps.TryGetListItem(xChunkPosition, yChunkPosition, out chunkMap))
            return;

        ChunkDataMap chunkDataMap = GetChunkDataMap(xChunkPosition, yChunkPosition);
        chunkDataMap.GenerateRegionContent();

        chunkMap = new ChunkMap(chunkDataMap);
        chunkDataMaps.AddItemToList(chunkDataMap, xChunkPosition, yChunkPosition);
        generatedChunkMaps.Add(chunkMap);
    }

    public void Dispose()
    {
        foreach (ChunkMap chunkMap in generatedChunkMaps)
            chunkMap.DisposeBuffers();
    }
}