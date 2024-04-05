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
