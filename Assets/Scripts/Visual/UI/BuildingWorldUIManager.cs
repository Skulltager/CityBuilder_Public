
using System.Collections.Generic;
using UnityEngine;

public class BuildingWorldUIManager : MonoBehaviour
{
    public static BuildingWorldUIManager instance { private set; get; }

    [SerializeField] private BuildingWorldUI_Construction constructionPrefab;
    [SerializeField] private BuildingWorldUI_Deconstruction deconstructionPrefab;
    [SerializeField] private BuildingWorldUI_CraftingStation craftingStationPrefab;
    [SerializeField] private RectTransform container;

    private readonly List<BuildingWorldUI_Construction> constructionInstances;
    private readonly List<BuildingWorldUI_Deconstruction> deconstructionInstances;
    private readonly List<BuildingWorldUI_CraftingStation> craftingStationInstances;

    private BuildingWorldUIManager()
    {
        constructionInstances = new List<BuildingWorldUI_Construction>();
        deconstructionInstances = new List<BuildingWorldUI_Deconstruction>();
        craftingStationInstances = new List<BuildingWorldUI_CraftingStation>();
    }

    private void Awake()
    {
        instance = this;
    }

    public void Show(ChunkMapPointContent_BuildingBase building)
    {
        switch(building)
        {
            case ChunkMapPointContent_BuildingConstruction construction:
                Show_Construction(construction);
                break;
            case ChunkMapPointContent_BuildingDeconstruction deconstruction:
                Show_Deconstruction(deconstruction);
                break;
            case ChunkMapPointContent_Building_CraftingStation craftingStation:
                Show_CraftingStation(craftingStation);
                break;
        }
    }

    public void Hide(ChunkMapPointContent_BuildingBase building)
    {
        switch (building)
        {
            case ChunkMapPointContent_BuildingConstruction construction:
                Hide_Construction(construction);
                break;
            case ChunkMapPointContent_BuildingDeconstruction deconstruction:
                Hide_Deconstruction(deconstruction);
                break;
            case ChunkMapPointContent_Building_CraftingStation craftingStation:
                Hide_CraftingStation(craftingStation);
                break;
        }
    }

    private void Show_Construction(ChunkMapPointContent_BuildingConstruction building)
    {
        BuildingWorldUI_Construction instance = GameObject.Instantiate(constructionPrefab, container);
        instance.data = building;
        constructionInstances.Add(instance);
    }

    private void Show_Deconstruction(ChunkMapPointContent_BuildingDeconstruction building)
    {
        BuildingWorldUI_Deconstruction instance = GameObject.Instantiate(deconstructionPrefab, container);
        instance.data = building;
        deconstructionInstances.Add(instance);
    }

    private void Show_CraftingStation(ChunkMapPointContent_Building_CraftingStation building)
    {
        BuildingWorldUI_CraftingStation instance = GameObject.Instantiate(craftingStationPrefab, container);
        instance.data = building;
        craftingStationInstances.Add(instance);
    }

    private void Hide_Construction(ChunkMapPointContent_BuildingConstruction building)
    {
        BuildingWorldUI_Construction instance = constructionInstances.Find(i => i.data == building);
        if (constructionInstances.Remove(instance))
            GameObject.Destroy(instance.gameObject);
    }

    private void Hide_Deconstruction(ChunkMapPointContent_BuildingDeconstruction building)
    {
        BuildingWorldUI_Deconstruction instance = deconstructionInstances.Find(i => i.data == building);
        if (deconstructionInstances.Remove(instance))
            GameObject.Destroy(instance.gameObject);
    }

    private void Hide_CraftingStation(ChunkMapPointContent_Building_CraftingStation building)
    {
        BuildingWorldUI_CraftingStation instance = craftingStationInstances.Find(i => i.data == building);
        if (craftingStationInstances.Remove(instance))
            GameObject.Destroy(instance.gameObject);
    }

    private void OnDestroy()
    {
        constructionInstances.Clear();
        deconstructionInstances.Clear();
        craftingStationInstances.Clear();
    }
}