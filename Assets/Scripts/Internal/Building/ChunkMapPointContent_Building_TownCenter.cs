
using SheetCodes;
using System.Collections.Generic;

public class ChunkMapPointContent_Building_TownCenter : ChunkMapPointContent_Building
{
    public readonly List<BuildingTask> villagerTasks;
    public override bool canBeDestroyed => false;
    public override bool allowAnyResourceDumping => true;

    public ChunkMapPointContent_Building_TownCenter(Map map, Point centerPoint, CardinalDirection direction, Player owner, ChunkMapPointContent_BuildingConstruction construction)
        : base(map, centerPoint, direction, BuildingIdentifier.TownCenter.GetRecord(), owner, construction)
    {
        villagerTasks = new List<BuildingTask>();
        foreach (Resource resource in inventory.resources.Values)
            resource.amount.value += 100;
    }

    public ChunkMapPointContent_Building_TownCenter(Map map, Point centerPoint, CardinalDirection direction, Player owner)
        : base(map, centerPoint, direction, BuildingIdentifier.TownCenter.GetRecord(), owner)
    {
        villagerTasks = new List<BuildingTask>();
        foreach (Resource resource in inventory.resources.Values)
            resource.amount.value += 100;
    }

    public override bool TryAssignVillagerTasks(Villager villager)
    {
        villagerTasks.Sort((v1, v2) =>
        {
            int compareResult = v1.fillRate.CompareTo(v2.fillRate);

            if (compareResult != 0)
                return compareResult;

            compareResult = v1.priority.value.CompareTo(v2.priority.value);
            if (compareResult != 0)
                return compareResult;

            compareResult = v1.assignedVillagers.CompareTo(v2.assignedVillagers);

            return compareResult;
        });

        for(int i = 0; i < villagerTasks.Count;i ++)
        {
            BuildingTask villagerTask = villagerTasks[i];

            if (villagerTask.fillRate == BuildingTaskFillRate.Full)
                break;

            if (villagerTask.TryReserveVillagerTaskData(villager))
                return true;
        }

        return false;
    }
}