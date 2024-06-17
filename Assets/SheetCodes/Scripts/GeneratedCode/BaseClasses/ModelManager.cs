using System;
using System.Collections.Generic;
using UnityEngine;

namespace SheetCodes
{
	//Generated code, do not edit!

	public static class ModelManager
	{
        private static Dictionary<DatasheetType, LoadRequest> loadRequests;

        static ModelManager()
        {
            loadRequests = new Dictionary<DatasheetType, LoadRequest>();
        }

        public static void InitializeAll()
        {
            DatasheetType[] values = Enum.GetValues(typeof(DatasheetType)) as DatasheetType[];
            foreach(DatasheetType value in values)
                Initialize(value);
        }
		
        public static void Unload(DatasheetType datasheetType)
        {
            switch (datasheetType)
            {
                case DatasheetType.WorldResource:
                    {
                        if (worldResourceModel == null || worldResourceModel.Equals(null))
                        {
                            Log(string.Format("Sheet Codes: Trying to unload model {0}. Model is not loaded.", datasheetType));
                            break;
                        }
                        Resources.UnloadAsset(worldResourceModel);
                        worldResourceModel = null;
                        LoadRequest request;
                        if (loadRequests.TryGetValue(DatasheetType.WorldResource, out request))
                        {
                            loadRequests.Remove(DatasheetType.WorldResource);
                            request.resourceRequest.completed -= OnLoadCompleted_WorldResourceModel;
							foreach (Action<bool> callback in request.callbacks)
								callback(false);
                        }
                        break;
                    }
                case DatasheetType.Building:
                    {
                        if (buildingModel == null || buildingModel.Equals(null))
                        {
                            Log(string.Format("Sheet Codes: Trying to unload model {0}. Model is not loaded.", datasheetType));
                            break;
                        }
                        Resources.UnloadAsset(buildingModel);
                        buildingModel = null;
                        LoadRequest request;
                        if (loadRequests.TryGetValue(DatasheetType.Building, out request))
                        {
                            loadRequests.Remove(DatasheetType.Building);
                            request.resourceRequest.completed -= OnLoadCompleted_BuildingModel;
							foreach (Action<bool> callback in request.callbacks)
								callback(false);
                        }
                        break;
                    }
                case DatasheetType.Resources:
                    {
                        if (resourcesModel == null || resourcesModel.Equals(null))
                        {
                            Log(string.Format("Sheet Codes: Trying to unload model {0}. Model is not loaded.", datasheetType));
                            break;
                        }
                        Resources.UnloadAsset(resourcesModel);
                        resourcesModel = null;
                        LoadRequest request;
                        if (loadRequests.TryGetValue(DatasheetType.Resources, out request))
                        {
                            loadRequests.Remove(DatasheetType.Resources);
                            request.resourceRequest.completed -= OnLoadCompleted_ResourcesModel;
							foreach (Action<bool> callback in request.callbacks)
								callback(false);
                        }
                        break;
                    }
                case DatasheetType.ResourceCost:
                    {
                        if (resourceCostModel == null || resourceCostModel.Equals(null))
                        {
                            Log(string.Format("Sheet Codes: Trying to unload model {0}. Model is not loaded.", datasheetType));
                            break;
                        }
                        Resources.UnloadAsset(resourceCostModel);
                        resourceCostModel = null;
                        LoadRequest request;
                        if (loadRequests.TryGetValue(DatasheetType.ResourceCost, out request))
                        {
                            loadRequests.Remove(DatasheetType.ResourceCost);
                            request.resourceRequest.completed -= OnLoadCompleted_ResourceCostModel;
							foreach (Action<bool> callback in request.callbacks)
								callback(false);
                        }
                        break;
                    }
                case DatasheetType.Blocker:
                    {
                        if (blockerModel == null || blockerModel.Equals(null))
                        {
                            Log(string.Format("Sheet Codes: Trying to unload model {0}. Model is not loaded.", datasheetType));
                            break;
                        }
                        Resources.UnloadAsset(blockerModel);
                        blockerModel = null;
                        LoadRequest request;
                        if (loadRequests.TryGetValue(DatasheetType.Blocker, out request))
                        {
                            loadRequests.Remove(DatasheetType.Blocker);
                            request.resourceRequest.completed -= OnLoadCompleted_BlockerModel;
							foreach (Action<bool> callback in request.callbacks)
								callback(false);
                        }
                        break;
                    }
                case DatasheetType.Regions:
                    {
                        if (regionsModel == null || regionsModel.Equals(null))
                        {
                            Log(string.Format("Sheet Codes: Trying to unload model {0}. Model is not loaded.", datasheetType));
                            break;
                        }
                        Resources.UnloadAsset(regionsModel);
                        regionsModel = null;
                        LoadRequest request;
                        if (loadRequests.TryGetValue(DatasheetType.Regions, out request))
                        {
                            loadRequests.Remove(DatasheetType.Regions);
                            request.resourceRequest.completed -= OnLoadCompleted_RegionsModel;
							foreach (Action<bool> callback in request.callbacks)
								callback(false);
                        }
                        break;
                    }
                case DatasheetType.ResourceSpawn:
                    {
                        if (resourceSpawnModel == null || resourceSpawnModel.Equals(null))
                        {
                            Log(string.Format("Sheet Codes: Trying to unload model {0}. Model is not loaded.", datasheetType));
                            break;
                        }
                        Resources.UnloadAsset(resourceSpawnModel);
                        resourceSpawnModel = null;
                        LoadRequest request;
                        if (loadRequests.TryGetValue(DatasheetType.ResourceSpawn, out request))
                        {
                            loadRequests.Remove(DatasheetType.ResourceSpawn);
                            request.resourceRequest.completed -= OnLoadCompleted_ResourceSpawnModel;
							foreach (Action<bool> callback in request.callbacks)
								callback(false);
                        }
                        break;
                    }
                case DatasheetType.Villager:
                    {
                        if (villagerModel == null || villagerModel.Equals(null))
                        {
                            Log(string.Format("Sheet Codes: Trying to unload model {0}. Model is not loaded.", datasheetType));
                            break;
                        }
                        Resources.UnloadAsset(villagerModel);
                        villagerModel = null;
                        LoadRequest request;
                        if (loadRequests.TryGetValue(DatasheetType.Villager, out request))
                        {
                            loadRequests.Remove(DatasheetType.Villager);
                            request.resourceRequest.completed -= OnLoadCompleted_VillagerModel;
							foreach (Action<bool> callback in request.callbacks)
								callback(false);
                        }
                        break;
                    }
                case DatasheetType.Recipe:
                    {
                        if (recipeModel == null || recipeModel.Equals(null))
                        {
                            Log(string.Format("Sheet Codes: Trying to unload model {0}. Model is not loaded.", datasheetType));
                            break;
                        }
                        Resources.UnloadAsset(recipeModel);
                        recipeModel = null;
                        LoadRequest request;
                        if (loadRequests.TryGetValue(DatasheetType.Recipe, out request))
                        {
                            loadRequests.Remove(DatasheetType.Recipe);
                            request.resourceRequest.completed -= OnLoadCompleted_RecipeModel;
							foreach (Action<bool> callback in request.callbacks)
								callback(false);
                        }
                        break;
                    }
                case DatasheetType.RecipeOption:
                    {
                        if (recipeOptionModel == null || recipeOptionModel.Equals(null))
                        {
                            Log(string.Format("Sheet Codes: Trying to unload model {0}. Model is not loaded.", datasheetType));
                            break;
                        }
                        Resources.UnloadAsset(recipeOptionModel);
                        recipeOptionModel = null;
                        LoadRequest request;
                        if (loadRequests.TryGetValue(DatasheetType.RecipeOption, out request))
                        {
                            loadRequests.Remove(DatasheetType.RecipeOption);
                            request.resourceRequest.completed -= OnLoadCompleted_RecipeOptionModel;
							foreach (Action<bool> callback in request.callbacks)
								callback(false);
                        }
                        break;
                    }
                case DatasheetType.Road:
                    {
                        if (roadModel == null || roadModel.Equals(null))
                        {
                            Log(string.Format("Sheet Codes: Trying to unload model {0}. Model is not loaded.", datasheetType));
                            break;
                        }
                        Resources.UnloadAsset(roadModel);
                        roadModel = null;
                        LoadRequest request;
                        if (loadRequests.TryGetValue(DatasheetType.Road, out request))
                        {
                            loadRequests.Remove(DatasheetType.Road);
                            request.resourceRequest.completed -= OnLoadCompleted_RoadModel;
							foreach (Action<bool> callback in request.callbacks)
								callback(false);
                        }
                        break;
                    }
                default:
                    break;
            }
        }

        public static void Initialize(DatasheetType datasheetType)
        {
            switch (datasheetType)
            {
                case DatasheetType.WorldResource:
                    {
                        if (worldResourceModel != null && !worldResourceModel.Equals(null))
                        {
                            Log(string.Format("Sheet Codes: Trying to Initialize {0}. Model is already initialized.", datasheetType));
                            break;
                        }

                        worldResourceModel = Resources.Load<WorldResourceModel>("ScriptableObjects/WorldResource");
                        LoadRequest request;
                        if (loadRequests.TryGetValue(DatasheetType.WorldResource, out request))
                        {
                            Log(string.Format("Sheet Codes: Trying to initialize {0} while also async loading. Async load has been canceled.", datasheetType));
                            loadRequests.Remove(DatasheetType.WorldResource);
                            request.resourceRequest.completed -= OnLoadCompleted_WorldResourceModel;
							foreach (Action<bool> callback in request.callbacks)
								callback(true);
                        }
                        break;
                    }
                case DatasheetType.Building:
                    {
                        if (buildingModel != null && !buildingModel.Equals(null))
                        {
                            Log(string.Format("Sheet Codes: Trying to Initialize {0}. Model is already initialized.", datasheetType));
                            break;
                        }

                        buildingModel = Resources.Load<BuildingModel>("ScriptableObjects/Building");
                        LoadRequest request;
                        if (loadRequests.TryGetValue(DatasheetType.Building, out request))
                        {
                            Log(string.Format("Sheet Codes: Trying to initialize {0} while also async loading. Async load has been canceled.", datasheetType));
                            loadRequests.Remove(DatasheetType.Building);
                            request.resourceRequest.completed -= OnLoadCompleted_BuildingModel;
							foreach (Action<bool> callback in request.callbacks)
								callback(true);
                        }
                        break;
                    }
                case DatasheetType.Resources:
                    {
                        if (resourcesModel != null && !resourcesModel.Equals(null))
                        {
                            Log(string.Format("Sheet Codes: Trying to Initialize {0}. Model is already initialized.", datasheetType));
                            break;
                        }

                        resourcesModel = Resources.Load<ResourcesModel>("ScriptableObjects/Resources");
                        LoadRequest request;
                        if (loadRequests.TryGetValue(DatasheetType.Resources, out request))
                        {
                            Log(string.Format("Sheet Codes: Trying to initialize {0} while also async loading. Async load has been canceled.", datasheetType));
                            loadRequests.Remove(DatasheetType.Resources);
                            request.resourceRequest.completed -= OnLoadCompleted_ResourcesModel;
							foreach (Action<bool> callback in request.callbacks)
								callback(true);
                        }
                        break;
                    }
                case DatasheetType.ResourceCost:
                    {
                        if (resourceCostModel != null && !resourceCostModel.Equals(null))
                        {
                            Log(string.Format("Sheet Codes: Trying to Initialize {0}. Model is already initialized.", datasheetType));
                            break;
                        }

                        resourceCostModel = Resources.Load<ResourceCostModel>("ScriptableObjects/ResourceCost");
                        LoadRequest request;
                        if (loadRequests.TryGetValue(DatasheetType.ResourceCost, out request))
                        {
                            Log(string.Format("Sheet Codes: Trying to initialize {0} while also async loading. Async load has been canceled.", datasheetType));
                            loadRequests.Remove(DatasheetType.ResourceCost);
                            request.resourceRequest.completed -= OnLoadCompleted_ResourceCostModel;
							foreach (Action<bool> callback in request.callbacks)
								callback(true);
                        }
                        break;
                    }
                case DatasheetType.Blocker:
                    {
                        if (blockerModel != null && !blockerModel.Equals(null))
                        {
                            Log(string.Format("Sheet Codes: Trying to Initialize {0}. Model is already initialized.", datasheetType));
                            break;
                        }

                        blockerModel = Resources.Load<BlockerModel>("ScriptableObjects/Blocker");
                        LoadRequest request;
                        if (loadRequests.TryGetValue(DatasheetType.Blocker, out request))
                        {
                            Log(string.Format("Sheet Codes: Trying to initialize {0} while also async loading. Async load has been canceled.", datasheetType));
                            loadRequests.Remove(DatasheetType.Blocker);
                            request.resourceRequest.completed -= OnLoadCompleted_BlockerModel;
							foreach (Action<bool> callback in request.callbacks)
								callback(true);
                        }
                        break;
                    }
                case DatasheetType.Regions:
                    {
                        if (regionsModel != null && !regionsModel.Equals(null))
                        {
                            Log(string.Format("Sheet Codes: Trying to Initialize {0}. Model is already initialized.", datasheetType));
                            break;
                        }

                        regionsModel = Resources.Load<RegionsModel>("ScriptableObjects/Regions");
                        LoadRequest request;
                        if (loadRequests.TryGetValue(DatasheetType.Regions, out request))
                        {
                            Log(string.Format("Sheet Codes: Trying to initialize {0} while also async loading. Async load has been canceled.", datasheetType));
                            loadRequests.Remove(DatasheetType.Regions);
                            request.resourceRequest.completed -= OnLoadCompleted_RegionsModel;
							foreach (Action<bool> callback in request.callbacks)
								callback(true);
                        }
                        break;
                    }
                case DatasheetType.ResourceSpawn:
                    {
                        if (resourceSpawnModel != null && !resourceSpawnModel.Equals(null))
                        {
                            Log(string.Format("Sheet Codes: Trying to Initialize {0}. Model is already initialized.", datasheetType));
                            break;
                        }

                        resourceSpawnModel = Resources.Load<ResourceSpawnModel>("ScriptableObjects/ResourceSpawn");
                        LoadRequest request;
                        if (loadRequests.TryGetValue(DatasheetType.ResourceSpawn, out request))
                        {
                            Log(string.Format("Sheet Codes: Trying to initialize {0} while also async loading. Async load has been canceled.", datasheetType));
                            loadRequests.Remove(DatasheetType.ResourceSpawn);
                            request.resourceRequest.completed -= OnLoadCompleted_ResourceSpawnModel;
							foreach (Action<bool> callback in request.callbacks)
								callback(true);
                        }
                        break;
                    }
                case DatasheetType.Villager:
                    {
                        if (villagerModel != null && !villagerModel.Equals(null))
                        {
                            Log(string.Format("Sheet Codes: Trying to Initialize {0}. Model is already initialized.", datasheetType));
                            break;
                        }

                        villagerModel = Resources.Load<VillagerModel>("ScriptableObjects/Villager");
                        LoadRequest request;
                        if (loadRequests.TryGetValue(DatasheetType.Villager, out request))
                        {
                            Log(string.Format("Sheet Codes: Trying to initialize {0} while also async loading. Async load has been canceled.", datasheetType));
                            loadRequests.Remove(DatasheetType.Villager);
                            request.resourceRequest.completed -= OnLoadCompleted_VillagerModel;
							foreach (Action<bool> callback in request.callbacks)
								callback(true);
                        }
                        break;
                    }
                case DatasheetType.Recipe:
                    {
                        if (recipeModel != null && !recipeModel.Equals(null))
                        {
                            Log(string.Format("Sheet Codes: Trying to Initialize {0}. Model is already initialized.", datasheetType));
                            break;
                        }

                        recipeModel = Resources.Load<RecipeModel>("ScriptableObjects/Recipe");
                        LoadRequest request;
                        if (loadRequests.TryGetValue(DatasheetType.Recipe, out request))
                        {
                            Log(string.Format("Sheet Codes: Trying to initialize {0} while also async loading. Async load has been canceled.", datasheetType));
                            loadRequests.Remove(DatasheetType.Recipe);
                            request.resourceRequest.completed -= OnLoadCompleted_RecipeModel;
							foreach (Action<bool> callback in request.callbacks)
								callback(true);
                        }
                        break;
                    }
                case DatasheetType.RecipeOption:
                    {
                        if (recipeOptionModel != null && !recipeOptionModel.Equals(null))
                        {
                            Log(string.Format("Sheet Codes: Trying to Initialize {0}. Model is already initialized.", datasheetType));
                            break;
                        }

                        recipeOptionModel = Resources.Load<RecipeOptionModel>("ScriptableObjects/RecipeOption");
                        LoadRequest request;
                        if (loadRequests.TryGetValue(DatasheetType.RecipeOption, out request))
                        {
                            Log(string.Format("Sheet Codes: Trying to initialize {0} while also async loading. Async load has been canceled.", datasheetType));
                            loadRequests.Remove(DatasheetType.RecipeOption);
                            request.resourceRequest.completed -= OnLoadCompleted_RecipeOptionModel;
							foreach (Action<bool> callback in request.callbacks)
								callback(true);
                        }
                        break;
                    }
                case DatasheetType.Road:
                    {
                        if (roadModel != null && !roadModel.Equals(null))
                        {
                            Log(string.Format("Sheet Codes: Trying to Initialize {0}. Model is already initialized.", datasheetType));
                            break;
                        }

                        roadModel = Resources.Load<RoadModel>("ScriptableObjects/Road");
                        LoadRequest request;
                        if (loadRequests.TryGetValue(DatasheetType.Road, out request))
                        {
                            Log(string.Format("Sheet Codes: Trying to initialize {0} while also async loading. Async load has been canceled.", datasheetType));
                            loadRequests.Remove(DatasheetType.Road);
                            request.resourceRequest.completed -= OnLoadCompleted_RoadModel;
							foreach (Action<bool> callback in request.callbacks)
								callback(true);
                        }
                        break;
                    }
                default:
                    break;
            }
        }

        public static void InitializeAsync(DatasheetType datasheetType, Action<bool> callback)
        {
            switch (datasheetType)
            {
                case DatasheetType.WorldResource:
                    {
                        if (worldResourceModel != null && !worldResourceModel.Equals(null))
                        {
                            Log(string.Format("Sheet Codes: Trying to InitializeAsync {0}. Model is already initialized.", datasheetType));
                            callback(true);
                            break;
                        }
                        if(loadRequests.ContainsKey(DatasheetType.WorldResource))
                        {
                            loadRequests[DatasheetType.WorldResource].callbacks.Add(callback);
                            break;
                        }
                        ResourceRequest request = Resources.LoadAsync<WorldResourceModel>("ScriptableObjects/WorldResource");
                        loadRequests.Add(DatasheetType.WorldResource, new LoadRequest(request, callback));
                        request.completed += OnLoadCompleted_WorldResourceModel;
                        break;
                    }
                case DatasheetType.Building:
                    {
                        if (buildingModel != null && !buildingModel.Equals(null))
                        {
                            Log(string.Format("Sheet Codes: Trying to InitializeAsync {0}. Model is already initialized.", datasheetType));
                            callback(true);
                            break;
                        }
                        if(loadRequests.ContainsKey(DatasheetType.Building))
                        {
                            loadRequests[DatasheetType.Building].callbacks.Add(callback);
                            break;
                        }
                        ResourceRequest request = Resources.LoadAsync<BuildingModel>("ScriptableObjects/Building");
                        loadRequests.Add(DatasheetType.Building, new LoadRequest(request, callback));
                        request.completed += OnLoadCompleted_BuildingModel;
                        break;
                    }
                case DatasheetType.Resources:
                    {
                        if (resourcesModel != null && !resourcesModel.Equals(null))
                        {
                            Log(string.Format("Sheet Codes: Trying to InitializeAsync {0}. Model is already initialized.", datasheetType));
                            callback(true);
                            break;
                        }
                        if(loadRequests.ContainsKey(DatasheetType.Resources))
                        {
                            loadRequests[DatasheetType.Resources].callbacks.Add(callback);
                            break;
                        }
                        ResourceRequest request = Resources.LoadAsync<ResourcesModel>("ScriptableObjects/Resources");
                        loadRequests.Add(DatasheetType.Resources, new LoadRequest(request, callback));
                        request.completed += OnLoadCompleted_ResourcesModel;
                        break;
                    }
                case DatasheetType.ResourceCost:
                    {
                        if (resourceCostModel != null && !resourceCostModel.Equals(null))
                        {
                            Log(string.Format("Sheet Codes: Trying to InitializeAsync {0}. Model is already initialized.", datasheetType));
                            callback(true);
                            break;
                        }
                        if(loadRequests.ContainsKey(DatasheetType.ResourceCost))
                        {
                            loadRequests[DatasheetType.ResourceCost].callbacks.Add(callback);
                            break;
                        }
                        ResourceRequest request = Resources.LoadAsync<ResourceCostModel>("ScriptableObjects/ResourceCost");
                        loadRequests.Add(DatasheetType.ResourceCost, new LoadRequest(request, callback));
                        request.completed += OnLoadCompleted_ResourceCostModel;
                        break;
                    }
                case DatasheetType.Blocker:
                    {
                        if (blockerModel != null && !blockerModel.Equals(null))
                        {
                            Log(string.Format("Sheet Codes: Trying to InitializeAsync {0}. Model is already initialized.", datasheetType));
                            callback(true);
                            break;
                        }
                        if(loadRequests.ContainsKey(DatasheetType.Blocker))
                        {
                            loadRequests[DatasheetType.Blocker].callbacks.Add(callback);
                            break;
                        }
                        ResourceRequest request = Resources.LoadAsync<BlockerModel>("ScriptableObjects/Blocker");
                        loadRequests.Add(DatasheetType.Blocker, new LoadRequest(request, callback));
                        request.completed += OnLoadCompleted_BlockerModel;
                        break;
                    }
                case DatasheetType.Regions:
                    {
                        if (regionsModel != null && !regionsModel.Equals(null))
                        {
                            Log(string.Format("Sheet Codes: Trying to InitializeAsync {0}. Model is already initialized.", datasheetType));
                            callback(true);
                            break;
                        }
                        if(loadRequests.ContainsKey(DatasheetType.Regions))
                        {
                            loadRequests[DatasheetType.Regions].callbacks.Add(callback);
                            break;
                        }
                        ResourceRequest request = Resources.LoadAsync<RegionsModel>("ScriptableObjects/Regions");
                        loadRequests.Add(DatasheetType.Regions, new LoadRequest(request, callback));
                        request.completed += OnLoadCompleted_RegionsModel;
                        break;
                    }
                case DatasheetType.ResourceSpawn:
                    {
                        if (resourceSpawnModel != null && !resourceSpawnModel.Equals(null))
                        {
                            Log(string.Format("Sheet Codes: Trying to InitializeAsync {0}. Model is already initialized.", datasheetType));
                            callback(true);
                            break;
                        }
                        if(loadRequests.ContainsKey(DatasheetType.ResourceSpawn))
                        {
                            loadRequests[DatasheetType.ResourceSpawn].callbacks.Add(callback);
                            break;
                        }
                        ResourceRequest request = Resources.LoadAsync<ResourceSpawnModel>("ScriptableObjects/ResourceSpawn");
                        loadRequests.Add(DatasheetType.ResourceSpawn, new LoadRequest(request, callback));
                        request.completed += OnLoadCompleted_ResourceSpawnModel;
                        break;
                    }
                case DatasheetType.Villager:
                    {
                        if (villagerModel != null && !villagerModel.Equals(null))
                        {
                            Log(string.Format("Sheet Codes: Trying to InitializeAsync {0}. Model is already initialized.", datasheetType));
                            callback(true);
                            break;
                        }
                        if(loadRequests.ContainsKey(DatasheetType.Villager))
                        {
                            loadRequests[DatasheetType.Villager].callbacks.Add(callback);
                            break;
                        }
                        ResourceRequest request = Resources.LoadAsync<VillagerModel>("ScriptableObjects/Villager");
                        loadRequests.Add(DatasheetType.Villager, new LoadRequest(request, callback));
                        request.completed += OnLoadCompleted_VillagerModel;
                        break;
                    }
                case DatasheetType.Recipe:
                    {
                        if (recipeModel != null && !recipeModel.Equals(null))
                        {
                            Log(string.Format("Sheet Codes: Trying to InitializeAsync {0}. Model is already initialized.", datasheetType));
                            callback(true);
                            break;
                        }
                        if(loadRequests.ContainsKey(DatasheetType.Recipe))
                        {
                            loadRequests[DatasheetType.Recipe].callbacks.Add(callback);
                            break;
                        }
                        ResourceRequest request = Resources.LoadAsync<RecipeModel>("ScriptableObjects/Recipe");
                        loadRequests.Add(DatasheetType.Recipe, new LoadRequest(request, callback));
                        request.completed += OnLoadCompleted_RecipeModel;
                        break;
                    }
                case DatasheetType.RecipeOption:
                    {
                        if (recipeOptionModel != null && !recipeOptionModel.Equals(null))
                        {
                            Log(string.Format("Sheet Codes: Trying to InitializeAsync {0}. Model is already initialized.", datasheetType));
                            callback(true);
                            break;
                        }
                        if(loadRequests.ContainsKey(DatasheetType.RecipeOption))
                        {
                            loadRequests[DatasheetType.RecipeOption].callbacks.Add(callback);
                            break;
                        }
                        ResourceRequest request = Resources.LoadAsync<RecipeOptionModel>("ScriptableObjects/RecipeOption");
                        loadRequests.Add(DatasheetType.RecipeOption, new LoadRequest(request, callback));
                        request.completed += OnLoadCompleted_RecipeOptionModel;
                        break;
                    }
                case DatasheetType.Road:
                    {
                        if (roadModel != null && !roadModel.Equals(null))
                        {
                            Log(string.Format("Sheet Codes: Trying to InitializeAsync {0}. Model is already initialized.", datasheetType));
                            callback(true);
                            break;
                        }
                        if(loadRequests.ContainsKey(DatasheetType.Road))
                        {
                            loadRequests[DatasheetType.Road].callbacks.Add(callback);
                            break;
                        }
                        ResourceRequest request = Resources.LoadAsync<RoadModel>("ScriptableObjects/Road");
                        loadRequests.Add(DatasheetType.Road, new LoadRequest(request, callback));
                        request.completed += OnLoadCompleted_RoadModel;
                        break;
                    }
                default:
                    break;
            }
        }

        private static void OnLoadCompleted_WorldResourceModel(AsyncOperation operation)
        {
            LoadRequest request = loadRequests[DatasheetType.WorldResource];
            worldResourceModel = request.resourceRequest.asset as WorldResourceModel;
            loadRequests.Remove(DatasheetType.WorldResource);
            operation.completed -= OnLoadCompleted_WorldResourceModel;
            foreach (Action<bool> callback in request.callbacks)
                callback(true);
        }

		private static WorldResourceModel worldResourceModel = default;
		public static WorldResourceModel WorldResourceModel
        {
            get
            {
                if (worldResourceModel == null)
                    Initialize(DatasheetType.WorldResource);

                return worldResourceModel;
            }
        }
        private static void OnLoadCompleted_BuildingModel(AsyncOperation operation)
        {
            LoadRequest request = loadRequests[DatasheetType.Building];
            buildingModel = request.resourceRequest.asset as BuildingModel;
            loadRequests.Remove(DatasheetType.Building);
            operation.completed -= OnLoadCompleted_BuildingModel;
            foreach (Action<bool> callback in request.callbacks)
                callback(true);
        }

		private static BuildingModel buildingModel = default;
		public static BuildingModel BuildingModel
        {
            get
            {
                if (buildingModel == null)
                    Initialize(DatasheetType.Building);

                return buildingModel;
            }
        }
        private static void OnLoadCompleted_ResourcesModel(AsyncOperation operation)
        {
            LoadRequest request = loadRequests[DatasheetType.Resources];
            resourcesModel = request.resourceRequest.asset as ResourcesModel;
            loadRequests.Remove(DatasheetType.Resources);
            operation.completed -= OnLoadCompleted_ResourcesModel;
            foreach (Action<bool> callback in request.callbacks)
                callback(true);
        }

		private static ResourcesModel resourcesModel = default;
		public static ResourcesModel ResourcesModel
        {
            get
            {
                if (resourcesModel == null)
                    Initialize(DatasheetType.Resources);

                return resourcesModel;
            }
        }
        private static void OnLoadCompleted_ResourceCostModel(AsyncOperation operation)
        {
            LoadRequest request = loadRequests[DatasheetType.ResourceCost];
            resourceCostModel = request.resourceRequest.asset as ResourceCostModel;
            loadRequests.Remove(DatasheetType.ResourceCost);
            operation.completed -= OnLoadCompleted_ResourceCostModel;
            foreach (Action<bool> callback in request.callbacks)
                callback(true);
        }

		private static ResourceCostModel resourceCostModel = default;
		public static ResourceCostModel ResourceCostModel
        {
            get
            {
                if (resourceCostModel == null)
                    Initialize(DatasheetType.ResourceCost);

                return resourceCostModel;
            }
        }
        private static void OnLoadCompleted_BlockerModel(AsyncOperation operation)
        {
            LoadRequest request = loadRequests[DatasheetType.Blocker];
            blockerModel = request.resourceRequest.asset as BlockerModel;
            loadRequests.Remove(DatasheetType.Blocker);
            operation.completed -= OnLoadCompleted_BlockerModel;
            foreach (Action<bool> callback in request.callbacks)
                callback(true);
        }

		private static BlockerModel blockerModel = default;
		public static BlockerModel BlockerModel
        {
            get
            {
                if (blockerModel == null)
                    Initialize(DatasheetType.Blocker);

                return blockerModel;
            }
        }
        private static void OnLoadCompleted_RegionsModel(AsyncOperation operation)
        {
            LoadRequest request = loadRequests[DatasheetType.Regions];
            regionsModel = request.resourceRequest.asset as RegionsModel;
            loadRequests.Remove(DatasheetType.Regions);
            operation.completed -= OnLoadCompleted_RegionsModel;
            foreach (Action<bool> callback in request.callbacks)
                callback(true);
        }

		private static RegionsModel regionsModel = default;
		public static RegionsModel RegionsModel
        {
            get
            {
                if (regionsModel == null)
                    Initialize(DatasheetType.Regions);

                return regionsModel;
            }
        }
        private static void OnLoadCompleted_ResourceSpawnModel(AsyncOperation operation)
        {
            LoadRequest request = loadRequests[DatasheetType.ResourceSpawn];
            resourceSpawnModel = request.resourceRequest.asset as ResourceSpawnModel;
            loadRequests.Remove(DatasheetType.ResourceSpawn);
            operation.completed -= OnLoadCompleted_ResourceSpawnModel;
            foreach (Action<bool> callback in request.callbacks)
                callback(true);
        }

		private static ResourceSpawnModel resourceSpawnModel = default;
		public static ResourceSpawnModel ResourceSpawnModel
        {
            get
            {
                if (resourceSpawnModel == null)
                    Initialize(DatasheetType.ResourceSpawn);

                return resourceSpawnModel;
            }
        }
        private static void OnLoadCompleted_VillagerModel(AsyncOperation operation)
        {
            LoadRequest request = loadRequests[DatasheetType.Villager];
            villagerModel = request.resourceRequest.asset as VillagerModel;
            loadRequests.Remove(DatasheetType.Villager);
            operation.completed -= OnLoadCompleted_VillagerModel;
            foreach (Action<bool> callback in request.callbacks)
                callback(true);
        }

		private static VillagerModel villagerModel = default;
		public static VillagerModel VillagerModel
        {
            get
            {
                if (villagerModel == null)
                    Initialize(DatasheetType.Villager);

                return villagerModel;
            }
        }
        private static void OnLoadCompleted_RecipeModel(AsyncOperation operation)
        {
            LoadRequest request = loadRequests[DatasheetType.Recipe];
            recipeModel = request.resourceRequest.asset as RecipeModel;
            loadRequests.Remove(DatasheetType.Recipe);
            operation.completed -= OnLoadCompleted_RecipeModel;
            foreach (Action<bool> callback in request.callbacks)
                callback(true);
        }

		private static RecipeModel recipeModel = default;
		public static RecipeModel RecipeModel
        {
            get
            {
                if (recipeModel == null)
                    Initialize(DatasheetType.Recipe);

                return recipeModel;
            }
        }
        private static void OnLoadCompleted_RecipeOptionModel(AsyncOperation operation)
        {
            LoadRequest request = loadRequests[DatasheetType.RecipeOption];
            recipeOptionModel = request.resourceRequest.asset as RecipeOptionModel;
            loadRequests.Remove(DatasheetType.RecipeOption);
            operation.completed -= OnLoadCompleted_RecipeOptionModel;
            foreach (Action<bool> callback in request.callbacks)
                callback(true);
        }

		private static RecipeOptionModel recipeOptionModel = default;
		public static RecipeOptionModel RecipeOptionModel
        {
            get
            {
                if (recipeOptionModel == null)
                    Initialize(DatasheetType.RecipeOption);

                return recipeOptionModel;
            }
        }
        private static void OnLoadCompleted_RoadModel(AsyncOperation operation)
        {
            LoadRequest request = loadRequests[DatasheetType.Road];
            roadModel = request.resourceRequest.asset as RoadModel;
            loadRequests.Remove(DatasheetType.Road);
            operation.completed -= OnLoadCompleted_RoadModel;
            foreach (Action<bool> callback in request.callbacks)
                callback(true);
        }

		private static RoadModel roadModel = default;
		public static RoadModel RoadModel
        {
            get
            {
                if (roadModel == null)
                    Initialize(DatasheetType.Road);

                return roadModel;
            }
        }
		
        private static void Log(string message)
        {
            Debug.LogWarning(message);
        }
	}
	
    public struct LoadRequest
    {
        public readonly ResourceRequest resourceRequest;
        public readonly List<Action<bool>> callbacks;

        public LoadRequest(ResourceRequest resourceRequest, Action<bool> callback)
        {
            this.resourceRequest = resourceRequest;
            callbacks = new List<Action<bool>>() { callback };
        }
    }
}
