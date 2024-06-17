using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingOption_Visual : DataDrivenUI<PlayerBuildingLimits>
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private LayoutElement layoutElement;
    [SerializeField] private Button selectButton;
    [SerializeField] private Button unselectButton;
    [SerializeField] private TextMeshProUGUI buildingNameText;
    [SerializeField] private TextMeshProUGUI buildingsRemainingText;
    [SerializeField] private GameObject selectedContent;
    [SerializeField] private GameObject unselectedContent;

    public static readonly EventVariable<BuildingOption_Visual, BuildingOption_Visual> selectedBuildingOption;

    static BuildingOption_Visual()
    {
        selectedBuildingOption = new EventVariable<BuildingOption_Visual, BuildingOption_Visual>(default, default);
    }

    private void Awake()
    {
        selectedBuildingOption.onValueChangeImmediate += OnValueChanged_SelectedBuildingOption;
        selectButton.onClick.AddListener(OnPress_SelectButton);
        unselectButton.onClick.AddListener(OnPress_UnselectButton);

        BuildingRemoverButton.selected.onValueChange += OnValueChanged_BuildingRemover_Selected;
    }

    private void OnValueChanged_BuildingRemover_Selected(bool oldValue, bool newValue)
    {
        if (newValue)
        {
            selectedBuildingOption.value = null;
        }
    }

    private void Start()
    {
        selectButton.onClick.AddListener(OnPress_SelectButton);
        unselectButton.onClick.AddListener(OnPress_UnselectButton);
    }

    private void OnPress_SelectButton()
    {
        selectedBuildingOption.value = this;
    }

    private void OnPress_UnselectButton()
    {
        selectedBuildingOption.value = null;
    }

    protected override void OnValueChanged_Data(PlayerBuildingLimits oldValue, PlayerBuildingLimits newValue)
    {
        if (oldValue != null)
        {
            oldValue.amountLeft.onValueChange -= OnValueChanged_AmountLeft;
            oldValue.limit.onValueChange -= OnValueChanged_Limit;
        }

        if (newValue != null)
        {
            newValue.amountLeft.onValueChangeImmediate += OnValueChanged_AmountLeft;
            newValue.limit.onValueChangeImmediate += OnValueChanged_Limit;
            buildingNameText.text = newValue.record.Name;
        }
    }

    private void OnValueChanged_AmountLeft(int oldValue, int newValue)
    {
        if (newValue > 0)
        {
            selectButton.interactable = true;
            buildingsRemainingText.text = newValue.ToString();
        }
        else
        {
            if(selectedBuildingOption.value == this)
                selectedBuildingOption.value = null;
            selectButton.interactable = false;
            buildingsRemainingText.text = "0";
        }
    }

    private void OnValueChanged_Limit(int oldValue, int newValue)
    {
        if (newValue == 0)
        {
            layoutElement.ignoreLayout = true;
            canvasGroup.Hide();
        }
        else
        {
            layoutElement.ignoreLayout = false;
            canvasGroup.Show();
        }
    }

    private void OnValueChanged_SelectedBuildingOption(BuildingOption_Visual oldValue, BuildingOption_Visual newValue)
    {
        if (newValue == this)
        {
            selectedContent.SetActive(true);
            unselectedContent.SetActive(false);
        }
        else
        {
            unselectedContent.SetActive(true);
            selectedContent.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        selectedBuildingOption.onValueChangeImmediate -= OnValueChanged_SelectedBuildingOption;
        selectButton.onClick.RemoveListener(OnPress_SelectButton);
        unselectButton.onClick.RemoveListener(OnPress_UnselectButton);

        BuildingRemoverButton.selected.onValueChange -= OnValueChanged_BuildingRemover_Selected;

        if (selectedBuildingOption.value == this)
            selectedBuildingOption.value = null;
    }
}