
public struct PathNode
{
    public readonly Point point;
    public readonly TileTraversalEdgeDirection travelEdgeDirection;

    public PathNode(Point point, TileTraversalEdgeDirection travelEdgeDirection)
    {
        this.point = point;
        this.travelEdgeDirection = travelEdgeDirection;
    }
}