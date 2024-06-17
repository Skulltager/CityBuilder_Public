
using SheetCodes;
using System.Collections.Generic;

public class ChunkRegionContent_WorldResource : ChunkRegionContent
{
    public readonly WorldResourceRecord record;
    public readonly int amount;

    public ChunkRegionContent_WorldResource(ChunkRegion chunkRegion, WorldResourceRecord record, int amount)
        : base(chunkRegion) 
    {
        this.record = record;
        this.amount = amount;
    }

    public override void GenerateContent(Map map)
    {
        Point offset = new Point(chunkRegion.xMin, chunkRegion.yMin);
        bool[,] viablePoints = new bool[chunkRegion.width, chunkRegion.height];
        bool[,] viableBorders = new bool[chunkRegion.width, chunkRegion.height];

        List<Point> checkPoints = new List<Point>();

        foreach (Point point in chunkRegion.points)
        {
            if (map.GetChunkRegionPointContent(point.xIndex, point.yIndex) != null)
                continue;

            Point adjustedPoint = point - offset;
            viablePoints[adjustedPoint.xIndex, adjustedPoint.yIndex] = true;
            viableBorders[adjustedPoint.xIndex, adjustedPoint.yIndex] = true;
            checkPoints.Add(point);
        }

        checkPoints.Shuffle(map.random);

        foreach (Point checkPoint in checkPoints)
        {
            List<Point> resourcePoints = new List<Point>(); 

            Point adjustedPoint = checkPoint - offset;
            if (!viablePoints[adjustedPoint.xIndex, adjustedPoint.yIndex])
                continue;

            viablePoints[adjustedPoint.xIndex, adjustedPoint.yIndex] = false;

            List<Point> pointsToHandle = new List<Point>();
            pointsToHandle.Add(adjustedPoint);

            while (pointsToHandle.Count > 0)
            {
                int randomIndex = map.random.Next(0, pointsToHandle.Count);
                Point pointToHandle = pointsToHandle[randomIndex];
                pointsToHandle.RemoveAt(randomIndex);

                resourcePoints.Add(pointToHandle + offset);

                if (resourcePoints.Count == amount)
                    break;

                foreach (CardinalDirection direction in CardinalDirectionExtensions.ALL_DIRECTIONS)
                {
                    Point adjacentPoint = pointToHandle.AddDirection(direction);

                    if (adjacentPoint.xIndex < 0 || adjacentPoint.xIndex >= chunkRegion.width || adjacentPoint.yIndex < 0 || adjacentPoint.yIndex >= chunkRegion.height)
                        continue;

                    if (!viablePoints[adjacentPoint.xIndex, adjacentPoint.yIndex])
                        continue;

                    viablePoints[adjacentPoint.xIndex, adjacentPoint.yIndex] = false;
                    pointsToHandle.Add(adjacentPoint);
                }
            }

            if (resourcePoints.Count != amount)
                continue;

            foreach(Point resourcePoint in resourcePoints)
                map.SetChunkRegionPointContent(resourcePoint.xIndex, resourcePoint.yIndex, new ChunkRegionPointContent_WorldResource(map, resourcePoint, record));

            chunkRegion.AddChunkRegionContentInfo(record, amount);
            break;
        }
    }
}