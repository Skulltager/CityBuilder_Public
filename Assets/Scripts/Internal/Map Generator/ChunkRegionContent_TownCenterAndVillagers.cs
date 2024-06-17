using SheetCodes;
using System;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class ChunkRegionContent_TownCenterAndVillagers : ChunkRegionContent
{
    public readonly int villagers;

    public ChunkRegionContent_TownCenterAndVillagers(ChunkRegion chunkRegion, int villagers) 
        : base(chunkRegion)
    {
        this.villagers = villagers;
    }

    public override void GenerateContent(Map map)
    {
        BuildingRecord record = BuildingIdentifier.TownCenter.GetRecord();

        CardinalDirection[] directions = new CardinalDirection[CardinalDirectionExtensions.ALL_DIRECTIONS.Length];
        Array.Copy(DiagonalDirectionExtensions.ALL_DIRECTIONS, directions, 4);
        directions.Shuffle(map.random);

        WorldContentSizeBounds[] buildingSizes = new WorldContentSizeBounds[4];
        Point[][] buildingEnterExitPointsArray = new Point[4][];

        for (int i = 0; i < directions.Length; i++)
        {
            CardinalDirection direction = directions[i];
            buildingSizes[i] = record.BuildingGridData.size.GetSizeBounds(direction);
            buildingEnterExitPointsArray[i] = new Point[record.BuildingGridData.enterExitPoints.Length];

            for (int j = 0; j < record.BuildingGridData.enterExitPoints.Length; j++)
                buildingEnterExitPointsArray[i][j] = record.BuildingGridData.enterExitPoints[j].RotateByDirection(direction);
        }


        Point offset = new Point(chunkRegion.xMin, chunkRegion.yMin);
        bool[,] emptyPoints = new bool[chunkRegion.width, chunkRegion.height];

        foreach (Point point in chunkRegion.points)
        {
            if (map.GetChunkRegionPointContent(point.xIndex, point.yIndex) != null)
                continue;

            Point adjustedPoint = point - offset;
            emptyPoints[adjustedPoint.xIndex, adjustedPoint.yIndex] = true;
        }

        List<Point> checkPoints = new List<Point>();
        bool[,] viablePoints = new bool[chunkRegion.width, chunkRegion.height];
        foreach (Point point in chunkRegion.points)
        {
            Point adjustedPoint = point - offset;
            if (!emptyPoints[adjustedPoint.xIndex, adjustedPoint.yIndex])
                continue;

            bool anyFound = false;
            foreach(DiagonalDirection direction in DiagonalDirectionExtensions.ALL_DIRECTIONS)
            {
                Point diagonalCheckPoint = adjustedPoint.AddDirection(direction);
                if (diagonalCheckPoint.xIndex < 0 || diagonalCheckPoint.xIndex >= chunkRegion.width || diagonalCheckPoint.yIndex < 0 || diagonalCheckPoint.yIndex >= chunkRegion.height)
                {
                    anyFound = true;
                    break;
                }

                if (!emptyPoints[diagonalCheckPoint.xIndex, diagonalCheckPoint.yIndex])
                {
                    anyFound = true;
                    break;
                }
            }

            if (anyFound)
                continue;

            viablePoints[adjustedPoint.xIndex, adjustedPoint.yIndex] = true;
            checkPoints.Add(point);
        }

        checkPoints.Shuffle(map.random);

        foreach (Point checkPoint in checkPoints)
        {
            Point adjustedPoint = checkPoint - offset;

            for(int i = 0; i < directions.Length; i++)
            {
                CardinalDirection direction = directions[i];
                WorldContentSizeBounds sizeBounds = buildingSizes[i];

                bool anyFound = false;

                for (int x = 0; x <= sizeBounds.width; x++)
                {
                    for (int y = 0; y <= sizeBounds.height; y++)
                    {
                        int localX = x + sizeBounds.xOffset + adjustedPoint.xIndex;
                        int localY = y + sizeBounds.yOffset + adjustedPoint.yIndex;
                        if (localX < 0 || localX >= chunkRegion.width || localY < 0 || localY >= chunkRegion.height)
                        {
                            anyFound = true;
                            break;
                        }

                        if (!viablePoints[localX, localY])
                        {
                            anyFound = true;
                            break;
                        }
                    }

                    if (anyFound)
                        break;
                }

                if (anyFound)
                    continue;

                Point[] buildingEnterExitPoints = buildingEnterExitPointsArray[i];
                foreach (Point buildingPoint in buildingEnterExitPoints)
                {
                    Point adjustedBuildingPoint = buildingPoint + adjustedPoint;

                    if (adjustedBuildingPoint.xIndex < 0 || adjustedBuildingPoint.xIndex >= chunkRegion.width || adjustedBuildingPoint.yIndex < 0 || adjustedBuildingPoint.yIndex >= chunkRegion.height)
                    {
                        anyFound = true;
                        break;
                    }

                    if (!viablePoints[adjustedBuildingPoint.xIndex, adjustedBuildingPoint.yIndex])
                    {
                        anyFound = true;
                        break;
                    }
                }

                if (anyFound)
                    continue;

                ChunkMapPointContent_Building building = new ChunkMapPointContent_Building_TownCenter(map, checkPoint, direction, chunkRegion.owner);

                bool firstIndex = true;

                List<Point> viableVillagerSpawns = new List<Point>();

                for (int x = 0; x < sizeBounds.width; x++)
                {
                    for (int y = 0; y < sizeBounds.height; y++)
                    {
                        Point buildingPoint = new Point(x + sizeBounds.xOffset, y + sizeBounds.yOffset);
                        Point adjustedBuildingPoint = buildingPoint + adjustedPoint + offset;
                        map.SetChunkRegionPointContent(adjustedBuildingPoint.xIndex, adjustedBuildingPoint.yIndex, new ChunkRegionPointContent_BuildingPoint(map, adjustedBuildingPoint, firstIndex, building));

                        firstIndex = false;
                        Point adjustedViablePoint = buildingPoint + adjustedPoint;
                        viablePoints[adjustedViablePoint.xIndex, adjustedViablePoint.yIndex] = false;
                    }
                }

                for (int x = 0; x < sizeBounds.width; x++)
                {
                    for (int y = 0; y < sizeBounds.height; y++)
                    {
                        Point buildingPoint = new Point(x + sizeBounds.xOffset, y + sizeBounds.yOffset);

                        foreach (DiagonalDirection diagonalDirection in DiagonalDirectionExtensions.ALL_DIRECTIONS)
                        {
                            Point villagerSpawnPoint = buildingPoint + adjustedPoint.AddDirection(diagonalDirection);

                            if (villagerSpawnPoint.xIndex < 0 || villagerSpawnPoint.xIndex >= chunkRegion.width || villagerSpawnPoint.yIndex < 0 || villagerSpawnPoint.yIndex >= chunkRegion.height)
                                continue;

                            if (!viablePoints[villagerSpawnPoint.xIndex, villagerSpawnPoint.yIndex])
                                continue;

                            viablePoints[villagerSpawnPoint.xIndex, villagerSpawnPoint.yIndex] = false;
                            viableVillagerSpawns.Add(villagerSpawnPoint);
                        }
                    }
                }

                foreach (Point buildingPoint in buildingEnterExitPoints)
                {
                    Point adjustedBuildingPoint = buildingPoint + adjustedPoint + offset;
                    map.SetChunkRegionPointContentReservation_BlockPrevention(adjustedBuildingPoint.xIndex, adjustedBuildingPoint.yIndex);
                }

                viableVillagerSpawns.Shuffle(map.random);

                for(int j = 0; j < villagers && j < viableVillagerSpawns.Count; j++)
                {
                    Point villagerSpawnPoint = viableVillagerSpawns[j] + offset;
                    map.SetChunkRegionPointContentReservation_Villager(villagerSpawnPoint.xIndex, villagerSpawnPoint.yIndex);
                }

                return;
            }
        }

        UnityEngine.Debug.LogError("Failed to generate building");
    }
}