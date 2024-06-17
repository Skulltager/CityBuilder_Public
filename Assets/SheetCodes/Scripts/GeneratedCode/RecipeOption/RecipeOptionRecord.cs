using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SheetCodes
{
	//Generated code, do not edit!

	[Serializable]
	public class RecipeOptionRecord : BaseRecord<RecipeOptionIdentifier>
	{
		[ColumnName("Resource Cost")] [SerializeField] private ResourceCostIdentifier[] _resourceCost = default;
		[NonSerialized] private ResourceCostRecord[] _resourceCostRecords = default;
		public ResourceCostRecord[] ResourceCost 
		{ 
			get 
			{ 
				if(_resourceCostRecords == null)
				{
					_resourceCostRecords = new ResourceCostRecord[_resourceCost.Length];
					for(int i = 0; i < _resourceCostRecords.Length; i++)
						_resourceCostRecords[i] = ModelManager.ResourceCostModel.GetRecord(_resourceCost[i]);
				}
				return _resourceCostRecords; 
			} 
			set
			{
				if(!CheckEdit())
					return;
					
				ResourceCostIdentifier[] newData = new ResourceCostIdentifier[value.Length];
				for(int i = 0; i < value.Length; i++)
				{
					ResourceCostRecord record = value[i];
					if(record == null)
						newData[i] = ResourceCostIdentifier.None;
					else
						newData[i] = record.Identifier;
				}
				_resourceCost = newData;
				_resourceCostRecords = null;
			}
		}

        protected bool runtimeEditingEnabled { get { return originalRecord != null; } }
        public RecipeOptionModel model { get { return ModelManager.RecipeOptionModel; } }
        private RecipeOptionRecord originalRecord = default;

        public override void CreateEditableCopy()
        {
#if UNITY_EDITOR
            if (runtimeEditingEnabled)
                return;

            RecipeOptionRecord editableCopy = new RecipeOptionRecord();
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

        private void CopyData(RecipeOptionRecord record)
        {
            record._resourceCost = _resourceCost;
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
