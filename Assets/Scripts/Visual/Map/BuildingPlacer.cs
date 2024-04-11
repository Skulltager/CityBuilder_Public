using SheetCodes;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;

public class BuildingPlacer : DataDrivenBehaviour<BuildingIdentifier>
{
    [SerializeField] private MapVisual mapVisual;
    [SerializeField] private BuildingIndicator buildingIndicatorPrefab;
    [SerializeField] private BuildingEnterExitIndicator buildingEnterExitIndicatorPrefab;
    [SerializeField] private Transform buildingContainer;
    [SerializeField] private Transform buildingIndicatorsContainer;

    private readonly EventVariable<BuildingPlacer, bool> isViable;
    private readonly EventVariable<BuildingPlacer, ChunkMapPoint> hoveringPoint;
    private readonly EventVariable<BuildingPlacer, CardinalDirection> buildingDirection;
    private readonly List<BuildingIndicator> buildingIndicatorInstances;
    private readonly List<BuildingEnterExitIndicator> buildingEnterExitIndicatorInstances;

    private GameObject buildingInstance;
    private BuildingGridSize buildingGridSize;
    private BuildingRecord buildingRecord;

    private BuildingPlacer()
    {
        buildingIndicatorInstances = new List<BuildingIndicator>();
        isViable = new EventVariable<BuildingPlacer, bool>(this, false);
        buildingEnterExitIndicatorInstances = new List<BuildingEnterExitIndicator>();
        hoveringPoint = new EventVariable<BuildingPlacer, ChunkMapPoint>(this, default);
        buildingDirection = new EventVariable<BuildingPlacer, CardinalDirection>(this, CardinalDirection.Right);
    }

    protected override void OnValueChanged_Data(BuildingIdentifier oldValue, BuildingIdentifier newValue)
    {
        if (oldValue != BuildingIdentifier.None)
        {
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

            buildingIndicatorInstances.Clear();
            buildingEnterExitIndicatorInstances.Clear();
            hoveringPoint.onValueChange -= OnValueChanged_HoveringPoint;
            buildingDirection.onValueChange -= OnValueChanged_BuildingDirection;
        }

        if (newValue != BuildingIdentifier.None)
        {
            buildingRecord = newValue.GetRecord();
            buildingGridSize = buildingRecord.BuildingGridData.GetBuildingGridSize();
            buildingInstance = GameObject.Instantiate(buildingRecord.Model, buildingContainer);

            for(int i = 0; i < buildingRecord.BuildingGridData.buildingGridPoints.Length; i++)
            {
                BuildingIndicator instance = GameObject.Instantiate(buildingIndicatorPrefab, buildingIndicatorsContainer);
                instance.viable.onValueChange += OnValueChanged_BuildingIndicator_IsViable;
                buildingIndicatorInstances.Add(instance);
            }

            for(int i = 0; i < buildingRecord.BuildingGridData.enterExitPoints.Length; i++)
            {
                BuildingEnterExitIndicator instance = GameObject.Instantiate(buildingEnterExitIndicatorPrefab, buildingIndicatorsContainer);
                instance.viable.onValueChange += OnValueChanged_BuildingIndicator_IsViable;
                buildingEnterExitIndicatorInstances.Add(instance);
            }

            SetHoverPosition();
            SetIsViable();

            hoveringPoint.onValueChangeImmediate += OnValueChanged_HoveringPoint;
            buildingDirection.onValueChangeImmediate += OnValueChanged_BuildingDirection;
        }
    }

    private void OnValueChanged_BuildingIndicator_IsViable(bool oldValue, bool newValue)
    {
        SetIsViable();
    }

    private void SetIsViable()
    {
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
        if (data != BuildingIdentifier.None)
        {
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
    }

    private void TryPlaceBuilding()
    {
        if (!isViable.value)
            return;

        Building building = new Building(buildingRecord, mapVisual.data, hoveringPoint.value.globalPoint, buildingDirection.value);
        building.PlaceOnMap(mapVisual.data);

        data = BuildingIdentifier.None;
    }

    private void SetHoverPosition()
    {
        if (mapVisual.TryGetMouseGridPoint(out Point point, -buildingGridSize.currentXOffset + 0.5f, -buildingGridSize.currentYOffset + 0.5f))
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

    private void OnValueChanged_BuildingDirection(CardinalDirection oldValue, CardinalDirection newValue)
    {
        buildingGridSize.SetRotation(newValue);

        switch (newValue)
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

        SetIndicatorPositions();
    }

    private void OnValueChanged_HoveringPoint(ChunkMapPoint oldValue, ChunkMapPoint newValue)
    {
        if (newValue != null)
        {
            buildingInstance.SetActive(true);
            buildingInstance.transform.position = new Vector3(newValue.globalPoint.xIndex, 0, newValue.globalPoint.yIndex);
            SetIndicatorPositions();
        }
        else
        {
            buildingInstance.SetActive(false);
            foreach (BuildingIndicator indicator in buildingIndicatorInstances)
                indicator.data = null;

            foreach (BuildingEnterExitIndicator indicator in buildingEnterExitIndicatorInstances)
                indicator.data = null;
        }
    }

    private void SetIndicatorPositions()
    {
        if (hoveringPoint.value == null)
            return;

        for (int i = 0; i < buildingRecord.BuildingGridData.buildingGridPoints.Length; i++)
        {
            BuildingGridPoint gridPoint = buildingRecord.BuildingGridData.buildingGridPoints[i];
            BuildingIndicator indicator = buildingIndicatorInstances[i];
            ChunkMapPoint chunkMapPoint = default;
            bool result = false;
            switch (buildingDirection.value)
            {
                case CardinalDirection.Right:
                    result = mapVisual.data.TryGetChunkMapPoint(gridPoint.centerOffset + hoveringPoint.value.globalPoint, out chunkMapPoint);
                    break;
                case CardinalDirection.Bottom:
                    result = mapVisual.data.TryGetChunkMapPoint(gridPoint.centerOffset.GetRotated90DegreesOffset() + hoveringPoint.value.globalPoint.AddDirection(CardinalDirection.Bottom), out chunkMapPoint);
                    break;
                case CardinalDirection.Left:
                    result = mapVisual.data.TryGetChunkMapPoint(gridPoint.centerOffset.GetRotated180DegreesOffset() + hoveringPoint.value.globalPoint.AddDirection(DiagonalDirection.BottomLeft), out chunkMapPoint);
                    break;
                case CardinalDirection.Top:
                    result = mapVisual.data.TryGetChunkMapPoint(gridPoint.centerOffset.GetRotated270DegreesOffset() + hoveringPoint.value.globalPoint.AddDirection(CardinalDirection.Left), out chunkMapPoint);
                    break;
            }

            if(result)
                indicator.data = chunkMapPoint;
            else
                indicator.data = null;
        }

        for (int i = 0; i < buildingRecord.BuildingGridData.enterExitPoints.Length; i++)
        {
            BuildingEnterExitGridPoint gridPoint = buildingRecord.BuildingGridData.enterExitPoints[i];
            BuildingEnterExitIndicator indicator = buildingEnterExitIndicatorInstances[i];

            Point enterExitPoint = default;
            CardinalDirection enterExitDirection = default;

            switch (buildingDirection.value)
            {
                case CardinalDirection.Right:
                    enterExitPoint = gridPoint.centerOffset + hoveringPoint.value.globalPoint;
                    switch (gridPoint.direction)
                    {
                        case CardinalDirection.Right:
                            enterExitDirection = CardinalDirection.Right;
                            break;
                        case CardinalDirection.Bottom:
                            enterExitDirection = CardinalDirection.Bottom ;
                            enterExitPoint = enterExitPoint.AddDirection(CardinalDirection.Bottom);
                            break;
                        case CardinalDirection.Left:
                            enterExitDirection = CardinalDirection.Left;
                            enterExitPoint = enterExitPoint.AddDirection(CardinalDirection.Left);
                            break;
                        case CardinalDirection.Top:
                            enterExitDirection = CardinalDirection.Top;
                            enterExitPoint = enterExitPoint.AddDirection(CardinalDirection.Top);
                            break;
                    }
                    break;
                case CardinalDirection.Bottom:
                    enterExitPoint = gridPoint.centerOffset.GetRotated90DegreesOffset() + hoveringPoint.value.globalPoint.AddDirection(CardinalDirection.Bottom);
                    switch (gridPoint.direction)
                    {
                        case CardinalDirection.Right:
                            enterExitDirection = CardinalDirection.Bottom;
                            break;
                        case CardinalDirection.Bottom:
                            enterExitDirection = CardinalDirection.Left;
                            break;
                        case CardinalDirection.Left:
                            enterExitDirection = CardinalDirection.Top;
                            break;
                        case CardinalDirection.Top:
                            enterExitDirection = CardinalDirection.Right;
                            break;
                    }
                    break;
                case CardinalDirection.Left:
                    enterExitPoint = gridPoint.centerOffset.GetRotated180DegreesOffset() + hoveringPoint.value.globalPoint.AddDirection(DiagonalDirection.BottomLeft);
                    switch (gridPoint.direction)
                    {
                        case CardinalDirection.Right:
                            enterExitDirection = CardinalDirection.Left;
                            break;
                        case CardinalDirection.Bottom:
                            enterExitDirection = CardinalDirection.Top;
                            break;
                        case CardinalDirection.Left:
                            enterExitDirection = CardinalDirection.Right;
                            break;
                        case CardinalDirection.Top:
                            enterExitDirection = CardinalDirection.Bottom;
                            break;
                    }
                    break;
                case CardinalDirection.Top:
                    enterExitPoint = gridPoint.centerOffset.GetRotated270DegreesOffset() + hoveringPoint.value.globalPoint.AddDirection(CardinalDirection.Left);
                    switch (gridPoint.direction)
                    {
                        case CardinalDirection.Right:
                            enterExitDirection = CardinalDirection.Top;
                            break;
                        case CardinalDirection.Bottom:
                            enterExitDirection = CardinalDirection.Right;
                            break;
                        case CardinalDirection.Left:
                            enterExitDirection = CardinalDirection.Bottom;
                            break;
                        case CardinalDirection.Top:
                            enterExitDirection = CardinalDirection.Left;
                            break;
                    }
                    break;
            }

            enterExitPoint = enterExitPoint.AddDirection(enterExitDirection);

            ChunkMapPoint chunkMapPoint;
            if (mapVisual.data.TryGetChunkMapPoint(enterExitPoint, out chunkMapPoint))
                indicator.data = new BuildingEnterExitIndicatorData(chunkMapPoint, enterExitDirection);
            else
                indicator.data = null;
        }
    }
}
