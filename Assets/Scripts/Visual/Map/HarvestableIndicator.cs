using UnityEngine;

public class HarvestableIndicatorData
{
    public readonly Player player;
    public readonly ChunkMapPoint chunkMapPoint;
    public readonly ResourceTypeFlags harvestableTypeFlags;

    public HarvestableIndicatorData(Player player, ChunkMapPoint chunkMapPoint, ResourceTypeFlags harvestableTypeFlags)
    {
        this.player = player;
        this.chunkMapPoint = chunkMapPoint;
        this.harvestableTypeFlags = harvestableTypeFlags;
    }
}

public class HarvestableIndicator : DataDrivenBehaviour<HarvestableIndicatorData>
{
    [SerializeField] private Material noResourceMaterial;
    [SerializeField] private Material resourceMaterial;
    [SerializeField] private MeshRenderer meshRenderer;

    protected override void OnValueChanged_Data(HarvestableIndicatorData oldValue, HarvestableIndicatorData newValue)
    {
        if (oldValue != null)
        {
            oldValue.chunkMapPoint.chunkRegion.onValueChange -= OnValueChanged_ChunkMapPoint_ChunkRegion;
            oldValue.chunkMapPoint.content.onValueChange -= OnValueChanged_ChunkMapPoint_Content;
        }

        if (newValue != null)
        {
            transform.position = new Vector3(newValue.chunkMapPoint.globalPoint.xIndex + 0.5f, 0, newValue.chunkMapPoint.globalPoint.yIndex + 0.5f);
            newValue.chunkMapPoint.chunkRegion.onValueChange += OnValueChanged_ChunkMapPoint_ChunkRegion;
            newValue.chunkMapPoint.content.onValueChange += OnValueChanged_ChunkMapPoint_Content;
        }

        SetIndicatorState();
    }

    private void OnValueChanged_ChunkMapPoint_ChunkRegion(ChunkRegion oldValue, ChunkRegion newValue)
    {
        SetIndicatorState();
    }

    private void OnValueChanged_ChunkMapPoint_Content(ChunkMapPointContent oldValue, ChunkMapPointContent newValue)
    {
        SetIndicatorState();
    }

    private void SetIndicatorState()
    {
        if (data == null)
        {
            meshRenderer.enabled = false;
            return;
        }

        if (data.chunkMapPoint.chunkRegion.value.owner != data.player)
        {
            meshRenderer.enabled = false;
            return;
        }

        if (data.chunkMapPoint.content.value == null)
        {
            meshRenderer.material = noResourceMaterial;
            meshRenderer.enabled = true;
            return;
        }

        ChunkMapPointContent_WorldResource worldResource = data.chunkMapPoint.content.value as ChunkMapPointContent_WorldResource;
        if (worldResource == null)
        {
            meshRenderer.enabled = false;
            return;
        }

        if (!worldResource.record.ResourceType.MatchesFlags(data.harvestableTypeFlags))
        {
            meshRenderer.material = noResourceMaterial;
            meshRenderer.enabled = true;
            return;
        }

        meshRenderer.material = resourceMaterial;
        meshRenderer.enabled = true;
        return;
    }
}