using SheetCodes;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuildingPlacer : DataDrivenBehaviour<Player>
{
    [SerializeField] private MapVisual mapVisual;
    [SerializeField] private BuildingIndicator buildingIndicatorPrefab;
    [SerializeField] private BuildingEnterExitIndicator buildingEnterExitIndicatorPrefab;
    [SerializeField] private HarvestableIndicator harvestableIndicatorPrefab;
    [SerializeField] private Transform buildingContainer;
    [SerializeField] private Transform buildingIndicatorsContainer;

    private readonly EventVariable<BuildingPlacer, bool> isViable;

    private readonly EventVariable<BuildingPlacer, bool> isEnabled;
    private readonly EventVariable<BuildingPlacer, BuildingRecord> selectedBuilding;
    private readonly EventVariable<BuildingPlacer, CardinalDirection> buildingDirection;
    private readonly EventVariable<BuildingPlacer, ChunkMapPoint> hoveringPoint;

    private readonly List<BuildingIndicator> buildingIndicatorInstances;
    private readonly List<BuildingEnterExitIndicator> buildingEnterExitIndicatorInstances;
    private readonly List<HarvestableIndicator> harvestableIndicatorInstances;

    private GameObject buildingInstance;
    private bool isShowingBuildingIndicators;
    private BuildingIdentifier buildingInstanceIdentifier;

    private BuildingPlacer()
    {
        buildingIndicatorInstances = new List<BuildingIndicator>();
        isEnabled = new EventVariable<BuildingPlacer, bool>(this, false);
        isViable = new EventVariable<BuildingPlacer, bool>(this, false);
        selectedBuilding = new EventVariable<BuildingPlacer, BuildingRecord>(this, null);
        buildingEnterExitIndicatorInstances = new List<BuildingEnterExitIndicator>();
        hoveringPoint = new EventVariable<BuildingPlacer, ChunkMapPoint>(this, default);
        harvestableIndicatorInstances = new List<HarvestableIndicator>();
        buildingDirection = new EventVariable<BuildingPlacer, CardinalDirection>(this, CardinalDirection.Right);
    }

    private void Awake()
    {
        isEnabled.onValueChange += OnValueChanged_IsEnabled;
        selectedBuilding.onValueChange += OnValueChanged_SelectedBuilding;
        buildingDirection.onValueChange += OnValueChanged_BuildingDirection;
        hoveringPoint.onValueChange += OnValueChanged_HoveringPoint;

        BuildingOption_Visual.selectedBuildingOption.onValueChange += OnValueChanged_BuildingOption_SelectedBuilding;
    }

    protected override void OnValueChanged_Data(Player oldValue, Player newValue)
    {
        SetBuildingIndicators();
    }

    private void OnValueChanged_IsEnabled(bool oldValue, bool newValue)
    {
        SetBuildingIndicators();
    }

    private void OnValueChanged_BuildingDirection(CardinalDirection oldValue, CardinalDirection newValue)
    {
        SetBuildingIndicators();
    }

    private void OnValueChanged_HoveringPoint(ChunkMapPoint oldValue, ChunkMapPoint newValue)
    {
        SetBuildingIndicators();
    }

    private void OnValueChanged_SelectedBuilding(BuildingRecord oldValue, BuildingRecord newValue)
    {
        SetBuildingIndicators();
    }

    private void OnValueChanged_BuildingOption_SelectedBuilding(BuildingOption_Visual oldValue, BuildingOption_Visual newValue)
    {
        if (newValue != null)
        {
            selectedBuilding.value = newValue.data.record;
            isEnabled.value = true;
        }
        else
        {
            selectedBuilding.value = null;
            isEnabled.value = false;
        }
    }

    private void ClearBuildingIndicators()
    {
        if (!isShowingBuildingIndicators)
            return;

        buildingInstanceIdentifier = BuildingIdentifier.None;
        isShowingBuildingIndicators = false;

        GameObject.Destroy(buildingInstance);
        buildingInstance = null;

        foreach (BuildingIndicator instance in buildingIndicatorInstances)
        {
            instance.viable.onValueChange -= OnValueChanged_BuildingIndicator_IsViable;
            GameObject.Destroy(instance.gameObject);
        }

        foreach (BuildingEnterExitIndicator instance in buildingEnterExitIndicatorInstances)
        {
            instance.viable.onValueChange -= OnValueChanged_BuildingIndicator_IsViable;
            GameObject.Destroy(instance.gameObject);
        }

        foreach(HarvestableIndicator instance in harvestableIndicatorInstances)
            GameObject.Destroy(instance.gameObject);


        buildingIndicatorInstances.Clear();
        buildingEnterExitIndicatorInstances.Clear();
        harvestableIndicatorInstances.Clear();
    }

    private bool ShouldShowBuildingIndicators()
    {
        if (!isEnabled.value)
            return false;

        if (data == null)
            return false;

        if (selectedBuilding.value == null)
            return false;

        if (hoveringPoint.value == null)
            return false;

        return true;
    }

    private void SetBuildingIndicators()
    {
        if (!ShouldShowBuildingIndicators())
        {
            ClearBuildingIndicators();
            return;
        }

        if (selectedBuilding.value.Identifier != buildingInstanceIdentifier)
        {
            ClearBuildingIndicators();

            buildingInstanceIdentifier = selectedBuilding.value.Identifier;
            isShowingBuildingIndicators = true;
            buildingInstance = GameObject.Instantiate(selectedBuilding.value.BuildingPlacementPrefab, buildingContainer);

            WorldContentSizeBounds sizeBounds = selectedBuilding.value.BuildingGridData.size.GetSizeBounds(buildingDirection.value);

            for (int x = 0; x < sizeBounds.width; x++)
            {
                for (int y = 0; y < sizeBounds.height; y++)
                {
                    BuildingIndicator instance = GameObject.Instantiate(buildingIndicatorPrefab, buildingIndicatorsContainer);
                    instance.viable.onValueChange += OnValueChanged_BuildingIndicator_IsViable;
                    buildingIndicatorInstances.Add(instance);
                }
            }

            for (int i = 0; i < selectedBuilding.value.BuildingGridData.enterExitPoints.Length; i++)
            {
                BuildingEnterExitIndicator instance = GameObject.Instantiate(buildingEnterExitIndicatorPrefab, buildingIndicatorsContainer);
                instance.viable.onValueChange += OnValueChanged_BuildingIndicator_IsViable;
                buildingEnterExitIndicatorInstances.Add(instance);
            }

            int gatheringRange = selectedBuilding.value.GatheringRange;
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

                    HarvestableIndicator instance = GameObject.Instantiate(harvestableIndicatorPrefab, buildingIndicatorsContainer);
                    harvestableIndicatorInstances.Add(instance);
                }
            }
        }

        SetIndicatorPositions();
    }

    private void OnValueChanged_BuildingIndicator_IsViable(bool oldValue, bool newValue)
    {
        SetIsViable();
    }

    private void SetIsViable()
    {
        if (!isEnabled.value)
        {
            isViable.value = false;
            return;
        }

        if (data == null)
        {
            isViable.value = false;
            return;
        }

        if (buildingIndicatorInstances.Any(i => !i.viable.value))
        {
            isViable.value = false;
            return;
        }

        if (buildingEnterExitIndicatorInstances.Any(i => !i.viable.value))
        {
            isViable.value = false;
            return;
        }

        isViable.value = true;
    }

    private void Update()
    {
        if (!isEnabled.value || data == null || selectedBuilding.value == null)
            return;

        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.R))
        {
            buildingDirection.value = buildingDirection.value.Rotate270Degrees();
        }

        if (!Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.R))
        {
            buildingDirection.value = buildingDirection.value.Rotate90Degrees();
        }

        SetHoverPosition();

        if (Input.GetMouseButtonDown(0))
            TryPlaceBuilding();
    }

    private void TryPlaceBuilding()
    {
        if (!isViable.value || hoveringPoint.value == null)
            return;

        GameDataManager.LAST_MOUSE_INTERACTION_HANDLED_MOMENT = Time.time;
        ChunkMapPointContent_BuildingConstruction construction = new ChunkMapPointContent_BuildingConstruction(mapVisual.data, hoveringPoint.value.globalPoint, buildingDirection.value, selectedBuilding.value, data);
        construction.PlaceOnMap();

        if (!Input.GetKey(KeyCode.LeftShift))
            BuildingOption_Visual.selectedBuildingOption.value = null;
    }

    private void SetHoverPosition()
    {
        WorldContentSizeBounds sizeBounds = selectedBuilding.value.BuildingGridData.size.GetSizeBounds(buildingDirection.value);

        if (mapVisual.TryGetMouseGridPoint(-sizeBounds.xCenter + 0.5f, -sizeBounds.yCenter + 0.5f, out Point point))
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

    private void SetIndicatorPositions()
    {
        buildingInstance.transform.position = new Vector3(hoveringPoint.value.globalPoint.xIndex, 0, hoveringPoint.value.globalPoint.yIndex);

        switch (buildingDirection.value)
        {
            case CardinalDirection.Right:
                buildingInstance.transform.rotation = Quaternion.Euler(0, 0, 0);
                break;
            case CardinalDirection.Bottom:
                buildingInstance.transform.rotation = Quaternion.Euler(0, 90, 0);
                break;
            case CardinalDirection.Left:
                buildingInstance.transform.rotation = Quaternion.Euler(0, 180, 0);
                break;
            case CardinalDirection.Top:
                buildingInstance.transform.rotation = Quaternion.Euler(0, 270, 0);
                break;
        }

        WorldContentSizeBounds sizeBounds = selectedBuilding.value.BuildingGridData.size.GetSizeBounds(buildingDirection.value);

        int index = 0;
        for (int x = 0; x < sizeBounds.width; x++)
        {
            for (int y = 0; y < sizeBounds.height; y++)
            {
                Point globalPoint = new Point(x + sizeBounds.xOffset, y + sizeBounds.yOffset) + hoveringPoint.value.globalPoint;

                BuildingIndicator indicator = buildingIndicatorInstances[index];

                ChunkMapPoint chunkMapPoint;
                if (mapVisual.data.TryGetChunkMapPoint(globalPoint, out chunkMapPoint))
                    indicator.data = new BuildingIndicatorData(data, chunkMapPoint);
                else
                    indicator.data = null;

                index++;
            }
        }

        for (int i = 0; i < selectedBuilding.value.BuildingGridData.enterExitPoints.Length; i++)
        {
            BuildingEnterExitGridPoint gridPoint = selectedBuilding.value.BuildingGridData.enterExitPoints[i];
            BuildingEnterExitIndicator indicator = buildingEnterExitIndicatorInstances[i];

            CardinalDirection enterExitDirection = gridPoint.direction.RotateByDirection(buildingDirection.value);
            Point enterExitPoint = gridPoint.centerOffset.RotateByDirection(buildingDirection.value).AddDirection(enterExitDirection) + hoveringPoint.value.globalPoint;

            ChunkMapPoint chunkMapPoint;
            if (mapVisual.data.TryGetChunkMapPoint(enterExitPoint, out chunkMapPoint))
                indicator.data = new BuildingEnterExitIndicatorData(data, chunkMapPoint, enterExitDirection);
            else
                indicator.data = null;
        }

        index = 0;
        int gatheringRange = selectedBuilding.value.GatheringRange;
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

                HarvestableIndicator indicator = harvestableIndicatorInstances[index];
                Point globalPoint = new Point(x + sizeBounds.xOffset, y + sizeBounds.yOffset) + hoveringPoint.value.globalPoint;
                ChunkMapPoint chunkMapPoint;
                if (mapVisual.data.TryGetChunkMapPoint(globalPoint, out chunkMapPoint))
                    indicator.data = new HarvestableIndicatorData(data, chunkMapPoint, selectedBuilding.value.GatherableType);
                else
                    indicator.data = null;
                index++;
            }
        }
    }

    private void OnDestroy()
    {
        isEnabled.onValueChange -= OnValueChanged_IsEnabled;
        selectedBuilding.onValueChange -= OnValueChanged_SelectedBuilding;
        buildingDirection.onValueChange -= OnValueChanged_BuildingDirection;
        hoveringPoint.onValueChange -= OnValueChanged_HoveringPoint;

        BuildingOption_Visual.selectedBuildingOption.onValueChange -= OnValueChanged_BuildingOption_SelectedBuilding;
    }
}
