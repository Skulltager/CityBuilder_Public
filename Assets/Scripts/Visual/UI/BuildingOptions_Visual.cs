
using System.Collections.Generic;
using UnityEngine;

public class BuildingOptions_Visual : DataDrivenUI<Player>
{
    [SerializeField] private BuildingOption_Visual prefab;
    [SerializeField] private RectTransform container;

    private readonly List<BuildingOption_Visual> instances;

    private BuildingOptions_Visual()
    {
        instances = new List<BuildingOption_Visual>();
    }

    protected override void OnValueChanged_Data(Player oldValue, Player newValue)
    {
        if (oldValue != null)
        {
            foreach (BuildingOption_Visual instance in instances)
                GameObject.Destroy(instance.gameObject);

            instances.Clear();
        }

        if(newValue != null)
        {
            foreach(PlayerBuildingLimits playerBuildingLimit in newValue.buildingLimits.Values)
            {
                BuildingOption_Visual instance = GameObject.Instantiate(prefab, container);
                instance.data = playerBuildingLimit;
                instances.Add(instance);
            }
        }
    }
}
