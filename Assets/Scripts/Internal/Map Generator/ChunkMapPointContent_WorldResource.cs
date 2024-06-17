using SheetCodes;
using System;
using UnityEngine;

public class ChunkMapPointContent_WorldResource : ChunkMapPointContent
{
    public override string name => record.Name;
    public override ChunkMapPointContentType contentType => ChunkMapPointContentType.WorldResource;
    public override ContentTaskLocation contentTaskLocation => ContentTaskLocation.Edge;
    public readonly WorldResourceRecord record;
    public readonly EventVariable<ChunkMapPointContent_WorldResource, int> maximumTimesHarvestable;
    public readonly EventVariable<ChunkMapPointContent_WorldResource, int> timesHarvestableRemaining;
    public readonly int modelIndex;
    public readonly Vector3 position;
    public readonly Quaternion rotation;
    public readonly ChunkMapPoint chunkMapPoint;

    public bool isInstanced;
    public uint instancedIndex;

    public ChunkMapPointContent_WorldResource(Map map, ChunkMapPoint chunkMapPoint, WorldResourceRecord record)
        : base(map, chunkMapPoint.globalPoint, CardinalDirection.Right, new WorldContentSize(1, 1))
    {
        this.record = record;
        this.chunkMapPoint = chunkMapPoint;
        modelIndex = UnityEngine.Random.Range(0, record.ModelVariations.Length);
        maximumTimesHarvestable = new EventVariable<ChunkMapPointContent_WorldResource, int>(this, record.TimesHarvestable);
        timesHarvestableRemaining = new EventVariable<ChunkMapPointContent_WorldResource, int>(this, record.TimesHarvestable);
        timesHarvestableRemaining.onValueChange += OnValueChanged_TimesHarvestableRemaining;

        position = new Vector3(pivotPoint.xIndex + 0.5f, 0, pivotPoint.yIndex + 0.5f);
        rotation = Quaternion.Euler(0, UnityEngine.Random.Range(0f, 360f), 0);
    }

    protected override void InitializeSub()
    {
        base.InitializeSub();
        SetVillagerTaskPoints_AroundEdge();
    }

    private void OnValueChanged_TimesHarvestableRemaining(int oldValue, int newValue)
    {
        if (newValue == 0)
        {
            timesHarvestableRemaining.onValueChange -= OnValueChanged_TimesHarvestableRemaining;
            RemoveFromMap();
        }
    }

    public override void Hide()
    {
        InstancingManager.instance.RemoveInstancingItem(this);
    }

    public override void Show()
    {
        InstancingManager.instance.AddInstancingItem(this);
    }
}