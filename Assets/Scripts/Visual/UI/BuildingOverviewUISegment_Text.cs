using TMPro;
using UnityEngine;

public class BuildingOverviewUISegment_Text : DataDrivenUI<string>
{
    [SerializeField] private TextMeshProUGUI titleText;

    protected override void OnValueChanged_Data(string oldValue, string newValue)
    {
        if (newValue != null)
        {
            titleText.text = newValue;
        }
    }
}
