
using SheetCodes;

public class ChunkMapPoint 
{
    public readonly ChunkMap chunkMap;
    public readonly Point globalPoint;
    public readonly Point localPoint;
    public readonly EventVariable<ChunkMapPoint, Building> building;
    public readonly EventVariable<ChunkMapPoint, WorldResource> worldResource;
    public readonly EventVariable<ChunkMapPoint, VisionState> visionState;
    public readonly EventVariable<ChunkMapPoint, bool> blocked;
    public readonly EventVariable<ChunkMapPoint, bool> canBeBlocked;
    public readonly EventVariable<ChunkMapPoint, int> blockPreventionCounter;

    public ChunkMapPoint(ChunkMap chunkMap, Point localPoint)
    {
        this.chunkMap = chunkMap;
        this.localPoint = localPoint;
        this.globalPoint = new Point(localPoint.xIndex + chunkMap.xChunkPosition * chunkMap.chunkWidth, localPoint.yIndex + chunkMap.yChunkPosition * chunkMap.chunkHeight);

        building = new EventVariable<ChunkMapPoint, Building>(this, null);
        worldResource = new EventVariable<ChunkMapPoint, WorldResource>(this, null);

        blocked = new EventVariable<ChunkMapPoint, bool>(this, false);
        canBeBlocked = new EventVariable<ChunkMapPoint, bool>(this, true);
        blockPreventionCounter = new EventVariable<ChunkMapPoint, int>(this, 0);

        visionState = new EventVariable<ChunkMapPoint, VisionState>(this, VisionState.None);

        building.onValueChange += OnValueChanged_Building;
        visionState.onValueChange += OnValueChanged_VisionState;
        worldResource.onValueChange += OnValueChanged_WorldResource;
        blockPreventionCounter.onValueChange += OnValueChanged_BlockPreventionCounter;
        blocked.onValueChange += OnValueChanged_Blocked;

        SetBlocked();
        SetCanBeBlocked();
    }

    public ChunkMapPoint(ChunkMap chunkMap, Point point, WorldResourceRecord worldResourceRecord)
        : this(chunkMap, point)
    {
        worldResource.value = new WorldResource(worldResourceRecord, 100);
    }

    private void OnValueChanged_VisionState(VisionState oldValue, VisionState newValue)
    {
        SetBlocked();
    }

    private void OnValueChanged_BlockPreventionCounter(int oldValue, int newValue)
    {
        SetCanBeBlocked();
    }

    private void OnValueChanged_Blocked(bool oldValue, bool newValue)
    {
        SetCanBeBlocked();
    }

    private void SetCanBeBlocked()
    {
        if (blocked.value)
        {
            canBeBlocked.value = false;
            return;
        }

        if(blockPreventionCounter.value > 0)
        {
            canBeBlocked.value = false;
            return;
        }

        canBeBlocked.value = true;
    }

    private void OnValueChanged_Building(Building oldValue, Building newValue)
    {
        SetBlocked();
    }

    private void OnValueChanged_WorldResource(WorldResource oldValue, WorldResource newValue)
    {
        SetBlocked();
    }

    private void SetBlocked()
    {
        if (worldResource.value != null)
        {
            blocked.value = true;
            return;
        }

        if (building.value != null)
        {
            blocked.value = true;
            return;
        }

        if (visionState.value != VisionState.Full)
        {
            blocked.value = true;
            return;
        }

        blocked.value = false;
    }
}