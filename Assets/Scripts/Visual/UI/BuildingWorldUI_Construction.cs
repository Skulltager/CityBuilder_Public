using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class BuildingWorldUI_Construction : DataDrivenWorldUI<ChunkMapPointContent_BuildingConstruction>
{
    [SerializeField] private Image fillConstructionProgressImage;

    [SerializeField] private BuildingConstructionResourceIndicator resourceIndicatorPrefab;
    [SerializeField] private RectTransform resourceIndicatorContainer;

    private readonly List<BuildingConstructionResourceIndicator> resourceIndicatorInstances;

    public override Vector3 followPosition => offset;
    private Vector3 offset;

    private BuildingWorldUI_Construction()
    {
        resourceIndicatorInstances = new List<BuildingConstructionResourceIndicator>();
    }

    protected override void OnValueChanged_Data(ChunkMapPointContent_BuildingConstruction oldValue, ChunkMapPointContent_BuildingConstruction newValue)
    {
        if (oldValue != null)
        {
            oldValue.constructionProgress.onValueChange -= OnValueChanged_ConstructionProgress;

            foreach (BuildingConstructionResourceIndicator instance in resourceIndicatorInstances)
                GameObject.Destroy(instance.gameObject);

            resourceIndicatorInstances.Clear();
        }

        if (newValue != null)
        {
            newValue.constructionProgress.onValueChangeImmediate += OnValueChanged_ConstructionProgress;
            offset = new Vector3(newValue.centerPosition.x, heightOffset, newValue.centerPosition.y);
            foreach (Resource resource in newValue.inventory.resources.Values)
            {
                if (resource.currentDesiredAmount.value == 0)
                    continue;

                BuildingConstructionResourceIndicator instance = GameObject.Instantiate(resourceIndicatorPrefab, resourceIndicatorContainer);
                instance.data = resource;
                resourceIndicatorInstances.Add(instance);
            }
        }
    }

    private void OnValueChanged_ConstructionProgress(float oldValue, float newValue)
    {
        fillConstructionProgressImage.fillAmount = newValue;
    }
}