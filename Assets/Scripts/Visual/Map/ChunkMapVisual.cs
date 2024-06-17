
using System.Collections.Generic;
using UnityEngine;

public class ChunkMapVisual : DataDrivenBehaviour<ChunkMap>
{
    private const string PROPERTY_FOG_OF_WAR_MAP = "fogOfWarMap";
    private const string PROPERTY_ROADS_MAP = "roadsMap";
    private const string PROPERTY_BIOME_MAP = "biomeMap";
    private const string PROPERTY_MAP_WIDTH = "biomeWidth";
    private const string PROPERTY_MAP_HEIGHT = "biomeHeight";
    private const string PROPERTY_MAP_CHUNK_X_OFFSET = "chunkXOffset";
    private const string PROPERTY_MAP_CHUNK_Y_OFFSET = "chunkYOffset";

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
            {
                chunkMapPoint.content.onValueChangeImmediate -= OnValueChanged_ChunkMapPoint_Content;
            }

            buildingInstances.Clear();
        }

        if (newValue != null)
        {
            worldResourceVisuals = new GameObject[newValue.chunkWidth * newValue.chunkHeight];

            transform.position = new Vector3(newValue.xChunkPosition * newValue.chunkWidth, 0, newValue.yChunkPosition * newValue.chunkHeight);
            terrainTransform.localPosition = new Vector3(newValue.chunkWidth / 2, 0, newValue.chunkHeight / 2);
            terrainTransform.localScale = new Vector3(newValue.chunkWidth, newValue.chunkHeight, 1);

            fogOfWarTransform.localScale = new Vector3(newValue.chunkWidth * 4, newValue.chunkHeight * 4, 1);

            foreach (ChunkMapPoint chunkMapPoint in newValue.chunkMapPoints)
                chunkMapPoint.content.onValueChangeImmediate += OnValueChanged_ChunkMapPoint_Content;

            terrainMaterial.SetBuffer(PROPERTY_BIOME_MAP, newValue.biomeTypeMapBuffer);
            terrainMaterial.SetBuffer(PROPERTY_ROADS_MAP, newValue.roadsBuffer);
            terrainMaterial.SetInt(PROPERTY_MAP_WIDTH, newValue.chunkWidth);
            terrainMaterial.SetInt(PROPERTY_MAP_HEIGHT, newValue.chunkHeight);

            fogOfWarMaterial.SetBuffer(PROPERTY_FOG_OF_WAR_MAP, newValue.fogOfWarMapBuffer);
            fogOfWarMaterial.SetInt(PROPERTY_MAP_CHUNK_X_OFFSET, newValue.xChunkPosition * newValue.chunkWidth);
            fogOfWarMaterial.SetInt(PROPERTY_MAP_CHUNK_Y_OFFSET, newValue.yChunkPosition * newValue.chunkHeight);
            fogOfWarMaterial.SetInt(PROPERTY_MAP_WIDTH, newValue.chunkWidth);
            fogOfWarMaterial.SetInt(PROPERTY_MAP_HEIGHT, newValue.chunkHeight);
        } 
    }

    private void OnValueChanged_ChunkMapPoint_Content(ChunkMapPointContent oldValue, ChunkMapPointContent newValue)
    {
        if (oldValue != null)
        {
            oldValue.Hide();
        }

        if (newValue != null)
        {
            newValue.Show();
        }
    }
}