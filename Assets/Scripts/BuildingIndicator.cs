
using UnityEngine;

public class BuildingIndicator : MonoBehaviour
{
    [SerializeField] private Material viableMaterial;
    [SerializeField] private Material notViableMaterial;
    [SerializeField] private MeshRenderer meshRenderer;

    private ChunkMap chunkMap;
    public readonly EventVariable<BuildingIndicator, Point> position;
    private readonly EventVariable<BuildingIndicator, bool> viable;
    private readonly EventVariable<BuildingIndicator, ChunkMapPoint> chunkMapPoint;

    public bool isViable => viable.value;

    private BuildingIndicator()
    {
        position = new EventVariable<BuildingIndicator, Point>(this, default);
        viable = new EventVariable<BuildingIndicator, bool>(this, default);
        chunkMapPoint = new EventVariable<BuildingIndicator, ChunkMapPoint>(this, default);
    }

    public void Initialize(ChunkMap chunkMap)
    {
        this.chunkMap = chunkMap;
        position.onValueChange += OnValueChanged_Position;
        chunkMapPoint.onValueChange += OnValueChanged_ChunkMapPoint;
        viable.onValueChangeImmediate += OnValueChanged_Viable;
    }

    private void OnValueChanged_Position(Point oldValue, Point newValue)
    {
        if (newValue != null)
        {
            transform.position = new Vector3(newValue.xIndex + 0.5f, 0, newValue.yIndex + 0.5f);
            meshRenderer.enabled = true;
            ChunkMapPoint newChunkMapPoint;
            if (chunkMap.TryGetChunkMapPoint(position.value, out newChunkMapPoint))
                chunkMapPoint.value = newChunkMapPoint;
            else
                chunkMapPoint.value = null;
        }
        else
        {
            meshRenderer.enabled = false;
            chunkMapPoint.value = null;
        }
    }

    private void OnValueChanged_ChunkMapPoint(ChunkMapPoint oldValue, ChunkMapPoint newValue)
    {
        if (oldValue != null)
            oldValue.canBeBlocked.onValueChange -= OnValueChanged_ChunkMapPoint_CanBeBlocked;

        if (newValue != null)
            newValue.canBeBlocked.onValueChange += OnValueChanged_ChunkMapPoint_CanBeBlocked;

        SetViableStatus();
    }

    private void OnValueChanged_ChunkMapPoint_CanBeBlocked(bool oldValue, bool newValue)
    {
        SetViableStatus();
    }

    private void SetViableStatus()
    {
        if (chunkMapPoint.value == null)
        {
            viable.value = false;
            return;
        }

        if (!chunkMapPoint.value.canBeBlocked.value)
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
        viable.onValueChange -= OnValueChanged_Viable;
    }
}