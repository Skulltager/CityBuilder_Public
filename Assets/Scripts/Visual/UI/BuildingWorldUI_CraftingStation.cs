using UnityEngine;

public class BuildingWorldUI_CraftingStation : DataDrivenWorldUI<ChunkMapPointContent_Building_CraftingStation>
{
    [SerializeField] private CraftingSlotsArray_Visual craftingSlotsVisual;

    public override Vector3 followPosition => offset;
    private Vector3 offset;

    protected override void OnValueChanged_Data(ChunkMapPointContent_Building_CraftingStation oldValue, ChunkMapPointContent_Building_CraftingStation newValue)
    {
        if (oldValue != null)
        {
            craftingSlotsVisual.data = null;
        }

        if (newValue != null)
        {
            craftingSlotsVisual.data = newValue.craftingSlots;
            offset = new Vector3(newValue.centerPosition.x, heightOffset, newValue.centerPosition.y);
        }
    }
}