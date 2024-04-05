
using UnityEngine;

public class BuildingVisual : DataDrivenBehaviour<Building>
{
    [SerializeField] private Transform buildingVisualContainer;

    private GameObject buildingInstance;

    protected override void OnValueChanged_Data(Building oldValue, Building newValue)
    {
        if (oldValue != null)
        {
            GameObject.Destroy(buildingInstance);
            buildingInstance = null;
        }    

        if(newValue != null)
        {
            buildingInstance = GameObject.Instantiate(newValue.record.Model, buildingVisualContainer);
            switch(newValue.rotation)
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
            buildingInstance.transform.position = new Vector3(newValue.centerPoint.xIndex, 0, newValue.centerPoint.yIndex);
        }
    }
}