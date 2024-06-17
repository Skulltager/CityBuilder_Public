using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingOverviewUISegment_InventoryCapacity : DataDrivenUI<Inventory_Limited>
{
    [SerializeField] private TextMeshProUGUI capacityText;
    [SerializeField] private Image capacityFillImage;
    [SerializeField] private Color emptyColorLerp;
    [SerializeField] private Color fullColorLerp;

    protected override void OnValueChanged_Data(Inventory_Limited oldValue, Inventory_Limited newValue)
    {
        if (oldValue != null)
        {
            oldValue.totalAmount.onValueChange -= OnValueChanged_TotalAmount;
            oldValue.capacity.onValueChange -= OnValueChanged_Capacity;
        }

        if (newValue != null)
        {
            newValue.totalAmount.onValueChange += OnValueChanged_TotalAmount;
            newValue.capacity.onValueChange += OnValueChanged_Capacity;

            SetCapacityText();
            SetCapacityFillImage();
        }
    }

    private void OnValueChanged_TotalAmount(int oldValue, int newValue)
    {
        SetCapacityText();
        SetCapacityFillImage();
    }

    private void OnValueChanged_Capacity(int oldValue, int newValue)
    {
        SetCapacityText();
        SetCapacityFillImage();
    }

    private void SetCapacityText()
    {
        capacityText.text = string.Format("{0}/{1}", data.totalAmount.value, data.capacity.value);
    }

    private void SetCapacityFillImage()
    {
        float factor = (float)data.totalAmount.value / data.capacity.value;
        capacityFillImage.fillAmount = factor;
        capacityFillImage.color = Color.Lerp(emptyColorLerp, fullColorLerp, factor);
    }
}
