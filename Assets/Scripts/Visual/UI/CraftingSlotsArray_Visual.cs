
using System.Collections.Generic;
using UnityEngine;

public class CraftingSlotsArray_Visual : DataDrivenUI<CraftingSlot[]>
{
    [SerializeField] private CraftingSlot_Visual craftingSlotPrefab;
    [SerializeField] private RectTransform craftingSlotsContainer;

    private readonly List<CraftingSlot_Visual> instances;

    private CraftingSlotsArray_Visual()
    {
        instances = new List<CraftingSlot_Visual>();
    }

    protected override void OnValueChanged_Data(CraftingSlot[] oldValue, CraftingSlot[] newValue)
    {
        if (oldValue != null)
        {
            foreach (CraftingSlot_Visual instance in instances)
                GameObject.Destroy(instance.gameObject);

            instances.Clear();
        }

        if (newValue != null)
        {
            foreach(CraftingSlot craftingSlot in newValue)
            {
                CraftingSlot_Visual instance = GameObject.Instantiate(craftingSlotPrefab, craftingSlotsContainer);
                instance.data = craftingSlot;
                instances.Add(instance);
            }
        }
    }
}