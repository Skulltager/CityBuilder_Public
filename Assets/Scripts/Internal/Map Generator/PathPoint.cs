
public struct PathPoint
{
    public readonly ChunkMapPoint mapPoint;
    public readonly TilePosition tilePosition;
    public readonly bool doViableCheck;

    public PathPoint(ChunkMapPoint mapPoint, TilePosition tilePosition, bool doViableCheck = true)
    {
        this.mapPoint = mapPoint;
        this.tilePosition = tilePosition;
        this.doViableCheck = doViableCheck;
    }
}