using System.Collections.Generic;
using UnityEngine;

public class ChunkRegionMap
{
    public readonly Map map;
    public readonly int xChunkPosition;
    public readonly int yChunkPosition;

    private readonly int xGridPositionOffset;
    private readonly int yGridPositionOffset;

    public bool regionsGenerated { private set; get; }
    public bool adjacentRegionsGenerated { private set; get; }

    public int chunkWidth => map.chunkWidth;
    public int chunkHeight => map.chunkHeight;

    public readonly ChunkRegion[,] assignedRegions;
    public readonly List<ChunkRegion> regions;
    public readonly ChunkRegion borderRegion;

    public readonly static ChunkRegion NO_REGION_DUMMY;
    private readonly static CardinalDirection[] DIRECTION_BUFFER;
    private readonly static ChunkRegion[] SWAP_REGION_BUFFER;

    static ChunkRegionMap()
    {
        NO_REGION_DUMMY = new ChunkRegion(false);
        DIRECTION_BUFFER = new CardinalDirection[4];
        SWAP_REGION_BUFFER = new ChunkRegion[4];
    }


    public ChunkRegionMap(Map map, int xChunkPosition, int yChunkPosition)
    {
        this.map = map;
        this.xChunkPosition = xChunkPosition;
        this.yChunkPosition = yChunkPosition;
        xGridPositionOffset = xChunkPosition * map.chunkWidth;
        yGridPositionOffset = yChunkPosition * map.chunkHeight;
        assignedRegions = new ChunkRegion[map.chunkWidth, map.chunkHeight];
        regions = new List<ChunkRegion>();
        borderRegion = new ChunkRegion(true);
    }

    public void GenerateRegions()
    {
        if (regionsGenerated)
            return;

        List<Point> pointsToGenerate = new List<Point>();

        for (int x = 0; x < map.chunkWidth; x++)
        {
            for (int y = 0; y < map.chunkHeight; y++)
            {
                if (assignedRegions[x, y] != null)
                    continue;

                pointsToGenerate.Add(new Point(x + xGridPositionOffset, y + yGridPositionOffset));
            }
        }

        pointsToGenerate.Shuffle(pointsToGenerate.Count);

        UnorganizedList<Point> pointsWithoutRegion = new UnorganizedList<Point>();

        ChunkRegion region = new ChunkRegion(false);
        for (int i = 0; i < pointsToGenerate.Count; i++)
        {
            Point startPoint = pointsToGenerate[i];
            int targetSize = Random.Range(map.regionMinSize, map.regionMaxSize);

            ChunkRegion startRegion = map.GetRegion(startPoint.xIndex, startPoint.yIndex);
            if (startRegion != null)
                continue;

            UnorganizedList<Point> pointsToHandle = new UnorganizedList<Point>();
            pointsToHandle.Add(startPoint);
            List<Point> regionPoints = new List<Point>();

            while (pointsToHandle.count > 0)
            {
                int randomIndex = Random.Range(0, pointsToHandle.count);
                Point handlePoint = pointsToHandle.GetItem(randomIndex);
                pointsToHandle.RemoveAt(randomIndex);

                ChunkRegion checkRegion = map.GetRegion(handlePoint.xIndex, handlePoint.yIndex);
                if (checkRegion != null)
                    continue;

                map.SetRegion(handlePoint.xIndex, handlePoint.yIndex, region);
                regionPoints.Add(handlePoint);

                if (regionPoints.Count == targetSize)
                    break;

                Point checkPoint = handlePoint.AddDirection(CardinalDirection.Left);
                checkRegion = map.GetRegion(checkPoint.xIndex, checkPoint.yIndex);
                if (checkRegion == null)
                    pointsToHandle.Add(checkPoint);

                checkPoint = handlePoint.AddDirection(CardinalDirection.Bottom);
                checkRegion = map.GetRegion(checkPoint.xIndex, checkPoint.yIndex);
                if (checkRegion == null)
                    pointsToHandle.Add(checkPoint);

                checkPoint = handlePoint.AddDirection(CardinalDirection.Right);
                checkRegion = map.GetRegion(checkPoint.xIndex, checkPoint.yIndex);
                if (checkRegion == null)
                    pointsToHandle.Add(checkPoint);

                checkPoint = handlePoint.AddDirection(CardinalDirection.Top);
                checkRegion = map.GetRegion(checkPoint.xIndex, checkPoint.yIndex);
                if (checkRegion == null)
                    pointsToHandle.Add(checkPoint);
            }

            if (regionPoints.Count < map.regionMinSize)
            {
                region.Clear();
                foreach (Point point in regionPoints)
                {
                    map.SetRegion(point.xIndex, point.yIndex, NO_REGION_DUMMY);
                    pointsWithoutRegion.Add(point);
                }
            }
            else
            {
                regions.Add(region);

                foreach (Point point in regionPoints)
                    SetAdjacentPointsToBorders(point);

                region = new ChunkRegion(false);
            }
        }

        while (pointsWithoutRegion.count > 0)
        {
            bool anyFound = false;
            pointsWithoutRegion.Shuffle();

            for (int i = 4; i >= 1; i--)
            {
                for (int j = pointsWithoutRegion.count - 1; j >= 0; j--)
                {
                    Point point = pointsWithoutRegion.GetItem(j);

                    ChunkRegion regionCheck = map.GetRegion(point.xIndex, point.yIndex);
                    if (regionCheck != NO_REGION_DUMMY)
                    {
                        pointsWithoutRegion.RemoveAt(j);
                        continue;
                    }

                    int adjacentRegions = GetSwappableDirections(point);
                    if (adjacentRegions != i)
                        continue;
   
                    int randomIndex = Random.Range(0, adjacentRegions);

                    Point swapPoint = point.AddDirection(DIRECTION_BUFFER[randomIndex]);
                    map.ChangePointFromBorderRegion(swapPoint.xIndex, swapPoint.yIndex, SWAP_REGION_BUFFER[randomIndex]);
                    map.AddPointToBorderRegion(point.xIndex, point.yIndex);
                    SetAdjacentPointsToBorders(swapPoint);
                    pointsWithoutRegion.RemoveAt(j);
                    anyFound = true;
                }
            
                if (anyFound)
                    break;
            }
            
            if (anyFound)
                continue;

            for (int j = pointsWithoutRegion.count - 1; j >= 0; j--)
            {
                Point point = pointsWithoutRegion.GetItem(j);
                map.AddPointToBorderRegion(point.xIndex, point.yIndex);
                pointsWithoutRegion.RemoveAt(j);
            }
        }

        bool anyBorderSwapFound = true;
        while(anyBorderSwapFound)
        {
            anyBorderSwapFound = false;
            for(int i = borderRegion.points.Count - 1; i >= 0; i--)
            {
                Point point = borderRegion.points[i];
                ChunkRegion top = map.GetRegion(point.xIndex, point.yIndex + 1);
                ChunkRegion bottom = map.GetRegion(point.xIndex, point.yIndex - 1);
                ChunkRegion right = map.GetRegion(point.xIndex + 1, point.yIndex);
                ChunkRegion left = map.GetRegion(point.xIndex - 1, point.yIndex);

                if (top == null || bottom == null || left == null || right == null)
                    continue;

                int borderRegionCount = 0;
                int bufferIndex = 0;
                if (top.isBorderRegion)
                    borderRegionCount++;
                else
                    SWAP_REGION_BUFFER[bufferIndex++] = top;

                if (bottom.isBorderRegion)
                    borderRegionCount++;
                else
                    SWAP_REGION_BUFFER[bufferIndex++] = bottom;

                if (left.isBorderRegion)
                    borderRegionCount++;
                else
                    SWAP_REGION_BUFFER[bufferIndex++] = left;

                if (right.isBorderRegion)
                    borderRegionCount++;
                else
                    SWAP_REGION_BUFFER[bufferIndex++] = right;

                if (borderRegionCount > 3)
                    continue;

                if (bufferIndex == 4 && (SWAP_REGION_BUFFER[0] != SWAP_REGION_BUFFER[1] || SWAP_REGION_BUFFER[0] != SWAP_REGION_BUFFER[2] || SWAP_REGION_BUFFER[0] != SWAP_REGION_BUFFER[3]))
                    continue;

                if (bufferIndex == 3 && (SWAP_REGION_BUFFER[0] != SWAP_REGION_BUFFER[1] || SWAP_REGION_BUFFER[0] != SWAP_REGION_BUFFER[2]))
                    continue;

                if (bufferIndex == 2 && (SWAP_REGION_BUFFER[0] != SWAP_REGION_BUFFER[1]))
                    continue;

                map.ChangePointFromBorderRegion(point.xIndex, point.yIndex, SWAP_REGION_BUFFER[0]);
                anyBorderSwapFound = true;
            }
        }


        regionsGenerated = true;
    }

    public void GenerateAdjacentRegions()
    {
        if (adjacentRegionsGenerated)
            return;

        adjacentRegionsGenerated = true;
        foreach (ChunkRegion chunkRegion in regions)
            chunkRegion.GenerateAndSetAdjacentChunkRegions(map);
    }

    public void GenerateAdjacentChunkDataMapRegions()
    {
        map.GenerateChunkMapRegions(xChunkPosition - 1, yChunkPosition - 1);
        map.GenerateChunkMapRegions(xChunkPosition, yChunkPosition - 1);
        map.GenerateChunkMapRegions(xChunkPosition + 1, yChunkPosition - 1);
        
        map.GenerateChunkMapRegions(xChunkPosition - 1, yChunkPosition);
        map.GenerateChunkMapRegions(xChunkPosition + 1, yChunkPosition);
        
        map.GenerateChunkMapRegions(xChunkPosition - 1, yChunkPosition + 1);
        map.GenerateChunkMapRegions(xChunkPosition, yChunkPosition + 1);
        map.GenerateChunkMapRegions(xChunkPosition + 1, yChunkPosition + 1);
    }

    private int GetSwappableDirections(Point globalPoint)
    {
        int count = 0;
        ChunkRegion left = map.GetRegion(globalPoint.xIndex - 2, globalPoint.yIndex);
        ChunkRegion topLeft = map.GetRegion(globalPoint.xIndex - 1, globalPoint.yIndex + 1);
        ChunkRegion top = map.GetRegion(globalPoint.xIndex, globalPoint.yIndex + 2);
        ChunkRegion topRight = map.GetRegion(globalPoint.xIndex + 1, globalPoint.yIndex + 1);
        ChunkRegion right = map.GetRegion(globalPoint.xIndex + 2, globalPoint.yIndex);
        ChunkRegion bottomRight = map.GetRegion(globalPoint.xIndex + 1, globalPoint.yIndex - 1);
        ChunkRegion bottom = map.GetRegion(globalPoint.xIndex, globalPoint.yIndex - 2);
        ChunkRegion bottomLeft = map.GetRegion(globalPoint.xIndex - 1, globalPoint.yIndex - 1);

        ChunkRegion swapRegion;
        if (Swappable(topLeft, top, topRight, out swapRegion))
        {
            DIRECTION_BUFFER[count] = CardinalDirection.Top;
            SWAP_REGION_BUFFER[count++] = swapRegion;
        }

        if (Swappable(topRight, right, bottomRight, out swapRegion))
        {
            DIRECTION_BUFFER[count] = CardinalDirection.Right;
            SWAP_REGION_BUFFER[count++] = swapRegion;
        }

        if (Swappable(bottomRight, bottom, bottomLeft, out swapRegion))
        {
            DIRECTION_BUFFER[count] = CardinalDirection.Bottom;
            SWAP_REGION_BUFFER[count++] = swapRegion;
        }

        if (Swappable(bottomLeft, left, topLeft, out swapRegion))
        {
            DIRECTION_BUFFER[count] = CardinalDirection.Left;
            SWAP_REGION_BUFFER[count++] = swapRegion;
        }

        return count;
    }

    private bool Swappable(ChunkRegion first, ChunkRegion second, ChunkRegion third, out ChunkRegion swapRegion)
    {
        if (Swappable(first))
        {
            swapRegion = first;
            if (Swappable(second) && first != second)
                return false;

            if (Swappable(third) && first != third)
                return false;

            return true;
        }
        else if(Swappable(second))
        {
            swapRegion = second;
            if (Swappable(third) && second != third)
                return false;

            return true;
        }
        else if (Swappable(third))
        {
            swapRegion = third;
            return true;
        }

        swapRegion = null;
        return false;
    }

    private bool Swappable(ChunkRegion chunkRegion)
    {
        return chunkRegion != null && !chunkRegion.isBorderRegion && chunkRegion != NO_REGION_DUMMY;
    }

    private void SetAdjacentPointsToBorders(Point globalPoint)
    {
        Point checkPoint = globalPoint.AddDirection(CardinalDirection.Left);
        ChunkRegion checkRegion = map.GetRegion(checkPoint.xIndex, checkPoint.yIndex);
        if (checkRegion == null || checkRegion == NO_REGION_DUMMY)
            map.AddPointToBorderRegion(checkPoint.xIndex, checkPoint.yIndex);

        checkPoint = globalPoint.AddDirection(CardinalDirection.Bottom);
        checkRegion = map.GetRegion(checkPoint.xIndex, checkPoint.yIndex);
        if (checkRegion == null || checkRegion == NO_REGION_DUMMY)
            map.AddPointToBorderRegion(checkPoint.xIndex, checkPoint.yIndex);

        checkPoint = globalPoint.AddDirection(CardinalDirection.Right);
        checkRegion = map.GetRegion(checkPoint.xIndex, checkPoint.yIndex);
        if (checkRegion == null || checkRegion == NO_REGION_DUMMY)
            map.AddPointToBorderRegion(checkPoint.xIndex, checkPoint.yIndex);

        checkPoint = globalPoint.AddDirection(CardinalDirection.Top);
        checkRegion = map.GetRegion(checkPoint.xIndex, checkPoint.yIndex);
        if (checkRegion == null || checkRegion == NO_REGION_DUMMY)
            map.AddPointToBorderRegion(checkPoint.xIndex, checkPoint.yIndex);

    }
}