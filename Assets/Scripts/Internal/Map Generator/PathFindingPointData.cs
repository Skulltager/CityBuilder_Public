
public class PathFindingPointData
{
    public readonly Point point;
    public readonly ChunkMapPoint chunkMapPoint;

    public int calculationIndex;
    public float traveledDistance;
    public float minimumTotalDistance;
    public bool inPathfindingNodeList;
    public bool possibleEndPoint;

    public PathFindingPointData previousPoint;

    public PathFindingPointData(ChunkMapPoint chunkMapPoint, Point point)
    {
        this.chunkMapPoint = chunkMapPoint;
        this.point = point;
    }
}