using UnityEngine;

public class BuildingEnterExitIndicator : MonoBehaviour
{
    [SerializeField] private Material viableMaterial;
    [SerializeField] private Material notViableMaterial;
    [SerializeField] private MeshRenderer meshRenderer;

    private ChunkMap chunkMap;
    public readonly EventVariable<BuildingEnterExitIndicator, Point> position;
    public readonly EventVariable<BuildingEnterExitIndicator, CardinalDirection> enterExitDirection;
    private readonly EventVariable<BuildingEnterExitIndicator, bool> viable;
    private readonly EventVariable<BuildingEnterExitIndicator, ChunkMapPoint> chunkMapPoint;

    public bool isViable => viable.value;

    private BuildingEnterExitIndicator()
    {
        position = new EventVariable<BuildingEnterExitIndicator, Point>(this, default);
        enterExitDirection = new EventVariable<BuildingEnterExitIndicator, CardinalDirection>(this, default);
        viable = new EventVariable<BuildingEnterExitIndicator, bool>(this, default);
        chunkMapPoint = new EventVariable<BuildingEnterExitIndicator, ChunkMapPoint>(this, default);
    }

    public void Initialize(ChunkMap chunkMap)
    {
        this.chunkMap = chunkMap;
        position.onValueChange += OnValueChanged_Position;
        enterExitDirection.onValueChange += OnValueChanged_EnterExitPosition;
        chunkMapPoint.onValueChange += OnValueChanged_ChunkMapPoint;
        viable.onValueChangeImmediate += OnValueChanged_Viable;
    }

    private void OnValueChanged_Position(Point oldValue, Point newValue)
    {
        SetChunkMapPoints();
    }

    private void OnValueChanged_EnterExitPosition(CardinalDirection oldValue, CardinalDirection newValue)
    {
        SetChunkMapPoints();
    }

    private void SetChunkMapPoints()
    {
        if (position.value != null)
        {
            switch (enterExitDirection.value)
            {
                case CardinalDirection.Right:
                    transform.position = new Vector3(position.value.xIndex + 1, 0, position.value.yIndex + 0.5f);
                    transform.rotation = Quaternion.Euler(0, 90, 0);
                    break;
                case CardinalDirection.Bottom:
                    transform.position = new Vector3(position.value.xIndex + 0.5f, 0, position.value.yIndex);
                    transform.rotation = Quaternion.Euler(0, 180, 0);
                    break;
                case CardinalDirection.Left:
                    transform.position = new Vector3(position.value.xIndex, 0, position.value.yIndex + 0.5f);
                    transform.rotation = Quaternion.Euler(0, 270, 0);
                    break;
                case CardinalDirection.Top:
                    transform.position = new Vector3(position.value.xIndex + 0.5f, 0, position.value.yIndex + 1);
                    transform.rotation = Quaternion.Euler(0, 0, 0);
                    break;
            }

            meshRenderer.enabled = true;
            ChunkMapPoint newChunkMapPoint;
            if (chunkMap.TryGetChunkMapPoint(position.value.AddDirection(enterExitDirection.value), out newChunkMapPoint))
                chunkMapPoint.value = newChunkMapPoint;
            else
                chunkMapPoint.value = null;
        }
        else
        {
            chunkMapPoint.value = null;
            meshRenderer.enabled = false;
        }

    }

    private void OnValueChanged_ChunkMapPoint(ChunkMapPoint oldValue, ChunkMapPoint newValue)
    {
        if (oldValue != null)
            oldValue.blocked.onValueChange -= OnValueChanged_ChunkMapPoint_Blocked;

        if (newValue != null)
            newValue.blocked.onValueChange += OnValueChanged_ChunkMapPoint_Blocked;

        SetViableStatus();
    }    

    private void OnValueChanged_ChunkMapPoint_Blocked(bool oldValue, bool newValue)
    {
        SetViableStatus();
    }

    private void SetViableStatus()
    {
        if(position.value == null)
        {
            viable.value = false;
            return;
        }

        if(chunkMapPoint.value == null)
        {
            viable.value = false;
            return;
        }

        if (chunkMapPoint.value.blocked.value)
        {
            viable.value = false;
            return;
        }

        viable.value = true;
    }

    private void OnValueChanged_Viable(bool oldValue, bool newValue)
    {
        meshRenderer.sharedMaterial = newValue ? viableMaterial : notViableMaterial;
    }

    private void OnDestroy()
    {
        chunkMapPoint.onValueChangeImmediate -= OnValueChanged_ChunkMapPoint;
        position.onValueChange -= OnValueChanged_Position;
        enterExitDirection.onValueChange -= OnValueChanged_EnterExitPosition;
        viable.onValueChange -= OnValueChanged_Viable;
    }
}
