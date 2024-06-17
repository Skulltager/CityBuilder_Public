
using UnityEngine;

public class BuildingIndicatorData
{
    public readonly Player player;
    public readonly ChunkMapPoint chunkMapPoint;

    public BuildingIndicatorData(Player player, ChunkMapPoint chunkMapPoint)
    {
        this.player = player;
        this.chunkMapPoint = chunkMapPoint;
    }
}

public class BuildingIndicator : DataDrivenBehaviour<BuildingIndicatorData>
{
    [SerializeField] private Material viableMaterial;
    [SerializeField] private Material notViableMaterial;
    [SerializeField] private MeshRenderer meshRenderer;

    public readonly EventVariable<BuildingIndicator, bool> viable;

    private BuildingIndicator()
        : base()
    {
        viable = new EventVariable<BuildingIndicator, bool>(this, default);
    }

    private void Awake()
    {
        viable.onValueChangeImmediate += OnValueChanged_Viable;
    }

    protected override void OnValueChanged_Data(BuildingIndicatorData oldValue, BuildingIndicatorData newValue)
    {
        if (oldValue != null)
        {
            oldValue.chunkMapPoint.canBeBlocked.onValueChange -= OnValueChanged_ChunkMapPoint_CanBeBlocked;
            oldValue.chunkMapPoint.chunkRegion.onValueChange -= OnValueChanged_ChunkMapPoint_ChunkRegion;
        }

        if (newValue != null)
        {
            transform.position = new Vector3(newValue.chunkMapPoint.globalPoint.xIndex + 0.5f, 0, newValue.chunkMapPoint.globalPoint.yIndex + 0.5f);
            newValue.chunkMapPoint.canBeBlocked.onValueChange += OnValueChanged_ChunkMapPoint_CanBeBlocked;
            newValue.chunkMapPoint.chunkRegion.onValueChange += OnValueChanged_ChunkMapPoint_ChunkRegion;
            meshRenderer.enabled = true;
        }
        else
        {
            meshRenderer.enabled = false;
        }

        SetViableStatus();
    }

    private void OnValueChanged_ChunkMapPoint_CanBeBlocked(bool oldValue, bool newValue)
    {
        SetViableStatus();
    }

    private void OnValueChanged_ChunkMapPoint_ChunkRegion(ChunkRegion oldValue, ChunkRegion newValue)
    {
        SetViableStatus();
    }

    private void SetViableStatus()
    {
        if (data == null)
        {
            viable.value = false;
            return;
        }

        if(data.chunkMapPoint.chunkRegion.value != data.player.playerRegion)
        {
            viable.value = false;
            return;
        }

        if (!data.chunkMapPoint.canBeBlocked.value)
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
        viable.onValueChange -= OnValueChanged_Viable;
    }
}