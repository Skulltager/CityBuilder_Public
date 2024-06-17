using SheetCodes;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RecipeInputUI : DataDrivenUI<CraftingSetting>
{
    [SerializeField] private Button adjustRecipeButton;
    [SerializeField] private GameObject adjustableRecipeContent;
    [SerializeField] private Image resourceIcon;
    [SerializeField] private TextMeshProUGUI amountText;

    private void Awake()
    {
        adjustRecipeButton.onClick.AddListener(OnPress_AdjustRecipeButton);
    }

    protected override void OnValueChanged_Data(CraftingSetting oldValue, CraftingSetting newValue)
    {
        if (oldValue != null)
        {
            oldValue.selectedCost.onValueChange -= OnValueChanged_SelectedCost;
        }
        if (newValue != null)
        {
            newValue.selectedCost.onValueChangeImmediate += OnValueChanged_SelectedCost;

            if (newValue.recipeOption.ResourceCost.Length > 1)
            {
                adjustableRecipeContent.SetActive(true);
                adjustRecipeButton.interactable = true;
            }
            else
            {
                adjustableRecipeContent.SetActive(false);
                adjustRecipeButton.interactable = false;
            }
        }
    }

    private void OnPress_AdjustRecipeButton()
    {
        int currentIndex = Array.IndexOf(data.recipeOption.ResourceCost, data.selectedCost.value);
        currentIndex = currentIndex < data.recipeOption.ResourceCost.Length - 1 ? currentIndex + 1 : 0;
        data.selectedCost.value = data.recipeOption.ResourceCost[currentIndex];
    }

    private void OnValueChanged_SelectedCost(ResourceCostRecord oldValue, ResourceCostRecord newValue)
    {
        amountText.text = string.Format("x{0}", newValue.Amount);
        resourceIcon.sprite = newValue.Resource.Sprite;
    }

    private void OnDestroy()
    {
        adjustRecipeButton.onClick.RemoveListener(OnPress_AdjustRecipeButton);
    }
}