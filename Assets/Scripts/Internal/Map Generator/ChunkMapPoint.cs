
using SheetCodes;

public class ChunkMapPoint 
{
    public readonly ChunkMap chunkMap;
    public readonly Point globalPoint;
    public readonly Point localPoint;
    public readonly EventVariable<ChunkMapPoint, ChunkMapPointContent> content;
    public readonly EventVariable<ChunkMapPoint, VisionState> visionState;
    public readonly EventVariable<ChunkMapPoint, TileTraversalEdgeDirection> tileTraversalDirection;
    public readonly EventVariable<ChunkMapPoint, bool> hasRoad;
    public readonly EventVariable<ChunkMapPoint, bool> blocked;
    public readonly EventVariable<ChunkMapPoint, bool> canBeBlocked;
    public readonly EventVariable<ChunkMapPoint, int> blockPreventionCounter;
    public readonly EventVariable<ChunkMapPoint, ChunkRegion> chunkRegion;

    public float movementSlowFactor { private set; get; }
    public float pathFindingFactor { private set; get; }
    public TilePosition enterExitTilePosition;
    public EnterExitDirection enterExitDirection;

    public ChunkMapPoint(ChunkMap chunkMap, ChunkRegion chunkRegion, Point localPoint)
    {
        this.chunkRegion = new EventVariable<ChunkMapPoint, ChunkRegion>(this, chunkRegion);
        this.chunkMap = chunkMap;
        this.localPoint = localPoint;
        globalPoint = new Point(localPoint.xIndex + chunkMap.xChunkPosition * chunkMap.chunkWidth, localPoint.yIndex + chunkMap.yChunkPosition * chunkMap.chunkHeight);

        content = new EventVariable<ChunkMapPoint, ChunkMapPointContent>(this, null);

        blocked = new EventVariable<ChunkMapPoint, bool>(this, false);
        canBeBlocked = new EventVariable<ChunkMapPoint, bool>(this, true);
        blockPreventionCounter = new EventVariable<ChunkMapPoint, int>(this, 0);
        hasRoad = new EventVariable<ChunkMapPoint, bool>(this, false);
        tileTraversalDirection = new EventVariable<ChunkMapPoint, TileTraversalEdgeDirection>(this, TileTraversalEdgeDirection.Free);

        visionState = new EventVariable<ChunkMapPoint, VisionState>(this, VisionState.None);

        hasRoad.onValueChange += OnValueChanged_HasRoad;
        visionState.onValueChange += OnValueChanged_VisionState;
        blockPreventionCounter.onValueChange += OnValueChanged_BlockPreventionCounter;
        tileTraversalDirection.onValueChange += OnValueChanged_TileTraversalDirection;
        blocked.onValueChange += OnValueChanged_Blocked;
        content.onValueChange += OnValueChanged_Content;

        CalculatePathFindingSpeed();
        SetBlocked();
        SetCanBeBlocked();
    }

    private void OnValueChanged_TileTraversalDirection(TileTraversalEdgeDirection oldValue, TileTraversalEdgeDirection newValue)
    {
        CalculatePathFindingSpeed();
    }

    private void OnValueChanged_VisionState(VisionState oldValue, VisionState newValue)
    {
        SetBlocked();
    }

    private void OnValueChanged_HasRoad(bool oldValue, bool newValue)
    {
        CalculatePathFindingSpeed();
        SetCanBeBlocked();
    }

    private void OnValueChanged_BlockPreventionCounter(int oldValue, int newValue)
    {
        SetCanBeBlocked();
    }

    private void OnValueChanged_Blocked(bool oldValue, bool newValue)
    {
        SetCanBeBlocked();
    }

    private void CalculatePathFindingSpeed()
    {
        if(tileTraversalDirection.value != TileTraversalEdgeDirection.Free)
        {
            movementSlowFactor = 1.15f;
            pathFindingFactor = 3f;
            return;
        }

        if(!hasRoad.value)
        {
            movementSlowFactor = 1.15f;
            pathFindingFactor = 1f;
            return;
        }

        pathFindingFactor = 0.3f;
        movementSlowFactor = 1f;
    }

    private void SetCanBeBlocked()
    {
        if (blocked.value)
        {
            canBeBlocked.value = false;
            return;
        }

        if (blockPreventionCounter.value > 0)
        {
            canBeBlocked.value = false;
            return;
        }

        if(hasRoad.value)
        {
            canBeBlocked.value = false;
            return;
        }    

        canBeBlocked.value = true;
    }

    private void OnValueChanged_Content(ChunkMapPointContent oldValue, ChunkMapPointContent newValue)
    {
        SetBlocked();
    }

    private void SetBlocked()
    {
        if (content.value != null)
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