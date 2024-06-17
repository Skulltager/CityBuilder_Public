using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class BuildingWorldUI_Deconstruction : DataDrivenWorldUI<ChunkMapPointContent_BuildingDeconstruction>
{
    [SerializeField] private Image fillDeconstructionProgressImage;
    [SerializeField] private BuildingDeconstructionResourceIndicator resourceIndicatorPrefab;
    [SerializeField] private RectTransform resourceIndicatorContainer;

    private readonly List<BuildingDeconstructionResourceIndicator> resourceIndicatorInstances;

    public override Vector3 followPosition => offset;
    private Vector3 offset;

    private BuildingWorldUI_Deconstruction()
    {
        resourceIndicatorInstances = new List<BuildingDeconstructionResourceIndicator>();
    }

    protected override void OnValueChanged_Data(ChunkMapPointContent_BuildingDeconstruction oldValue, ChunkMapPointContent_BuildingDeconstruction newValue)
    {
        if (oldValue != null)
        {
            oldValue.deconstructionProgress.onValueChange -= OnValueChanged_ConstructionProgress;

            foreach (BuildingDeconstructionResourceIndicator instance in resourceIndicatorInstances)
                GameObject.Destroy(instance.gameObject);

            resourceIndicatorInstances.Clear();
        }

        if (newValue != null)
        {
            newValue.deconstructionProgress.onValueChangeImmediate += OnValueChanged_ConstructionProgress;
            offset = new Vector3(newValue.centerPosition.x, heightOffset, newValue.centerPosition.y);

            foreach (Resource resource in newValue.inventory.resources.Values)
            {
                if (resource.startingAmount.value == 0)
                    continue;

                BuildingDeconstructionResourceIndicator instance = GameObject.Instantiate(resourceIndicatorPrefab, resourceIndicatorContainer);
                instance.data = resource;
                resourceIndicatorInstances.Add(instance);
            }
        }
    }

    private void OnValueChanged_ConstructionProgress(float oldValue, float newValue)
    {
        fillDeconstructionProgressImage.fillAmount = 1 - newValue;
    }
}