namespace SheetCodes
{
	//Generated code, do not edit!

	public enum VillagerIdentifier
	{
		[Identifier("None")] None = 0,
		[Identifier("Male")] Male = 1,
		[Identifier("Female")] Female = 2,
	}

	public static class VillagerIdentifierExtension
	{
		public static VillagerRecord GetRecord(this VillagerIdentifier identifier, bool editableRecord = false)
		{
			return ModelManager.VillagerModel.GetRecord(identifier, editableRecord);
		}
	}
}
