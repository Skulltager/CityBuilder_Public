using SheetCodes;

public class Building
{
    public readonly BuildingRecord record;
    public readonly ChunkMap chunkMap;
    public readonly Point centerPoint;
    public readonly CardinalDirection rotation;

    public Building(BuildingRecord buildingRecord, ChunkMap chunkMap, Point centerPoint, CardinalDirection rotation)
    {
        this.record = buildingRecord;
        this.chunkMap = chunkMap;
        this.centerPoint = centerPoint;
        this.rotation = rotation;
    }

    public void PlaceOnMap()
    {
        foreach(BuildingGridPoint gridPoint in record.BuildingGridData.buildingGridPoints)
        {
            ChunkMapPoint chunkMapPoint = default;

            switch(rotation)
            {
                case CardinalDirection.Right:
                    chunkMapPoint = chunkMap.GetChunkMapPoint(centerPoint + gridPoint.centerOffset);
                    break;

                case CardinalDirection.Bottom:
                    chunkMapPoint = chunkMap.GetChunkMapPoint(centerPoint + gridPoint.centerOffset.GetRotated90DegreesOffset() + new Point(0, -1));
                    break;

                case CardinalDirection.Left:
                    chunkMapPoint = chunkMap.GetChunkMapPoint(centerPoint + gridPoint.centerOffset.GetRotated180DegreesOffset() + new Point(-1, -1));
                    break;

                case CardinalDirection.Top:
                    chunkMapPoint = chunkMap.GetChunkMapPoint(centerPoint + gridPoint.centerOffset.GetRotated270DegreesOffset() + new Point(-1, 0));
                    break;
            }

            chunkMapPoint.building.value = this;
        }


        foreach (BuildingEnterExitGridPoint gridPoint in record.BuildingGridData.enterExitPoints)
        {
            ChunkMapPoint chunkMapPoint = default;

            switch (rotation)
            {
                case CardinalDirection.Right:
                    chunkMapPoint = chunkMap.GetChunkMapPoint(centerPoint + gridPoint.centerOffset.AddDirection(gridPoint.direction));
                    break;

                case CardinalDirection.Bottom:
                    chunkMapPoint = chunkMap.GetChunkMapPoint(centerPoint + gridPoint.centerOffset.GetRotated90DegreesOffset().AddDirection(gridPoint.direction.Rotate90Degrees()) + new Point(0, -1));
                    break;

                case CardinalDirection.Left:
                    chunkMapPoint = chunkMap.GetChunkMapPoint(centerPoint + gridPoint.centerOffset.GetRotated180DegreesOffset().AddDirection(gridPoint.direction.Rotate180Degrees()) + new Point(-1, -1));
                    break;

                case CardinalDirection.Top:
                    chunkMapPoint = chunkMap.GetChunkMapPoint(centerPoint + gridPoint.centerOffset.GetRotated270DegreesOffset().AddDirection(gridPoint.direction.Rotate270Degrees()) + new Point(-1, 0));
                    break;
            }

            chunkMapPoint.blockPreventionCounter.value++;
        }
    }

    public void RemoveFromMap()
    {
        foreach (BuildingGridPoint gridPoint in record.BuildingGridData.buildingGridPoints)
        {
            ChunkMapPoint chunkMapPoint = default;

            switch (rotation)
            {
                case CardinalDirection.Right:
                    chunkMapPoint = chunkMap.GetChunkMapPoint(centerPoint + gridPoint.centerOffset);
                    break;

                case CardinalDirection.Bottom:
                    chunkMapPoint = chunkMap.GetChunkMapPoint(centerPoint + gridPoint.centerOffset.GetRotated90DegreesOffset() + new Point(0, -1));
                    break;

                case CardinalDirection.Left:
                    chunkMapPoint = chunkMap.GetChunkMapPoint(centerPoint + gridPoint.centerOffset.GetRotated180DegreesOffset() + new Point(-1, -1));
                    break;

                case CardinalDirection.Top:
                    chunkMapPoint = chunkMap.GetChunkMapPoint(centerPoint + gridPoint.centerOffset.GetRotated270DegreesOffset() + new Point(-1, 0));
                    break;
            }

            chunkMapPoint.building.value = null;
        }


        foreach (BuildingEnterExitGridPoint gridPoint in record.BuildingGridData.enterExitPoints)
        {
            ChunkMapPoint chunkMapPoint = default;

            switch (rotation)
            {
                case CardinalDirection.Right:
                    chunkMapPoint = chunkMap.GetChunkMapPoint(centerPoint + gridPoint.centerOffset.AddDirection(gridPoint.direction));
                    break;

                case CardinalDirection.Bottom:
                    chunkMapPoint = chunkMap.GetChunkMapPoint(centerPoint + gridPoint.centerOffset.GetRotated90DegreesOffset().AddDirection(gridPoint.direction.Rotate90Degrees()) + new Point(0, -1));
                    break;

                case CardinalDirection.Left:
                    chunkMapPoint = chunkMap.GetChunkMapPoint(centerPoint + gridPoint.centerOffset.GetRotated180DegreesOffset().AddDirection(gridPoint.direction.Rotate180Degrees()) + new Point(-1, -1));
                    break;

                case CardinalDirection.Top:
                    chunkMapPoint = chunkMap.GetChunkMapPoint(centerPoint + gridPoint.centerOffset.GetRotated270DegreesOffset().AddDirection(gridPoint.direction.Rotate270Degrees()) + new Point(-1, 0));
                    break;
            }

            chunkMapPoint.blockPreventionCounter.value--;
        }
    }
}