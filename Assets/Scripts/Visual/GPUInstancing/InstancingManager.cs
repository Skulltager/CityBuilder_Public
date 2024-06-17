
using SheetCodes;
using System;
using System.Collections.Generic;
using UnityEngine;

public class InstancingManager : MonoBehaviour
{
    public static InstancingManager instance { private set; get; }

    private InstancedBlocker instancedBlocker;
    private readonly Dictionary<WorldResourceIdentifier, InstancedWorldResource[]> instancedWorldResources;

    private InstancingManager()
    {
        instancedWorldResources = new Dictionary<WorldResourceIdentifier, InstancedWorldResource[]>();
    }

    private void Awake()
    {
        instance = this;
        instancedBlocker = new InstancedBlocker();
        WorldResourceIdentifier[] identifiers = Enum.GetValues(typeof(WorldResourceIdentifier)) as WorldResourceIdentifier[];
        foreach(WorldResourceIdentifier identifier in identifiers)
        {
            if (identifier == WorldResourceIdentifier.None)
                continue;

            WorldResourceRecord record = identifier.GetRecord();
            InstancedWorldResource[] instancedWorldResourcesArray = new InstancedWorldResource[record.ModelVariations.Length];
            for(int i = 0; i < instancedWorldResourcesArray.Length; i++)
                instancedWorldResourcesArray[i] = new InstancedWorldResource(record, i);

            instancedWorldResources.Add(identifier, instancedWorldResourcesArray);
        }
    }

    public void AddInstancingItem(ChunkMapPointContent_WorldResource worldResource)
    {
        InstancedWorldResource[] instancedWorldResourcesArray = instancedWorldResources[worldResource.record.Identifier];
        instancedWorldResourcesArray[worldResource.modelIndex].AddWorldResource(worldResource);
    }

    public void AddInstancingItem(ChunkMapPointContent_Blocker blocker)
    {
        instancedBlocker.AddBlocker(blocker);
    }

    public void RemoveInstancingItem(ChunkMapPointContent_WorldResource worldResource)
    {
        InstancedWorldResource[] instancedWorldResourcesArray = instancedWorldResources[worldResource.record.Identifier];
        instancedWorldResourcesArray[worldResource.modelIndex].RemoveWorldResource(worldResource);
    }

    public void RemoveInstancingItem(ChunkMapPointContent_Blocker blocker)
    {
        instancedBlocker.RemoveBlocker(blocker);
    }

    private void LateUpdate()
    {
        foreach(InstancedWorldResource[] instancedWorldResourceArray in instancedWorldResources.Values)
            foreach (InstancedWorldResource instancedWorldResource in instancedWorldResourceArray)
                instancedWorldResource.Draw();

        instancedBlocker.Draw();
    }

    private void OnDestroy()
    {
        foreach (InstancedWorldResource[] instancedWorldResourceArray in instancedWorldResources.Values)
            foreach (InstancedWorldResource instancedWorldResource in instancedWorldResourceArray)
                instancedWorldResource.Dispose();

        instancedBlocker.Dispose();
    }
}
