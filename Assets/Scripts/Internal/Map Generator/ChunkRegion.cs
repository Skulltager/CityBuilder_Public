using SheetCodes;
using System.Collections.Generic;
using UnityEngine;

public class ChunkRegion
{
    public readonly List<Point> points;
    public readonly List<Point> borderPoints;
    public readonly List<ChunkRegionMap> chunkRegionMaps;
    public readonly List<ChunkRegion> adjacentRegions;
    public int xMin { private set; get; }
    public int yMin { private set; get; }
    public int xMax { private set; get; }
    public int yMax { private set; get; }
    public bool isBorderRegion;
    public VisionState visionState { private set; get; }

    public ChunkRegion(bool isBorderRegion)
    {
        points = new List<Point>();
        borderPoints = new List<Point>();
        chunkRegionMaps = new List<ChunkRegionMap>();
        adjacentRegions = new List<ChunkRegion>();
        xMin = int.MaxValue;
        yMin = int.MaxValue;
        xMax = int.MinValue;
        yMax = int.MinValue;
        this.isBorderRegion = isBorderRegion;
    }

    public void Clear()
    {
        points.Clear();
        xMin = int.MaxValue;
        yMin = int.MaxValue;
        xMax = int.MinValue;
        yMax = int.MinValue;
    }

    public void AddPoint(ChunkRegionMap chunkRegionMap, Point point)
    {
        points.Add(point);
        xMin = Mathf.Min(xMin, point.xIndex);
        yMin = Mathf.Min(yMin, point.yIndex);
        xMax = Mathf.Max(xMax, point.xIndex);
        yMax = Mathf.Max(yMax, point.yIndex);

        if (!chunkRegionMaps.Contains(chunkRegionMap))
            chunkRegionMaps.Add(chunkRegionMap);
    }

    public void RemovePoint(Point point)
    {
        points.Remove(points.Find(i => i.Equals(point)));
    }

    public void GenerateAndSetAdjacentChunkRegions(Map map)
    {
        for (int i = 0; i < points.Count; i++)
        {
            Point point = points[i];

            Point topLeft = new Point(point.xIndex - 1, point.yIndex + 1);
            Point topRight = new Point(point.xIndex + 1, point.yIndex + 1);
            Point bottomLeft = new Point(point.xIndex - 1, point.yIndex - 1);
            Point bottomRight = new Point(point.xIndex + 1, point.yIndex - 1);

            ChunkRegion topLeftRegion = map.GetGeneratedRegion(topLeft);
            ChunkRegion topRightRegion = map.GetGeneratedRegion(topRight);
            ChunkRegion bottomLeftRegion = map.GetGeneratedRegion(bottomLeft);
            ChunkRegion bottomRightRegion = map.GetGeneratedRegion(bottomRight);

            Point checkPoint = new Point(point.xIndex - 2, point.yIndex);
            ChunkRegion adjacentRegion = map.GetGeneratedRegion(checkPoint);
            if (adjacentRegion != this && !adjacentRegion.isBorderRegion && !adjacentRegions.Contains(adjacentRegion))
            {
                if ((bottomLeftRegion == adjacentRegion || bottomLeftRegion == this || bottomLeftRegion.isBorderRegion) && (topLeftRegion == adjacentRegion || topLeftRegion == this || topLeftRegion.isBorderRegion))
                    adjacentRegions.Add(adjacentRegion);
            }

            checkPoint = new Point(point.xIndex, point.yIndex + 2);
            adjacentRegion = map.GetGeneratedRegion(checkPoint);
            if (adjacentRegion != this && !adjacentRegion.isBorderRegion && !adjacentRegions.Contains(adjacentRegion))
            {
                if ((topLeftRegion == adjacentRegion || topLeftRegion == this || topLeftRegion.isBorderRegion) && (topRightRegion == adjacentRegion || topRightRegion == this || topRightRegion.isBorderRegion))
                    adjacentRegions.Add(adjacentRegion);
            }

            checkPoint = new Point(point.xIndex + 2, point.yIndex);
            adjacentRegion = map.GetGeneratedRegion(checkPoint);
            if (adjacentRegion != this && !adjacentRegion.isBorderRegion && !adjacentRegions.Contains(adjacentRegion))
            {
                if ((topRightRegion == adjacentRegion || topRightRegion == this || topRightRegion.isBorderRegion) && (bottomRightRegion == adjacentRegion || bottomRightRegion == this || bottomRightRegion.isBorderRegion))
                    adjacentRegions.Add(adjacentRegion);
            }

            checkPoint = new Point(point.xIndex, point.yIndex - 2);
            adjacentRegion = map.GetGeneratedRegion(checkPoint);
            if (adjacentRegion != this && !adjacentRegion.isBorderRegion && !adjacentRegions.Contains(adjacentRegion))
            {
                if ((bottomRightRegion == adjacentRegion || bottomRightRegion == this || bottomRightRegion.isBorderRegion) && (bottomLeftRegion == adjacentRegion || bottomLeftRegion == this || bottomLeftRegion.isBorderRegion))
                    adjacentRegions.Add(adjacentRegion);
            }
        }

        for (int i = 0; i < points.Count; i++)
        {
            Point point = points[i];

            Point checkPoint = point.AddDirection(CardinalDirection.Left);
            ChunkRegion adjacentRegion = map.GetGeneratedRegion(checkPoint.xIndex, checkPoint.yIndex);
            if (adjacentRegion != this && !borderPoints.Contains(checkPoint))
                borderPoints.Add(checkPoint);

            checkPoint = point.AddDirection(CardinalDirection.Right);
            adjacentRegion = map.GetGeneratedRegion(checkPoint.xIndex, checkPoint.yIndex);
            if (adjacentRegion != this && !borderPoints.Contains(checkPoint))
                borderPoints.Add(checkPoint);

            checkPoint = point.AddDirection(CardinalDirection.Top);
            adjacentRegion = map.GetGeneratedRegion(checkPoint.xIndex, checkPoint.yIndex);
            if (adjacentRegion != this && !borderPoints.Contains(checkPoint))
                borderPoints.Add(checkPoint);

            checkPoint = point.AddDirection(CardinalDirection.Bottom);
            adjacentRegion = map.GetGeneratedRegion(checkPoint.xIndex, checkPoint.yIndex);
            if (adjacentRegion != this && !borderPoints.Contains(checkPoint))
                borderPoints.Add(checkPoint);

            if (AllAdjacentBorders(map, checkPoint) && !borderPoints.Contains(checkPoint))
                borderPoints.Add(checkPoint);

            checkPoint = point.AddDirection(DiagonalDirection.TopRight);
            if (AllAdjacentBorders(map, checkPoint) && !borderPoints.Contains(checkPoint))
                borderPoints.Add(checkPoint);

            checkPoint = point.AddDirection(DiagonalDirection.BottomLeft);
            if (AllAdjacentBorders(map, checkPoint) && !borderPoints.Contains(checkPoint))
                borderPoints.Add(checkPoint);

            checkPoint = point.AddDirection(DiagonalDirection.BottomRight);
            if (AllAdjacentBorders(map, checkPoint) && !borderPoints.Contains(checkPoint))
                borderPoints.Add(checkPoint);
        }
    }

    private bool AllAdjacentBorders(Map map, Point point)
    {
        Point checkPoint = point.AddDirection(CardinalDirection.Left);
        ChunkRegion adjacentRegion = map.GetGeneratedRegion(checkPoint);
        if (!adjacentRegion.isBorderRegion)
            return false;

        checkPoint = point.AddDirection(CardinalDirection.Right);
        adjacentRegion = map.GetGeneratedRegion(checkPoint);
        if (!adjacentRegion.isBorderRegion)
            return false;

        checkPoint = point.AddDirection(CardinalDirection.Top);
        adjacentRegion = map.GetGeneratedRegion(checkPoint);
        if (!adjacentRegion.isBorderRegion)
            return false;

        checkPoint = point.AddDirection(CardinalDirection.Bottom);
        adjacentRegion = map.GetGeneratedRegion(checkPoint);
        if (!adjacentRegion.isBorderRegion)
            return false;

        return true;
    }

    public void GenerateContent(Map map)
    {
        if (isBorderRegion)
        {
            WorldResourceRecord treeRecord = WorldResourceIdentifier.Tree.GetRecord();
            for (int i = 0; i < points.Count; i++)
            {
                Point point = points[i];
                map.SetWorldResource(point.xIndex, point.yIndex, treeRecord);
            }
        }
    }

    public void SetFullyVisible(Map map)
    {
        if (visionState == VisionState.Full)
            return;

        visionState = VisionState.Full;

        map.SetChunkMapFogState_Visible(points);
        map.SetChunkMapFogState_Visible(borderPoints);

        foreach (ChunkRegion adjacentRegion in adjacentRegions)
            adjacentRegion.SetHalfVisable(map);

        RemoveUnlockedBorderTiles(map);
    }

    public void SetHalfVisable(Map map)
    {
        if (visionState == VisionState.Half || visionState == VisionState.Full)
            return;

        visionState = VisionState.Half;
        map.SetChunkMapFogState_HalfVisible(points);
        map.SetChunkMapFogState_HalfVisible(borderPoints);
    }

    public void RemoveUnlockedBorderTiles(Map map)
    {
        for(int i = borderPoints.Count - 1; i >= 0; i--)
        {
            Point point = borderPoints[i];
            ChunkRegion checkRegion = map.GetGeneratedRegion(point);

            // Borders can be removed by other regions before this has a change to update.
            if (!checkRegion.isBorderRegion)
            {
                borderPoints.RemoveAt(i);
                continue;
            }

            Point checkPoint = point.AddDirection(CardinalDirection.Left);
            checkRegion = map.GetGeneratedRegion(checkPoint);
            if (checkRegion.visionState != VisionState.Full && !checkRegion.isBorderRegion)
                continue;

            checkPoint = point.AddDirection(CardinalDirection.Right);
            checkRegion = map.GetGeneratedRegion(checkPoint);
            if (checkRegion.visionState != VisionState.Full && !checkRegion.isBorderRegion)
                continue;

            checkPoint = point.AddDirection(CardinalDirection.Top);
            checkRegion = map.GetGeneratedRegion(checkPoint);
            if (checkRegion.visionState != VisionState.Full && !checkRegion.isBorderRegion)
                continue;

            checkPoint = point.AddDirection(CardinalDirection.Bottom);
            checkRegion = map.GetGeneratedRegion(checkPoint);
            if (checkRegion.visionState != VisionState.Full && !checkRegion.isBorderRegion)
                continue;

            map.ChangePointFromBorderRegionAndRemoveResource(point.xIndex, point.yIndex, this);
        }
    }
}
