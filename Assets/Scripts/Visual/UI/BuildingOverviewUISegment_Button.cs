using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingOverviewUISegment_ButtonData
{
    public readonly string buttonText;
    public readonly Action callback;

    public BuildingOverviewUISegment_ButtonData(Action callback, string buttonText)
    {
        this.buttonText = buttonText;
        this.callback = callback;
    }
}

public class BuildingOverviewUISegment_Button : DataDrivenUI<BuildingOverviewUISegment_ButtonData>
{
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI buttonText;

    private void Awake()
    {
        button.onClick.AddListener(OnPress_Button);
    }

    private void OnPress_Button()
    {
        data.callback();
    }

    protected override void OnValueChanged_Data(BuildingOverviewUISegment_ButtonData oldValue, BuildingOverviewUISegment_ButtonData newValue)
    {
        if(newValue != null)
        {
            buttonText.text = newValue.buttonText;
        }
    }

    private void OnDestroy()
    {
        button.onClick.RemoveListener(OnPress_Button);
    }
}
