using SheetCodes;
using System.Collections.Generic;

public class Building
{
    public readonly BuildingRecord record;
    public readonly Map map;
    public readonly Point centerPoint;
    public readonly CardinalDirection rotation;

    public Building(BuildingRecord buildingRecord, Map map, Point centerPoint, CardinalDirection rotation)
    {
        this.record = buildingRecord;
        this.map = map;
        this.centerPoint = centerPoint;
        this.rotation = rotation;
    }

    public void PlaceOnMap(Map map)
    {
        List<ChunkMap> chunkMaps = new List<ChunkMap>();
        foreach(BuildingGridPoint gridPoint in record.BuildingGridData.buildingGridPoints)
        {
            ChunkMapPoint chunkMapPoint = default;

            switch(rotation)
            {
                case CardinalDirection.Right:
                    chunkMapPoint = map.GetChunkMapPoint(centerPoint + gridPoint.centerOffset);
                    break;

                case CardinalDirection.Bottom:
                    chunkMapPoint = map.GetChunkMapPoint(centerPoint + gridPoint.centerOffset.GetRotated90DegreesOffset() + new Point(0, -1));
                    break;

                case CardinalDirection.Left:
                    chunkMapPoint = map.GetChunkMapPoint(centerPoint + gridPoint.centerOffset.GetRotated180DegreesOffset() + new Point(-1, -1));
                    break;

                case CardinalDirection.Top:
                    chunkMapPoint = map.GetChunkMapPoint(centerPoint + gridPoint.centerOffset.GetRotated270DegreesOffset() + new Point(-1, 0));
                    break;
            }

            if (!chunkMaps.Contains(chunkMapPoint.chunkMap))
                chunkMaps.Add(chunkMapPoint.chunkMap);

            chunkMapPoint.building.value = this;
        }

        foreach (ChunkMap chunkMap in chunkMaps)
            chunkMap.buildings.Add(this);

        foreach (BuildingEnterExitGridPoint gridPoint in record.BuildingGridData.enterExitPoints)
        {
            ChunkMapPoint chunkMapPoint = default;

            switch (rotation)
            {
                case CardinalDirection.Right:
                    chunkMapPoint = map.GetChunkMapPoint(centerPoint + gridPoint.centerOffset);
                    break;

                case CardinalDirection.Bottom:
                    chunkMapPoint = map.GetChunkMapPoint(centerPoint + gridPoint.centerOffset.GetRotated90DegreesOffset() + new Point(0, -1));
                    break;

                case CardinalDirection.Left:
                    chunkMapPoint = map.GetChunkMapPoint(centerPoint + gridPoint.centerOffset.GetRotated180DegreesOffset() + new Point(-1, -1));
                    break;

                case CardinalDirection.Top:
                    chunkMapPoint = map.GetChunkMapPoint(centerPoint + gridPoint.centerOffset.GetRotated270DegreesOffset() + new Point(-1, 0));
                    break;
            }

            chunkMapPoint.blockPreventionCounter.value++;
        }
    }

    public void RemoveFromMap()
    {
        List<ChunkMap> chunkMaps = new List<ChunkMap>();
        foreach (BuildingGridPoint gridPoint in record.BuildingGridData.buildingGridPoints)
        {
            ChunkMapPoint chunkMapPoint = default;

            switch (rotation)
            {
                case CardinalDirection.Right:
                    chunkMapPoint = map.GetChunkMapPoint(centerPoint + gridPoint.centerOffset);
                    break;

                case CardinalDirection.Bottom:
                    chunkMapPoint = map.GetChunkMapPoint(centerPoint + gridPoint.centerOffset.GetRotated90DegreesOffset() + new Point(0, -1));
                    break;

                case CardinalDirection.Left:
                    chunkMapPoint = map.GetChunkMapPoint(centerPoint + gridPoint.centerOffset.GetRotated180DegreesOffset() + new Point(-1, -1));
                    break;

                case CardinalDirection.Top:
                    chunkMapPoint = map.GetChunkMapPoint(centerPoint + gridPoint.centerOffset.GetRotated270DegreesOffset() + new Point(-1, 0));
                    break;
            }

            if (!chunkMaps.Contains(chunkMapPoint.chunkMap))
                chunkMaps.Add(chunkMapPoint.chunkMap);

            chunkMapPoint.building.value = null;
        }

        foreach (ChunkMap chunkMap in chunkMaps)
            chunkMap.buildings.Remove(this);

        foreach (BuildingEnterExitGridPoint gridPoint in record.BuildingGridData.enterExitPoints)
        {
            ChunkMapPoint chunkMapPoint = default;

            switch (rotation)
            {
                case CardinalDirection.Right:
                    chunkMapPoint = map.GetChunkMapPoint(centerPoint + gridPoint.centerOffset);
                    break;

                case CardinalDirection.Bottom:
                    chunkMapPoint = map.GetChunkMapPoint(centerPoint + gridPoint.centerOffset.GetRotated90DegreesOffset() + new Point(0, -1));
                    break;

                case CardinalDirection.Left:
                    chunkMapPoint = map.GetChunkMapPoint(centerPoint + gridPoint.centerOffset.GetRotated180DegreesOffset() + new Point(-1, -1));
                    break;

                case CardinalDirection.Top:
                    chunkMapPoint = map.GetChunkMapPoint(centerPoint + gridPoint.centerOffset.GetRotated270DegreesOffset() + new Point(-1, 0));
                    break;
            }

            chunkMapPoint.blockPreventionCounter.value--;
        }
    }
}