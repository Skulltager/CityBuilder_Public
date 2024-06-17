public class VillagerTaskPoint
{
    public readonly EventVariable<VillagerTaskPoint, bool> enabled;
    public readonly ChunkMapPointContent owner;
    public Point point;
    public TilePosition tilePosition;
    public object extraData;

    public VillagerTaskPoint(ChunkMapPointContent owner, Point point, TilePosition tilePosition, object extraData = null)
    {
        this.owner = owner;
        this.enabled = new EventVariable<VillagerTaskPoint, bool>(this, true);
        this.point = point;
        this.tilePosition = tilePosition;
        this.extraData = extraData;
    }
}