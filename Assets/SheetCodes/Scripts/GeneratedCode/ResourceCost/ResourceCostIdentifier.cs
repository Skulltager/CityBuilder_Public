namespace SheetCodes
{
	//Generated code, do not edit!

	public enum ResourceCostIdentifier
	{
		[Identifier("None")] None = 0,
		[Identifier("House - Wood")] HouseWood = 1,
		[Identifier("House - Berries")] HouseBerries = 2,
		[Identifier("Crafting - Station Wood")] CraftingStationWood = 3,
		[Identifier("Crafting - Station Iron")] CraftingStationIron = 4,
		[Identifier("Iron Bar - Iron Ore")] IronBarIronOre = 5,
		[Identifier("Tool - Iron Bar")] ToolIronBar = 6,
		[Identifier("Tool - Plank")] ToolPlank = 7,
		[Identifier("Stone Road - Iron Ore")] StoneRoadIronOre = 8,
	}

	public static class ResourceCostIdentifierExtension
	{
		public static ResourceCostRecord GetRecord(this ResourceCostIdentifier identifier, bool editableRecord = false)
		{
			return ModelManager.ResourceCostModel.GetRecord(identifier, editableRecord);
		}
	}
}
