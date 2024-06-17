using SheetCodes;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Map
{
    public readonly int regionMinSize;
    public readonly int regionMaxSize;
    public readonly int chunkWidth;
    public readonly int chunkHeight;
    public readonly System.Random random;
    public readonly FourDirectionalList<ChunkRegionMap> chunkRegionMaps;
    public readonly FourDirectionalList<ChunkDataMap> chunkDataMaps;
    public readonly FourDirectionalList<ChunkMap> chunkMaps;
    public readonly EventList<ChunkMap> generatedChunkMaps;
    private int pathFindingCalculationIndex;

    public int xMaxRegion;
    public int xMinRegion;
    public int yMaxRegion;
    public int yMinRegion;

    public Map(System.Random random, int chunkWidth, int chunkHeight, int regionMinSize, int regionMaxSize)
    {
        this.random = random;
        this.chunkWidth = chunkWidth;
        this.chunkHeight = chunkHeight;
        this.regionMinSize = regionMinSize;
        this.regionMaxSize = regionMaxSize;
        chunkRegionMaps = new FourDirectionalList<ChunkRegionMap>(null);
        chunkDataMaps = new FourDirectionalList<ChunkDataMap>(null);
        chunkMaps = new FourDirectionalList<ChunkMap>(null);
        generatedChunkMaps = new EventList<ChunkMap>();
    }

    public bool TryGenerateSingleRegion(Point chunkPoint, RegionsRecord regionRecord, out ChunkRegion chunkRegion)
    {
        return TryGenerateSingleRegion(chunkPoint.xIndex, chunkPoint.yIndex, regionRecord, out chunkRegion);
    }

    public bool TryGenerateSingleRegion(int xChunkPosition, int yChunkPosition, RegionsRecord regionRecord, out ChunkRegion chunkRegion)
    {
        ChunkRegionMap chunkMap = GetRegionMap(xChunkPosition, yChunkPosition);
        return chunkMap.TryGenerateSingleRegion(regionRecord, out chunkRegion);
    }

    public void GenerateChunkMapRegions(Point chunkPoint)
    {
        GenerateChunkMapRegions(chunkPoint.xIndex, chunkPoint.yIndex);
    }

    public void GenerateChunkMapRegions(int xChunkPosition, int yChunkPosition)
    {
        ChunkRegionMap chunkMap = GetRegionMap(xChunkPosition, yChunkPosition);
        chunkMap.GenerateChunkRegions();
        chunkMap.GenerateAdjacentRegions();
        chunkMap.FixEdgeBorders();
    }

    public bool GetRegionIfOtherAndNotUsed(Point globalPoint, ChunkRegion chunkRegion, out ChunkRegion other)
    {
        return GetRegionIfOtherAndNotUsed(globalPoint.xIndex, globalPoint.yIndex, chunkRegion, out other);
    }

    public bool GetRegionIfOtherAndNotUsed(int xGridGlobalPosition, int yGridGlobalPosition, ChunkRegion chunkRegion, out ChunkRegion other)
    {
        int xChunkPosition = Mathf.FloorToInt(xGridGlobalPosition / (float)chunkWidth);
        int yChunkPosition = Mathf.FloorToInt(yGridGlobalPosition / (float)chunkHeight);
        int xGridLocalPosition = xGridGlobalPosition - xChunkPosition * chunkWidth;
        int yGridLocalPosition = yGridGlobalPosition - yChunkPosition * chunkHeight;

        ChunkRegionMap chunkMap = GetRegionMap(xChunkPosition, yChunkPosition);
        if (chunkMap.usedBuffer[xGridLocalPosition, yGridLocalPosition])
        {
            other = null;
            return false;
        }
        chunkMap.GenerateChunkRegions();
        other = chunkMap.assignedRegions[xGridLocalPosition, yGridLocalPosition];

        if(chunkRegion == other)
            return false;

        chunkMap.usedBuffer[xGridLocalPosition, yGridLocalPosition] = true;
        return true;
    }

    public ChunkRegion GetGeneratedRegion(Point globalPoint)
    {
        return GetGeneratedRegion(globalPoint.xIndex, globalPoint.yIndex);
    }

    public ChunkRegion GetGeneratedRegion(int xGridGlobalPosition, int yGridGlobalPosition)
    {
        int xChunkPosition = Mathf.FloorToInt(xGridGlobalPosition / (float)chunkWidth);
        int yChunkPosition = Mathf.FloorToInt(yGridGlobalPosition / (float)chunkHeight);
        int xGridLocalPosition = xGridGlobalPosition - xChunkPosition * chunkWidth;
        int yGridLocalPosition = yGridGlobalPosition - yChunkPosition * chunkHeight;

        ChunkRegionMap chunkMap = GetRegionMap(xChunkPosition, yChunkPosition);
        chunkMap.GenerateChunkRegions();

        return chunkMap.assignedRegions[xGridLocalPosition, yGridLocalPosition];
    }

    public ChunkRegion GetRegion(Point globalPoint)
    {
        return GetRegion(globalPoint.xIndex, globalPoint.yIndex);
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

    public void SetUsedIndex(Point globalPoint)
    {
        SetUsedIndex(globalPoint.xIndex, globalPoint.yIndex);
    }

    public void SetUsedIndex(int xGridGlobalPosition, int yGridGlobalPosition)
    {
        int xChunkPosition = Mathf.FloorToInt(xGridGlobalPosition / (float)chunkWidth);
        int yChunkPosition = Mathf.FloorToInt(yGridGlobalPosition / (float)chunkHeight);
        int xGridLocalPosition = xGridGlobalPosition - xChunkPosition * chunkWidth;
        int yGridLocalPosition = yGridGlobalPosition - yChunkPosition * chunkHeight;
        ChunkRegionMap chunkMap = GetRegionMap(xChunkPosition, yChunkPosition);
        chunkMap.usedBuffer[xGridLocalPosition, yGridLocalPosition] = true;
    }

    public bool SetUsedIndexIfUnused(Point globalPoint)
    {
        return SetUsedIndexIfUnused(globalPoint.xIndex, globalPoint.yIndex);
    }

    public bool SetUsedIndexIfUnused(int xGridGlobalPosition, int yGridGlobalPosition)
    {
        int xChunkPosition = Mathf.FloorToInt(xGridGlobalPosition / (float)chunkWidth);
        int yChunkPosition = Mathf.FloorToInt(yGridGlobalPosition / (float)chunkHeight);
        int xGridLocalPosition = xGridGlobalPosition - xChunkPosition * chunkWidth;
        int yGridLocalPosition = yGridGlobalPosition - yChunkPosition * chunkHeight;
        ChunkRegionMap chunkMap = GetRegionMap(xChunkPosition, yChunkPosition);
        if (chunkMap.usedBuffer[xGridLocalPosition, yGridLocalPosition])
            return false;

        chunkMap.usedBuffer[xGridLocalPosition, yGridLocalPosition] = true;
        return true;
    }

    public bool GetUsedState(Point globalPoint)
    {
        return GetUsedState(globalPoint.xIndex, globalPoint.yIndex);
    }

    public bool GetUsedState(int xGridGlobalPosition, int yGridGlobalPosition)
    {
        int xChunkPosition = Mathf.FloorToInt(xGridGlobalPosition / (float)chunkWidth);
        int yChunkPosition = Mathf.FloorToInt(yGridGlobalPosition / (float)chunkHeight);
        int xGridLocalPosition = xGridGlobalPosition - xChunkPosition * chunkWidth;
        int yGridLocalPosition = yGridGlobalPosition - yChunkPosition * chunkHeight;
        ChunkRegionMap chunkMap = GetRegionMap(xChunkPosition, yChunkPosition);

        return chunkMap.usedBuffer[xGridLocalPosition, yGridLocalPosition];
    }

    public void ClearUsedIndices(List<Point> globalPoints)
    {
        foreach(Point point in globalPoints)
        {
            int xChunkPosition = Mathf.FloorToInt(point.xIndex / (float)chunkWidth);
            int yChunkPosition = Mathf.FloorToInt(point.yIndex / (float)chunkHeight);
            int xGridLocalPosition = point.xIndex - xChunkPosition * chunkWidth;
            int yGridLocalPosition = point.yIndex - yChunkPosition * chunkHeight;
            ChunkRegionMap chunkMap = GetRegionMap(xChunkPosition, yChunkPosition);
            chunkMap.usedBuffer[xGridLocalPosition, yGridLocalPosition] = false;
        }
    }

    public void AddPointToBorderRegion(int xGridGlobalPosition, int yGridGlobalPosition)
    {
        int xChunkPosition = Mathf.FloorToInt(xGridGlobalPosition / (float)chunkWidth);
        int yChunkPosition = Mathf.FloorToInt(yGridGlobalPosition / (float)chunkHeight);
        int xGridLocalPosition = xGridGlobalPosition - xChunkPosition * chunkWidth;
        int yGridLocalPosition = yGridGlobalPosition - yChunkPosition * chunkHeight;

        ChunkRegionMap regionMap = GetRegionMap(xChunkPosition, yChunkPosition);
        regionMap.borderRegion.AddPoint(new Point(xGridGlobalPosition, yGridGlobalPosition));
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
        region.AddPoint(new Point(xGridGlobalPosition, yGridGlobalPosition));
        regionMap.assignedRegions[xGridLocalPosition, yGridLocalPosition] = region;
    }

    public void SetChunkRegion(Point point, ChunkRegion region)
    {
        SetChunkRegion(point.xIndex, point.yIndex, region);
    }

    public void SetChunkRegion(int xGridGlobalPosition, int yGridGlobalPosition, ChunkRegion region)
    {
        int xChunkPosition = Mathf.FloorToInt(xGridGlobalPosition / (float)chunkWidth);
        int yChunkPosition = Mathf.FloorToInt(yGridGlobalPosition / (float)chunkHeight);
        int xGridLocalPosition = xGridGlobalPosition - xChunkPosition * chunkWidth;
        int yGridLocalPosition = yGridGlobalPosition - yChunkPosition * chunkHeight;

        ChunkMap chunkMap = GetChunkMap(xChunkPosition, yChunkPosition);
        chunkMap.chunkDataMap.regionMap.assignedRegions[xGridLocalPosition, yGridLocalPosition] = region;
        chunkMap.chunkMapPoints[xGridLocalPosition, yGridLocalPosition].chunkRegion.value = region;
    }

    public void ChangePointFromBorderRegionAndRemoveResource(int xGridGlobalPosition, int yGridGlobalPosition, ChunkRegion region)
    {
        int xChunkPosition = Mathf.FloorToInt(xGridGlobalPosition / (float)chunkWidth);
        int yChunkPosition = Mathf.FloorToInt(yGridGlobalPosition / (float)chunkHeight);
        int xGridLocalPosition = xGridGlobalPosition - xChunkPosition * chunkWidth;
        int yGridLocalPosition = yGridGlobalPosition - yChunkPosition * chunkHeight;

        ChunkMap chunkMap = GetChunkMap(xChunkPosition, yChunkPosition);
        chunkMap.chunkDataMap.regionMap.borderRegion.RemovePoint(new Point(xGridGlobalPosition, yGridGlobalPosition));
        region.AddPoint(new Point(xGridGlobalPosition, yGridGlobalPosition));
        chunkMap.chunkDataMap.regionMap.assignedRegions[xGridLocalPosition, yGridLocalPosition] = region;
        ChunkMapPoint chunkMapPoint = chunkMap.chunkMapPoints[xGridLocalPosition, yGridLocalPosition];
        chunkMapPoint.content.value = null;
        chunkMapPoint.tileTraversalDirection.value = TileTraversalEdgeDirection.Free;
        chunkMapPoint.chunkRegion.value = region;
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

    public void SetChunkMapFogState_Visible(List<Point> globalPoints, ref List<ChunkMap> usedChunkMaps)
    {
        foreach (Point point in globalPoints)
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
                usedChunkMaps.Add(chunkMap);
            }

            if (xGridLocalPosition == 0)
            {
                chunkMap = GetChunkMap(xChunkPosition - 1, yChunkPosition);
                chunkMap.chunkDataMap.fogOfWarMap[chunkWidth + 1, yGridLocalPosition + 1] = VisionState.Full;

                if (!chunkMap.usedCheck)
                {
                    chunkMap.usedCheck = true;
                    usedChunkMaps.Add(chunkMap);
                }
            }

            if (yGridLocalPosition == 0)
            {
                chunkMap = GetChunkMap(xChunkPosition, yChunkPosition - 1);
                chunkMap.chunkDataMap.fogOfWarMap[xGridLocalPosition + 1, chunkHeight + 1] = VisionState.Full;

                if (!chunkMap.usedCheck)
                {
                    chunkMap.usedCheck = true;
                    usedChunkMaps.Add(chunkMap);
                }
            }

            if (xGridLocalPosition == chunkWidth - 1)
            {
                chunkMap = GetChunkMap(xChunkPosition + 1, yChunkPosition);
                chunkMap.chunkDataMap.fogOfWarMap[0, yGridLocalPosition + 1] = VisionState.Full;

                if (!chunkMap.usedCheck)
                {
                    chunkMap.usedCheck = true;
                    usedChunkMaps.Add(chunkMap);
                }
            }

            if (yGridLocalPosition == chunkHeight - 1)
            {
                chunkMap = GetChunkMap(xChunkPosition, yChunkPosition + 1);
                chunkMap.chunkDataMap.fogOfWarMap[xGridLocalPosition + 1, 0] = VisionState.Full;

                if (!chunkMap.usedCheck)
                {
                    chunkMap.usedCheck = true;
                    usedChunkMaps.Add(chunkMap);
                }
            }

            if (xGridLocalPosition == 0 && yGridLocalPosition == 0)
            {
                chunkMap = GetChunkMap(xChunkPosition - 1, yChunkPosition - 1);
                chunkMap.chunkDataMap.fogOfWarMap[chunkWidth + 1, chunkHeight + 1] = VisionState.Full;

                if (!chunkMap.usedCheck)
                {
                    chunkMap.usedCheck = true;
                    usedChunkMaps.Add(chunkMap);
                }
            }

            if (xGridLocalPosition == chunkWidth - 1 && yGridLocalPosition == 0)
            {
                chunkMap = GetChunkMap(xChunkPosition + 1, yChunkPosition - 1);
                chunkMap.chunkDataMap.fogOfWarMap[0, chunkHeight + 1] = VisionState.Full;

                if (!chunkMap.usedCheck)
                {
                    chunkMap.usedCheck = true;
                    usedChunkMaps.Add(chunkMap);
                }
            }

            if (xGridLocalPosition == chunkWidth - 1 && yGridLocalPosition == chunkHeight - 1)
            {
                chunkMap = GetChunkMap(xChunkPosition + 1, yChunkPosition + 1);
                chunkMap.chunkDataMap.fogOfWarMap[0, 0] = VisionState.Full;

                if (!chunkMap.usedCheck)
                {
                    chunkMap.usedCheck = true;
                    usedChunkMaps.Add(chunkMap);
                }
            }

            if (xGridLocalPosition == 0 && yGridLocalPosition == chunkHeight - 1)
            {
                chunkMap = GetChunkMap(xChunkPosition - 1, yChunkPosition + 1);
                chunkMap.chunkDataMap.fogOfWarMap[chunkWidth + 1, 0] = VisionState.Full;

                if (!chunkMap.usedCheck)
                {
                    chunkMap.usedCheck = true;
                    usedChunkMaps.Add(chunkMap);
                }
            }
        }
    }

    public void SetChunkMapFogState_HalfVisible(List<Point> globalPoints, ref List<ChunkMap> usedChunkMaps)
    {
        foreach (Point point in globalPoints)
        {
            int xChunkPosition = Mathf.FloorToInt(point.xIndex / (float)chunkWidth);
            int yChunkPosition = Mathf.FloorToInt(point.yIndex / (float)chunkHeight);
            int xGridLocalPosition = point.xIndex - xChunkPosition * chunkWidth;
            int yGridLocalPosition = point.yIndex - yChunkPosition * chunkHeight;

            ChunkMap chunkMap = GetChunkMap(xChunkPosition, yChunkPosition);
            if (chunkMap.chunkDataMap.fogOfWarMap[xGridLocalPosition + 1, yGridLocalPosition + 1] != VisionState.None)
                continue;


            xMinRegion = Mathf.Min(xMinRegion, point.xIndex);
            xMaxRegion = Mathf.Max(xMaxRegion, point.xIndex);
            yMinRegion = Mathf.Min(yMinRegion, point.yIndex);
            yMaxRegion = Mathf.Max(yMaxRegion, point.yIndex);

            chunkMap.chunkDataMap.fogOfWarMap[xGridLocalPosition + 1, yGridLocalPosition + 1] = VisionState.Half;
            chunkMap.chunkMapPoints[xGridLocalPosition, yGridLocalPosition].visionState.value = VisionState.Half;

            if (!chunkMap.usedCheck)
            {
                chunkMap.usedCheck = true;
                usedChunkMaps.Add(chunkMap);
            }

            if (xGridLocalPosition == 0)
            {
                chunkMap = GetChunkMap(xChunkPosition - 1, yChunkPosition);
                chunkMap.chunkDataMap.fogOfWarMap[chunkWidth + 1, yGridLocalPosition + 1] = VisionState.Half;

                if (!chunkMap.usedCheck)
                {
                    chunkMap.usedCheck = true;
                    usedChunkMaps.Add(chunkMap);
                }
            }

            if (yGridLocalPosition == 0)
            {
                chunkMap = GetChunkMap(xChunkPosition, yChunkPosition - 1);
                chunkMap.chunkDataMap.fogOfWarMap[xGridLocalPosition + 1, chunkHeight + 1] = VisionState.Half;

                if (!chunkMap.usedCheck)
                {
                    chunkMap.usedCheck = true;
                    usedChunkMaps.Add(chunkMap);
                }
            }

            if (xGridLocalPosition == chunkWidth - 1)
            {
                chunkMap = GetChunkMap(xChunkPosition + 1, yChunkPosition);
                chunkMap.chunkDataMap.fogOfWarMap[0, yGridLocalPosition + 1] = VisionState.Half;

                if (!chunkMap.usedCheck)
                {
                    chunkMap.usedCheck = true;
                    usedChunkMaps.Add(chunkMap);
                }
            }

            if (yGridLocalPosition == chunkHeight - 1)
            {
                chunkMap = GetChunkMap(xChunkPosition, yChunkPosition + 1);
                chunkMap.chunkDataMap.fogOfWarMap[xGridLocalPosition + 1, 0] = VisionState.Half;

                if (!chunkMap.usedCheck)
                {
                    chunkMap.usedCheck = true;
                    usedChunkMaps.Add(chunkMap);
                }
            }

            if (xGridLocalPosition == 0 && yGridLocalPosition == 0)
            {
                chunkMap = GetChunkMap(xChunkPosition - 1, yChunkPosition - 1);
                chunkMap.chunkDataMap.fogOfWarMap[chunkWidth + 1, chunkHeight + 1] = VisionState.Half;

                if (!chunkMap.usedCheck)
                {
                    chunkMap.usedCheck = true;
                    usedChunkMaps.Add(chunkMap);
                }
            }

            if (xGridLocalPosition == chunkWidth - 1 && yGridLocalPosition == 0)
            {
                chunkMap = GetChunkMap(xChunkPosition + 1, yChunkPosition - 1);
                chunkMap.chunkDataMap.fogOfWarMap[0, chunkHeight + 1] = VisionState.Half;

                if (!chunkMap.usedCheck)
                {
                    chunkMap.usedCheck = true;
                    usedChunkMaps.Add(chunkMap);
                }
            }

            if (xGridLocalPosition == chunkWidth - 1 && yGridLocalPosition == chunkHeight - 1)
            {
                chunkMap = GetChunkMap(xChunkPosition + 1, yChunkPosition + 1);
                chunkMap.chunkDataMap.fogOfWarMap[0, 0] = VisionState.Half;

                if (!chunkMap.usedCheck)
                {
                    chunkMap.usedCheck = true;
                    usedChunkMaps.Add(chunkMap);
                }
            }

            if (xGridLocalPosition == 0 && yGridLocalPosition == chunkHeight - 1)
            {
                chunkMap = GetChunkMap(xChunkPosition - 1, yChunkPosition + 1);
                chunkMap.chunkDataMap.fogOfWarMap[chunkWidth + 1, 0] = VisionState.Half;

                if (!chunkMap.usedCheck)
                {
                    chunkMap.usedCheck = true;
                    usedChunkMaps.Add(chunkMap);
                }
            }
        }
    }
    public void SetChunkMapRoadState(Point globalPoint, bool state)
    {
        SetChunkMapRoadState(globalPoint.xIndex, globalPoint.yIndex, state);
    }

    public void SetChunkMapRoadState(int xIndex, int yIndex, bool state)
    {
        int xChunkPosition = Mathf.FloorToInt(xIndex / (float)chunkWidth);
        int yChunkPosition = Mathf.FloorToInt(yIndex / (float)chunkHeight);
        int xGridLocalPosition = xIndex - xChunkPosition * chunkWidth;
        int yGridLocalPosition = yIndex - yChunkPosition * chunkHeight;

        ChunkMap chunkMap = GetChunkMap(xChunkPosition, yChunkPosition);
        chunkMap.chunkDataMap.roadsMap[xGridLocalPosition + 1, yGridLocalPosition + 1] = state;
        chunkMap.chunkMapPoints[xGridLocalPosition, yGridLocalPosition].hasRoad.value = state;

        List<ChunkMap> usedChunkMaps = new List<ChunkMap>();

        if (!chunkMap.usedCheck)
        {
            chunkMap.usedCheck = true;
            usedChunkMaps.Add(chunkMap);
        }

        if (xGridLocalPosition == 0)
        {
            chunkMap = GetChunkMap(xChunkPosition - 1, yChunkPosition);
            chunkMap.chunkDataMap.roadsMap[chunkWidth + 1, yGridLocalPosition + 1] = state;

            if (!chunkMap.usedCheck)
            {
                chunkMap.usedCheck = true;
                usedChunkMaps.Add(chunkMap);
            }
        }

        if (yGridLocalPosition == 0)
        {
            chunkMap = GetChunkMap(xChunkPosition, yChunkPosition - 1);
            chunkMap.chunkDataMap.roadsMap[xGridLocalPosition + 1, chunkHeight + 1] = state;

            if (!chunkMap.usedCheck)
            {
                chunkMap.usedCheck = true;
                usedChunkMaps.Add(chunkMap);
            }
        }

        if (xGridLocalPosition == chunkWidth - 1)
        {
            chunkMap = GetChunkMap(xChunkPosition + 1, yChunkPosition);
            chunkMap.chunkDataMap.roadsMap[0, yGridLocalPosition + 1] = state;

            if (!chunkMap.usedCheck)
            {
                chunkMap.usedCheck = true;
                usedChunkMaps.Add(chunkMap);
            }
        }

        if (yGridLocalPosition == chunkHeight - 1)
        {
            chunkMap = GetChunkMap(xChunkPosition, yChunkPosition + 1);
            chunkMap.chunkDataMap.roadsMap[xGridLocalPosition + 1, 0] = state;

            if (!chunkMap.usedCheck)
            {
                chunkMap.usedCheck = true;
                usedChunkMaps.Add(chunkMap);
            }
        }

        if (xGridLocalPosition == 0 && yGridLocalPosition == 0)
        {
            chunkMap = GetChunkMap(xChunkPosition - 1, yChunkPosition - 1);
            chunkMap.chunkDataMap.roadsMap[chunkWidth + 1, chunkHeight + 1] = state;

            if (!chunkMap.usedCheck)
            {
                chunkMap.usedCheck = true;
                usedChunkMaps.Add(chunkMap);
            }
        }

        if (xGridLocalPosition == chunkWidth - 1 && yGridLocalPosition == 0)
        {
            chunkMap = GetChunkMap(xChunkPosition + 1, yChunkPosition - 1);
            chunkMap.chunkDataMap.roadsMap[0, chunkHeight + 1] = state;

            if (!chunkMap.usedCheck)
            {
                chunkMap.usedCheck = true;
                usedChunkMaps.Add(chunkMap);
            }
        }

        if (xGridLocalPosition == chunkWidth - 1 && yGridLocalPosition == chunkHeight - 1)
        {
            chunkMap = GetChunkMap(xChunkPosition + 1, yChunkPosition + 1);
            chunkMap.chunkDataMap.roadsMap[0, 0] = state;

            if (!chunkMap.usedCheck)
            {
                chunkMap.usedCheck = true;
                usedChunkMaps.Add(chunkMap);
            }
        }

        if (xGridLocalPosition == 0 && yGridLocalPosition == chunkHeight - 1)
        {
            chunkMap = GetChunkMap(xChunkPosition - 1, yChunkPosition + 1);
            chunkMap.chunkDataMap.roadsMap[chunkWidth + 1, 0] = state;

            if (!chunkMap.usedCheck)
            {
                chunkMap.usedCheck = true;
                usedChunkMaps.Add(chunkMap);
            }
        }

        foreach(ChunkMap usedChunkMap in usedChunkMaps)
        {
            usedChunkMap.UpdateRoadsBuffer();
            usedChunkMap.usedCheck = false;
        }
    }

    public ChunkRegionPointContent GetChunkRegionPointContent(int xGridGlobalPosition, int yGridGlobalPosition)
    {
        int xChunkPosition = Mathf.FloorToInt(xGridGlobalPosition / (float)chunkWidth);
        int yChunkPosition = Mathf.FloorToInt(yGridGlobalPosition / (float)chunkHeight);
        int xGridLocalPosition = xGridGlobalPosition - xChunkPosition * chunkWidth;
        int yGridLocalPosition = yGridGlobalPosition - yChunkPosition * chunkHeight;

        ChunkDataMap chunkMap = GetChunkDataMap(xChunkPosition, yChunkPosition);
        return chunkMap.contentMap[xGridLocalPosition, yGridLocalPosition];
    }

    public void SetChunkRegionPointContentReservation_BlockPrevention(int xGridGlobalPosition, int yGridGlobalPosition)
    {
        int xChunkPosition = Mathf.FloorToInt(xGridGlobalPosition / (float)chunkWidth);
        int yChunkPosition = Mathf.FloorToInt(yGridGlobalPosition / (float)chunkHeight);
        int xGridLocalPosition = xGridGlobalPosition - xChunkPosition * chunkWidth;
        int yGridLocalPosition = yGridGlobalPosition - yChunkPosition * chunkHeight;

        ChunkDataMap chunkMap = GetChunkDataMap(xChunkPosition, yChunkPosition);
        ChunkRegionPointContent_Reservation reservation = chunkMap.contentMap[xGridLocalPosition, yGridLocalPosition] as ChunkRegionPointContent_Reservation;
        if (reservation == null)
        {
            reservation = new ChunkRegionPointContent_Reservation(this, new Point(xGridGlobalPosition, yGridGlobalPosition));
            chunkMap.contentMap[xGridLocalPosition, yGridLocalPosition] = reservation;
        }
        reservation.enterExitCount++;
    }

    public void SetChunkRegionPointContentReservation_Villager(int xGridGlobalPosition, int yGridGlobalPosition)
    {
        int xChunkPosition = Mathf.FloorToInt(xGridGlobalPosition / (float)chunkWidth);
        int yChunkPosition = Mathf.FloorToInt(yGridGlobalPosition / (float)chunkHeight);
        int xGridLocalPosition = xGridGlobalPosition - xChunkPosition * chunkWidth;
        int yGridLocalPosition = yGridGlobalPosition - yChunkPosition * chunkHeight;

        ChunkDataMap chunkMap = GetChunkDataMap(xChunkPosition, yChunkPosition);
        ChunkRegionPointContent_Reservation reservation = chunkMap.contentMap[xGridLocalPosition, yGridLocalPosition] as ChunkRegionPointContent_Reservation;  
        if(reservation == null)
        {
            reservation = new ChunkRegionPointContent_Reservation(this, new Point(xGridGlobalPosition, yGridGlobalPosition));
            chunkMap.contentMap[xGridLocalPosition, yGridLocalPosition] = reservation;
        }
        reservation.spawnVillager = true;
    }

    public void SetChunkRegionPointContent(int xGridGlobalPosition, int yGridGlobalPosition, ChunkRegionPointContent content)
    {
        int xChunkPosition = Mathf.FloorToInt(xGridGlobalPosition / (float)chunkWidth);
        int yChunkPosition = Mathf.FloorToInt(yGridGlobalPosition / (float)chunkHeight);
        int xGridLocalPosition = xGridGlobalPosition - xChunkPosition * chunkWidth;
        int yGridLocalPosition = yGridGlobalPosition - yChunkPosition * chunkHeight;

        ChunkDataMap chunkMap = GetChunkDataMap(xChunkPosition, yChunkPosition);
        chunkMap.contentMap[xGridLocalPosition, yGridLocalPosition] = content;
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
            regionMap.GenerateChunkRegions();
            regionMap.GenerateAdjacentChunkDataMapRegions();
            regionMap.FixEdgeBorders();

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
            chunkMap.GenerateContent();
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
        chunkMaps.AddItemToList(chunkMap, xChunkPosition, yChunkPosition);
        chunkDataMaps.AddItemToList(chunkDataMap, xChunkPosition, yChunkPosition);
        chunkMap.GenerateContent();
        generatedChunkMaps.Add(chunkMap);
    }

    public void MergeRegionsTogether(ChunkRegion mainRegion, ChunkRegion mergeRegion)
    {
        MakeChunkRegionVisible(mergeRegion);
        mainRegion.MergeChunkRegion(mergeRegion);
    }

    public void MakeFirstRegionVisible(int xChunkPosition, int yChunkPosition)
    {
        ChunkMap chunkMap = GetChunkMap(xChunkPosition, yChunkPosition);
        MakeChunkRegionVisible(chunkMap.chunkDataMap.regionMap.regions[0]);
    }

    public void MakeAdjacentRegionVisible(int xChunkPosition, int yChunkPosition)
    {
        ChunkMap chunkMap = GetChunkMap(xChunkPosition, yChunkPosition);
        for (int i = 0; i < chunkMap.chunkDataMap.regionMap.regions.Count; i++)
        {
            ChunkRegion region = chunkMap.chunkDataMap.regionMap.regions[i];
            if (region.visionState != VisionState.Half)
                continue;

            MakeChunkRegionVisible(region);
            break;
        }
    }

    public bool TryGetChunkMapPoint(Point globalPoint, out ChunkMapPoint chunkMapPoint)
    {
        return TryGetChunkMapPoint(globalPoint.xIndex, globalPoint.yIndex, out chunkMapPoint);
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

    public ChunkMapPoint GetChunkMapPoint(Point globalPoint)
    {
        return GetChunkMapPoint(globalPoint.xIndex, globalPoint.yIndex);
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

    public void MakeChunkRegionVisible(ChunkRegion chunkRegion)
    {
        if (chunkRegion.visionState == VisionState.Full)
            return;

        List<ChunkMap> usedChunkMaps = new List<ChunkMap>();
        chunkRegion.visionState = VisionState.Full;

        SetChunkMapFogState_Visible(chunkRegion.points, ref usedChunkMaps);
        SetChunkMapFogState_Visible(chunkRegion.borderPoints, ref usedChunkMaps);

        foreach (ChunkRegion adjacentRegion in chunkRegion.adjacentRegions)
        {
            if (adjacentRegion.visionState != VisionState.None)
                continue;

            SetChunkMapFogState_HalfVisible(adjacentRegion.points, ref usedChunkMaps);
            SetChunkMapFogState_HalfVisible(adjacentRegion.borderPoints, ref usedChunkMaps);
            adjacentRegion.visionState = VisionState.Half;
        }

        foreach (ChunkMap chunkMap in usedChunkMaps)
        {
            chunkMap.usedCheck = false;
            chunkMap.UpdateFogOfWarBuffer();
        }
    }

    public void RemoveUnlockedBorderTiles(ChunkRegion chunkRegion)
    {
        for (int i = chunkRegion.borderPoints.Count - 1; i >= 0; i--)
        {
            Point point = chunkRegion.borderPoints[i];
            ChunkRegion checkRegion = GetRegion(point);

            // Borders can be removed by other regions before this has a change to update.
            if (!checkRegion.isBorderRegion)
            {
                chunkRegion.borderPoints.RemoveAt(i);
                continue;
            }

            Point checkPoint = point.AddDirection(CardinalDirection.Left);
            checkRegion = GetRegion(checkPoint);
            if (checkRegion.visionState != VisionState.Full && !checkRegion.isBorderRegion)
                continue;

            checkPoint = point.AddDirection(CardinalDirection.Right);
            checkRegion = GetRegion(checkPoint);
            if (checkRegion.visionState != VisionState.Full && !checkRegion.isBorderRegion)
                continue;

            checkPoint = point.AddDirection(CardinalDirection.Top);
            checkRegion = GetRegion(checkPoint);
            if (checkRegion.visionState != VisionState.Full && !checkRegion.isBorderRegion)
                continue;

            checkPoint = point.AddDirection(CardinalDirection.Bottom);
            checkRegion = GetRegion(checkPoint);
            if (checkRegion.visionState != VisionState.Full && !checkRegion.isBorderRegion)
                continue;

            ChangePointFromBorderRegionAndRemoveResource(point.xIndex, point.yIndex, chunkRegion);
        }
    }
    NonScalingList<PathFindingPointData> pointsToCalculate = new NonScalingList<PathFindingPointData>();

    public bool TryCalculatePath(Point startPoint, Point endPoint, ref List<Point> path, ref List<Point> checkedPoints)
    {
        pointsToCalculate.Clear();

        pathFindingCalculationIndex++;
        CardinalDirection[] directions = Enum.GetValues(typeof(CardinalDirection)) as CardinalDirection[];


        int xChunkPosition = Mathf.FloorToInt(endPoint.xIndex / (float)chunkWidth);
        int yChunkPosition = Mathf.FloorToInt(endPoint.yIndex / (float)chunkHeight);

        int xGridLocalPosition = endPoint.xIndex - xChunkPosition * chunkWidth;
        int yGridLocalPosition = endPoint.yIndex - yChunkPosition * chunkHeight;

        ChunkMap chunkMap = GetChunkMap(xChunkPosition, yChunkPosition);
        PathFindingPointData pointData = chunkMap.pathFindingPreviousPointBuffer[xGridLocalPosition, yGridLocalPosition];
        pointData.calculationIndex = pathFindingCalculationIndex;
        pointData.inPathfindingNodeList = true;
        pointData.traveledDistance = 0;
        pointData.minimumTotalDistance = endPoint.DistanceToPoint(startPoint);
        pointData.previousPoint = pointData;
        pointsToCalculate.Add(pointData);

        int pointshandled = 0;
        bool found = false;
        while(pointsToCalculate.Count > 0)
        {
            PathFindingPointData pointToCalculate = pointsToCalculate[pointsToCalculate.Count - 1];
            pointsToCalculate.RemoveAt(pointsToCalculate.Count - 1);
            pointshandled++;
            if(pointToCalculate.point == startPoint)
            {
                found = true;
                break;
            }

            pointToCalculate.inPathfindingNodeList = false;

            foreach (CardinalDirection direction in directions)
            {
                // Check Left
                Point adjacentPoint = pointToCalculate.point.AddDirection(direction);

                xChunkPosition = Mathf.FloorToInt(adjacentPoint.xIndex / (float)chunkWidth);
                yChunkPosition = Mathf.FloorToInt(adjacentPoint.yIndex / (float)chunkHeight);

                xGridLocalPosition = adjacentPoint.xIndex - xChunkPosition * chunkWidth;
                yGridLocalPosition = adjacentPoint.yIndex - yChunkPosition * chunkHeight;

                chunkMap = GetChunkMap(xChunkPosition, yChunkPosition);
                ChunkMapPoint chunkMapPoint = chunkMap.chunkMapPoints[xGridLocalPosition, yGridLocalPosition];
                if (chunkMapPoint.blocked.value)
                    continue;

                if (SetUsedIndexIfUnused(pointToCalculate.point))
                    checkedPoints.Add(pointToCalculate.point);

                pointData = chunkMap.pathFindingPreviousPointBuffer[xGridLocalPosition, yGridLocalPosition];
                float travelDistanceCheck;
                if (chunkMap.chunkDataMap.roadsMap[xGridLocalPosition + 1, yGridLocalPosition + 1])
                    travelDistanceCheck = pointToCalculate.traveledDistance + 0.9f;
                else
                    travelDistanceCheck = pointToCalculate.traveledDistance + 1f;

                if (pointData.calculationIndex < pathFindingCalculationIndex || pointData.traveledDistance > travelDistanceCheck)
                {
                    pointData.traveledDistance = travelDistanceCheck;
                    pointData.previousPoint = pointToCalculate;
                    pointData.minimumTotalDistance = travelDistanceCheck + adjacentPoint.DistanceToPoint(startPoint);

                    if (pointData.calculationIndex < pathFindingCalculationIndex || !pointData.inPathfindingNodeList)
                    {
                        InsertPathfindingPoint(pointsToCalculate, pointData);
                        pointData.inPathfindingNodeList = true;
                    }
                    else if (pointData.calculationIndex == pathFindingCalculationIndex && pointData.inPathfindingNodeList)
                    {
                        UpdatePathfindingPointIndex(pointsToCalculate, pointData);
                    }

                    pointData.calculationIndex = pathFindingCalculationIndex;
                }
            }
        }

        ClearUsedIndices(checkedPoints);

        if (!found)
            return false;

        Debug.Log(pointshandled);
        xChunkPosition = Mathf.FloorToInt(startPoint.xIndex / (float)chunkWidth);
        yChunkPosition = Mathf.FloorToInt(startPoint.yIndex / (float)chunkHeight);

        xGridLocalPosition = startPoint.xIndex - xChunkPosition * chunkWidth;
        yGridLocalPosition = startPoint.yIndex - yChunkPosition * chunkHeight;

        chunkMap = GetChunkMap(xChunkPosition, yChunkPosition);
        pointData = chunkMap.pathFindingPreviousPointBuffer[xGridLocalPosition, yGridLocalPosition];

        while(pointData.point != endPoint)
        {
            path.Add(pointData.point);
            pointData = pointData.previousPoint;
        }

        path.Add(pointData.point);
        return true;
    }

    private void InsertPathfindingPoint(NonScalingList<PathFindingPointData> pathFindingPoints, PathFindingPointData pathFindingPoint)
    {
        for(int i = pathFindingPoints.Count - 1; i >= 0; i--)
        {
            if (pathFindingPoints[i].minimumTotalDistance <= pathFindingPoint.minimumTotalDistance)
                continue;

            pathFindingPoints.Insert(i + 1, pathFindingPoint);
            return;
        }

        pathFindingPoints.Insert(0, pathFindingPoint);
    }

    public void UpdatePathfindingPointIndex(NonScalingList<PathFindingPointData> pathFindingPoints, PathFindingPointData pathFindingPoint)
    {
        int oldIndex = -1;
        for (int i = pathFindingPoints.Count - 1; i >= 0; i--)
        {
            if (pathFindingPoints[i] != pathFindingPoint)
                continue;

            oldIndex = i;
            break;
        }

        int newIndex = pathFindingPoints.FindIndex(oldIndex + 1, i => i.minimumTotalDistance <= pathFindingPoint.minimumTotalDistance);
        if (newIndex == -1)
            newIndex = pathFindingPoints.Count - 1;

        for (int i = oldIndex; i < newIndex; i++)
            pathFindingPoints[i] = pathFindingPoints[i + 1];

        pathFindingPoints[newIndex] = pathFindingPoint;
    }

    public void Dispose()
    {
        foreach (ChunkMap chunkMap in generatedChunkMaps)
            chunkMap.DisposeBuffers();
    }
}