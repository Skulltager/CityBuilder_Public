
using SheetCodes;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    public void GenerateChunkMapRegions(Point point)
    {
        GenerateChunkMapRegions(point.xIndex, point.yIndex);
    }

    public void GenerateChunkMapRegions(int xChunkPosition, int yChunkPosition)
    {
        ChunkRegionMap chunkMap = GetRegionMap(xChunkPosition, yChunkPosition);
        chunkMap.GenerateRegions();
        chunkMap.GenerateAdjacentRegions();
    }

    public ChunkRegion GetGeneratedRegion(Point point)
    {
        return GetGeneratedRegion(point.xIndex, point.yIndex);
    }

    public ChunkRegion GetGeneratedRegion(int xGridGlobalPosition, int yGridGlobalPosition)
    {
        int xChunkPosition = Mathf.FloorToInt(xGridGlobalPosition / (float)chunkWidth);
        int yChunkPosition = Mathf.FloorToInt(yGridGlobalPosition / (float)chunkHeight);
        int xGridLocalPosition = xGridGlobalPosition - xChunkPosition * chunkWidth;
        int yGridLocalPosition = yGridGlobalPosition - yChunkPosition * chunkHeight;

        ChunkRegionMap chunkMap = GetRegionMap(xChunkPosition, yChunkPosition);
        chunkMap.GenerateRegions();

        return chunkMap.assignedRegions[xGridLocalPosition, yGridLocalPosition];
    }

    public ChunkRegion GetRegion(Point point)
    {
        return GetRegion(point.xIndex, point.yIndex);
    }

    public ChunkRegion GetRegion(int xGridGlobalPosition, int yGridGlobalPosition)
    {
        int xChunkPosition = Mathf.FloorToInt(xGridGlobalPosition / (float)chunkWidth);
        int yChunkPosition = Mathf.FloorToInt(yGridGlobalPosition / (float)chunkHeight);
        int xGridLocalPosition = xGridGlobalPosition - xChunkPosition * chunkWidth;
        int yGridLocalPosition = yGridGlobalPosition - yChunkPosition * chunkHeight;

        ChunkRegionMap regionMap = GetRegionMap(xChunkPosition, yChunkPosition);
        return regionMap.assignedRegions[xGridLocalPosition, yGridLocalPosition];
    }

    public void AddPointToBorderRegion(int xGridGlobalPosition, int yGridGlobalPosition)
    {
        int xChunkPosition = Mathf.FloorToInt(xGridGlobalPosition / (float)chunkWidth);
        int yChunkPosition = Mathf.FloorToInt(yGridGlobalPosition / (float)chunkHeight);
        int xGridLocalPosition = xGridGlobalPosition - xChunkPosition * chunkWidth;
        int yGridLocalPosition = yGridGlobalPosition - yChunkPosition * chunkHeight;

        ChunkRegionMap regionMap = GetRegionMap(xChunkPosition, yChunkPosition);
        regionMap.borderRegion.AddPoint(regionMap, new Point(xGridGlobalPosition, yGridGlobalPosition));
        regionMap.assignedRegions[xGridLocalPosition, yGridLocalPosition] = regionMap.borderRegion;
    }

    public void ChangePointFromBorderRegion(int xGridGlobalPosition, int yGridGlobalPosition, ChunkRegion region)
    {
        int xChunkPosition = Mathf.FloorToInt(xGridGlobalPosition / (float)chunkWidth);
        int yChunkPosition = Mathf.FloorToInt(yGridGlobalPosition / (float)chunkHeight);
        int xGridLocalPosition = xGridGlobalPosition - xChunkPosition * chunkWidth;
        int yGridLocalPosition = yGridGlobalPosition - yChunkPosition * chunkHeight;

        ChunkRegionMap regionMap = GetRegionMap(xChunkPosition, yChunkPosition);
        regionMap.borderRegion.RemovePoint(new Point(xGridGlobalPosition, yGridGlobalPosition));
        region.AddPoint(regionMap, new Point(xGridGlobalPosition, yGridGlobalPosition));
        regionMap.assignedRegions[xGridLocalPosition, yGridLocalPosition] = region;
    }

    public void ChangePointFromBorderRegionAndRemoveResource(int xGridGlobalPosition, int yGridGlobalPosition, ChunkRegion region)
    {
        int xChunkPosition = Mathf.FloorToInt(xGridGlobalPosition / (float)chunkWidth);
        int yChunkPosition = Mathf.FloorToInt(yGridGlobalPosition / (float)chunkHeight);
        int xGridLocalPosition = xGridGlobalPosition - xChunkPosition * chunkWidth;
        int yGridLocalPosition = yGridGlobalPosition - yChunkPosition * chunkHeight;

        ChunkMap chunkMap = GetChunkMap(xChunkPosition, yChunkPosition);
        chunkMap.chunkDataMap.regionMap.borderRegion.RemovePoint(new Point(xGridGlobalPosition, yGridGlobalPosition));
        region.AddPoint(chunkMap.chunkDataMap.regionMap, new Point(xGridGlobalPosition, yGridGlobalPosition));
        chunkMap.chunkDataMap.regionMap.assignedRegions[xGridLocalPosition, yGridLocalPosition] = region;
        chunkMap.chunkMapPoints[xGridLocalPosition, yGridLocalPosition].worldResource.value = null;
    }

    public void SetRegion(int xGridGlobalPosition, int yGridGlobalPosition, ChunkRegion region)
    {
        int xChunkPosition = Mathf.FloorToInt(xGridGlobalPosition / (float)chunkWidth);
        int yChunkPosition = Mathf.FloorToInt(yGridGlobalPosition / (float)chunkHeight);
        int xGridLocalPosition = xGridGlobalPosition - xChunkPosition * chunkWidth;
        int yGridLocalPosition = yGridGlobalPosition - yChunkPosition * chunkHeight;

        ChunkRegionMap chunkMap = GetRegionMap(xChunkPosition, yChunkPosition);
        region.AddPoint(chunkMap, new Point(xGridGlobalPosition, yGridGlobalPosition));
        chunkMap.assignedRegions[xGridLocalPosition, yGridLocalPosition] = region;
    }

    public void SetBiomeType(int xGridGlobalPosition, int yGridGlobalPosition, BiomeType biomeType)
    {
        int xChunkPosition = Mathf.FloorToInt(xGridGlobalPosition / (float)chunkWidth);
        int yChunkPosition = Mathf.FloorToInt(yGridGlobalPosition / (float)chunkHeight);
        int xGridLocalPosition = xGridGlobalPosition - xChunkPosition * chunkWidth;
        int yGridLocalPosition = yGridGlobalPosition - yChunkPosition * chunkHeight;

        ChunkDataMap chunkMap = GetChunkDataMap(xChunkPosition, yChunkPosition);
        chunkMap.biomeTypeMap[xGridLocalPosition + 1, yGridLocalPosition + 1] = biomeType;

        if (xGridLocalPosition == 0)
        {
            chunkMap = GetChunkDataMap(xChunkPosition - 1, yChunkPosition);
            chunkMap.biomeTypeMap[65, yGridLocalPosition + 1] = biomeType;
        }

        if (yGridLocalPosition == 0)
        {
            chunkMap = GetChunkDataMap(xChunkPosition, yChunkPosition - 1);
            chunkMap.biomeTypeMap[xGridLocalPosition + 1, 65] = biomeType;
        }

        if (xGridLocalPosition == chunkWidth - 1)
        {
            chunkMap = GetChunkDataMap(xChunkPosition + 1, yChunkPosition);
            chunkMap.biomeTypeMap[0, yGridLocalPosition + 1] = biomeType;
        }

        if (yGridLocalPosition == chunkHeight - 1)
        {
            chunkMap = GetChunkDataMap(xChunkPosition, yChunkPosition + 1);
            chunkMap.biomeTypeMap[xGridLocalPosition + 1, 0] = biomeType;
        }

        if (xGridLocalPosition == 0 && yGridLocalPosition == 0)
        {
            chunkMap = GetChunkDataMap(xChunkPosition - 1, yChunkPosition - 1);
            chunkMap.biomeTypeMap[chunkWidth + 1, chunkHeight + 1] = biomeType;
        }

        if (xGridLocalPosition == chunkWidth - 1 && yGridLocalPosition == 0)
        {
            chunkMap = GetChunkDataMap(xChunkPosition + 1, yChunkPosition - 1);
            chunkMap.biomeTypeMap[0, chunkHeight + 1] = biomeType;
        }

        if (xGridLocalPosition == 0 && yGridLocalPosition == chunkHeight - 1)
        {
            chunkMap = GetChunkDataMap(xChunkPosition - 1, yChunkPosition + 1);
            chunkMap.biomeTypeMap[chunkWidth + 1, 0] = biomeType;
        }

        if (xGridLocalPosition == chunkWidth - 1 && yGridLocalPosition == chunkHeight - 1)
        {
            chunkMap = GetChunkDataMap(xChunkPosition + 1, yChunkPosition + 1);
            chunkMap.biomeTypeMap[0, 0] = biomeType;
        }
    }

    public void SetChunkDataMapFogState(int xGridGlobalPosition, int yGridGlobalPosition, VisionState visionState)
    {
        int xChunkPosition = Mathf.FloorToInt(xGridGlobalPosition / (float)chunkWidth);
        int yChunkPosition = Mathf.FloorToInt(yGridGlobalPosition / (float)chunkHeight);
        int xGridLocalPosition = xGridGlobalPosition - xChunkPosition * chunkWidth;
        int yGridLocalPosition = yGridGlobalPosition - yChunkPosition * chunkHeight;

        ChunkDataMap chunkMap = GetChunkDataMap(xChunkPosition, yChunkPosition);
        chunkMap.fogOfWarMap[xGridLocalPosition + 1, yGridLocalPosition + 1] = visionState;

        if(xGridLocalPosition == 0)
        {
            chunkMap = GetChunkDataMap(xChunkPosition - 1, yChunkPosition);
            chunkMap.fogOfWarMap[65, yGridLocalPosition + 1] = visionState;
        }

        if (yGridLocalPosition == 0)
        {
            chunkMap = GetChunkDataMap(xChunkPosition, yChunkPosition - 1);
            chunkMap.fogOfWarMap[xGridLocalPosition + 1, chunkHeight + 1] = visionState;
        }

        if (xGridLocalPosition == 0 && yGridLocalPosition == 0)
        {
            chunkMap = GetChunkDataMap(xChunkPosition - 1, yChunkPosition - 1);
            chunkMap.fogOfWarMap[chunkHeight + 1, chunkHeight + 1] = visionState;
        }
    }

    public void SetChunkMapFogState_Visible(List<Point> points)
    {
        List<ChunkMap> adjustedChunkMaps = new List<ChunkMap>();

        foreach (Point point in points)
        {
            int xChunkPosition = Mathf.FloorToInt(point.xIndex / (float)chunkWidth);
            int yChunkPosition = Mathf.FloorToInt(point.yIndex / (float)chunkHeight);
            int xGridLocalPosition = point.xIndex - xChunkPosition * chunkWidth;
            int yGridLocalPosition = point.yIndex - yChunkPosition * chunkHeight;

            ChunkMap chunkMap = GetChunkMap(xChunkPosition, yChunkPosition);
            chunkMap.chunkDataMap.fogOfWarMap[xGridLocalPosition + 1, yGridLocalPosition + 1] = VisionState.Full;
            chunkMap.chunkMapPoints[xGridLocalPosition, yGridLocalPosition].visionState.value = VisionState.Full;

            if (!chunkMap.usedCheck)
            {
                chunkMap.usedCheck = true;
                adjustedChunkMaps.Add(chunkMap);
            }

            if (xGridLocalPosition == 0)
            {
                chunkMap = GetChunkMap(xChunkPosition - 1, yChunkPosition);
                chunkMap.chunkDataMap.fogOfWarMap[chunkWidth + 1, yGridLocalPosition + 1] = VisionState.Full;

                if (!chunkMap.usedCheck)
                {
                    chunkMap.usedCheck = true;
                    adjustedChunkMaps.Add(chunkMap);
                }
            }

            if (yGridLocalPosition == 0)
            {
                chunkMap = GetChunkMap(xChunkPosition, yChunkPosition - 1);
                chunkMap.chunkDataMap.fogOfWarMap[xGridLocalPosition + 1, chunkHeight + 1] = VisionState.Full;

                if (!chunkMap.usedCheck)
                {
                    chunkMap.usedCheck = true;
                    adjustedChunkMaps.Add(chunkMap);
                }
            }

            if (xGridLocalPosition == chunkWidth - 1)
            {
                chunkMap = GetChunkMap(xChunkPosition + 1, yChunkPosition);
                chunkMap.chunkDataMap.fogOfWarMap[0, yGridLocalPosition + 1] = VisionState.Full;

                if (!chunkMap.usedCheck)
                {
                    chunkMap.usedCheck = true;
                    adjustedChunkMaps.Add(chunkMap);
                }
            }

            if (yGridLocalPosition == chunkHeight - 1)
            {
                chunkMap = GetChunkMap(xChunkPosition, yChunkPosition + 1);
                chunkMap.chunkDataMap.fogOfWarMap[xGridLocalPosition + 1, 0] = VisionState.Full;

                if (!chunkMap.usedCheck)
                {
                    chunkMap.usedCheck = true;
                    adjustedChunkMaps.Add(chunkMap);
                }
            }

            if (xGridLocalPosition == 0 && yGridLocalPosition == 0)
            {
                chunkMap = GetChunkMap(xChunkPosition - 1, yChunkPosition - 1);
                chunkMap.chunkDataMap.fogOfWarMap[chunkWidth + 1, chunkHeight + 1] = VisionState.Full;

                if (!chunkMap.usedCheck)
                {
                    chunkMap.usedCheck = true;
                    adjustedChunkMaps.Add(chunkMap);
                }
            }

            if (xGridLocalPosition == chunkWidth - 1 && yGridLocalPosition == 0)
            {
                chunkMap = GetChunkMap(xChunkPosition + 1, yChunkPosition - 1);
                chunkMap.chunkDataMap.fogOfWarMap[0, chunkHeight + 1] = VisionState.Full;

                if (!chunkMap.usedCheck)
                {
                    chunkMap.usedCheck = true;
                    adjustedChunkMaps.Add(chunkMap);
                }
            }

            if (xGridLocalPosition == chunkWidth - 1 && yGridLocalPosition == chunkHeight - 1)
            {
                chunkMap = GetChunkMap(xChunkPosition + 1, yChunkPosition + 1);
                chunkMap.chunkDataMap.fogOfWarMap[0, 0] = VisionState.Full;

                if (!chunkMap.usedCheck)
                {
                    chunkMap.usedCheck = true;
                    adjustedChunkMaps.Add(chunkMap);
                }
            }

            if (xGridLocalPosition == 0 && yGridLocalPosition == chunkHeight - 1)
            {
                chunkMap = GetChunkMap(xChunkPosition - 1, yChunkPosition + 1);
                chunkMap.chunkDataMap.fogOfWarMap[chunkWidth + 1, 0] = VisionState.Full;

                if (!chunkMap.usedCheck)
                {
                    chunkMap.usedCheck = true;
                    adjustedChunkMaps.Add(chunkMap);
                }
            }
        }

        foreach(ChunkMap chunkMap in adjustedChunkMaps)
        {
            chunkMap.usedCheck = false;
            chunkMap.UpdateFogOfWarBuffer();
        }
    }

    public void SetChunkMapFogState_HalfVisible(List<Point> points)
    {
        List<ChunkMap> adjustedChunkMaps = new List<ChunkMap>();

        foreach (Point point in points)
        {
            int xChunkPosition = Mathf.FloorToInt(point.xIndex / (float)chunkWidth);
            int yChunkPosition = Mathf.FloorToInt(point.yIndex / (float)chunkHeight);
            int xGridLocalPosition = point.xIndex - xChunkPosition * chunkWidth;
            int yGridLocalPosition = point.yIndex - yChunkPosition * chunkHeight;

            ChunkMap chunkMap = GetChunkMap(xChunkPosition, yChunkPosition);
            if (chunkMap.chunkDataMap.fogOfWarMap[xGridLocalPosition + 1, yGridLocalPosition + 1] == VisionState.None)
            {
                chunkMap.chunkDataMap.fogOfWarMap[xGridLocalPosition + 1, yGridLocalPosition + 1] = VisionState.Half;
                chunkMap.chunkMapPoints[xGridLocalPosition, yGridLocalPosition].visionState.value = VisionState.Half;
            }

            if (!chunkMap.usedCheck)
            {
                chunkMap.usedCheck = true;
                adjustedChunkMaps.Add(chunkMap);
            }

            if (xGridLocalPosition == 0)
            {
                chunkMap = GetChunkMap(xChunkPosition - 1, yChunkPosition);
                if (chunkMap.chunkDataMap.fogOfWarMap[chunkWidth + 1, yGridLocalPosition + 1] == VisionState.None)
                    chunkMap.chunkDataMap.fogOfWarMap[chunkWidth + 1, yGridLocalPosition + 1] = VisionState.Half;

                if (!chunkMap.usedCheck)
                {
                    chunkMap.usedCheck = true;
                    adjustedChunkMaps.Add(chunkMap);
                }
            }

            if (yGridLocalPosition == 0)
            {
                chunkMap = GetChunkMap(xChunkPosition, yChunkPosition - 1);
                chunkMap.chunkDataMap.fogOfWarMap[xGridLocalPosition + 1, chunkHeight + 1] = VisionState.Full;

                if (!chunkMap.usedCheck)
                {
                    chunkMap.usedCheck = true;
                    adjustedChunkMaps.Add(chunkMap);
                }
            }

            if (xGridLocalPosition == chunkWidth - 1)
            {
                chunkMap = GetChunkMap(xChunkPosition + 1, yChunkPosition);
                chunkMap.chunkDataMap.fogOfWarMap[0, yGridLocalPosition + 1] = VisionState.Full;

                if (!chunkMap.usedCheck)
                {
                    chunkMap.usedCheck = true;
                    adjustedChunkMaps.Add(chunkMap);
                }
            }

            if (yGridLocalPosition == chunkHeight - 1)
            {
                chunkMap = GetChunkMap(xChunkPosition, yChunkPosition + 1);
                chunkMap.chunkDataMap.fogOfWarMap[xGridLocalPosition + 1, 0] = VisionState.Full;

                if (!chunkMap.usedCheck)
                {
                    chunkMap.usedCheck = true;
                    adjustedChunkMaps.Add(chunkMap);
                }
            }

            if (xGridLocalPosition == 0 && yGridLocalPosition == 0)
            {
                chunkMap = GetChunkMap(xChunkPosition - 1, yChunkPosition - 1);
                chunkMap.chunkDataMap.fogOfWarMap[chunkWidth + 1, chunkHeight + 1] = VisionState.Full;

                if (!chunkMap.usedCheck)
                {
                    chunkMap.usedCheck = true;
                    adjustedChunkMaps.Add(chunkMap);
                }
            }

            if (xGridLocalPosition == chunkWidth - 1 && yGridLocalPosition == 0)
            {
                chunkMap = GetChunkMap(xChunkPosition + 1, yChunkPosition - 1);
                chunkMap.chunkDataMap.fogOfWarMap[0, chunkHeight + 1] = VisionState.Full;

                if (!chunkMap.usedCheck)
                {
                    chunkMap.usedCheck = true;
                    adjustedChunkMaps.Add(chunkMap);
                }
            }

            if (xGridLocalPosition == chunkWidth - 1 && yGridLocalPosition == chunkHeight - 1)
            {
                chunkMap = GetChunkMap(xChunkPosition + 1, yChunkPosition + 1);
                chunkMap.chunkDataMap.fogOfWarMap[0, 0] = VisionState.Full;

                if (!chunkMap.usedCheck)
                {
                    chunkMap.usedCheck = true;
                    adjustedChunkMaps.Add(chunkMap);
                }
            }

            if (xGridLocalPosition == 0 && yGridLocalPosition == chunkHeight - 1)
            {
                chunkMap = GetChunkMap(xChunkPosition - 1, yChunkPosition + 1);
                chunkMap.chunkDataMap.fogOfWarMap[chunkWidth + 1, 0] = VisionState.Full;

                if (!chunkMap.usedCheck)
                {
                    chunkMap.usedCheck = true;
                    adjustedChunkMaps.Add(chunkMap);
                }
            }
        }

        foreach (ChunkMap chunkMap in adjustedChunkMaps)
        {
            chunkMap.usedCheck = false;
            chunkMap.UpdateFogOfWarBuffer();
        }
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
            regionMap.GenerateAdjacentChunkDataMapRegions();

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
            chunkDataMap.GenerateAdjacentChunkMapRegionContent();

            chunkMap = new ChunkMap(chunkDataMap);
            chunkDataMaps.AddItemToList(chunkDataMap, xChunkPosition, yChunkPosition);
            chunkMaps.AddItemToList(chunkMap, xChunkPosition, yChunkPosition);
            generatedChunkMaps.Add(chunkMap);
        }

        return chunkMap;
    }

    public bool TryGetChunkMap(int xChunkPosition, int yChunkPosition, out ChunkMap chunkMap)
    {
        if (!chunkMaps.TryGetListItem(xChunkPosition, yChunkPosition, out chunkMap))
            return false;

        return true;
    }

    public void GenerateChunkMap(int xChunkPosition, int yChunkPosition)
    {
        ChunkMap chunkMap;
        if (chunkMaps.TryGetListItem(xChunkPosition, yChunkPosition, out chunkMap))
            return;

        ChunkDataMap chunkDataMap = GetChunkDataMap(xChunkPosition, yChunkPosition);
        chunkDataMap.GenerateRegionContent();
        chunkDataMap.GenerateAdjacentChunkMapRegionContent();

        chunkMap = new ChunkMap(chunkDataMap);
        chunkDataMaps.AddItemToList(chunkDataMap, xChunkPosition, yChunkPosition);
        generatedChunkMaps.Add(chunkMap);
    }

    public void MakeFirstRegionVisible(int xChunkPosition, int yChunkPosition)
    {
        ChunkMap chunkMap = GetChunkMap(xChunkPosition, yChunkPosition);
        chunkMap.chunkDataMap.regionMap.regions[0].SetFullyVisible(this);
    }

    public void MakeAdjacentRegionVisible(int xChunkPosition, int yChunkPosition)
    {
        ChunkMap chunkMap = GetChunkMap(xChunkPosition, yChunkPosition);
        for (int i = 0; i < chunkMap.chunkDataMap.regionMap.regions.Count; i++)
        {
            ChunkRegion region = chunkMap.chunkDataMap.regionMap.regions[i];
            if (region.visionState != VisionState.Half)
                continue;

            region.SetFullyVisible(this);
            break;
        }
    }

    public bool TryGetChunkMapPoint(Point point, out ChunkMapPoint chunkMapPoint)
    {
        return TryGetChunkMapPoint(point.xIndex, point.yIndex, out chunkMapPoint);
    }

    public bool TryGetChunkMapPoint(int xGridGlobalPosition, int yGridGlobalPosition, out ChunkMapPoint chunkMapPoint)
    {
        int xChunkPosition = Mathf.FloorToInt(xGridGlobalPosition / (float)chunkWidth);
        int yChunkPosition = Mathf.FloorToInt(yGridGlobalPosition / (float)chunkHeight);
        ChunkMap chunkMap;
        if(!TryGetChunkMap(xChunkPosition, yChunkPosition, out chunkMap))
        {
            chunkMapPoint = default;
            return false;
        }

        int xGridLocalPosition = xGridGlobalPosition - xChunkPosition * chunkWidth;
        int yGridLocalPosition = yGridGlobalPosition - yChunkPosition * chunkHeight;

        chunkMapPoint = chunkMap.chunkMapPoints[xGridLocalPosition, yGridLocalPosition];
        return true;
    }

    public ChunkMapPoint GetChunkMapPoint(Point point)
    {
        return GetChunkMapPoint(point.xIndex, point.yIndex);
    }

    public ChunkMapPoint GetChunkMapPoint(int xGridGlobalPosition, int yGridGlobalPosition)
    {
        int xChunkPosition = Mathf.FloorToInt(xGridGlobalPosition / (float)chunkWidth);
        int yChunkPosition = Mathf.FloorToInt(yGridGlobalPosition / (float)chunkHeight);

        int xGridLocalPosition = xGridGlobalPosition - xChunkPosition * chunkWidth;
        int yGridLocalPosition = yGridGlobalPosition - yChunkPosition * chunkHeight;
        ChunkMap chunkMap = GetChunkMap(xChunkPosition, yChunkPosition);

        return chunkMap.chunkMapPoints[xGridLocalPosition, yGridLocalPosition];
    }

    public void Dispose()
    {
        foreach (ChunkMap chunkMap in generatedChunkMaps)
            chunkMap.DisposeBuffers();
    }
}