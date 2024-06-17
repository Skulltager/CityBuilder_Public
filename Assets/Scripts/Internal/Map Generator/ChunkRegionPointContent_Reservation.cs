using SheetCodes;

public class ChunkRegionPointContent_Reservation : ChunkRegionPointContent
{
    public int enterExitCount;
    public bool spawnVillager;

    public ChunkRegionPointContent_Reservation(Map map, Point point)
        : base(map, point) { }

    public override void SetChunkMapPointData(ChunkMapPoint chunkMapPoint)
    {
        chunkMapPoint.blockPreventionCounter.value += enterExitCount;

        if(spawnVillager)
        {
            string villagerName = VillagerNames.GetRandomName(map.random);
            VillagerIdentifier identifier = UnityEngine.Random.value < 0.5f ? VillagerIdentifier.Male : VillagerIdentifier.Female;
            Villager villager = new Villager(chunkMapPoint.chunkRegion.value.owner, identifier.GetRecord(), chunkMapPoint, villagerName);
            chunkMapPoint.chunkRegion.value.owner.villagers.Add(villager);
        }
    }
}