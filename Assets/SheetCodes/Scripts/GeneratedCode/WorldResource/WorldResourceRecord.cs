using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SheetCodes
{
	//Generated code, do not edit!

	[Serializable]
	public class WorldResourceRecord : BaseRecord<WorldResourceIdentifier>
	{
		//Does this type no longer exist? Delete from here..
		[ColumnName("Model Variations")] [SerializeField] private UnityEngine.GameObject[] _modelVariations = default;
		public UnityEngine.GameObject[] ModelVariations 
		{ 
			get { return _modelVariations; } 
            set
            {
                if (!CheckEdit())
                    return;
#if UNITY_EDITOR
                if (value == null)
                {
                    Debug.LogWarning("SheetCodes: Array cannot be null. Converting to an empty array instead.");
                    _modelVariations = new UnityEngine.GameObject[0];
                }
                else
                {
                    UnityEngine.GameObject[] copyArray = new UnityEngine.GameObject[value.Length];
                    for(int i = 0; i < value.Length; i++)
                    {
                        UnityEngine.GameObject item = value[i];
                        if (item != null)
                        {
                            string assetPath = AssetDatabase.GetAssetPath(item);
                            if (string.IsNullOrEmpty(assetPath))
                            {
                                Debug.LogError(string.Format("SheetCodes: Item at element {0} is not a direct reference from your project folder. Index value set to null.", i));
                                copyArray[i] = null;
                                continue;
                            }
                            else
                                copyArray[i] = item;
                        }
                        else
                            copyArray[i] = null;
                    }
					_modelVariations = copyArray;
                }
#endif
            }
        }
		//..To here

		//Does this type no longer exist? Delete from here..
		[ColumnName("Material")] [SerializeField] private UnityEngine.Material[] _material = default;
		public UnityEngine.Material[] Material 
		{ 
			get { return _material; } 
            set
            {
                if (!CheckEdit())
                    return;
#if UNITY_EDITOR
                if (value == null)
                {
                    Debug.LogWarning("SheetCodes: Array cannot be null. Converting to an empty array instead.");
                    _material = new UnityEngine.Material[0];
                }
                else
                {
                    UnityEngine.Material[] copyArray = new UnityEngine.Material[value.Length];
                    for(int i = 0; i < value.Length; i++)
                    {
                        UnityEngine.Material item = value[i];
                        if (item != null)
                        {
                            string assetPath = AssetDatabase.GetAssetPath(item);
                            if (string.IsNullOrEmpty(assetPath))
                            {
                                Debug.LogError(string.Format("SheetCodes: Item at element {0} is not a direct reference from your project folder. Index value set to null.", i));
                                copyArray[i] = null;
                                continue;
                            }
                            else
                                copyArray[i] = item;
                        }
                        else
                            copyArray[i] = null;
                    }
					_material = copyArray;
                }
#endif
            }
        }
		//..To here

		[ColumnName("Resource Drops")] [SerializeField] private ResourcesIdentifier _resourceDrops = default;
		[NonSerialized] private ResourcesRecord _resourceDropsRecord = default;
		public ResourcesRecord ResourceDrops 
		{ 
			get 
			{ 
				if(_resourceDropsRecord == null)
					_resourceDropsRecord = ModelManager.ResourcesModel.GetRecord(_resourceDrops);
				return _resourceDropsRecord; 
			} 
			set
			{
				if(!CheckEdit())
					return;
					
                if (value == null)
                    _resourceDrops = ResourcesIdentifier.None;
                else
                    _resourceDrops = value.Identifier;
				_resourceDropsRecord = null;
			}
		}

		[ColumnName("Harvest Duration")] [SerializeField] private float _harvestDuration = default;
		public float HarvestDuration { get { return _harvestDuration; } set { if(!CheckEdit()) return; _harvestDuration = value; }}

		//Does this type no longer exist? Delete from here..
		[ColumnName("Resource Type")] [SerializeField] private ResourceType _resourceType = default;
		public ResourceType ResourceType { get { return _resourceType; } set { if(!CheckEdit()) return; _resourceType = value; }}
		//..To here

		[ColumnName("Name")] [SerializeField] private string _name = default;
		public string Name { get { return _name; } set { if(!CheckEdit()) return; _name = value; }}

		[ColumnName("Times Harvestable")] [SerializeField] private int _timesHarvestable = default;
		public int TimesHarvestable { get { return _timesHarvestable; } set { if(!CheckEdit()) return; _timesHarvestable = value; }}

		//Does this type no longer exist? Delete from here..
		[ColumnName("Icon")] [SerializeField] private UnityEngine.Sprite _icon = default;
		public UnityEngine.Sprite Icon 
		{ 
			get { return _icon; } 
            set
            {
                if (!CheckEdit())
                    return;
#if UNITY_EDITOR
                if (value != null)
                {
                    string assetPath = AssetDatabase.GetAssetPath(value);
                    if(string.IsNullOrEmpty(assetPath))
                    {
                        Debug.LogError("SheetCodes: Reference Objects must be a direct reference from your project folder.");
                        return;
                    }
                }
                _icon = value;
#endif
            }
        }
		//..To here

        protected bool runtimeEditingEnabled { get { return originalRecord != null; } }
        public WorldResourceModel model { get { return ModelManager.WorldResourceModel; } }
        private WorldResourceRecord originalRecord = default;

        public override void CreateEditableCopy()
        {
#if UNITY_EDITOR
            if (runtimeEditingEnabled)
                return;

            WorldResourceRecord editableCopy = new WorldResourceRecord();
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

        private void CopyData(WorldResourceRecord record)
        {
            record._modelVariations = _modelVariations;
            record._material = _material;
            record._resourceDrops = _resourceDrops;
            record._harvestDuration = _harvestDuration;
            record._resourceType = _resourceType;
            record._name = _name;
            record._timesHarvestable = _timesHarvestable;
            record._icon = _icon;
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
