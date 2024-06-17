using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SheetCodes
{
	//Generated code, do not edit!

	[Serializable]
	public class BlockerRecord : BaseRecord<BlockerIdentifier>
	{
		//Does this type no longer exist? Delete from here..
		[ColumnName("Prefab")] [SerializeField] private UnityEngine.GameObject _prefab = default;
		public UnityEngine.GameObject Prefab 
		{ 
			get { return _prefab; } 
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
                _prefab = value;
#endif
            }
        }
		//..To here

		//Does this type no longer exist? Delete from here..
		[ColumnName("Materials")] [SerializeField] private UnityEngine.Material[] _materials = default;
		public UnityEngine.Material[] Materials 
		{ 
			get { return _materials; } 
            set
            {
                if (!CheckEdit())
                    return;
#if UNITY_EDITOR
                if (value == null)
                {
                    Debug.LogWarning("SheetCodes: Array cannot be null. Converting to an empty array instead.");
                    _materials = new UnityEngine.Material[0];
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
					_materials = copyArray;
                }
#endif
            }
        }
		//..To here

        protected bool runtimeEditingEnabled { get { return originalRecord != null; } }
        public BlockerModel model { get { return ModelManager.BlockerModel; } }
        private BlockerRecord originalRecord = default;

        public override void CreateEditableCopy()
        {
#if UNITY_EDITOR
            if (runtimeEditingEnabled)
                return;

            BlockerRecord editableCopy = new BlockerRecord();
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

        private void CopyData(BlockerRecord record)
        {
            record._prefab = _prefab;
            record._materials = _materials;
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
