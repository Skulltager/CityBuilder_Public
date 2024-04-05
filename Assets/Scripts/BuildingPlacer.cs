using SheetCodes;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;

public class BuildingPlacer : DataDrivenBehaviour<BuildingIdentifier>
{
    [SerializeField] private ChunkMapVisual chunkMapVisual;
    [SerializeField] private BuildingIndicator buildingIndicatorPrefab;
    [SerializeField] private BuildingEnterExitIndicator buildingEnterExitIndicatorPrefab;
    [SerializeField] private Transform buildingContainer;
    [SerializeField] private Transform buildingIndicatorsContainer;

    private readonly EventVariable<BuildingPlacer, Point> hoveringPoint;
    private readonly EventVariable<BuildingPlacer, CardinalDirection> buildingDirection;
    private readonly List<BuildingIndicator> buildingIndicatorInstances;
    private readonly List<BuildingEnterExitIndicator> buildingEnterExitIndicatorInstances;

    private GameObject buildingInstance;
    private BuildingGridSize buildingGridSize;
    private BuildingRecord buildingRecord;

    private BuildingPlacer()
    {
        buildingIndicatorInstances = new List<BuildingIndicator>();
        buildingEnterExitIndicatorInstances = new List<BuildingEnterExitIndicator>();
        hoveringPoint = new EventVariable<BuildingPlacer, Point>(this, default);
        buildingDirection = new EventVariable<BuildingPlacer, CardinalDirection>(this, CardinalDirection.Right);
    }

    protected override void OnValueChanged_Data(BuildingIdentifier oldValue, BuildingIdentifier newValue)
    {
        if (oldValue != BuildingIdentifier.None)
        {
            GameObject.Destroy(buildingInstance);
            buildingInstance = null;

            foreach (BuildingIndicator instance in buildingIndicatorInstances)
                GameObject.Destroy(instance.gameObject);

            foreach (BuildingEnterExitIndicator instance in buildingEnterExitIndicatorInstances)
                GameObject.Destroy(instance.gameObject);

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
                instance.Initialize(chunkMapVisual.data);
                buildingIndicatorInstances.Add(instance);
            }

            for(int i = 0; i < buildingRecord.BuildingGridData.enterExitPoints.Length; i++)
            {
                BuildingEnterExitIndicator instance = GameObject.Instantiate(buildingEnterExitIndicatorPrefab, buildingIndicatorsContainer);
                instance.Initialize(chunkMapVisual.data);
                buildingEnterExitIndicatorInstances.Add(instance);
            }

            SetHoverPosition();

            hoveringPoint.onValueChangeImmediate += OnValueChanged_HoveringPoint;
            buildingDirection.onValueChangeImmediate += OnValueChanged_BuildingDirection;
        }
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
        if (buildingEnterExitIndicatorInstances.Any(i => !i.isViable))
            return;

        if (buildingIndicatorInstances.Any(i => !i.isViable))
            return;

        chunkMapVisual.data.buildings.Add(new Building(buildingRecord, chunkMapVisual.data, hoveringPoint.value, buildingDirection.value));
        data = BuildingIdentifier.None;
    }

    private void SetHoverPosition()
    {
        if (chunkMapVisual.TryGetMouseGridPoint(out Point point, -buildingGridSize.currentXOffset + 0.5f, -buildingGridSize.currentYOffset + 0.5f))
        {
            hoveringPoint.value = point;
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

        for (int i = 0; i < buildingRecord.BuildingGridData.enterExitPoints.Length; i++)
        {
            BuildingEnterExitGridPoint gridPoint = buildingRecord.BuildingGridData.enterExitPoints[i];
            BuildingEnterExitIndicator indicator = buildingEnterExitIndicatorInstances[i];
            switch(newValue)
            {
                case CardinalDirection.Right:
                    indicator.enterExitDirection.value = gridPoint.direction;
                    break;

                case CardinalDirection.Bottom:
                    indicator.enterExitDirection.value = gridPoint.direction.Rotate90Degrees();
                    break;

                case CardinalDirection.Left:
                    indicator.enterExitDirection.value = gridPoint.direction.Rotate180Degrees();
                    break;

                case CardinalDirection.Top:
                    indicator.enterExitDirection.value = gridPoint.direction.Rotate270Degrees();
                    break;
            }
        }

        SetIndicatorPositions();
    }

    private void OnValueChanged_HoveringPoint(Point oldValue, Point newValue)
    {
        if (newValue != null)
        {
            buildingInstance.SetActive(true);
            buildingInstance.transform.position = new Vector3(newValue.xIndex, 0, newValue.yIndex);
            SetIndicatorPositions();
        }
        else
        {
            buildingInstance.SetActive(false);
            foreach (BuildingIndicator indicator in buildingIndicatorInstances)
                indicator.position.value = null;

            foreach (BuildingEnterExitIndicator indicator in buildingEnterExitIndicatorInstances)
                indicator.position.value = null;
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
            switch (buildingDirection.value)
            {
                case CardinalDirection.Right:
                    indicator.position.value = gridPoint.centerOffset + hoveringPoint.value;
                    break;
                case CardinalDirection.Bottom:
                    indicator.position.value = gridPoint.centerOffset.GetRotated90DegreesOffset() + hoveringPoint.value.AddDirection(CardinalDirection.Bottom);
                    break;
                case CardinalDirection.Left:
                    indicator.position.value = gridPoint.centerOffset.GetRotated180DegreesOffset() + hoveringPoint.value.AddDirection(DiagonalDirection.BottomLeft);
                    break;
                case CardinalDirection.Top:
                    indicator.position.value = gridPoint.centerOffset.GetRotated270DegreesOffset() + hoveringPoint.value.AddDirection(CardinalDirection.Left);
                    break;
            }
        }

        for (int i = 0; i < buildingRecord.BuildingGridData.enterExitPoints.Length; i++)
        {
            BuildingEnterExitGridPoint gridPoint = buildingRecord.BuildingGridData.enterExitPoints[i];
            BuildingEnterExitIndicator indicator = buildingEnterExitIndicatorInstances[i];
            switch (buildingDirection.value)
            {
                case CardinalDirection.Right:
                    indicator.position.value = gridPoint.centerOffset + hoveringPoint.value;
                    break;
                case CardinalDirection.Bottom:
                    indicator.position.value = gridPoint.centerOffset.GetRotated90DegreesOffset() + hoveringPoint.value.AddDirection(CardinalDirection.Bottom);
                    break;
                case CardinalDirection.Left:
                    indicator.position.value = gridPoint.centerOffset.GetRotated180DegreesOffset() + hoveringPoint.value.AddDirection(DiagonalDirection.BottomLeft);
                    break;
                case CardinalDirection.Top:
                    indicator.position.value = gridPoint.centerOffset.GetRotated270DegreesOffset() + hoveringPoint.value.AddDirection(CardinalDirection.Left);
                    break;
            }
        }
    }
}
