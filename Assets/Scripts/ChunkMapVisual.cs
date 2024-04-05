﻿
using System.Collections.Generic;
using UnityEngine;

public class ChunkMapVisual : DataDrivenBehaviour<ChunkMap>
{
    private const string PROPERTY_FOG_OF_WAR_MAP = "fogOfWarMap";
    private const string PROPERTY_BIOME_MAP = "biomeMap";
    private const string PROPERTY_MAP_WIDTH = "biomeWidth";
    private const string PROPERTY_MAP_HEIGHT = "biomeHeight";

    [SerializeField] private BuildingVisual buildingPrefab;
    [SerializeField] private Transform buildingInstancesContainer;
    [SerializeField] private Transform contentContainer;
    [SerializeField] private Transform terrainTransform;
    [SerializeField] private Transform fogOfWarTransform;
    [SerializeField] private MeshRenderer terrainMeshRenderer;
    [SerializeField] private MeshRenderer fogOfWarMeshRenderer;
    [SerializeField] private float fogOfWarHeight;

    private Material terrainMaterial;
    private Material fogOfWarMaterial;

    private readonly List<BuildingVisual> buildingInstances;
    private GameObject[] worldResourceVisuals;

    private ChunkMapVisual()
        : base()
    {
        buildingInstances = new List<BuildingVisual>();
    }

    private void Awake()
    {
        terrainMaterial = terrainMeshRenderer.material;
        fogOfWarMaterial = fogOfWarMeshRenderer.material;
    }

    protected override void OnValueChanged_Data(ChunkMap oldValue, ChunkMap newValue)
    {
        if (oldValue != null)
        {
            for (int i = 0; i < worldResourceVisuals.Length; i++)
            {
                if (worldResourceVisuals[i] != null)
                    GameObject.Destroy(worldResourceVisuals[i]);
            }

            foreach (ChunkMapPoint chunkMapPoint in oldValue.chunkMapPoints)
                chunkMapPoint.worldResource.onValueChangeSource -= OnValueChanged_ChunkMapPoint_WorldResource;

            oldValue.buildings.onAdd -= OnAdd_Building;
            oldValue.buildings.onRemove -= OnRemove_Building;

            foreach (BuildingVisual instance in buildingInstances)
                GameObject.Destroy(instance.gameObject);

            buildingInstances.Clear();
        }

        if (newValue != null)
        {
            worldResourceVisuals = new GameObject[newValue.chunkWidth * newValue.chunkHeight];

            transform.position = new Vector3(newValue.xChunkPosition * newValue.chunkWidth, 0, newValue.yChunkPosition * newValue.chunkHeight);
            terrainTransform.localPosition = new Vector3(newValue.chunkWidth / 2, 0, newValue.chunkHeight / 2);
            terrainTransform.localScale = new Vector3(newValue.chunkWidth, newValue.chunkHeight, 1);

            fogOfWarTransform.localPosition = new Vector3(newValue.chunkWidth / 2, fogOfWarHeight, newValue.chunkHeight / 2);
            fogOfWarTransform.localScale = new Vector3(newValue.chunkWidth * 2, newValue.chunkHeight * 2, 1);

            foreach (ChunkMapPoint chunkMapPoint in newValue.chunkMapPoints)
                chunkMapPoint.worldResource.onValueChangeImmediateSource += OnValueChanged_ChunkMapPoint_WorldResource;

            terrainMaterial.SetBuffer(PROPERTY_BIOME_MAP, newValue.biomeTypeMapBuffer);
            terrainMaterial.SetInt(PROPERTY_MAP_WIDTH, newValue.chunkWidth);
            terrainMaterial.SetInt(PROPERTY_MAP_HEIGHT, newValue.chunkHeight);

            fogOfWarMaterial.SetBuffer(PROPERTY_FOG_OF_WAR_MAP, newValue.fogOfWarMapBuffer);
            fogOfWarMaterial.SetInt(PROPERTY_MAP_WIDTH, newValue.chunkWidth);
            fogOfWarMaterial.SetInt(PROPERTY_MAP_HEIGHT, newValue.chunkHeight);

            newValue.buildings.onAdd += OnAdd_Building;
            newValue.buildings.onRemove += OnRemove_Building;
            foreach (Building building in newValue.buildings)
                OnAdd_Building(building);
        } 
    }

    private void OnAdd_Building(Building item)
    {
        BuildingVisual instance = GameObject.Instantiate(buildingPrefab, buildingInstancesContainer);
        instance.data = item;
        buildingInstances.Add(instance);
    }

    private void OnRemove_Building(Building item)
    {
        BuildingVisual instance = buildingInstances.Find(i => i.data == item);
        buildingInstances.Remove(instance);
        GameObject.Destroy(instance.gameObject);
    }

    private void OnValueChanged_ChunkMapPoint_WorldResource(ChunkMapPoint source, WorldResource oldValue, WorldResource newValue)
    {
        if (oldValue != null)
        {
            int index = source.point.yIndex + data.chunkWidth * source.point.xIndex;
            GameObject.Destroy(worldResourceVisuals[index]);
            worldResourceVisuals[index] = null;
        }

        if (newValue != null)
        {
            int index = source.point.yIndex + data.chunkWidth * source.point.xIndex;
            GameObject instance = GameObject.Instantiate(newValue.record.ModelVariations[newValue.modelIndex], contentContainer);
            worldResourceVisuals[index] = instance;
            instance.transform.localPosition = new Vector3(source.point.xIndex + 0.5f, 0, source.point.yIndex + 0.5f);
            instance.transform.localRotation = Quaternion.Euler(0, newValue.modelRotation, 0);
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Y))
        {
            if (data.buildings.Count > 0)
                data.buildings.RemoveAt(data.buildings.Count - 1);
        }
    }

    public bool TryGetMouseGridPoint(out Point point, float xOffset, float yOffset)
    {
        Plane plane = new Plane(Vector3.up, transform.position.y);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!plane.Raycast(ray, out float distance))
        {
            point = default;
            return false;
        }

        Vector3 hitPosition = ray.origin + ray.direction * distance;
        int xIndex = Mathf.FloorToInt(hitPosition.x - transform.position.x + xOffset);
        int yIndex = Mathf.FloorToInt(hitPosition.z - transform.position.z + yOffset);

        if (xIndex < 0)
        {
            point = default;
            return false;
        }

        if (yIndex < 0)
        {
            point = default;
            return false;
        }

        if (xIndex >= data.chunkWidth)
        {
            point = default;
            return false;
        }

        if (yIndex >= data.chunkHeight)
        {
            point = default;
            return false;
        }

        point = new Point(xIndex, yIndex);
        return true;
    }
}