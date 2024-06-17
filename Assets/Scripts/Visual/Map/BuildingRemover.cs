using System.Collections.Generic;
using UnityEngine;

public class BuildingRemover : DataDrivenBehaviour<Player>
{
    [SerializeField] private MapVisual mapVisual;
    [SerializeField] private GameObject deconstructIndicatorPrefab;
    [SerializeField] private Transform deconstructIndicatorContainer;

    private readonly EventVariable<BuildingRemover, bool> isEnabled;
    private readonly EventVariable<BuildingRemover, ChunkMapPoint> hoveringPoint;
    private readonly EventVariable<BuildingRemover, ChunkMapPointContent_BuildingBase> hoveringBuilding;

    private readonly List<GameObject> deconstructIndicatorInstances;

    private BuildingRemover()
    {
        deconstructIndicatorInstances = new List<GameObject>();
        isEnabled = new EventVariable<BuildingRemover, bool>(this, false);
        hoveringPoint = new EventVariable<BuildingRemover, ChunkMapPoint>(this, default);
        hoveringBuilding = new EventVariable<BuildingRemover, ChunkMapPointContent_BuildingBase>(this, default);
    }

    private void Awake()
    {
        isEnabled.onValueChange += OnValueChanged_IsEnabled;
        hoveringPoint.onValueChange += OnValueChanged_HoveringPoint;
        hoveringBuilding.onValueChange += OnValueChanged_HoveringBuilding;

        BuildingRemoverButton.selected.onValueChange += OnValueChanged_BuildingRemover_Selected;
    }

    private void OnValueChanged_BuildingRemover_Selected(bool oldValue, bool newValue)
    {
        isEnabled.value = newValue;
    }

    private void OnValueChanged_IsEnabled(bool oldValue, bool newValue)
    {
        SetBuildingIndicators();
    }

    protected override void OnValueChanged_Data(Player oldValue, Player newValue)
    {
        SetBuildingIndicators();
    }

    private void OnValueChanged_HoveringPoint(ChunkMapPoint oldValue, ChunkMapPoint newValue)
    {
        if (oldValue != null)
        {
            oldValue.content.onValueChangeImmediate -= OnValueChanged_HoveringPoint_Content;
        }
        
        if (newValue != null)
        {
            newValue.content.onValueChangeImmediate += OnValueChanged_HoveringPoint_Content;
        }
    }

    private void OnValueChanged_HoveringPoint_Content(ChunkMapPointContent oldValue, ChunkMapPointContent newValue)
    {
        if (oldValue != null)
        {
            foreach (GameObject instance in deconstructIndicatorInstances)
                GameObject.Destroy(instance);

            deconstructIndicatorInstances.Clear();
        }

        if (newValue != null && newValue is ChunkMapPointContent_BuildingBase buildingBase && buildingBase.owner == data)
        {
            hoveringBuilding.value = buildingBase;
        }
        else
        {
            hoveringBuilding.value = null;
        }
    }

    private void OnValueChanged_HoveringBuilding(ChunkMapPointContent_BuildingBase oldValue, ChunkMapPointContent_BuildingBase newValue)
    {
        SetBuildingIndicators();
    }

    private void ClearBuildingIndicators()
    {
        foreach (GameObject instance in deconstructIndicatorInstances)
            GameObject.Destroy(instance);

        deconstructIndicatorInstances.Clear();
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

        foreach (ChunkMapTileDirectionPoint placedPoint in hoveringBuilding.value.mapPoints)
        {
            GameObject instance = GameObject.Instantiate(deconstructIndicatorPrefab, deconstructIndicatorContainer);
            instance.transform.position = new Vector3(placedPoint.chunkMapPoint.globalPoint.xIndex + 0.5f, 0, placedPoint.chunkMapPoint.globalPoint.yIndex + 0.5f);
            deconstructIndicatorInstances.Add(instance);
        }
    }

    private void Update()
    {
        if (!isEnabled.value || data == null)
            return;

        SetHoverPosition();

        if (Input.GetMouseButtonDown(0))
            TryRemoveBuilding();
    }

    private void TryRemoveBuilding()
    {
        if (hoveringPoint.value == null)
            return;

        if (hoveringPoint.value.content.value == null)
            return;

        GameDataManager.LAST_MOUSE_INTERACTION_HANDLED_MOMENT = Time.time;

        if (hoveringPoint.value.content.value.contentType == ChunkMapPointContentType.Building )
        {
            ChunkMapPointContent_Building building = hoveringPoint.value.content.value as ChunkMapPointContent_Building;
            if (building.canBeDestroyed)
                building.Deconstruct();
        }
        else if (hoveringPoint.value.content.value.contentType == ChunkMapPointContentType.BuildingConstruction)
        {
            ChunkMapPointContent_BuildingConstruction construction = hoveringPoint.value.content.value as ChunkMapPointContent_BuildingConstruction;
            construction.CancelConstruction();
        }

        if (!Input.GetKey(KeyCode.LeftShift))
            BuildingRemoverButton.selected.value = false;
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

    private void OnDestroy()
    {
        isEnabled.onValueChange -= OnValueChanged_IsEnabled;
        hoveringPoint.onValueChange -= OnValueChanged_HoveringPoint;
        hoveringBuilding.onValueChange -= OnValueChanged_HoveringBuilding;
    }
}
