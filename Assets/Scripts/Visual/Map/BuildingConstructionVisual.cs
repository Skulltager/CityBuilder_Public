
using UnityEngine;

public class BuildingConstructionVisual : DataDrivenBehaviour<ChunkMapPointContent_BuildingConstruction>
{
    protected override void OnValueChanged_Data(ChunkMapPointContent_BuildingConstruction oldValue, ChunkMapPointContent_BuildingConstruction newValue)
    {
        if (newValue != null)
        {
            switch (newValue.direction)
            {
                case CardinalDirection.Right:
                    transform.rotation = Quaternion.Euler(0, 0, 0);
                    break;

                case CardinalDirection.Bottom:
                    transform.rotation = Quaternion.Euler(0, 90, 0);
                    break;

                case CardinalDirection.Left:
                    transform.rotation = Quaternion.Euler(0, 180, 0);
                    break;

                case CardinalDirection.Top:
                    transform.rotation = Quaternion.Euler(0, 270, 0);
                    break;
            }
            transform.position = new Vector3(newValue.pivotPoint.xIndex, 0, newValue.pivotPoint.yIndex);
        }
    }
}