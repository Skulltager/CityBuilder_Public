using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SheetCodes
{
	//Generated code, do not edit!

	[Serializable]
	public class RegionsRecord : BaseRecord<RegionsIdentifier>
	{
		[ColumnName("Min Size")] [SerializeField] private int _minSize = default;
		public int MinSize { get { return _minSize; } set { if(!CheckEdit()) return; _minSize = value; }}

		[ColumnName("Max Size")] [SerializeField] private int _maxSize = default;
		public int MaxSize { get { return _maxSize; } set { if(!CheckEdit()) return; _maxSize = value; }}

		[ColumnName("Weight")] [SerializeField] private int _weight = default;
		public int Weight { get { return _weight; } set { if(!CheckEdit()) return; _weight = value; }}

		[ColumnName("Resource Spawns")] [SerializeField] private ResourceSpawnIdentifier[] _resourceSpawns = default;
		[NonSerialized] private ResourceSpawnRecord[] _resourceSpawnsRecords = default;
		public ResourceSpawnRecord[] ResourceSpawns 
		{ 
			get 
			{ 
				if(_resourceSpawnsRecords == null)
				{
					_resourceSpawnsRecords = new ResourceSpawnRecord[_resourceSpawns.Length];
					for(int i = 0; i < _resourceSpawnsRecords.Length; i++)
						_resourceSpawnsRecords[i] = ModelManager.ResourceSpawnModel.GetRecord(_resourceSpawns[i]);
				}
				return _resourceSpawnsRecords; 
			} 
			set
			{
				if(!CheckEdit())
					return;
					
				ResourceSpawnIdentifier[] newData = new ResourceSpawnIdentifier[value.Length];
				for(int i = 0; i < value.Length; i++)
				{
					ResourceSpawnRecord record = value[i];
					if(record == null)
						newData[i] = ResourceSpawnIdentifier.None;
					else
						newData[i] = record.Identifier;
				}
				_resourceSpawns = newData;
				_resourceSpawnsRecords = null;
			}
		}

		[ColumnName("Name")] [SerializeField] private string _name = default;
		public string Name { get { return _name; } set { if(!CheckEdit()) return; _name = value; }}

        protected bool runtimeEditingEnabled { get { return originalRecord != null; } }
        public RegionsModel model { get { return ModelManager.RegionsModel; } }
        private RegionsRecord originalRecord = default;

        public override void CreateEditableCopy()
        {
#if UNITY_EDITOR
            if (runtimeEditingEnabled)
                return;

            RegionsRecord editableCopy = new RegionsRecord();
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

        private void CopyData(RegionsRecord record)
        {
            record._minSize = _minSize;
            record._maxSize = _maxSize;
            record._weight = _weight;
            record._resourceSpawns = _resourceSpawns;
            record._name = _name;
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
