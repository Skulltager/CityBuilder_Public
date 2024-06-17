namespace SheetCodes
{
	//Generated code, do not edit!

	public enum RecipeOptionIdentifier
	{
		[Identifier("None")] None = 0,
		[Identifier("Iron Bar 1")] IronBar1 = 3,
		[Identifier("Tool 1")] Tool1 = 1,
	}

	public static class RecipeOptionIdentifierExtension
	{
		public static RecipeOptionRecord GetRecord(this RecipeOptionIdentifier identifier, bool editableRecord = false)
		{
			return ModelManager.RecipeOptionModel.GetRecord(identifier, editableRecord);
		}
	}
}
