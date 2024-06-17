using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SheetCodes
{
	//Generated code, do not edit!

	[Serializable]
	public class ResourceCostRecord : BaseRecord<ResourceCostIdentifier>
	{
		[ColumnName("Resource")] [SerializeField] private ResourcesIdentifier _resource = default;
		[NonSerialized] private ResourcesRecord _resourceRecord = default;
		public ResourcesRecord Resource 
		{ 
			get 
			{ 
				if(_resourceRecord == null)
					_resourceRecord = ModelManager.ResourcesModel.GetRecord(_resource);
				return _resourceRecord; 
			} 
			set
			{
				if(!CheckEdit())
					return;
					
                if (value == null)
                    _resource = ResourcesIdentifier.None;
                else
                    _resource = value.Identifier;
				_resourceRecord = null;
			}
		}

		[ColumnName("Amount")] [SerializeField] private int _amount = default;
		public int Amount { get { return _amount; } set { if(!CheckEdit()) return; _amount = value; }}

        protected bool runtimeEditingEnabled { get { return originalRecord != null; } }
        public ResourceCostModel model { get { return ModelManager.ResourceCostModel; } }
        private ResourceCostRecord originalRecord = default;

        public override void CreateEditableCopy()
        {
#if UNITY_EDITOR
            if (runtimeEditingEnabled)
                return;

            ResourceCostRecord editableCopy = new ResourceCostRecord();
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

        private void CopyData(ResourceCostRecord record)
        {
            record._resource = _resource;
            record._amount = _amount;
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
