
using UnityEngine;
using UnityEngine.UI;

public class BuildingRemoverButton : MonoBehaviour
{
    public static readonly EventVariable<BuildingRemoverButton, bool> selected;

    [SerializeField] private GameObject selectedContent;
    [SerializeField] private GameObject unselectedContent;
    [SerializeField] private Button selectButton;
    [SerializeField] private Button unselectButton;

    static BuildingRemoverButton()
    {
        selected = new EventVariable<BuildingRemoverButton, bool>(null, false);
    }

    private void Awake()
    {
        selected.onValueChangeImmediate += OnValueChanged_Selected;
        unselectButton.onClick.AddListener(OnPress_UnselectButton);
        selectButton.onClick.AddListener(OnPress_SelectButton);

        BuildingOption_Visual.selectedBuildingOption.onValueChange += OnValueChanged_BuildingOption_SelectedBuilding;
    }

    private void OnValueChanged_BuildingOption_SelectedBuilding(BuildingOption_Visual oldValue, BuildingOption_Visual newValue)
    {
        if (newValue != null)
        {
            selected.value = false;
        }
    }

    private void OnValueChanged_Selected(bool oldValue, bool newValue)
    {
        if (newValue)
        {
            selectedContent.SetActive(true);
            unselectedContent.SetActive(false);
        }
        else
        {
            selectedContent.SetActive(false);
            unselectedContent.SetActive(true);
        }
    }

    private void OnPress_SelectButton()
    {
        selected.value = true;
    }

    private void OnPress_UnselectButton()
    {
        selected.value = false;
    }

    private void OnDestroy()
    {
        selected.onValueChangeImmediate -= OnValueChanged_Selected;
        unselectButton.onClick.RemoveListener(OnPress_UnselectButton);
        selectButton.onClick.RemoveListener(OnPress_SelectButton);
        BuildingOption_Visual.selectedBuildingOption.onValueChange -= OnValueChanged_BuildingOption_SelectedBuilding;
    }
}