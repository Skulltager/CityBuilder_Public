
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VillagerOverviewUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI taskText;
    [SerializeField] private TextMeshProUGUI resourceAmountText;
    [SerializeField] private Image resourceIcon;

    [SerializeField] private GameObject inventoryContent;
    [SerializeField] private GameObject noInventoryContent;

    private void Awake()
    {
        VillagerVisual.selectedVillager.onValueChangeImmediate += OnValueChanged_SelectedVillager;
    }

    private void OnValueChanged_SelectedVillager(VillagerVisual oldValue, VillagerVisual newValue)
    {
        if (oldValue != null)
        {
            oldValue.data.inventory.onValueChangeImmediate -= OnValueChanged_Villager_Inventory;
            oldValue.data.currentTaskDescription.onValueChange -= OnValueChanged_Villager_CurrentTaskDescription;
        }

        if (newValue != null)
        {
            newValue.data.inventory.onValueChangeImmediate += OnValueChanged_Villager_Inventory;
            newValue.data.currentTaskDescription.onValueChangeImmediate += OnValueChanged_Villager_CurrentTaskDescription;
            nameText.text = newValue.data.name;
            canvasGroup.Show();
        }
        else
        {
            canvasGroup.Hide();
        }
    }

    private void OnValueChanged_Villager_Inventory(ResourceCount oldValue, ResourceCount newValue)
    {
        if (oldValue != null)
        {
            oldValue.amount.onValueChange -= OnValueChanged_Villager_Inventory_Amount;
        }

        if (newValue != null)
        {
            resourceIcon.sprite = newValue.record.Sprite;
            newValue.amount.onValueChangeImmediate += OnValueChanged_Villager_Inventory_Amount;
            inventoryContent.SetActive(true);
            noInventoryContent.SetActive(false);
        }
        else
        {
            inventoryContent.SetActive(false);
            noInventoryContent.SetActive(true);
        }
    }

    private void OnValueChanged_Villager_CurrentTaskDescription(string oldValue, string newValue)
    {
        taskText.text = newValue;
    }

    private void OnValueChanged_Villager_Inventory_Amount(int oldValue, int newValue)
    {
        resourceAmountText.text = newValue.ToString();
    }

    private void OnDestroy()
    {
        VillagerVisual.selectedVillager.onValueChange -= OnValueChanged_SelectedVillager;
    }
}