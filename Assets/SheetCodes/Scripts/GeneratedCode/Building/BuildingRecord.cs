using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SheetCodes
{
	//Generated code, do not edit!

	[Serializable]
	public class BuildingRecord : BaseRecord<BuildingIdentifier>
	{
		[ColumnName("Name")] [SerializeField] private string _name = default;
		public string Name { get { return _name; } set { if(!CheckEdit()) return; _name = value; }}

		[ColumnName("Starting Limit")] [SerializeField] private int _startingLimit = default;
		public int StartingLimit { get { return _startingLimit; } set { if(!CheckEdit()) return; _startingLimit = value; }}

		//Does this type no longer exist? Delete from here..
		[ColumnName("Building Grid Data")] [SerializeField] private BuildingGridData _buildingGridData = default;
		public BuildingGridData BuildingGridData 
		{ 
			get { return _buildingGridData; } 
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
                _buildingGridData = value;
#endif
            }
        }
		//..To here

		//Does this type no longer exist? Delete from here..
		[ColumnName("Building Placement Prefab")] [SerializeField] private UnityEngine.GameObject _buildingPlacementPrefab = default;
		public UnityEngine.GameObject BuildingPlacementPrefab 
		{ 
			get { return _buildingPlacementPrefab; } 
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
                _buildingPlacementPrefab = value;
#endif
            }
        }
		//..To here

		//Does this type no longer exist? Delete from here..
		[ColumnName("Building Prefab")] [SerializeField] private BuildingVisual _buildingPrefab = default;
		public BuildingVisual BuildingPrefab 
		{ 
			get { return _buildingPrefab; } 
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
                _buildingPrefab = value;
#endif
            }
        }
		//..To here

		//Does this type no longer exist? Delete from here..
		[ColumnName("Building Deconstruction Prefab")] [SerializeField] private BuildingDeconstructionVisual _buildingDeconstructionPrefab = default;
		public BuildingDeconstructionVisual BuildingDeconstructionPrefab 
		{ 
			get { return _buildingDeconstructionPrefab; } 
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
                _buildingDeconstructionPrefab = value;
#endif
            }
        }
		//..To here

		//Does this type no longer exist? Delete from here..
		[ColumnName("Building Construction Prefab")] [SerializeField] private BuildingConstructionVisual _buildingConstructionPrefab = default;
		public BuildingConstructionVisual BuildingConstructionPrefab 
		{ 
			get { return _buildingConstructionPrefab; } 
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
                _buildingConstructionPrefab = value;
#endif
            }
        }
		//..To here

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

		[ColumnName("Construction Duration")] [SerializeField] private float _constructionDuration = default;
		public float ConstructionDuration { get { return _constructionDuration; } set { if(!CheckEdit()) return; _constructionDuration = value; }}

		[ColumnName("Maximum Assigned Villagers")] [SerializeField] private int _maximumAssignedVillagers = default;
		public int MaximumAssignedVillagers { get { return _maximumAssignedVillagers; } set { if(!CheckEdit()) return; _maximumAssignedVillagers = value; }}

		[ColumnName("Infinite Storage")] [SerializeField] private bool _infiniteStorage = default;
		public bool InfiniteStorage { get { return _infiniteStorage; } set { if(!CheckEdit()) return; _infiniteStorage = value; }}

		[ColumnName("Inventory Size")] [SerializeField] private int _inventorySize = default;
		public int InventorySize { get { return _inventorySize; } set { if(!CheckEdit()) return; _inventorySize = value; }}

		[ColumnName("Can Build")] [SerializeField] private bool _canBuild = default;
		public bool CanBuild { get { return _canBuild; } set { if(!CheckEdit()) return; _canBuild = value; }}

		[ColumnName("Crafting Recipes")] [SerializeField] private RecipeIdentifier _craftingRecipes = default;
		[NonSerialized] private RecipeRecord _craftingRecipesRecord = default;
		public RecipeRecord CraftingRecipes 
		{ 
			get 
			{ 
				if(_craftingRecipesRecord == null)
					_craftingRecipesRecord = ModelManager.RecipeModel.GetRecord(_craftingRecipes);
				return _craftingRecipesRecord; 
			} 
			set
			{
				if(!CheckEdit())
					return;
					
                if (value == null)
                    _craftingRecipes = RecipeIdentifier.None;
                else
                    _craftingRecipes = value.Identifier;
				_craftingRecipesRecord = null;
			}
		}

		[ColumnName("Gathering Range")] [SerializeField] private int _gatheringRange = default;
		public int GatheringRange { get { return _gatheringRange; } set { if(!CheckEdit()) return; _gatheringRange = value; }}

		//Does this type no longer exist? Delete from here..
		[ColumnName("Gatherable Type")] [SerializeField] private ResourceTypeFlags _gatherableType = default;
		public ResourceTypeFlags GatherableType { get { return _gatherableType; } set { if(!CheckEdit()) return; _gatherableType = value; }}
		//..To here

		[ColumnName("Adjustable Storage Demand")] [SerializeField] private bool _adjustableStorageDemand = default;
		public bool AdjustableStorageDemand { get { return _adjustableStorageDemand; } set { if(!CheckEdit()) return; _adjustableStorageDemand = value; }}

        protected bool runtimeEditingEnabled { get { return originalRecord != null; } }
        public BuildingModel model { get { return ModelManager.BuildingModel; } }
        private BuildingRecord originalRecord = default;

        public override void CreateEditableCopy()
        {
#if UNITY_EDITOR
            if (runtimeEditingEnabled)
                return;

            BuildingRecord editableCopy = new BuildingRecord();
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

        private void CopyData(BuildingRecord record)
        {
            record._name = _name;
            record._startingLimit = _startingLimit;
            record._buildingGridData = _buildingGridData;
            record._buildingPlacementPrefab = _buildingPlacementPrefab;
            record._buildingPrefab = _buildingPrefab;
            record._buildingDeconstructionPrefab = _buildingDeconstructionPrefab;
            record._buildingConstructionPrefab = _buildingConstructionPrefab;
            record._resourceCosts = _resourceCosts;
            record._constructionDuration = _constructionDuration;
            record._maximumAssignedVillagers = _maximumAssignedVillagers;
            record._infiniteStorage = _infiniteStorage;
            record._inventorySize = _inventorySize;
            record._canBuild = _canBuild;
            record._craftingRecipes = _craftingRecipes;
            record._gatheringRange = _gatheringRange;
            record._gatherableType = _gatherableType;
            record._adjustableStorageDemand = _adjustableStorageDemand;
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
