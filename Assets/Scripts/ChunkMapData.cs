using SheetCodes;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChunkMapData
{
    public readonly int xIndex;
    public readonly int yIndex;
    public readonly int width;
    public readonly int height;
    public readonly WorldResourceRecord[,] resourceMap;
    public readonly BiomeType[,] biomeTypeMap;
    public readonly bool[,] fogOfWarMap;

    private readonly bool[,] handleBuffer;

    public ChunkMapData(int width, int height, BiomeType defaultBiomeType)
    {
        this.width = width;
        this.height = height;
        resourceMap = new WorldResourceRecord[width, height];
        handleBuffer = new bool[width, height];
        biomeTypeMap = new BiomeType[width, height];
        fogOfWarMap = new bool[width, height];
        for(int x = 0; x < biomeTypeMap.GetLength(0); x++)
        {
            for(int y = 0; y < biomeTypeMap.GetLength(1); y++)
            {
                fogOfWarMap[x, y] = true;
                biomeTypeMap[x, y] = defaultBiomeType;
            }
        }
    }

    public void Fill(WorldResourceIdentifier resourceIdentifier)
    {
        WorldResourceRecord resourceRecord = resourceIdentifier.GetRecord();

        for (int x = 0; x < resourceMap.GetLength(0); x++)
        {
            for (int y = 0; y < resourceMap.GetLength(1); y++)
            {
                resourceMap[x, y] = resourceRecord;
            }
        }
    }

    public bool TryFill_AmountRandomly(Point startPoint, int pointsToSpawn, WorldResourceIdentifier resourceIdentifier, BiomeType biomeType, out List<Point> filledPoints, params WorldResourceIdentifier[] typesToNotOverride)
    {
        List<Point> possiblePoints = new List<Point>();
        List<Point> spawnPoints = new List<Point>();

        WorldResourceRecord resourceRecord = resourceIdentifier.GetRecord();
        possiblePoints.Add(startPoint);
        handleBuffer[startPoint.xIndex, startPoint.yIndex] = true;

        bool quotaFilled = false;

        while(possiblePoints.Count > 0)
        {
            int randomIndex = Random.Range(0, possiblePoints.Count);
            Point possiblePoint = possiblePoints[randomIndex];
            possiblePoints.RemoveAt(randomIndex);
            spawnPoints.Add(possiblePoint);

            if (spawnPoints.Count == pointsToSpawn)
            {
                quotaFilled = true;
                break;
            }

            Point point = possiblePoint.AddDirection(CardinalDirection.Left);

            if (IsInBounds(point) && !typesToNotOverride.Contains(resourceMap[point.xIndex, point.yIndex] != null ? resourceMap[point.xIndex, point.yIndex].Identifier : WorldResourceIdentifier.None) && !handleBuffer[point.xIndex, point.yIndex])
            {
                handleBuffer[point.xIndex, point.yIndex] = true;
                possiblePoints.Add(point);
            }

            point = possiblePoint.AddDirection(CardinalDirection.Right);

            if (IsInBounds(point) && !typesToNotOverride.Contains(resourceMap[point.xIndex, point.yIndex] != null ? resourceMap[point.xIndex, point.yIndex].Identifier : WorldResourceIdentifier.None) && !handleBuffer[point.xIndex, point.yIndex])
            {
                handleBuffer[point.xIndex, point.yIndex] = true;
                possiblePoints.Add(point);
            }

            point = possiblePoint.AddDirection(CardinalDirection.Top);

            if (IsInBounds(point) && !typesToNotOverride.Contains(resourceMap[point.xIndex, point.yIndex] != null ? resourceMap[point.xIndex, point.yIndex].Identifier : WorldResourceIdentifier.None) && !handleBuffer[point.xIndex, point.yIndex])
            {
                handleBuffer[point.xIndex, point.yIndex] = true;
                possiblePoints.Add(point);
            }

            point = possiblePoint.AddDirection(CardinalDirection.Bottom);

            if (IsInBounds(point) && !typesToNotOverride.Contains(resourceMap[point.xIndex, point.yIndex] != null ? resourceMap[point.xIndex, point.yIndex].Identifier : WorldResourceIdentifier.None) && !handleBuffer[point.xIndex, point.yIndex])
            {
                handleBuffer[point.xIndex, point.yIndex] = true;
                possiblePoints.Add(point);
            }
        }

        if (!quotaFilled)
        {
            foreach(Point point in spawnPoints)
                handleBuffer[point.xIndex, point.yIndex] = false;

            foreach (Point point in possiblePoints)
                handleBuffer[point.xIndex, point.yIndex] = false;

            filledPoints = new List<Point>();
            return false;
        }

        foreach (Point point in spawnPoints)
        {
            resourceMap[point.xIndex, point.yIndex] = resourceRecord;
            biomeTypeMap[point.xIndex, point.yIndex] = biomeType;
            handleBuffer[point.xIndex, point.yIndex] = false;
        }

        foreach (Point point in possiblePoints)
            handleBuffer[point.xIndex, point.yIndex] = false;

        filledPoints = spawnPoints;
        return true;
    }

    public void SetFogOfWarInRadius(List<Point> points, int radius, bool enableFog)
    {
        foreach (Point point in points)
            SetFogOfWarInRadius(point, radius, enableFog);
    }

    public void SetFogOfWarInRadius(Point point, int radius, bool enableFog)
    {
        int startX = Mathf.Max(point.xIndex - radius, 0);
        int endX = Mathf.Min(point.xIndex + radius, width - 1);

        for(int x = startX; x <= endX; x++)
        {
            int offset = radius - Mathf.Abs(point.xIndex - x);

            int startY = Mathf.Max(point.yIndex - offset, 0);
            int endY = Mathf.Min(point.yIndex + offset, height - 1);

            for(int y = startY; y <= endY; y++)
            {
                fogOfWarMap[x, y] = enableFog;
            }
        }
    }

    private bool IsInBounds(Point point)
    {
        if (point.xIndex < 0)
            return false;

        if (point.xIndex >= width)
            return false;

        if (point.yIndex < 0)
            return false;

        if (point.yIndex >= height)
            return false;

        return true;
    }
}