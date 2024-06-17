using SheetCodes;
using System;
using System.Collections.Generic;
using UnityEngine;

public class BuildingOverviewUIManager : DataDrivenUI<ChunkMapPointContent>
{
    public static BuildingOverviewUIManager instance { private set; get; }

    [SerializeField] private BuildingOverviewUISegment_AssignedVillagers assignedVillagersSegmentPrefab;
    [SerializeField] private BuildingOverviewUISegment_AssignedVillagersAdjustable assignedVillagersAdjustableSegmentPrefab;
    [SerializeField] private BuildingOverviewUISegment_Button buttonSegmentPrefab;
    [SerializeField] private BuildingOverviewUISegment_DeconstructionProgress deconstructionProgressSegmentPrefab;
    [SerializeField] private BuildingOverviewUISegment_ConstructionProgress constructionProgressSegmentPrefab;
    [SerializeField] private BuildingOverviewUISegment_Inventory inventorySegmentPrefab;
    [SerializeField] private BuildingOverviewUISegment_InventoryWithCapacity inventoryWithCapacitySegmentPrefab;
    [SerializeField] private BuildingOverviewUISegment_InventoryAdjustable inventoryAdjustableSegmentPrefab;
    [SerializeField] private BuildingOverviewUISegment_GatherableResources gatherableResourcesSegmentPrefab;
    [SerializeField] private BuildingOverviewUISegment_RecipeDisplay recipeDisplaySegmentPrefab;
    [SerializeField] private BuildingOverviewUISegment_ResourcesLeft resourcesLeftSegmentPrefab;
    [SerializeField] private BuildingOverviewUISegment_TaskPriority taskPrioritySegmentPrefab;
    [SerializeField] private BuildingOverviewUISegment_Text titleTextSegmentPrefab;
    [SerializeField] private BuildingOverviewUISegment_Text textSegmentPrefab;
    [SerializeField] private BuildingOverviewUISegment_Line lineSegmentPrefab;
    [SerializeField] private CanvasGroup canvasGroup;

    [SerializeField] private RectTransform segmentContainer;

    private readonly List<GameObject> segmentInstances;

    private BuildingOverviewUIManager()
        : base()
    {
        segmentInstances = new List<GameObject>();
    }

    private void Awake()
    {
        instance = this;
    }

    protected override void OnValueChanged_Data(ChunkMapPointContent oldValue, ChunkMapPointContent newValue)
    {
        if (oldValue != null)
        {
            oldValue.onRemovedFromMap -= OnEvent_BuildingRemovedFromMap;
            ClearSegments();
        }

        if (newValue != null)
        {
            switch (newValue)
            {
                case ChunkMapPointContent_Building_TownCenter townCenter:
                    CreateSegments_TownCenter(townCenter);
                    break;
                case ChunkMapPointContent_Building_Warehouse warehouse:
                    CreateSegments_Warehouse(warehouse);
                    break;
                case ChunkMapPointContent_BuildingDeconstruction deconstruction:
                    CreateSegments_Deconstruction(deconstruction);
                    break;
                case ChunkMapPointContent_BuildingConstruction construction:
                    CreateSegments_Construction(construction);
                    break;
                case ChunkMapPointContent_WorldResource worldResource:
                    CreateSegments_WorldResource(worldResource);
                    break;
                case ChunkMapPointContent_Building_GatheringHut gatheringHut:
                    CreateSegments_GatheringHut(gatheringHut);
                    break;
                case ChunkMapPointContent_Building_CraftingStation craftingStation:
                    CreateSegments_CraftingStation(craftingStation);
                    break;
            }
            canvasGroup.Show();
            newValue.onRemovedFromMap += OnEvent_BuildingRemovedFromMap;
        }
        else
        {
            canvasGroup.Hide();
        }
    }

    private void OnEvent_BuildingRemovedFromMap(ChunkMapPointContent replacement)
    {
        data = replacement;
    }

    private void CreateSegments_TownCenter(ChunkMapPointContent_Building_TownCenter content)
    {
        AddSegment_TitleText(content.record.Name);
        AddSegment_Line();
        AddSegment_AssignedVillagers(content);
        AddSegment_Line();
        AddSegment_Inventory(content.inventory);
    }

    private void CreateSegments_GatheringHut(ChunkMapPointContent_Building_GatheringHut content)
    {
        AddSegment_TitleText(content.record.Name);
        AddSegment_Line();
        AddSegment_AssignedVillagersAdjustable(content);
        AddSegment_Line();
        AddSegment_GatherableResources(content.gatherableResources);
        AddSegment_Line();
        AddSegment_Inventory(content.inventory);
    }

    private void CreateSegments_WorldResource(ChunkMapPointContent_WorldResource content)
    {
        AddSegment_TitleText(content.record.Name);
        AddSegment_Line();
        AddSegment_ResourcesLeft(content);
    }

    private void CreateSegments_Warehouse(ChunkMapPointContent_Building_Warehouse content)
    {
        AddSegment_TitleText(content.record.Name);
        AddSegment_Line();
        AddSegment_AssignedVillagersAdjustable(content);
        AddSegment_Line();
        AddSegment_InventoryAdjustable(content.inventory);
    }

    private void CreateSegments_CraftingStation(ChunkMapPointContent_Building_CraftingStation content)
    {
        AddSegment_TitleText(content.record.Name);
        AddSegment_Line();
        AddSegment_AssignedVillagersAdjustable(content);
        AddSegment_Line();
        AddSegment_RecipeDisplay(content);
        AddSegment_Line();
        AddSegment_InventoryWithCapacity(content.inventory as Inventory_Limited);
    }

    private void CreateSegments_Construction(ChunkMapPointContent_BuildingConstruction content)
    {
        AddSegment_TitleText(string.Format("{0}({1})", content.record.Name, "construction"));
        AddSegment_Line();
        AddSegment_ConstructionProgress(content);
        AddSegment_Line();
        AddSegment_TaskPriority(content.buildingTask);
        AddSegment_Line();
        AddSegment_Inventory(content.inventory);
    }

    private void CreateSegments_Deconstruction(ChunkMapPointContent_BuildingDeconstruction content)
    {
        AddSegment_TitleText(string.Format("{0}({1})", content.record.Name, "deconstruction"));
        AddSegment_Line();
        AddSegment_DeconstructionProgress(content);
        AddSegment_Line();
        AddSegment_TaskPriority(content.buildingTask);
        AddSegment_Line();
        AddSegment_Inventory(content.inventory);
        AddSegment_Line();
        AddSegment_Button(content.CancelDeconstruction, "Cancel Deconstruction");
    }

    #region Segments
    private void ClearSegments()
    {
        foreach(GameObject instance in segmentInstances)
            GameObject.Destroy(instance);

        segmentInstances.Clear();
    }

    private void AddSegment_AssignedVillagers(ChunkMapPointContent_Building building)
    {
        BuildingOverviewUISegment_AssignedVillagers instance = GameObject.Instantiate(assignedVillagersSegmentPrefab, segmentContainer);
        instance.data = building;
        segmentInstances.Add(instance.gameObject);
    }

    private void AddSegment_AssignedVillagersAdjustable(ChunkMapPointContent_Building building)
    {
        BuildingOverviewUISegment_AssignedVillagersAdjustable instance = GameObject.Instantiate(assignedVillagersAdjustableSegmentPrefab, segmentContainer);
        instance.data = building;
        segmentInstances.Add(instance.gameObject);
    }

    private void AddSegment_Button(Action callback, string buttonText)
    {
        BuildingOverviewUISegment_Button instance = GameObject.Instantiate(buttonSegmentPrefab, segmentContainer);
        instance.data = new BuildingOverviewUISegment_ButtonData(callback, buttonText);
        segmentInstances.Add(instance.gameObject);
    }

    private void AddSegment_ConstructionProgress(ChunkMapPointContent_BuildingConstruction construction)
    {
        BuildingOverviewUISegment_ConstructionProgress instance = GameObject.Instantiate(constructionProgressSegmentPrefab, segmentContainer);
        instance.data = construction;
        segmentInstances.Add(instance.gameObject);
    }

    private void AddSegment_DeconstructionProgress(ChunkMapPointContent_BuildingDeconstruction deconstruction)
    {
        BuildingOverviewUISegment_DeconstructionProgress instance = GameObject.Instantiate(deconstructionProgressSegmentPrefab, segmentContainer);
        instance.data = deconstruction;
        segmentInstances.Add(instance.gameObject);
    }

    private void AddSegment_Inventory(Inventory inventory)
    {
        BuildingOverviewUISegment_Inventory instance = GameObject.Instantiate(inventorySegmentPrefab, segmentContainer);
        instance.data = inventory;
        segmentInstances.Add(instance.gameObject);
    }

    private void AddSegment_InventoryWithCapacity(Inventory_Limited inventory)
    {
        BuildingOverviewUISegment_InventoryWithCapacity instance = GameObject.Instantiate(inventoryWithCapacitySegmentPrefab, segmentContainer);
        instance.data = inventory;
        segmentInstances.Add(instance.gameObject);
    }

    private void AddSegment_InventoryAdjustable(Inventory_Limited inventory)
    {
        BuildingOverviewUISegment_InventoryAdjustable instance = GameObject.Instantiate(inventoryAdjustableSegmentPrefab, segmentContainer);
        instance.data = inventory;
        segmentInstances.Add(instance.gameObject);
    }

    private void AddSegment_GatherableResources(EventDictionary<ResourcesIdentifier, ResourceCount> gatherableResources)
    {
        BuildingOverviewUISegment_GatherableResources instance = GameObject.Instantiate(gatherableResourcesSegmentPrefab, segmentContainer);
        instance.data = gatherableResources;
        segmentInstances.Add(instance.gameObject);
    }

    private void AddSegment_RecipeDisplay(ChunkMapPointContent_Building_CraftingStation craftingStation)
    {
        BuildingOverviewUISegment_RecipeDisplay instance = GameObject.Instantiate(recipeDisplaySegmentPrefab, segmentContainer);
        instance.data = craftingStation;
        segmentInstances.Add(instance.gameObject);
    }

    private void AddSegment_ResourcesLeft(ChunkMapPointContent_WorldResource worldResource)
    {
        BuildingOverviewUISegment_ResourcesLeft instance = GameObject.Instantiate(resourcesLeftSegmentPrefab, segmentContainer);
        instance.data = worldResource;
        segmentInstances.Add(instance.gameObject);
    }

    private void AddSegment_TaskPriority(BuildingTask buildingTask)
    {
        BuildingOverviewUISegment_TaskPriority instance = GameObject.Instantiate(taskPrioritySegmentPrefab, segmentContainer);
        instance.data = buildingTask;
        segmentInstances.Add(instance.gameObject);
    }

    private void AddSegment_TitleText(string titleText)
    {
        BuildingOverviewUISegment_Text instance = GameObject.Instantiate(titleTextSegmentPrefab, segmentContainer);
        instance.data = titleText;
        segmentInstances.Add(instance.gameObject);
    }

    private void AddSegment_Text(string titleText)
    {
        BuildingOverviewUISegment_Text instance = GameObject.Instantiate(textSegmentPrefab, segmentContainer);
        instance.data = titleText;
        segmentInstances.Add(instance.gameObject);
    }

    private void AddSegment_Line()
    {
        BuildingOverviewUISegment_Line instance = GameObject.Instantiate(lineSegmentPrefab, segmentContainer);
        segmentInstances.Add(instance.gameObject);
    }
    #endregion
}