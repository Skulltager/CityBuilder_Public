using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SheetCodes
{
	//Generated code, do not edit!

	[Serializable]
	public class RecipeRecord : BaseRecord<RecipeIdentifier>
	{
		[ColumnName("Crafted Resource")] [SerializeField] private ResourcesIdentifier _craftedResource = default;
		[NonSerialized] private ResourcesRecord _craftedResourceRecord = default;
		public ResourcesRecord CraftedResource 
		{ 
			get 
			{ 
				if(_craftedResourceRecord == null)
					_craftedResourceRecord = ModelManager.ResourcesModel.GetRecord(_craftedResource);
				return _craftedResourceRecord; 
			} 
			set
			{
				if(!CheckEdit())
					return;
					
                if (value == null)
                    _craftedResource = ResourcesIdentifier.None;
                else
                    _craftedResource = value.Identifier;
				_craftedResourceRecord = null;
			}
		}

		[ColumnName("Crafting Time")] [SerializeField] private float _craftingTime = default;
		public float CraftingTime { get { return _craftingTime; } set { if(!CheckEdit()) return; _craftingTime = value; }}

		[ColumnName("Recipe Costs")] [SerializeField] private RecipeOptionIdentifier[] _recipeCosts = default;
		[NonSerialized] private RecipeOptionRecord[] _recipeCostsRecords = default;
		public RecipeOptionRecord[] RecipeCosts 
		{ 
			get 
			{ 
				if(_recipeCostsRecords == null)
				{
					_recipeCostsRecords = new RecipeOptionRecord[_recipeCosts.Length];
					for(int i = 0; i < _recipeCostsRecords.Length; i++)
						_recipeCostsRecords[i] = ModelManager.RecipeOptionModel.GetRecord(_recipeCosts[i]);
				}
				return _recipeCostsRecords; 
			} 
			set
			{
				if(!CheckEdit())
					return;
					
				RecipeOptionIdentifier[] newData = new RecipeOptionIdentifier[value.Length];
				for(int i = 0; i < value.Length; i++)
				{
					RecipeOptionRecord record = value[i];
					if(record == null)
						newData[i] = RecipeOptionIdentifier.None;
					else
						newData[i] = record.Identifier;
				}
				_recipeCosts = newData;
				_recipeCostsRecords = null;
			}
		}

        protected bool runtimeEditingEnabled { get { return originalRecord != null; } }
        public RecipeModel model { get { return ModelManager.RecipeModel; } }
        private RecipeRecord originalRecord = default;

        public override void CreateEditableCopy()
        {
#if UNITY_EDITOR
            if (runtimeEditingEnabled)
                return;

            RecipeRecord editableCopy = new RecipeRecord();
            editableCopy.Identifier = Identifier;
            editableCopy.originalRecord = this;
            CopyData(editableCopy);
            model.SetEditableCopy(editableCopy);
#else
            Debug.LogError("SheetCodes: Creating an editable record does not work in buolds. See documentation 'Editing your data at runtime' for more information.");
#endif
        }

        public override void SaveToScriptableObject()
        {
#if UNITY_EDITOR
            if (!runtimeEditingEnabled)
            {
                Debug.LogWarning("SheetCodes: Runtime Editing is not enabled for this object. Either you are not using the editable copy or you're trying to edit in a build.");
                return;
            }
            CopyData(originalRecord);
            model.SaveModel();
#else
            Debug.LogError("SheetCodes: Saving to ScriptableObject does not work in builds. See documentation 'Editing your data at runtime' for more information.");
#endif
        }

        private void CopyData(RecipeRecord record)
        {
            record._craftedResource = _craftedResource;
            record._craftingTime = _craftingTime;
            record._recipeCosts = _recipeCosts;
        }

        private bool CheckEdit()
        {
            if (runtimeEditingEnabled)
                return true;

            Debug.LogWarning("SheetCodes: Runtime Editing is not enabled for this object. Either you are not using the editable copy or you're trying to edit in a build.");
            return false;
        }
    }
}
