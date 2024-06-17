using SheetCodes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RecipeOutputUI : DataDrivenUI<RecipeRecord>
{
    [SerializeField] private Image resourceIcon;
    [SerializeField] private TextMeshProUGUI amountText;

    protected override void OnValueChanged_Data(RecipeRecord oldValue, RecipeRecord newValue)
    {

        if (newValue != null)
        {
            amountText.text = string.Format("x{0}", 1);
            resourceIcon.sprite = newValue.CraftedResource.Sprite;
        }
    }
}