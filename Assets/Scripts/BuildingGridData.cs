
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/BuildingGridData", order = 1)]
public class BuildingGridData : ScriptableObject
{
    [field: SerializeField] public BuildingEnterExitGridPoint[] enterExitPoints;
    [field: SerializeField] public BuildingGridPoint[] buildingGridPoints;

    public BuildingGridSize GetBuildingGridSize()
    {
        int minX = int.MaxValue;
        int maxX = int.MinValue;
        int minY = int.MaxValue;
        int maxY = int.MinValue;

        foreach (BuildingGridPoint gridPoint in buildingGridPoints)
        {
            minX = Mathf.Min(minX, gridPoint.centerOffset.xIndex);
            maxX = Mathf.Max(maxX, gridPoint.centerOffset.xIndex);
            minY = Mathf.Min(minY, gridPoint.centerOffset.yIndex);
            maxY = Mathf.Max(maxY, gridPoint.centerOffset.yIndex);
        }

        int width = maxX - minX + 1;
        int height = maxY - minY + 1;

        float xOffset = minX + width / 2f;
        float yOffset = minY + height / 2f;
        return new BuildingGridSize(width, height, xOffset, yOffset, CardinalDirection.Right);
    }
}