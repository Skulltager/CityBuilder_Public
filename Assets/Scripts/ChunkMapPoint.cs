
using SheetCodes;

public class ChunkMapPoint 
{
    public readonly Point point;
    public readonly EventVariable<ChunkMapPoint, Building> building;
    public readonly EventVariable<ChunkMapPoint, WorldResource> worldResource;
    public readonly EventVariable<ChunkMapPoint, bool> blocked;
    public readonly EventVariable<ChunkMapPoint, bool> canBeBlocked;
    public readonly EventVariable<ChunkMapPoint, int> blockPreventionCounter;

    public ChunkMapPoint(Point point)
    {
        this.point = point;

        building = new EventVariable<ChunkMapPoint, Building>(this, null);
        worldResource = new EventVariable<ChunkMapPoint, WorldResource>(this, null);

        blocked = new EventVariable<ChunkMapPoint, bool>(this, false);
        canBeBlocked = new EventVariable<ChunkMapPoint, bool>(this, true);
        blockPreventionCounter = new EventVariable<ChunkMapPoint, int>(this, 0);

        building.onValueChange += OnValueChanged_Building;
        worldResource.onValueChange += OnValueChanged_WorldResource;
        blockPreventionCounter.onValueChange += OnValueChanged_BlockPreventionCounter;
        blocked.onValueChange += OnValueChanged_Blocked;

        SetCanBeBlocked();
    }

    public ChunkMapPoint(Point point, WorldResourceRecord worldResourceRecord)
        : this(point)
    {
        worldResource.value = new WorldResource(worldResourceRecord, 100);
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
        canBeBlocked.value = !blocked.value && blockPreventionCounter.value == 0;
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
        if(worldResource.value != null)
        {
            blocked.value = true;
            return;
        }

        if(building.value != null)
        {
            blocked.value = true;
            return;
        }

        blocked.value = false;
    }
}