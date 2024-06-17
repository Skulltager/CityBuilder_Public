namespace SheetCodes
{
	//Generated code, do not edit!

	public enum RegionsIdentifier
	{
		[Identifier("None")] None = 0,
		[Identifier("Border")] Border = 6,
		[Identifier("Player Start")] PlayerStart = 1,
		[Identifier("Small Trees")] SmallTrees = 2,
		[Identifier("Small Iron Ore")] SmallIronOre = 3,
		[Identifier("Small Berry Bushes")] SmallBerryBushes = 4,
		[Identifier("Medium")] Medium = 5,
	}

	public static class RegionsIdentifierExtension
	{
		public static RegionsRecord GetRecord(this RegionsIdentifier identifier, bool editableRecord = false)
		{
			return ModelManager.RegionsModel.GetRecord(identifier, editableRecord);
		}
	}
}
