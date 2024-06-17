
public struct WorldResourceDirection
{
    public readonly ChunkMapPointContent_WorldResource worldResource;
    public readonly CardinalDirection direction;

    public WorldResourceDirection(ChunkMapPointContent_WorldResource worldResource, CardinalDirection direction)
    {
        this.worldResource = worldResource;
        this.direction = direction;
    }
}