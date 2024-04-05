namespace SheetCodes
{
	//Generated code, do not edit!

	public enum BuildingIdentifier
	{
		[Identifier("None")] None = 0,
		[Identifier("House")] House = 1,
		[Identifier("Test")] Test = 2,
	}

	public static class BuildingIdentifierExtension
	{
		public static BuildingRecord GetRecord(this BuildingIdentifier identifier, bool editableRecord = false)
		{
			return ModelManager.BuildingModel.GetRecord(identifier, editableRecord);
		}
	}
}
