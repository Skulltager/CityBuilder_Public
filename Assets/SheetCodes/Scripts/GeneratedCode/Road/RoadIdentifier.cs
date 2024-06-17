namespace SheetCodes
{
	//Generated code, do not edit!

	public enum RoadIdentifier
	{
		[Identifier("None")] None = 0,
		[Identifier("Stone")] Stone = 1,
	}

	public static class RoadIdentifierExtension
	{
		public static RoadRecord GetRecord(this RoadIdentifier identifier, bool editableRecord = false)
		{
			return ModelManager.RoadModel.GetRecord(identifier, editableRecord);
		}
	}
}
