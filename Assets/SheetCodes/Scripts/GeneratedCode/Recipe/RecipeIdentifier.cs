namespace SheetCodes
{
	//Generated code, do not edit!

	public enum RecipeIdentifier
	{
		[Identifier("None")] None = 0,
		[Identifier("Iron Bar")] IronBar = 1,
		[Identifier("Tools")] Tools = 2,
	}

	public static class RecipeIdentifierExtension
	{
		public static RecipeRecord GetRecord(this RecipeIdentifier identifier, bool editableRecord = false)
		{
			return ModelManager.RecipeModel.GetRecord(identifier, editableRecord);
		}
	}
}
