using SheetCodes;
using System.Collections.Generic;

public class ChunkMapPointContent_Building_GatheringHut : ChunkMapPointContent_Building
{
    public readonly List<ChunkMapPoint> gatheringPoints;
    public readonly List<ChunkMapPointContent_WorldResource> worldResourcesInRange;
    public readonly BuildingTask_GatheringHut villagerTask;
    public readonly EventDictionary<ResourcesIdentifier, ResourceCount> gatherableResources;
    
    public ChunkMapPointContent_Building_GatheringHut(Map map, Point centerPoint, CardinalDirection direction, BuildingRecord record, Player owner, ChunkMapPointContent_BuildingConstruction construction) 
        : base(map, centerPoint, direction, record, owner, construction)
    {
        gatheringPoints = new List<ChunkMapPoint>();
        villagerTask = new BuildingTask_GatheringHut(this, 2, 2);
        worldResourcesInRange = new List<ChunkMapPointContent_WorldResource>();
        gatherableResources = new EventDictionary<ResourcesIdentifier, ResourceCount>();
    }

    protected override void InitializeSub()
    {
        base.InitializeSub();

        WorldContentSizeBounds sizeBounds = size.GetSizeBounds(direction);

        int gatheringRange = record.GatheringRange;
        for (int x = -gatheringRange; x < sizeBounds.width + gatheringRange; x++)
        {
            int distance;
            if (x < 0)
                distance = gatheringRange + x;
            else if (x >= sizeBounds.width)
                distance = gatheringRange - (x - sizeBounds.width) - 1;
            else
                distance = gatheringRange;

            for (int y = -distance; y < sizeBounds.height + distance; y++)
            {
                if (x >= 0 && x < sizeBounds.width && y >= 0 && y < sizeBounds.height)
                    continue;

                gatheringPoints.Add(map.GetChunkMapPoint(x + sizeBounds.xOffset + pivotPoint.xIndex, y + sizeBounds.yOffset + pivotPoint.yIndex));
            }
        }
    }

    public override void PlaceOnMap()
    {
        base.PlaceOnMap();
        foreach(ChunkMapPoint chunkMapPoint in gatheringPoints)
            chunkMapPoint.chunkRegion.onValueChangeImmediateSource += OnValueChanged_ChunkMapPoint_ChunkRegion;
    }

    public override void RemoveFromMap(ChunkMapPointContent replacement = null)
    {
        base.RemoveFromMap(replacement);
        foreach (ChunkMapPoint chunkMapPoint in gatheringPoints)
            chunkMapPoint.chunkRegion.onValueChangeImmediateSource -= OnValueChanged_ChunkMapPoint_ChunkRegion;
    }

    public override bool TryAssignVillagerTasks(Villager villager)
    {
        return villagerTask.TryReserveVillagerTaskData(villager);
    }

    private void OnValueChanged_ChunkMapPoint_ChunkRegion(ChunkMapPoint source, ChunkRegion oldValue, ChunkRegion newValue)
    {
        if (oldValue != null && oldValue.owner == owner)
        {
            source.content.onValueChangeImmediateSource -= OnValueChanged_ChunkMapPoint_Content;
        }

        if (newValue != null && newValue.owner == owner)
        {
            source.content.onValueChangeImmediateSource += OnValueChanged_ChunkMapPoint_Content;
        }
    }

    private void OnValueChanged_ChunkMapPoint_Content(ChunkMapPoint source, ChunkMapPointContent oldValue, ChunkMapPointContent newValue)
    {
        if (oldValue != null && oldValue is ChunkMapPointContent_WorldResource oldValueCast && oldValueCast.record.ResourceType.MatchesFlags(record.GatherableType))
        {
            oldValueCast.timesHarvestableRemaining.onValueChangeSource -= OnValueChanged_WorldResource_HarvestableTimeRemaining;
            contentRelations.Remove(contentRelations.Find(i => i.content == oldValue));
            worldResourcesInRange.Remove(oldValueCast);
            foreach(VillagerTaskPoint taskPoint in oldValueCast.villagerTaskPoints)
                villagerTask.worldResourceTaskPoints.Remove(taskPoint);
        }

        if (newValue != null && newValue is ChunkMapPointContent_WorldResource newValueCast && newValueCast.record.ResourceType.MatchesFlags(record.GatherableType))
        {
            newValueCast.timesHarvestableRemaining.onValueChangeImmediateSource += OnValueChanged_WorldResource_HarvestableTimeRemaining;
            contentRelations.Add(new ContentRelation(newValue, ContentRelationType.GatherableResource));
            worldResourcesInRange.Add(newValueCast);
            villagerTask.worldResourceTaskPoints.AddRange(newValue.villagerTaskPoints);
        }
    }

    private void OnValueChanged_WorldResource_HarvestableTimeRemaining(ChunkMapPointContent_WorldResource source, int oldValue, int newValue)
    {
        ResourceCount resourceCount;
        if (!gatherableResources.TryGetValue(source.record.ResourceDrops.Identifier, out resourceCount))
        {
            resourceCount = new ResourceCount(source.record.ResourceDrops, 0);
            gatherableResources.Add(source.record.ResourceDrops.Identifier, resourceCount);
        }
        resourceCount.amount.value += newValue - oldValue;
    }
}