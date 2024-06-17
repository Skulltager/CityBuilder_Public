using SheetCodes;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ChunkRegion
{
    public readonly Map map;
    public readonly RegionsRecord record;
    public readonly List<Point> points;
    public readonly List<Point> borderPoints;
    public readonly EventList<ChunkRegion> adjacentRegions;
    public readonly List<ChunkRegionContent> contents;
    public readonly ChunkRegionMap chunkRegionMap;
    public readonly List<ChunkRegionContentInfo> chunkRegionContentInfos;

    public int xMin { private set; get; }
    public int yMin { private set; get; }
    public int xMax { private set; get; }
    public int yMax { private set; get; }
    public int width { private set; get; }
    public int height { private set; get; }
    public int pathFindingWidth => width + 2;
    public int pathFindingHeight => height + 2;

    public Player owner;
    public VisionState visionState;

    public bool used;
    public bool merged;
    public bool isBorderRegion;

    private readonly NonScalingList<PathFindingPointData> pointsToCalculate;
    private PathFindingPointData[,] pathFindingPointDatas;
    private int pathFindingCalculationIndex;
    private bool pathFindingBuffersUpToDate;

    public ChunkRegion(Map map, ChunkRegionMap chunkRegionMap, RegionsRecord record, bool isBorderRegion)
        : this(isBorderRegion)
    {
        this.map = map;
        this.record = record;
        this.chunkRegionMap = chunkRegionMap;

        foreach (ResourceSpawnRecord resourceSpawn in record.ResourceSpawns)
            contents.Add(new ChunkRegionContent_WorldResource(this, resourceSpawn.PossibleWorldResources, map.random.Next(resourceSpawn.MinAmount, resourceSpawn.MaxAmount + 1)));
    }

    public ChunkRegion(bool isBorderRegion)
    {
        this.isBorderRegion = isBorderRegion;

        points = new List<Point>();
        borderPoints = new List<Point>();
        adjacentRegions = new EventList<ChunkRegion>();
        contents = new List<ChunkRegionContent>();
        pointsToCalculate = new NonScalingList<PathFindingPointData>();
        chunkRegionContentInfos = new List<ChunkRegionContentInfo>();

        pathFindingCalculationIndex = 0;

        xMin = int.MaxValue;
        yMin = int.MaxValue;
        xMax = int.MinValue;
        yMax = int.MinValue;
        width = 0;
        height = 0;

        if (isBorderRegion)
            contents.Add(new ChunkRegionContent_Borders(this));
    }

    public void Clear()
    {
        points.Clear();
        xMin = int.MaxValue;
        yMin = int.MaxValue;
        xMax = int.MinValue;
        yMax = int.MinValue;
        width = 0;
        height = 0;
    }

    public void AddChunkRegionContentInfo(WorldResourceRecord record, int amount)
    {
        ChunkRegionContentInfo contentInfo;
        if (!chunkRegionContentInfos.TryFind(i => i.record == record, out contentInfo))
        {
            contentInfo = new ChunkRegionContentInfo(record);
            chunkRegionContentInfos.Add(contentInfo);
        }

        contentInfo.count += amount;
    }

    public void MergeChunkRegion(ChunkRegion region)
    {
        foreach (Point point in region.points)
        {
            points.Add(point);
            xMin = Mathf.Min(xMin, point.xIndex);
            yMin = Mathf.Min(yMin, point.yIndex);
            xMax = Mathf.Max(xMax, point.xIndex);
            yMax = Mathf.Max(yMax, point.yIndex);

            width = xMax - xMin + 1;
            height = yMax - yMin + 1;
            map.SetChunkRegion(point, this);
        }

        foreach(ChunkRegion adjacentRegion in region.adjacentRegions)
        {
            if (adjacentRegion == this)
                continue;

            if (adjacentRegion.merged)
                continue;

            if (adjacentRegions.Contains(adjacentRegion))
                continue;

            adjacentRegions.Add(adjacentRegion);
        }

        adjacentRegions.Remove(region);
        region.merged = true;

        for (int i = 0; i < region.borderPoints.Count; i++)
        {
            Point point = region.borderPoints[i];
            ChunkRegion checkRegion = map.GetRegion(point);

            // Borders can be already removed by other merged regions
            if (!checkRegion.isBorderRegion)
                continue;

            bool anyFound = false;
            foreach (CardinalDirection direction in CardinalDirectionExtensions.ALL_DIRECTIONS)
            {
                Point checkPoint = point.AddDirection(direction);
                checkRegion = map.GetRegion(checkPoint);
                if (checkRegion == this || checkRegion.isBorderRegion)
                    continue;

                anyFound = true;
                break;
            }

            if (anyFound)
            {
                borderPoints.Add(point);
                continue;
            }

            map.ChangePointFromBorderRegionAndRemoveResource(point.xIndex, point.yIndex, this);
        }

        pathFindingBuffersUpToDate = false;
    }

    public void AddPoint(Point point)
    {
        points.Add(point);
        xMin = Mathf.Min(xMin, point.xIndex);
        yMin = Mathf.Min(yMin, point.yIndex);
        xMax = Mathf.Max(xMax, point.xIndex);
        yMax = Mathf.Max(yMax, point.yIndex);

        width = xMax - xMin + 1;
        height = yMax - yMin + 1;
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
            if (adjacentRegion != this && !adjacentRegion.isBorderRegion && !adjacentRegion.used)
            {
                if ((bottomLeftRegion == adjacentRegion || bottomLeftRegion == this || bottomLeftRegion.isBorderRegion) && (topLeftRegion == adjacentRegion || topLeftRegion == this || topLeftRegion.isBorderRegion))
                {
                    adjacentRegions.Add(adjacentRegion);
                    adjacentRegion.used = true;
                }
            }

            checkPoint = new Point(point.xIndex, point.yIndex + 2);
            adjacentRegion = map.GetGeneratedRegion(checkPoint);
            if (adjacentRegion != this && !adjacentRegion.isBorderRegion && !adjacentRegion.used)
            {
                if ((topLeftRegion == adjacentRegion || topLeftRegion == this || topLeftRegion.isBorderRegion) && (topRightRegion == adjacentRegion || topRightRegion == this || topRightRegion.isBorderRegion))
                {
                    adjacentRegions.Add(adjacentRegion);
                    adjacentRegion.used = true;
                }
            }

            checkPoint = new Point(point.xIndex + 2, point.yIndex);
            adjacentRegion = map.GetGeneratedRegion(checkPoint);
            if (adjacentRegion != this && !adjacentRegion.isBorderRegion && !adjacentRegion.used)
            {
                if ((topRightRegion == adjacentRegion || topRightRegion == this || topRightRegion.isBorderRegion) && (bottomRightRegion == adjacentRegion || bottomRightRegion == this || bottomRightRegion.isBorderRegion))
                {
                    adjacentRegions.Add(adjacentRegion);
                    adjacentRegion.used = true;
                }
            }

            checkPoint = new Point(point.xIndex, point.yIndex - 2);
            adjacentRegion = map.GetGeneratedRegion(checkPoint);
            if (adjacentRegion != this && !adjacentRegion.isBorderRegion && !adjacentRegion.used)
            {
                if ((bottomRightRegion == adjacentRegion || bottomRightRegion == this || bottomRightRegion.isBorderRegion) && (bottomLeftRegion == adjacentRegion || bottomLeftRegion == this || bottomLeftRegion.isBorderRegion))
                {
                    adjacentRegions.Add(adjacentRegion);
                    adjacentRegion.used = true;
                }
            }
        }

        foreach(ChunkRegion adjacentRegion in adjacentRegions)
            adjacentRegion.used = false;

        for (int i = 0; i < points.Count; i++)
        {
            Point point = points[i];

            ChunkRegion adjacentRegion;

            Point checkPoint = point.AddDirection(CardinalDirection.Left);
            if (map.GetRegionIfOtherAndNotUsed(checkPoint.xIndex, checkPoint.yIndex, this, out adjacentRegion))
                borderPoints.Add(checkPoint);

            checkPoint = point.AddDirection(CardinalDirection.Right);
            if (map.GetRegionIfOtherAndNotUsed(checkPoint.xIndex, checkPoint.yIndex, this, out adjacentRegion))
                borderPoints.Add(checkPoint);

            checkPoint = point.AddDirection(CardinalDirection.Top);
            if (map.GetRegionIfOtherAndNotUsed(checkPoint.xIndex, checkPoint.yIndex, this, out adjacentRegion))
                borderPoints.Add(checkPoint);

            checkPoint = point.AddDirection(CardinalDirection.Bottom);
            if (map.GetRegionIfOtherAndNotUsed(checkPoint.xIndex, checkPoint.yIndex, this, out adjacentRegion))
                borderPoints.Add(checkPoint);

            checkPoint = point.AddDirection(DiagonalDirection.TopLeft);
            if (AllAdjacentBorders(checkPoint) && map.SetUsedIndexIfUnused(checkPoint))
                borderPoints.Add(checkPoint);

            checkPoint = point.AddDirection(DiagonalDirection.TopRight);
            if (AllAdjacentBorders(checkPoint) && map.SetUsedIndexIfUnused(checkPoint))
                borderPoints.Add(checkPoint);

            checkPoint = point.AddDirection(DiagonalDirection.BottomLeft);
            if (AllAdjacentBorders(checkPoint) && map.SetUsedIndexIfUnused(checkPoint))
                borderPoints.Add(checkPoint);

            checkPoint = point.AddDirection(DiagonalDirection.BottomRight);
            if (AllAdjacentBorders(checkPoint) && map.SetUsedIndexIfUnused(checkPoint))
                borderPoints.Add(checkPoint);
        }

        map.ClearUsedIndices(borderPoints);
    }

    private bool AllAdjacentBorders(Point point)
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
        foreach (ChunkRegionContent content in contents)
            content.GenerateContent(map);
    }

    private void ResetPathfindingBuffers()
    {
        if (pathFindingBuffersUpToDate)
            return;

        Point offset = new Point(xMin - 1, yMin - 1);
        pathFindingPointDatas = new PathFindingPointData[pathFindingWidth,pathFindingHeight];

        pathFindingBuffersUpToDate = true;
        foreach (Point point in points)
        {
            ChunkMapPoint chunkMapPoint = map.GetChunkMapPoint(point);

            Point adjustedPoint = point - offset;
            pathFindingPointDatas[adjustedPoint.xIndex, adjustedPoint.yIndex] = new PathFindingPointData(chunkMapPoint, adjustedPoint);

            foreach(CardinalDirection direction in CardinalDirectionExtensions.ALL_DIRECTIONS)
            {
                Point adjacentAdjustedPoint = adjustedPoint.AddDirection(direction);
                if (pathFindingPointDatas[adjacentAdjustedPoint.xIndex, adjacentAdjustedPoint.yIndex] != null)
                    continue;

                Point adjacentPoint = point.AddDirection(direction);
                chunkMapPoint = map.GetChunkMapPoint(adjacentPoint);
                pathFindingPointDatas[adjacentAdjustedPoint.xIndex, adjacentAdjustedPoint.yIndex] = new PathFindingPointData(chunkMapPoint, adjacentAdjustedPoint);

            }
        }
    }

    public PathData RecalculatePath(Point startPoint, TilePosition tilePosition, VillagerTaskPoint villagerTaskPoint, ContentTaskLocation contentTaskLocation)
    {
        bool wasReserved = villagerTaskPoint.enabled.value;
        villagerTaskPoint.enabled.value = true;

        PathData pathData = CalculatePath(startPoint, tilePosition, new List<VillagerTaskPoint>() { villagerTaskPoint }, contentTaskLocation);

        villagerTaskPoint.enabled.value = wasReserved;
        return pathData;
    }

    public PathData CalculatePath(Point startPoint, TilePosition tilePosition, List<VillagerTaskPoint> villagerTaskPoints, ContentTaskLocation contentTaskLocation)
    {
        if (contentTaskLocation == ContentTaskLocation.Inside)
            return CalculatePath_Inside(startPoint, tilePosition, villagerTaskPoints);
        else
            return CalculatePath_Edge(startPoint, tilePosition, villagerTaskPoints);
    }

    private PathData CalculatePath_Inside(Point startPoint, TilePosition startTilePosition, List<VillagerTaskPoint> possibleEndPoints)
    {
        TilePosition flippedTilePosition = startTilePosition.Flip();
        foreach (VillagerTaskPoint taskPoint in possibleEndPoints)
        {
            if (!taskPoint.enabled.value)
                continue;

            if (taskPoint.point != startPoint || taskPoint.tilePosition != flippedTilePosition)
                continue;

            ChunkMapPoint startMapPoint = map.GetChunkMapPoint(startPoint);
            return new PathData(taskPoint, new List<PathPoint>() { new PathPoint(startMapPoint, startTilePosition) });
        }

        PathData pathData = CalculatePath(startPoint, startTilePosition, possibleEndPoints);
        pathData.pathPoints.Add(new PathPoint(map.GetChunkMapPoint(pathData.taskPoint.point), pathData.taskPoint.tilePosition.Flip(), false));
        return pathData;
    }

    private PathData CalculatePath_Edge(Point startPoint, TilePosition startTilePosition, List<VillagerTaskPoint> possibleEndPoints)
    {
        foreach (VillagerTaskPoint taskPoint in possibleEndPoints)
        {
            if (!taskPoint.enabled.value)
                continue;

            if (taskPoint.point != startPoint || taskPoint.tilePosition != startTilePosition)
                continue;

            ChunkMapPoint startMapPoint = map.GetChunkMapPoint(startPoint);
            return new PathData(taskPoint, new List<PathPoint>() { new PathPoint(startMapPoint, startTilePosition) });
        }

        return CalculatePath(startPoint, startTilePosition, possibleEndPoints);
    }

    private PathData CalculatePath(Point startPoint, TilePosition startTilePosition, List<VillagerTaskPoint> possibleEndPoints)
    {
        ResetPathfindingBuffers();
        pointsToCalculate.Clear();

        Point closestEndPoint = default;
        int closestEndPointDistance = int.MaxValue;

        foreach (VillagerTaskPoint taskPoint in possibleEndPoints)
        {
            if (!taskPoint.enabled.value)
                continue;

            int distanceToEnd = taskPoint.point.DistanceToPoint(startPoint);
            if (distanceToEnd >= closestEndPointDistance)
                continue;

            closestEndPoint = taskPoint.point;
            closestEndPointDistance = distanceToEnd;
        }

        Point offset = new Point(xMin - 1, yMin - 1);
        Point adjustedEndPoint = closestEndPoint - offset;
        Point adjustedStartPoint = startPoint - offset;

        pathFindingCalculationIndex++;
        CardinalDirection[] directions = Enum.GetValues(typeof(CardinalDirection)) as CardinalDirection[];

        foreach (VillagerTaskPoint taskPoint in possibleEndPoints)
        {
            if (!taskPoint.enabled.value)
                continue;

            Point adjustedPoint = taskPoint.point - offset;
            pathFindingPointDatas[adjustedPoint.xIndex, adjustedPoint.yIndex].possibleEndPoint = true;
        }

        PathFindingPointData startPointData = pathFindingPointDatas[adjustedStartPoint.xIndex, adjustedStartPoint.yIndex];
        while (startPointData.chunkMapPoint.tileTraversalDirection.value == TileTraversalEdgeDirection.Impossible)
        {
            CardinalDirection forceMoveDirection;
            Point difference = adjustedEndPoint - adjustedStartPoint;
            if(Mathf.Abs(difference.xIndex) > Mathf.Abs(difference.yIndex))
            {
                if (difference.xIndex > 0)
                    forceMoveDirection = CardinalDirection.Right;
                else
                    forceMoveDirection = CardinalDirection.Left;
            }
            else
            {
                if (difference.yIndex > 0)
                    forceMoveDirection = CardinalDirection.Top;
                else
                    forceMoveDirection = CardinalDirection.Bottom;
            }
            Point movedStartPoint = adjustedStartPoint.AddDirection(forceMoveDirection);
            PathFindingPointData movedPointData = pathFindingPointDatas[movedStartPoint.xIndex, movedStartPoint.yIndex];
            movedPointData.previousPoint = startPointData;
            startPointData = movedPointData;
            adjustedStartPoint = movedStartPoint;
        }

        bool startPositionAdjusted = false;
        if (!startPointData.chunkMapPoint.tileTraversalDirection.value.IsViablePosition(startTilePosition))
        {
            startTilePosition = startPointData.chunkMapPoint.enterExitTilePosition;
            startPositionAdjusted = true;
        }

        startPointData.calculationIndex = pathFindingCalculationIndex;
        startPointData.inPathfindingNodeList = true;
        startPointData.traveledDistance = 0;
        startPointData.minimumTotalDistance = adjustedStartPoint.DistanceToPoint(adjustedEndPoint);
        startPointData.previousPoint = startPointData;
        pointsToCalculate.Add(startPointData);

        int pointshandled = 0;

        PathFindingPointData endPointData = default;
        while (pointsToCalculate.Count > 0)
        {
            PathFindingPointData pointToCalculate = pointsToCalculate[pointsToCalculate.Count - 1];
            pointsToCalculate.RemoveAt(pointsToCalculate.Count - 1);
            pointshandled++;
            if (pointToCalculate.possibleEndPoint)
            {
                endPointData = pointToCalculate;
                break;
            }

            pointToCalculate.inPathfindingNodeList = false;

            foreach (CardinalDirection direction in directions)
            {
                Point adjacentPoint = pointToCalculate.point.AddDirection(direction);

                if ((startPointData != pointToCalculate || !startPositionAdjusted) && !pointToCalculate.chunkMapPoint.tileTraversalDirection.value.CanTravelInDirection(direction))
                    continue;

                if (adjacentPoint.xIndex >= pathFindingWidth || adjacentPoint.yIndex >= pathFindingHeight || adjacentPoint.xIndex < 0 || adjacentPoint.yIndex < 0)
                    continue;

                PathFindingPointData pointData = pathFindingPointDatas[adjacentPoint.xIndex, adjacentPoint.yIndex];

                if (pointData == null)
                    continue;

                float travelDistanceCheck = pointToCalculate.traveledDistance + pointData.chunkMapPoint.pathFindingFactor;
                if (pointData.calculationIndex < pathFindingCalculationIndex || pointData.traveledDistance > travelDistanceCheck)
                {
                    pointData.traveledDistance = travelDistanceCheck;
                    pointData.previousPoint = pointToCalculate;
                    pointData.minimumTotalDistance = pointData.traveledDistance + adjacentPoint.DistanceToPoint(adjustedEndPoint);

                    if (pointData.calculationIndex < pathFindingCalculationIndex || !pointData.inPathfindingNodeList)
                    {
                        InsertPathFindingPoint(pointsToCalculate, pointData);
                        pointData.inPathfindingNodeList = true;
                    }
                    else if (pointData.calculationIndex == pathFindingCalculationIndex && pointData.inPathfindingNodeList)
                    {
                        UpdatePathFindingPointIndex(pointsToCalculate, pointData);
                    }

                    pointData.calculationIndex = pathFindingCalculationIndex;
                }
            }
        }
        VillagerTaskPoint endTaskPoint = possibleEndPoints.Find(i => i.enabled.value && i.point - offset == endPointData.point);

        foreach (VillagerTaskPoint taskPoint in possibleEndPoints)
        {
            if (!taskPoint.enabled.value)
                continue;

            Point adjustedPoint = taskPoint.point - offset;
            pathFindingPointDatas[adjustedPoint.xIndex, adjustedPoint.yIndex].possibleEndPoint = false;
        }

        List<PathNode> path = new List<PathNode>();
        while (endPointData.point != adjustedStartPoint)
        {
            TileTraversalEdgeDirection edgeDirection = pathFindingPointDatas[endPointData.point.xIndex, endPointData.point.yIndex].chunkMapPoint.tileTraversalDirection.value;
            path.Add(new PathNode(endPointData.point + offset, edgeDirection));
            endPointData = endPointData.previousPoint;
        }

        TileTraversalEdgeDirection finalEdgeDirection = pathFindingPointDatas[endPointData.point.xIndex, endPointData.point.yIndex].chunkMapPoint.tileTraversalDirection.value;
        path.Add(new PathNode(endPointData.point + offset, finalEdgeDirection));
        path.Reverse();
        PathData pathData = new PathData(endTaskPoint, ConvertGridPathToWalkPath(path, startTilePosition, endTaskPoint.tilePosition));
        return pathData;
    }

    private void InsertPathFindingPoint(NonScalingList<PathFindingPointData> pathFindingPoints, PathFindingPointData pathFindingPoint)
    {
        for (int i = pathFindingPoints.Count - 1; i >= 0; i--)
        {
            if (pathFindingPoints[i].minimumTotalDistance <= pathFindingPoint.minimumTotalDistance)
                continue;

            pathFindingPoints.Insert(i + 1, pathFindingPoint);
            return;
        }

        pathFindingPoints.Insert(0, pathFindingPoint);
    }

    private void UpdatePathFindingPointIndex(NonScalingList<PathFindingPointData> pathFindingPoints, PathFindingPointData pathFindingPoint)
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

    private List<PathPoint> ConvertGridPathToWalkPath(List<PathNode> pathNodes, TilePosition startTilePosition, TilePosition finalTilePosition)
    {
        ChunkMapPoint[] chunkMapPoints = new ChunkMapPoint[pathNodes.Count];
        for(int i = 0; i < chunkMapPoints.Length; i++)
            chunkMapPoints[i] = map.GetChunkMapPoint(pathNodes[i].point);


        List<PathPoint> path = new List<PathPoint>();
        path.Add(new PathPoint(map.GetChunkMapPoint(pathNodes[0].point), startTilePosition));

        for (int i = 0; i < pathNodes.Count - 1; i++)
        {
            PathNode currentPathNode = pathNodes[i];
            ChunkMapPoint currentChunkMapPoint = chunkMapPoints[i];
            PathNode nextPathNode = pathNodes[i + 1];
            ChunkMapPoint nextChunkMapPoint = chunkMapPoints[i + 1];
            CardinalDirection pathDirection = GetDirection(currentPathNode.point, nextPathNode.point);
            if (MatchesDirection(startTilePosition, pathDirection, false))
            {
                switch (pathDirection)
                {
                    case CardinalDirection.Top:
                        switch (startTilePosition)
                        {
                            case TilePosition.TopLeft:
                                startTilePosition = TilePosition.BottomLeft;
                                break;
                            case TilePosition.TopCenter:
                                startTilePosition = TilePosition.BottomCenter;
                                break;
                            case TilePosition.TopRight:
                                startTilePosition = TilePosition.BottomRight;
                                break;
                        }
                        break;
                    case CardinalDirection.Right:
                        switch (startTilePosition)
                        {
                            case TilePosition.TopRight:
                                startTilePosition = TilePosition.TopLeft;
                                break;
                            case TilePosition.CenterRight:
                                startTilePosition = TilePosition.CenterLeft;
                                break;
                            case TilePosition.BottomRight:
                                startTilePosition = TilePosition.BottomLeft;
                                break;
                        }
                        break;
                    case CardinalDirection.Bottom:
                        switch (startTilePosition)
                        {
                            case TilePosition.BottomLeft:
                                startTilePosition = TilePosition.TopLeft;
                                break;
                            case TilePosition.BottomCenter:
                                startTilePosition = TilePosition.TopCenter;
                                break;
                            case TilePosition.BottomRight:
                                startTilePosition = TilePosition.TopRight;
                                break;
                        }
                        break;
                    case CardinalDirection.Left:
                        switch (startTilePosition)
                        {
                            case TilePosition.TopLeft:
                                startTilePosition = TilePosition.TopRight;
                                break;
                            case TilePosition.CenterLeft:
                                startTilePosition = TilePosition.CenterRight;
                                break;
                            case TilePosition.BottomLeft:
                                startTilePosition = TilePosition.BottomRight;
                                break;
                        }
                        break;
                }
            }
            else
            {
                switch (pathDirection)
                {
                    case CardinalDirection.Right:
                        {
                            VerticalDirection verticalDirection = CalculateNextDesiredDirection_Vertical(pathNodes, startTilePosition, finalTilePosition, i);
                            switch (verticalDirection)
                            {
                                case VerticalDirection.Up:
                                    AddPositionsToPath_TopRight(currentChunkMapPoint, startTilePosition, currentPathNode.travelEdgeDirection, ref path);
                                    startTilePosition = TilePosition.TopLeft;
                                    break;
                                case VerticalDirection.Center:
                                    AddPositionsToPath_CenterRight(currentChunkMapPoint, startTilePosition, currentPathNode.travelEdgeDirection, ref path);
                                    startTilePosition = TilePosition.CenterLeft;
                                    break;
                                case VerticalDirection.Down:
                                    AddPositionsToPath_BottomRight(currentChunkMapPoint, startTilePosition, currentPathNode.travelEdgeDirection, ref path);
                                    startTilePosition = TilePosition.BottomLeft;
                                    break;
                            }
                            break;
                        }
                    case CardinalDirection.Left:
                        {
                            VerticalDirection verticalDirection = CalculateNextDesiredDirection_Vertical(pathNodes, startTilePosition, finalTilePosition, i);
                            switch (verticalDirection)
                            {
                                case VerticalDirection.Up:
                                    AddPositionsToPath_TopLeft(currentChunkMapPoint, startTilePosition, currentPathNode.travelEdgeDirection, ref path);
                                    startTilePosition = TilePosition.TopRight;
                                    break;
                                case VerticalDirection.Center:
                                    AddPositionsToPath_CenterLeft(currentChunkMapPoint, startTilePosition, currentPathNode.travelEdgeDirection, ref path);
                                    startTilePosition = TilePosition.CenterRight;
                                    break;
                                case VerticalDirection.Down:
                                    AddPositionsToPath_BottomLeft(currentChunkMapPoint, startTilePosition, currentPathNode.travelEdgeDirection, ref path);
                                    startTilePosition = TilePosition.BottomRight;
                                    break;
                            }
                            break;
                        }
                    case CardinalDirection.Bottom:
                        {
                            HorizontalDirection horizontalDirection = CalculateNextDesiredDirection_Horizontal(pathNodes, startTilePosition, finalTilePosition, i);
                            switch (horizontalDirection)
                            {
                                case HorizontalDirection.Left:
                                    AddPositionsToPath_BottomLeft(currentChunkMapPoint, startTilePosition, currentPathNode.travelEdgeDirection, ref path);
                                    startTilePosition = TilePosition.TopLeft;
                                    break;
                                case HorizontalDirection.Center:
                                    AddPositionsToPath_BottomCenter(currentChunkMapPoint, startTilePosition, currentPathNode.travelEdgeDirection, ref path);
                                    startTilePosition = TilePosition.TopCenter;
                                    break;
                                case HorizontalDirection.Right:
                                    AddPositionsToPath_BottomRight(currentChunkMapPoint, startTilePosition, currentPathNode.travelEdgeDirection, ref path);
                                    startTilePosition = TilePosition.TopRight;
                                    break;
                            }
                            break;
                        }
                    case CardinalDirection.Top:
                        {
                            HorizontalDirection horizontalDirection = CalculateNextDesiredDirection_Horizontal(pathNodes, startTilePosition, finalTilePosition, i);
                            switch (horizontalDirection)
                            {
                                case HorizontalDirection.Left:
                                    AddPositionsToPath_TopLeft(currentChunkMapPoint, startTilePosition, currentPathNode.travelEdgeDirection, ref path);
                                    startTilePosition = TilePosition.BottomLeft;
                                    break;
                                case HorizontalDirection.Center:
                                    AddPositionsToPath_TopCenter(currentChunkMapPoint, startTilePosition, currentPathNode.travelEdgeDirection, ref path);
                                    startTilePosition = TilePosition.BottomCenter;
                                    break;
                                case HorizontalDirection.Right:
                                    AddPositionsToPath_TopRight(currentChunkMapPoint, startTilePosition, currentPathNode.travelEdgeDirection, ref path);
                                    startTilePosition = TilePosition.BottomRight;
                                    break;
                            }
                            break;
                        }
                }
            }
            path.Add(new PathPoint(nextChunkMapPoint, startTilePosition));
        }

        PathNode lastNode = pathNodes[pathNodes.Count - 1];
        ChunkMapPoint lastChunkMapPoint = chunkMapPoints[pathNodes.Count - 1];
        switch (finalTilePosition)
        {
            case TilePosition.Center:
                switch (startTilePosition)
                {
                    case TilePosition.TopLeft:
                        path.Add(new PathPoint(lastChunkMapPoint, TilePosition.TopCenter));
                        break;
                    case TilePosition.TopRight:
                        path.Add(new PathPoint(lastChunkMapPoint, TilePosition.CenterRight));
                        break;
                    case TilePosition.BottomRight:
                        path.Add(new PathPoint(lastChunkMapPoint, TilePosition.BottomCenter));
                        break;
                    case TilePosition.BottomLeft:
                        path.Add(new PathPoint(lastChunkMapPoint, TilePosition.CenterLeft));
                        break;
                }

                path.Add(new PathPoint(lastChunkMapPoint, TilePosition.Center));
                break;
            case TilePosition.BottomLeft:
                AddPositionsToPath_BottomLeft(lastChunkMapPoint, startTilePosition, lastNode.travelEdgeDirection, ref path);
                break;
            case TilePosition.BottomCenter:
                AddPositionsToPath_BottomCenter(lastChunkMapPoint, startTilePosition, lastNode.travelEdgeDirection, ref path);
                break;
            case TilePosition.BottomRight:
                AddPositionsToPath_BottomRight(lastChunkMapPoint, startTilePosition, lastNode.travelEdgeDirection, ref path);
                break;
            case TilePosition.CenterLeft:
                AddPositionsToPath_CenterLeft(lastChunkMapPoint, startTilePosition, lastNode.travelEdgeDirection, ref path);
                break;
            case TilePosition.CenterRight:
                AddPositionsToPath_CenterRight(lastChunkMapPoint, startTilePosition, lastNode.travelEdgeDirection, ref path);
                break;
            case TilePosition.TopLeft:
                AddPositionsToPath_TopLeft(lastChunkMapPoint, startTilePosition, lastNode.travelEdgeDirection, ref path);
                break;
            case TilePosition.TopCenter:
                AddPositionsToPath_TopCenter(lastChunkMapPoint, startTilePosition, lastNode.travelEdgeDirection, ref path);
                break;
            case TilePosition.TopRight:
                AddPositionsToPath_TopRight(lastChunkMapPoint, startTilePosition, lastNode.travelEdgeDirection, ref path);
                break;
        }

        return path;
    }

    private CardinalDirection GetDirection(Point fromPoint, Point toPoint)
    {
        if (fromPoint.xIndex < toPoint.xIndex)
            return CardinalDirection.Right;
        if (fromPoint.xIndex > toPoint.xIndex)
            return CardinalDirection.Left;
        if (fromPoint.yIndex < toPoint.yIndex)
            return CardinalDirection.Top;

        return CardinalDirection.Bottom;
    }

    private bool MatchesDirection(TilePosition tilePosition, CardinalDirection cardinalDirection, bool centerIsFree)
    {
        switch (cardinalDirection)
        {
            case CardinalDirection.Top:
                switch (tilePosition)
                {
                    case TilePosition.BottomLeft:
                    case TilePosition.BottomCenter:
                    case TilePosition.BottomRight:
                        return false;
                    case TilePosition.Center:
                    case TilePosition.CenterLeft:
                    case TilePosition.CenterRight:
                        return centerIsFree;
                    case TilePosition.TopLeft:
                    case TilePosition.TopCenter:
                    case TilePosition.TopRight:
                        return true;
                }
                break;
            case CardinalDirection.Right:
                switch (tilePosition)
                {
                    case TilePosition.BottomLeft:
                    case TilePosition.CenterLeft:
                    case TilePosition.TopLeft:
                        return false;
                    case TilePosition.Center:
                    case TilePosition.BottomCenter:
                    case TilePosition.TopCenter:
                        return centerIsFree;
                    case TilePosition.BottomRight:
                    case TilePosition.CenterRight:
                    case TilePosition.TopRight:
                        return true;
                }
                break;
            case CardinalDirection.Bottom:
                switch (tilePosition)
                {
                    case TilePosition.TopLeft:
                    case TilePosition.TopCenter:
                    case TilePosition.TopRight:
                        return false;
                    case TilePosition.Center:
                    case TilePosition.CenterLeft:
                    case TilePosition.CenterRight:
                        return centerIsFree;
                    case TilePosition.BottomLeft:
                    case TilePosition.BottomCenter:
                    case TilePosition.BottomRight:
                        return true;
                }
                break;
            case CardinalDirection.Left:
                switch (tilePosition)
                {
                    case TilePosition.TopRight:
                    case TilePosition.CenterRight:
                    case TilePosition.BottomRight:
                        return false;
                    case TilePosition.Center:
                    case TilePosition.TopCenter:
                    case TilePosition.BottomCenter:
                        return centerIsFree;
                    case TilePosition.CenterLeft:
                    case TilePosition.TopLeft:
                    case TilePosition.BottomLeft:
                        return true;
                }
                break;
        }

        throw new System.Exception(string.Format("MatchesDirection is missing implementation for {0} and {1}", tilePosition, cardinalDirection));
    }

    private VerticalDirection CalculateNextDesiredDirection_Vertical(List<PathNode> pathNodes, TilePosition currentTilePosition, TilePosition finalTilePosition, int currentIndex)
    {
        VerticalDirection firstDesiredDirectionChange = TilePositionToVerticalDirection(finalTilePosition);
        VerticalDirection currentVerticalDirection = TilePositionToVerticalDirection(currentTilePosition);

        int pathChangeIndex = currentIndex;
        while (pathChangeIndex < pathNodes.Count - 1)
        {
            PathNode currentPathNode = pathNodes[pathChangeIndex];
            PathNode nextPathNode = pathNodes[pathChangeIndex + 1];

            CardinalDirection pathDirection = GetDirection(currentPathNode.point, nextPathNode.point);
            if (pathDirection == CardinalDirection.Left || pathDirection == CardinalDirection.Right)
            {
                pathChangeIndex++;
                continue;
            }

            if (pathDirection == CardinalDirection.Top)
                firstDesiredDirectionChange = VerticalDirection.Up;
            else
                firstDesiredDirectionChange = VerticalDirection.Down;

            break;
        }

        switch (currentVerticalDirection)
        {
            case VerticalDirection.Center:
                {
                    PathNode nextPathNode = pathNodes[currentIndex];
                    if (nextPathNode.travelEdgeDirection.CanGoThroughMiddle())
                        return VerticalDirection.Center;

                    if (firstDesiredDirectionChange == VerticalDirection.Up)
                    {
                        if (nextPathNode.travelEdgeDirection.CanGoThroughTop())
                            return VerticalDirection.Up;

                        return VerticalDirection.Down;
                    }

                    if (nextPathNode.travelEdgeDirection.CanGoThroughBottom())
                        return VerticalDirection.Down;

                    return VerticalDirection.Up;
                }
            case VerticalDirection.Up:
                {
                    PathNode nextPathNode = pathNodes[currentIndex];

                    if (nextPathNode.travelEdgeDirection.CanGoThroughMiddle() && firstDesiredDirectionChange == VerticalDirection.Center)
                        return VerticalDirection.Center;

                    if (nextPathNode.travelEdgeDirection.CanGoThroughTop())
                        return VerticalDirection.Up;

                    return VerticalDirection.Down;
                }
            case VerticalDirection.Down:
                {
                    PathNode nextPathNode = pathNodes[currentIndex];
                    if (nextPathNode.travelEdgeDirection.CanGoThroughMiddle() && firstDesiredDirectionChange == VerticalDirection.Center)
                        return VerticalDirection.Center;

                    if (nextPathNode.travelEdgeDirection.CanGoThroughBottom())
                        return VerticalDirection.Down;

                    return VerticalDirection.Up;
                }
        }

        throw new System.Exception(string.Format("CalculateNextDesiredDirection_Vertical is missing implementation for {0}", currentVerticalDirection));
    }

    private HorizontalDirection CalculateNextDesiredDirection_Horizontal(List<PathNode> pathNodes, TilePosition currentTilePosition, TilePosition finalTilePosition, int currentIndex)
    {
        HorizontalDirection firstDesiredDirectionChange = TilePositionToHorizontalDirection(finalTilePosition);
        HorizontalDirection currentVerticalDirection = TilePositionToHorizontalDirection(currentTilePosition);
        int pathChangeIndex = currentIndex;
        while (pathChangeIndex < pathNodes.Count - 1)
        {
            PathNode currentPathNode = pathNodes[pathChangeIndex];
            PathNode nextPathNode = pathNodes[pathChangeIndex + 1];

            CardinalDirection pathDirection = GetDirection(currentPathNode.point, nextPathNode.point);
            if (pathDirection == CardinalDirection.Top || pathDirection == CardinalDirection.Bottom)
            {
                pathChangeIndex++;
                continue;
            }

            if (pathDirection == CardinalDirection.Left)
                firstDesiredDirectionChange = HorizontalDirection.Left;
            else
                firstDesiredDirectionChange = HorizontalDirection.Right;

            break;
        }

        switch (currentVerticalDirection)
        {
            case HorizontalDirection.Center:
                {
                    PathNode nextPathNode = pathNodes[currentIndex];
                    if (nextPathNode.travelEdgeDirection.CanGoThroughMiddle())
                        return HorizontalDirection.Center;

                    if (firstDesiredDirectionChange == HorizontalDirection.Left)
                    {
                        if (nextPathNode.travelEdgeDirection.CanGoThroughLeft())
                            return HorizontalDirection.Left;

                        return HorizontalDirection.Right;
                    }

                    if (nextPathNode.travelEdgeDirection.CanGoThroughRight())
                        return HorizontalDirection.Right;

                    return HorizontalDirection.Left;
                }
            case HorizontalDirection.Left:
                {
                    PathNode nextPathNode = pathNodes[currentIndex];
                    if (nextPathNode.travelEdgeDirection.CanGoThroughMiddle() && firstDesiredDirectionChange == HorizontalDirection.Center)
                        return HorizontalDirection.Center;

                    if (nextPathNode.travelEdgeDirection.CanGoThroughLeft())
                        return HorizontalDirection.Left;

                    return HorizontalDirection.Right;
                }
            case HorizontalDirection.Right:
                {
                    PathNode nextPathNode = pathNodes[currentIndex];
                    if (nextPathNode.travelEdgeDirection.CanGoThroughMiddle() && firstDesiredDirectionChange == HorizontalDirection.Center)
                        return HorizontalDirection.Center;

                    if (nextPathNode.travelEdgeDirection.CanGoThroughRight())
                        return HorizontalDirection.Right;

                    return HorizontalDirection.Left;
                }
        }

        throw new System.Exception(string.Format("CalculateNextDesiredDirection_Horizontal is missing implementation for {0}", currentVerticalDirection));
    }

    private HorizontalDirection TilePositionToHorizontalDirection(TilePosition tilePosition)
    {
        switch (tilePosition)
        {
            case TilePosition.TopLeft:
            case TilePosition.CenterLeft:
            case TilePosition.BottomLeft:
                return HorizontalDirection.Left;
            case TilePosition.TopCenter:
            case TilePosition.Center:
            case TilePosition.BottomCenter:
                return HorizontalDirection.Center;
            case TilePosition.TopRight:
            case TilePosition.CenterRight:
            case TilePosition.BottomRight:
                return HorizontalDirection.Right;
        }

        throw new System.Exception(string.Format("TilePositionToHorizontalDirection is missing implementation for {0}", tilePosition));
    }

    private VerticalDirection TilePositionToVerticalDirection(TilePosition tilePosition)
    {
        switch (tilePosition)
        {
            case TilePosition.TopLeft:
            case TilePosition.TopCenter:
            case TilePosition.TopRight:
                return VerticalDirection.Up;
            case TilePosition.CenterLeft:
            case TilePosition.Center:
            case TilePosition.CenterRight:
                return VerticalDirection.Center;
            case TilePosition.BottomLeft:
            case TilePosition.BottomCenter:
            case TilePosition.BottomRight:
                return VerticalDirection.Down;
        }

        throw new System.Exception(string.Format("TilePositionToVerticalDirection is missing implementation for {0}", tilePosition));
    }

    private void AddPositionsToPath_TopRight(ChunkMapPoint chunkMapPoint, TilePosition currentTilePosition, TileTraversalEdgeDirection fromEdgeDirection, ref List<PathPoint> pathPositions)
    {
        switch (currentTilePosition)
        {
            case TilePosition.Center:
                pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.TopCenter));
                break;
            case TilePosition.BottomLeft:
                if (fromEdgeDirection.CanGoThroughLeft() && fromEdgeDirection.CanGoThroughTop())
                {
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.TopLeft));
                    break;
                }
                else
                {
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.BottomRight));
                    break;
                }

            case TilePosition.BottomCenter:
                if (fromEdgeDirection.CanGoThroughMiddle())
                {
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.TopCenter));
                }
                else if (fromEdgeDirection.CanGoThroughRight())
                {
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.BottomRight));
                }
                else
                {
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.BottomLeft));
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.TopLeft));
                }
                break;
            case TilePosition.BottomRight:
                if (!fromEdgeDirection.CanGoThroughRight())
                {
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.BottomLeft));
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.TopLeft));
                }
                break;
            case TilePosition.CenterLeft:
                if (fromEdgeDirection.CanGoThroughMiddle())
                {
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.CenterRight));
                }
                else if (fromEdgeDirection.CanGoThroughTop())
                {
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.TopLeft));
                }
                else
                {
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.BottomLeft));
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.BottomRight));
                }
                break;
            case TilePosition.TopLeft:
                if (!fromEdgeDirection.CanGoThroughTop())
                {
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.BottomLeft));
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.BottomRight));
                }
                break;
        }

        pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.TopRight));
    }

    private void AddPositionsToPath_BottomRight(ChunkMapPoint chunkMapPoint, TilePosition currentTilePosition, TileTraversalEdgeDirection fromEdgeDirection, ref List<PathPoint> pathPositions)
    {
        switch (currentTilePosition)
        {
            case TilePosition.Center:
                pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.CenterRight));
                break;
            case TilePosition.TopLeft:
                if (fromEdgeDirection.CanGoThroughTop() && fromEdgeDirection.CanGoThroughRight())
                {
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.TopRight));
                    break;
                }
                else
                {
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.BottomLeft));
                    break;
                }

            case TilePosition.CenterLeft:
                if (fromEdgeDirection.CanGoThroughMiddle())
                {
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.CenterRight));
                }
                else if (fromEdgeDirection.CanGoThroughBottom())
                {
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.BottomLeft));
                }
                else
                {
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.TopLeft));
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.TopRight));
                }
                break;
            case TilePosition.BottomLeft:
                if (!fromEdgeDirection.CanGoThroughBottom())
                {
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.TopLeft));
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.TopRight));
                }
                break;
            case TilePosition.TopCenter:
                if (fromEdgeDirection.CanGoThroughMiddle())
                {
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.BottomCenter));
                }
                else if (fromEdgeDirection.CanGoThroughRight())
                {
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.TopRight));
                }
                else
                {
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.TopLeft));
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.BottomLeft));
                }
                break;
            case TilePosition.TopRight:
                if (!fromEdgeDirection.CanGoThroughRight())
                {
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.TopLeft));
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.BottomLeft));
                }
                break;
        }

        pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.BottomRight));
    }

    private void AddPositionsToPath_BottomLeft(ChunkMapPoint chunkMapPoint, TilePosition currentTilePosition, TileTraversalEdgeDirection fromEdgeDirection, ref List<PathPoint> pathPositions)
    {
        switch (currentTilePosition)
        {
            case TilePosition.Center:
                pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.BottomCenter));
                break;
            case TilePosition.TopRight:
                if (fromEdgeDirection.CanGoThroughRight() && fromEdgeDirection.CanGoThroughBottom())
                {
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.BottomRight));
                }
                else
                {
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.TopLeft));
                }
                break;
            case TilePosition.TopCenter:
                if (fromEdgeDirection.CanGoThroughMiddle())
                {
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.BottomCenter));
                }
                else if (fromEdgeDirection.CanGoThroughLeft())
                {
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.TopLeft));
                }
                else
                {
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.TopRight));
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.BottomRight));
                }
                break;
            case TilePosition.TopLeft:
                if (!fromEdgeDirection.CanGoThroughLeft())
                {
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.TopRight));
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.BottomRight));
                }
                break;
            case TilePosition.CenterRight:
                if (fromEdgeDirection.CanGoThroughMiddle())
                {
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.CenterLeft));
                }
                else if (fromEdgeDirection.CanGoThroughBottom())
                {
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.BottomRight));
                }
                else
                {
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.TopRight));
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.TopLeft));
                }
                break;
            case TilePosition.BottomRight:
                if (!fromEdgeDirection.CanGoThroughBottom())
                {
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.TopRight));
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.TopLeft));
                }
                break;
        }

        pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.BottomLeft));
    }

    private void AddPositionsToPath_TopLeft(ChunkMapPoint chunkMapPoint, TilePosition currentTilePosition, TileTraversalEdgeDirection fromEdgeDirection, ref List<PathPoint> pathPositions)
    { 
        switch (currentTilePosition)
        {
            case TilePosition.Center:
                pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.CenterLeft));
                break;
            case TilePosition.BottomRight:
                if (fromEdgeDirection.CanGoThroughBottom() && fromEdgeDirection.CanGoThroughLeft())
                {
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.BottomLeft));
                    break;
                }
                else
                {
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.TopRight));
                    break;
                }

            case TilePosition.CenterRight:
                if (fromEdgeDirection.CanGoThroughMiddle())
                {
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.CenterLeft));
                }
                else if (fromEdgeDirection.CanGoThroughTop())
                {
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.TopRight));
                }
                else
                {
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.BottomRight));
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.BottomLeft));
                }
                break;
            case TilePosition.TopRight:
                if (!fromEdgeDirection.CanGoThroughTop())
                {
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.BottomRight));
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.BottomLeft));
                }
                break;
            case TilePosition.BottomCenter:
                if (fromEdgeDirection.CanGoThroughMiddle())
                {
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.TopCenter));
                }
                else if (fromEdgeDirection.CanGoThroughLeft())
                {
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.BottomLeft));
                }
                else
                {
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.BottomRight));
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.TopRight));
                }
                break;
            case TilePosition.BottomLeft:
                if (!fromEdgeDirection.CanGoThroughLeft())
                {
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.BottomRight));
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.TopRight));
                }
                break;
        }

        pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.TopLeft));
    }

    private void AddPositionsToPath_TopCenter(ChunkMapPoint chunkMapPoint, TilePosition currentTilePosition, TileTraversalEdgeDirection fromEdgeDirection, ref List<PathPoint> pathPositions)
    {
        switch (currentTilePosition)
        {
            case TilePosition.BottomLeft:
                if (fromEdgeDirection.CanGoThroughMiddle())
                {
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.CenterLeft));
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.Center));
                }
                else if (fromEdgeDirection.CanGoThroughLeft())
                {
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.TopLeft));
                }
                else
                {
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.BottomRight));
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.TopRight));
                }
                break;
            case TilePosition.BottomCenter:
                if (fromEdgeDirection.CanGoThroughMiddle())
                    break;

                if (fromEdgeDirection.CanGoThroughRight())
                {
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.BottomRight));
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.TopRight));
                }
                else
                {
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.BottomLeft));
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.TopLeft));
                }
                break;
            case TilePosition.BottomRight:
                if (fromEdgeDirection.CanGoThroughMiddle())
                {
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.BottomCenter));
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.Center));
                }
                else if (fromEdgeDirection.CanGoThroughRight())
                {
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.TopRight));
                }
                else
                {
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.BottomLeft));
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.TopLeft));
                }
                break;
            case TilePosition.CenterLeft:
                if (fromEdgeDirection.CanGoThroughMiddle())
                {
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.Center));
                }
                else
                {
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.TopLeft));
                }
                break;
            case TilePosition.CenterRight:
                if (fromEdgeDirection.CanGoThroughMiddle())
                {
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.Center));
                }
                else
                {
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.TopRight));
                }
                break;
        }

        pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.TopCenter));
    }

    private void AddPositionsToPath_BottomCenter(ChunkMapPoint chunkMapPoint, TilePosition currentTilePosition, TileTraversalEdgeDirection fromEdgeDirection, ref List<PathPoint> pathPositions)
    {
        switch (currentTilePosition)
        {
            case TilePosition.TopLeft:
                if (fromEdgeDirection.CanGoThroughMiddle())
                {
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.TopCenter));
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.Center));
                }
                else if (fromEdgeDirection.CanGoThroughLeft())
                {
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.BottomLeft));
                }
                else
                {
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.TopRight));
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.BottomRight));
                }
                break;
            case TilePosition.TopCenter:
                if (fromEdgeDirection.CanGoThroughMiddle())
                    break;

                if (fromEdgeDirection.CanGoThroughRight())
                {
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.TopRight));
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.BottomRight));
                }
                else
                {
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.TopLeft));
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.BottomLeft));
                }
                break;
            case TilePosition.TopRight:
                if (fromEdgeDirection.CanGoThroughMiddle())
                {
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.CenterRight));
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.Center));
                }
                else if (fromEdgeDirection.CanGoThroughRight())
                {
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.BottomRight));
                }
                else
                {
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.TopLeft));
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.BottomLeft));
                }
                break;
            case TilePosition.CenterLeft:
                if (fromEdgeDirection.CanGoThroughMiddle())
                {
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.Center));
                }
                else
                {
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.BottomLeft));
                }
                break;
            case TilePosition.CenterRight:
                if (fromEdgeDirection.CanGoThroughMiddle())
                {
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.Center));
                }
                else
                {
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.BottomRight));
                }
                break;
        }

        pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.BottomCenter));
    }

    private void AddPositionsToPath_CenterLeft(ChunkMapPoint chunkMapPoint, TilePosition currentTilePosition, TileTraversalEdgeDirection fromEdgeDirection, ref List<PathPoint> pathPositions)
    {
        switch (currentTilePosition)
        {
            case TilePosition.BottomRight:
                if (fromEdgeDirection.CanGoThroughMiddle())
                {
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.BottomCenter));
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.Center));
                }
                else if (fromEdgeDirection.CanGoThroughBottom())
                {
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.BottomLeft));
                }
                else
                {
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.TopRight));
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.TopLeft));
                }
                break;
            case TilePosition.CenterRight:
                if (fromEdgeDirection.CanGoThroughMiddle())
                    break;

                if (fromEdgeDirection.CanGoThroughBottom())
                {
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.BottomRight));
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.BottomLeft));
                }
                else
                {
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.TopRight));
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.TopLeft));
                }
                break;
            case TilePosition.TopRight:
                if (fromEdgeDirection.CanGoThroughMiddle())
                {
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.CenterRight));
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.Center));
                }
                else if (fromEdgeDirection.CanGoThroughTop())
                {
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.TopLeft));
                }
                else
                {
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.BottomRight));
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.BottomLeft));
                }
                break;
            case TilePosition.TopCenter:
                if (fromEdgeDirection.CanGoThroughMiddle())
                {
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.Center));
                }
                else
                {
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.TopLeft));
                }
                break;
            case TilePosition.BottomCenter:
                if (fromEdgeDirection.CanGoThroughMiddle())
                {
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.Center));
                }
                else
                {
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.BottomLeft));
                }
                break;
        }

        pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.CenterLeft));
    }

    private void AddPositionsToPath_CenterRight(ChunkMapPoint chunkMapPoint, TilePosition currentTilePosition, TileTraversalEdgeDirection fromEdgeDirection, ref List<PathPoint> pathPositions)
    {
        switch (currentTilePosition)
        {
            case TilePosition.BottomLeft:
                if (fromEdgeDirection.CanGoThroughMiddle())
                {
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.CenterLeft));
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.Center));
                }
                else if (fromEdgeDirection.CanGoThroughBottom())
                {
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.BottomRight));
                }
                else
                {
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.TopLeft));
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.TopRight));
                }
                break;
            case TilePosition.CenterLeft:
                if (fromEdgeDirection.CanGoThroughMiddle())
                    break;

                if (fromEdgeDirection.CanGoThroughTop())
                {
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.TopLeft));
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.TopRight));
                }
                else
                {
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.BottomLeft));
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.BottomRight));
                }
                break;
            case TilePosition.TopLeft:
                if (fromEdgeDirection.CanGoThroughMiddle())
                {
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.TopCenter));
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.Center));
                }
                else if (fromEdgeDirection.CanGoThroughTop())
                {
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.TopRight));
                }
                else
                {
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.BottomLeft));
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.BottomRight));
                }
                break;
            case TilePosition.TopCenter:
                if (fromEdgeDirection.CanGoThroughMiddle())
                {
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.Center));
                }
                else
                {
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.TopRight));
                }
                break;
            case TilePosition.BottomCenter:
                if (fromEdgeDirection.CanGoThroughMiddle())
                {
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.Center));
                }
                else
                {
                    pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.BottomRight));
                }
                break;
        }

        pathPositions.Add(new PathPoint(chunkMapPoint, TilePosition.CenterRight));
    }

}
