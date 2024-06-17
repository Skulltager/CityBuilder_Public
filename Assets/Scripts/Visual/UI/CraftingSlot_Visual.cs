
using UnityEngine;
using UnityEngine.UI;

public class CraftingSlot_Visual : DataDrivenUI<CraftingSlot>
{
    [SerializeField] private Image craftingItemIcon;
    [SerializeField] private Image craftingProgressImage;

    protected override void OnValueChanged_Data(CraftingSlot oldValue, CraftingSlot newValue)
    { 
        if (oldValue != null)
        {
            oldValue.craftingProgress.onValueChange -= OnValueChanged_CraftingProgress;
            oldValue.resourcesAssigned.onValueChange -= OnValueChanged_ResourcesAssigned;
        }

        if(newValue != null)
        {
            newValue.craftingProgress.onValueChangeImmediate += OnValueChanged_CraftingProgress;
            newValue.resourcesAssigned.onValueChangeImmediate += OnValueChanged_ResourcesAssigned;
        }
    }

    private void OnValueChanged_CraftingProgress(float oldValue, float newValue)
    {
        craftingProgressImage.fillAmount = newValue;
    }

    private void OnValueChanged_ResourcesAssigned(bool oldValue, bool newValue)
    {
        if (newValue)
        {
            craftingItemIcon.sprite = data.craftingStation.record.CraftingRecipes.CraftedResource.Sprite;
            craftingItemIcon.enabled = true;
        }
        else
        {
            craftingItemIcon.enabled = false;
        }
    }
}