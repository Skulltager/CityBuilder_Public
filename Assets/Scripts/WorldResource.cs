using SheetCodes;
using UnityEngine;

public class WorldResource
{
    public readonly WorldResourceRecord record;
    public readonly EventVariable<WorldResource, int> maximumTimesHarvestable;
    public readonly EventVariable<WorldResource, int> timesHarvestableRemaining;
    public readonly int modelIndex;
    public readonly float modelRotation;

    public WorldResource(WorldResourceRecord record, int timesHarvestable)
    {
        this.record = record;
        modelIndex = Random.Range(0, record.ModelVariations.Length);
        modelRotation = Random.Range(0f, 360f);
        maximumTimesHarvestable = new EventVariable<WorldResource, int>(this, timesHarvestable);
        timesHarvestableRemaining = new EventVariable<WorldResource, int>(this, timesHarvestable);
    }
}