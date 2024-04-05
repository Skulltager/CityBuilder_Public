namespace SheetCodes
{
	//Generated code, do not edit!

	public enum WorldResourceIdentifier
	{
		[Identifier("None")] None = 0,
		[Identifier("Tree")] Tree = 1,
		[Identifier("Iron Ore")] IronOre = 2,
		[Identifier("Berry Bush")] BerryBush = 3,
	}

	public static class WorldResourceIdentifierExtension
	{
		public static WorldResourceRecord GetRecord(this WorldResourceIdentifier identifier, bool editableRecord = false)
		{
			return ModelManager.WorldResourceModel.GetRecord(identifier, editableRecord);
		}
	}
}
