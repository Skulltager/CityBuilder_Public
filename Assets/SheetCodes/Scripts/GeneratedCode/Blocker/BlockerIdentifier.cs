namespace SheetCodes
{
	//Generated code, do not edit!

	public enum BlockerIdentifier
	{
		[Identifier("None")] None = 0,
		[Identifier("Wall")] Wall = 1,
	}

	public static class BlockerIdentifierExtension
	{
		public static BlockerRecord GetRecord(this BlockerIdentifier identifier, bool editableRecord = false)
		{
			return ModelManager.BlockerModel.GetRecord(identifier, editableRecord);
		}
	}
}
