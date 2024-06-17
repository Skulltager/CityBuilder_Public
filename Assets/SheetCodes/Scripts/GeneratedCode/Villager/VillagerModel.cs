using System;
using UnityEngine;
namespace SheetCodes
{
	//Generated code, this script is only generated once and will not be overwritted!
	//You can add code to this script to iterate your records but don't remove the generated code!

	[Serializable]
	public class VillagerModel : BaseModel<VillagerRecord, VillagerIdentifier>
	{
		[SerializeField] private VillagerRecord[] records = default;
		protected override VillagerRecord[] Records { get { return records; } }

		//Add your code below this line
	}
}
