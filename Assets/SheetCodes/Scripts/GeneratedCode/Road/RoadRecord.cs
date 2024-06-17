using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SheetCodes
{
	//Generated code, do not edit!

	[Serializable]
	public class RoadRecord : BaseRecord<RoadIdentifier>
	{
		[ColumnName("Resource Costs")] [SerializeField] private ResourceCostIdentifier[] _resourceCosts = default;
		[NonSerialized] private ResourceCostRecord[] _resourceCostsRecords = default;
		public ResourceCostRecord[] ResourceCosts 
		{ 
			get 
			{ 
				if(_resourceCostsRecords == null)
				{
					_resourceCostsRecords = new ResourceCostRecord[_resourceCosts.Length];
					for(int i = 0; i < _resourceCostsRecords.Length; i++)
						_resourceCostsRecords[i] = ModelManager.ResourceCostModel.GetRecord(_resourceCosts[i]);
				}
				return _resourceCostsRecords; 
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
				_resourceCosts = newData;
				_resourceCostsRecords = null;
			}
		}

		[ColumnName("Construction Time")] [SerializeField] private int _constructionTime = default;
		public int ConstructionTime { get { return _constructionTime; } set { if(!CheckEdit()) return; _constructionTime = value; }}

        protected bool runtimeEditingEnabled { get { return originalRecord != null; } }
        public RoadModel model { get { return ModelManager.RoadModel; } }
        private RoadRecord originalRecord = default;

        public override void CreateEditableCopy()
        {
#if UNITY_EDITOR
            if (runtimeEditingEnabled)
                return;

            RoadRecord editableCopy = new RoadRecord();
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

        private void CopyData(RoadRecord record)
        {
            record._resourceCosts = _resourceCosts;
            record._constructionTime = _constructionTime;
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
