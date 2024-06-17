namespace SheetCodes
{
	//Generated code, do not edit!

	public enum ResourceSpawnIdentifier
	{
		[Identifier("None")] None = 0,
		[Identifier("Small Trees")] SmallTrees = 1,
		[Identifier("Small Iron Ore")] SmallIronOre = 2,
		[Identifier("Small Berry Bushes")] SmallBerryBushes = 3,
	}

	public static class ResourceSpawnIdentifierExtension
	{
		public static ResourceSpawnRecord GetRecord(this ResourceSpawnIdentifier identifier, bool editableRecord = false)
		{
			return ModelManager.ResourceSpawnModel.GetRecord(identifier, editableRecord);
		}
	}
}
