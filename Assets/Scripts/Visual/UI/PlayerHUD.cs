using UnityEngine;

public class PlayerHUD : DataDrivenUI<Player>
{
    [SerializeField] private BuildingOptions_Visual buildingOptions;
    [SerializeField] private PlayerResourceTracker_Visual totalResourceData;
    [SerializeField] private BuildingPlacer buildingPlacer;
    [SerializeField] private BuildingRemover buildingRemover;
    [SerializeField] private BuildingSelecter buildingSelecter;

    protected override void OnValueChanged_Data(Player oldValue, Player newValue)
    {

        if (newValue != null)
        {
            buildingPlacer.data = newValue;
            buildingRemover.data = newValue;
            buildingSelecter.data = newValue;
            buildingOptions.data = newValue;
            totalResourceData.data = newValue.resourceTracker;
        }
        else
        {
            buildingPlacer.data = null;
            buildingRemover.data = null;
            buildingSelecter.data = null;
            buildingOptions.data = null;
            totalResourceData.data = null;
        }
    }
}
