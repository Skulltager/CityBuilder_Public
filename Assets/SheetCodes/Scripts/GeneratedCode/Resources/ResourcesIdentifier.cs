namespace SheetCodes
{
	//Generated code, do not edit!

	public enum ResourcesIdentifier
	{
		[Identifier("None")] None = 0,
		[Identifier("Iron Ore")] IronOre = 1,
		[Identifier("Berries")] Berries = 2,
		[Identifier("Wood")] Wood = 3,
		[Identifier("Iron Bar")] IronBar = 4,
		[Identifier("Plank")] Plank = 5,
		[Identifier("Tools")] Tools = 6,
	}

	public static class ResourcesIdentifierExtension
	{
		public static ResourcesRecord GetRecord(this ResourcesIdentifier identifier, bool editableRecord = false)
		{
			return ModelManager.ResourcesModel.GetRecord(identifier, editableRecord);
		}
	}
}
