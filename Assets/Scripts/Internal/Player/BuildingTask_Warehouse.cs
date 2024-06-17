using System.Linq;

public class BuildingTask_Warehouse : BuildingTask
{
    public readonly new ChunkMapPointContent_Building_Warehouse building;
    private const int BONUS_CARRY_CAPACITY = 40;

    public BuildingTask_Warehouse(ChunkMapPointContent_Building_Warehouse building, int targetVillagers, int maxVillager)
        : base(building, targetVillagers, maxVillager)
    {
        this.building = building;
    }

    public override bool TryReserveVillagerTaskData(Villager villager)
    {
        if (building.insideEnterExitPoints.Any(i => i.globalPoint == villager.point.value))
        {
            if (TryStartTask_DeliverResources(villager, BONUS_CARRY_CAPACITY))
                return true;

            if (TryStartTask_OffloadResources(villager, BONUS_CARRY_CAPACITY))
                return true;

            if (TryStartTask_GetDesiredResources(villager, BONUS_CARRY_CAPACITY))
                return true;
        }
        else
        {
            if (TryStartTask_GetDesiredResources(villager, BONUS_CARRY_CAPACITY))
                return true;

            if (TryStartTask_DeliverResources(villager, BONUS_CARRY_CAPACITY))
                return true;

            if (TryStartTask_OffloadResources(villager, BONUS_CARRY_CAPACITY))
                return true;
        }

        return false;
    }
}