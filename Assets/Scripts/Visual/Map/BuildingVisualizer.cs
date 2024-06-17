using System.Collections.Generic;
using UnityEngine;

public class BuildingVisualizer : MonoBehaviour
{
    public static BuildingVisualizer instance { private set; get; }

    [SerializeField] private Transform buildingInstanceContainer;

    private readonly List<BuildingVisual> buildingInstances;
    private readonly List<BuildingConstructionVisual> buildingConstructionInstances;
    private readonly List<BuildingDeconstructionVisual> buildingDeconstructionInstances;

    public BuildingVisualizer()
    {
        buildingInstances = new List<BuildingVisual>();
        buildingConstructionInstances = new List<BuildingConstructionVisual>();
        buildingDeconstructionInstances = new List<BuildingDeconstructionVisual>();
    }

    private void Awake()
    {
        instance = this;
    }

    public void Show(ChunkMapPointContent_BuildingBase building)
    {
        switch (building)
        {
            case ChunkMapPointContent_BuildingConstruction construction:
                Show(construction);
                break;
            case ChunkMapPointContent_BuildingDeconstruction deconstruction:
                Show(deconstruction);
                break;
            case ChunkMapPointContent_Building craftingStation:
                Show(craftingStation);
                break;
        }
    }

    private void Show(ChunkMapPointContent_Building building)
    {
        BuildingVisual instance = GameObject.Instantiate(building.record.BuildingPrefab, buildingInstanceContainer);
        instance.data = building;
        buildingInstances.Add(instance);
    }

    private void Show(ChunkMapPointContent_BuildingConstruction construction)
    {
        BuildingConstructionVisual instance = GameObject.Instantiate(construction.record.BuildingConstructionPrefab, buildingInstanceContainer);
        instance.data = construction;
        buildingConstructionInstances.Add(instance);
    }

    private void Show(ChunkMapPointContent_BuildingDeconstruction deconstruction)
    {
        BuildingDeconstructionVisual instance = GameObject.Instantiate(deconstruction.record.BuildingDeconstructionPrefab, buildingInstanceContainer);
        instance.data = deconstruction;
        buildingDeconstructionInstances.Add(instance);
    }

    public void Hide(ChunkMapPointContent_BuildingBase building)
    {
        switch (building)
        {
            case ChunkMapPointContent_BuildingConstruction construction:
                Hide(construction);
                break;
            case ChunkMapPointContent_BuildingDeconstruction deconstruction:
                Hide(deconstruction);
                break;
            case ChunkMapPointContent_Building craftingStation:
                Hide(craftingStation);
                break;
        }
    }

    private void Hide(ChunkMapPointContent_Building building)
    {
        BuildingVisual instance = buildingInstances.Find(i => i.data == building);
        if (buildingInstances.Remove(instance))
            GameObject.Destroy(instance.gameObject);
    }

    private void Hide(ChunkMapPointContent_BuildingConstruction construction)
    {
        BuildingConstructionVisual instance = buildingConstructionInstances.Find(i => i.data == construction);
        if (buildingConstructionInstances.Remove(instance))
            GameObject.Destroy(instance.gameObject);
    }

    private void Hide(ChunkMapPointContent_BuildingDeconstruction deconstruction)
    {
        BuildingDeconstructionVisual instance = buildingDeconstructionInstances.Find(i => i.data == deconstruction);
        if (buildingDeconstructionInstances.Remove(instance))
            GameObject.Destroy(instance.gameObject);
    }

    private void OnDestroy()
    {
        buildingInstances.Clear();
        buildingConstructionInstances.Clear();
        buildingDeconstructionInstances.Clear();
    }
}