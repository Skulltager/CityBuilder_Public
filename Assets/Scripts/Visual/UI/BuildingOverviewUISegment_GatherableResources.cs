
using SheetCodes;
using System.Collections.Generic;
using UnityEngine;

public class BuildingOverviewUISegment_GatherableResources : DataDrivenUI<EventDictionary<ResourcesIdentifier, ResourceCount>>
{
    [SerializeField] private BuildingOverviewUISegment_GatherableResourceItem gatherableResourcePrefab;
    [SerializeField] private RectTransform gatherableResourceContainer;

    private readonly List<BuildingOverviewUISegment_GatherableResourceItem> gatherableResourceInstances;

    private BuildingOverviewUISegment_GatherableResources()
        :base()
    {
        gatherableResourceInstances = new List<BuildingOverviewUISegment_GatherableResourceItem>();
    }

    protected override void OnValueChanged_Data(EventDictionary<ResourcesIdentifier, ResourceCount> oldValue, EventDictionary<ResourcesIdentifier, ResourceCount> newValue)
    {
        if (oldValue != null)
        {
            foreach (BuildingOverviewUISegment_GatherableResourceItem instance in gatherableResourceInstances)
                GameObject.Destroy(instance.gameObject);

            gatherableResourceInstances.Clear();
            oldValue.onAdd -= OnAdd_GatherableResource;
            oldValue.onRemove -= OnRemove_GatherableResource;
        }
        if (newValue != null)
        {
            newValue.onAdd += OnAdd_GatherableResource;
            newValue.onRemove += OnRemove_GatherableResource;

            foreach (KeyValuePair<ResourcesIdentifier, ResourceCount> item in data)
                OnAdd_GatherableResource(item.Key, item.Value);
        }
    }

    private void OnAdd_GatherableResource(ResourcesIdentifier identifier, ResourceCount item)
    {
        BuildingOverviewUISegment_GatherableResourceItem instance = GameObject.Instantiate(gatherableResourcePrefab, gatherableResourceContainer);
        instance.data = item;
        gatherableResourceInstances.Add(instance);
    }

    private void OnRemove_GatherableResource(ResourcesIdentifier identifier, ResourceCount item)
    {
        BuildingOverviewUISegment_GatherableResourceItem instance = gatherableResourceInstances.Find(i => i.data == item);
        gatherableResourceInstances.Remove(instance);
        GameObject.Destroy(instance.gameObject);
    }
}
