namespace SheetCodes
{
	//Generated code, do not edit!

	public enum BuildingIdentifier
	{
		[Identifier("None")] None = 0,
		[Identifier("House")] House = 1,
		[Identifier("Town Center")] TownCenter = 2,
		[Identifier("Crafting Station")] CraftingStation = 3,
		[Identifier("Gathering Hut")] GatheringHut = 4,
		[Identifier("Warehouse")] Warehouse = 5,
		[Identifier("Woodcutters Hut")] WoodcuttersHut = 6,
		[Identifier("Mining Camp")] MiningCamp = 7,
		[Identifier("Road Construction")] RoadConstruction = 8,
	}

	public static class BuildingIdentifierExtension
	{
		public static BuildingRecord GetRecord(this BuildingIdentifier identifier, bool editableRecord = false)
		{
			return ModelManager.BuildingModel.GetRecord(identifier, editableRecord);
		}
	}
}
