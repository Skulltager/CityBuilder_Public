
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using UnityEngine;

public class BuildingSelecter : DataDrivenUI<Player>
{
    [SerializeField] private MapVisual mapVisual;
    [SerializeField] private GameObject unlockableRegionIndicatorPrefab;
    [SerializeField] private GameObject selectedBuildingIndicatorPrefab;
    [SerializeField] private GameObject selectedBuildingEnterExitIndicatorPrefab;
    [SerializeField] private Transform selectedBuildingIndicatorContainer;

    private readonly EventVariable<BuildingSelecter, bool> isEnabled;
    private readonly EventVariable<BuildingSelecter, ChunkMapPoint> hoveringPoint;
    private readonly EventVariable<BuildingSelecter, ChunkMapPointContent> hoveringBuilding;

    private readonly List<GameObject> unlockableRegionIndicatorInstances;
    private readonly List<GameObject> selectedBuildingIndicatorInstances;

    private BuildingSelecter()
    {
        isEnabled = new EventVariable<BuildingSelecter, bool>(this, false);
        hoveringPoint = new EventVariable<BuildingSelecter, ChunkMapPoint>(this, null);
        hoveringBuilding = new EventVariable<BuildingSelecter, ChunkMapPointContent>(this, null);

        selectedBuildingIndicatorInstances = new List<GameObject>();
        unlockableRegionIndicatorInstances = new List<GameObject>();
    }

    private void Awake()
    {
        isEnabled.onValueChange += OnValueChanged_IsEnabled;
        hoveringPoint.onValueChange += OnValueChanged_HoveringPoint;
        hoveringBuilding.onValueChange += OnValueChanged_HoveringBuilding;
        BuildingOption_Visual.selectedBuildingOption.onValueChange += OnValueChanged_BuildingOption_SelectedBuilding;
        BuildingRemoverButton.selected.onValueChange += OnValueChanged_BuildingRemover_IsSelected;
        SetIsEnabled();
    }

    private void OnValueChanged_BuildingOption_SelectedBuilding(BuildingOption_Visual oldValue, BuildingOption_Visual newValue)
    {
        SetIsEnabled();
    }

    private void OnValueChanged_BuildingRemover_IsSelected(bool oldValue, bool newValue)
    {
        SetIsEnabled();
    }

    private void SetIsEnabled()
    {
        if (BuildingOption_Visual.selectedBuildingOption.value != null)
        {
            isEnabled.value = false;
            return;
        }

        if (BuildingRemoverButton.selected.value)
        {
            isEnabled.value = false;
            return;
        }

        isEnabled.value = true;
    }

    private void OnValueChanged_IsEnabled(bool oldValue, bool newValue)
    {
        if(!newValue)
        {
            VillagerVisual.selectedVillager.value = null;
            BuildingOverviewUIManager.instance.data = null;
        }

        SetIndicators();
    }

    protected override void OnValueChanged_Data(Player oldValue, Player newValue)
    {
        SetIndicators();
    }

    private void OnValueChanged_HoveringPoint(ChunkMapPoint oldValue, ChunkMapPoint newValue)
    {
        if (oldValue != null)
        {
            oldValue.visionState.onValueChange -= OnValueChanged_HoveringPoint_VisionState;
            oldValue.content.onValueChangeImmediate -= OnValueChanged_HoveringPoint_Content;
            oldValue.chunkRegion.onValueChangeImmediate -= OnValueChanged_HoveringPoint_ChunkRegion;
        }

        if (newValue != null)
        {
            newValue.visionState.onValueChange += OnValueChanged_HoveringPoint_VisionState;
            newValue.content.onValueChangeImmediate += OnValueChanged_HoveringPoint_Content;
            newValue.chunkRegion.onValueChangeImmediate += OnValueChanged_HoveringPoint_ChunkRegion;
        }
    }

    private void OnValueChanged_HoveringPoint_ChunkRegion(ChunkRegion oldValue, ChunkRegion newValue)
    {
        SetIndicators();
    }

    private void OnValueChanged_HoveringPoint_Content(ChunkMapPointContent oldValue, ChunkMapPointContent newValue)
    {
        if (newValue != null)
        {
            hoveringBuilding.value = newValue;
        }
        else
        {
            hoveringBuilding.value = null;
        }
    }

    private void OnValueChanged_HoveringBuilding(ChunkMapPointContent oldValue, ChunkMapPointContent newValue)
    {
        SetIndicators();
    }

    private void OnValueChanged_HoveringPoint_VisionState(VisionState oldValue, VisionState newValue)
    {
        SetIndicators();
    }

    private void ClearBuildingIndicators()
    {
        foreach (GameObject instance in selectedBuildingIndicatorInstances)
            GameObject.Destroy(instance);

        selectedBuildingIndicatorInstances.Clear();
    }

    private void ClearUnlockableRegionIndicators()
    {
        foreach (GameObject instance in unlockableRegionIndicatorInstances)
            GameObject.Destroy(instance);

        unlockableRegionIndicatorInstances.Clear();
    }

    private void SetIndicators()
    {
        SetBuildingIndicators();
        SetUnlockRegionIndicators();
    }

    private void SetBuildingIndicators()
    {
        ClearBuildingIndicators();

        if (data == null)
            return;

        if (!isEnabled.value)
            return;

        if (hoveringBuilding.value == null)
            return;

        if (hoveringPoint.value.visionState.value == VisionState.None)
            return;

        foreach (ChunkMapTileDirectionPoint point in hoveringBuilding.value.mapPoints)
        {
            GameObject instance = GameObject.Instantiate(selectedBuildingIndicatorPrefab, selectedBuildingIndicatorContainer);
            instance.transform.position = new Vector3(point.chunkMapPoint.globalPoint.xIndex + 0.5f, 0, point.chunkMapPoint.globalPoint.yIndex + 0.5f);
            selectedBuildingIndicatorInstances.Add(instance);
        }

        foreach (ContentRelation relatedContent in hoveringBuilding.value.contentRelations)
        {
            foreach (ChunkMapTileDirectionPoint point in relatedContent.content.mapPoints)
            {
                GameObject instance = GameObject.Instantiate(selectedBuildingIndicatorPrefab, selectedBuildingIndicatorContainer);
                instance.transform.position = new Vector3(point.chunkMapPoint.globalPoint.xIndex + 0.5f, 0, point.chunkMapPoint.globalPoint.yIndex + 0.5f);
                selectedBuildingIndicatorInstances.Add(instance);
            }
        }

        if (hoveringBuilding.value is ChunkMapPointContent_BuildingBase building)
        {
            for (int i = 0; i < building.record.BuildingGridData.enterExitPoints.Length; i++)
            {
                BuildingEnterExitGridPoint gridPoint = building.record.BuildingGridData.enterExitPoints[i];
                GameObject instance = GameObject.Instantiate(selectedBuildingEnterExitIndicatorPrefab, selectedBuildingIndicatorContainer);
                selectedBuildingIndicatorInstances.Add(instance);

                CardinalDirection enterExitDirection = gridPoint.direction.RotateByDirection(building.direction);
                Point enterExitPoint = gridPoint.centerOffset.RotateByDirection(building.direction).AddDirection(enterExitDirection) + building.pivotPoint;

                switch (enterExitDirection)
                {
                    case CardinalDirection.Right:
                        instance.transform.position = new Vector3(enterExitPoint.xIndex, 0, enterExitPoint.yIndex + 0.5f);
                        instance.transform.rotation = Quaternion.Euler(0, 90, 0);
                        break;

                    case CardinalDirection.Bottom:
                        instance.transform.position = new Vector3(enterExitPoint.xIndex +  0.5f, 0, enterExitPoint.yIndex + 1);
                        instance.transform.rotation = Quaternion.Euler(0, 180, 0);
                        break;

                    case CardinalDirection.Left:
                        instance.transform.position = new Vector3(enterExitPoint.xIndex + 1, 0, enterExitPoint.yIndex + 0.5f);
                        instance.transform.rotation = Quaternion.Euler(0, 270, 0);
                        break;

                    case CardinalDirection.Top:
                        instance.transform.position = new Vector3(enterExitPoint.xIndex + 0.5f, 0, enterExitPoint.yIndex + 0);
                        instance.transform.rotation = Quaternion.Euler(0, 0, 0);
                        break;
                }
            }
        }
    }

    private void SetUnlockRegionIndicators()
    {
        ClearUnlockableRegionIndicators();

        if (data == null)
            return;

        if (!isEnabled.value)
            return;

        if (hoveringPoint.value == null)
            return;

        if (hoveringBuilding.value != null)
            return;

        if (hoveringPoint.value.chunkRegion.value.owner != null)
            return;

        if (hoveringPoint.value.visionState.value != VisionState.Half)
            return;

        foreach (Point chunkMapPoint in hoveringPoint.value.chunkRegion.value.points)
        {
            GameObject instance = GameObject.Instantiate(unlockableRegionIndicatorPrefab, selectedBuildingIndicatorContainer);
            instance.transform.position = new Vector3(chunkMapPoint.xIndex + 0.5f, 0, chunkMapPoint.yIndex + 0.5f);
            unlockableRegionIndicatorInstances.Add(instance);
        }
    }

    private void Update()
    {
        if (!isEnabled.value || data == null)
            return;

        if (!TryHoverVillager())
        {
            SetHoverPosition();

            if (Input.GetMouseButtonDown(0))
            {
                TrySelectBuilding();
                TrySelectRegion();
            }
        }
        else
        {
            hoveringPoint.value = null;
        }
    }

    private bool TryHoverVillager()
    {
        int layerMask = (int)LayerMaskValue.Villagers;
        Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(cameraRay, out RaycastHit hit, int.MaxValue, layerMask, QueryTriggerInteraction.Collide))
        {
            VillagerVisual villager = hit.collider.GetComponent<VillagerVisual>();
            VillagerVisual.hoveringVillager.value = villager;

            if (Input.GetMouseButtonDown(0))
            {
                VillagerVisual.selectedVillager.value = villager;
                RegionOverviewUI.instance.data = null;
                BuildingOverviewUIManager.instance.data = null;
            }

            return true;
        }
        else
        {
            VillagerVisual.hoveringVillager.value = null;

            return false;
        }

    }

    private void SetHoverPosition()
    {
        if (mapVisual.TryGetMouseGridPoint(0, 0, out Point point))
        {
            ChunkMapPoint mapPoint;
            if (mapVisual.data.TryGetChunkMapPoint(point, out mapPoint))
                hoveringPoint.value = mapPoint;
            else
                hoveringPoint.value = null;
        }
        else
            hoveringPoint.value = null;
    }

    private void TrySelectRegion()
    {
        if (GameDataManager.LAST_MOUSE_INTERACTION_HANDLED_MOMENT == Time.time)
            return;

        if (hoveringPoint.value == null)
            return;

        if (hoveringBuilding.value != null)
            return;

        if (hoveringPoint.value.chunkRegion.value.owner != null)
            return;

        if (hoveringPoint.value.visionState.value != VisionState.Half)
            return;

        RegionOverviewUI.instance.data = new RegionOverviewUIData(data, hoveringPoint.value.chunkRegion.value);
        BuildingOverviewUIManager.instance.data = null;
        VillagerVisual.selectedVillager.value = null;
    }

    private void TrySelectBuilding()
    {
        if (GameDataManager.LAST_MOUSE_INTERACTION_HANDLED_MOMENT == Time.time)
            return;
        
        if (hoveringBuilding.value == null)
            return;

        BuildingOverviewUIManager.instance.data = hoveringBuilding.value;
        VillagerVisual.selectedVillager.value = null;
        RegionOverviewUI.instance.data = null;
    }

    private void OnDestroy()
    {
        isEnabled.onValueChange -= OnValueChanged_IsEnabled;
        hoveringPoint.onValueChange -= OnValueChanged_HoveringPoint;
        hoveringBuilding.onValueChange -= OnValueChanged_HoveringBuilding;
        BuildingOption_Visual.selectedBuildingOption.onValueChange -= OnValueChanged_BuildingOption_SelectedBuilding;
        BuildingRemoverButton.selected.onValueChange -= OnValueChanged_BuildingRemover_IsSelected;

    }
}