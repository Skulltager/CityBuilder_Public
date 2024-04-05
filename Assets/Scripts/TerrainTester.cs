using SheetCodes;
using System.Runtime.InteropServices;
using UnityEngine;

public class TerrainTester : MonoBehaviour
{
    [SerializeField] private CameraController cameraController;
    [SerializeField] private MeshRenderer terrainMeshRenderer;
    [SerializeField] private MeshRenderer fogOfWarMeshRenderer;
    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private float gridSize;
    [SerializeField] private float fogOfWarHeight;

    private ComputeBuffer biomeMapComputeBuffer;
    private ComputeBuffer fogOfWarComputeBuffer;
    private Material terrainMaterial;
    private Material fogOfWarMaterial;

    private float[] fogOfWarMap;
    private int[] biomeMap;
    private WorldResourceRecord[] resourceTypeMap;

    private void Awake()
    {
        terrainMaterial = terrainMeshRenderer.sharedMaterial;
        fogOfWarMaterial = fogOfWarMeshRenderer.sharedMaterial;
        GenerateTerrain();
    }

    private void Start()
    {
        Vector2 bottomLeftBounds = new Vector2(0, 0);
        Vector2 topRightBounds = new Vector2(gridSize * width, gridSize * height);

        cameraController.InitializeCameraBounds(bottomLeftBounds, topRightBounds);
        Vector2 cameraPosition = new Vector2(gridSize * width / 2, gridSize * height / 2);
        cameraController.SetCameraPosition(cameraPosition);

    }

    private void GenerateTerrain()
    {
        DisposeBuffers();
        biomeMapComputeBuffer = new ComputeBuffer(width * height, Marshal.SizeOf<int>());
        fogOfWarComputeBuffer = new ComputeBuffer(width * height, Marshal.SizeOf<float>());

        WorldResourceRecord worldResourceNone = null;
        WorldResourceRecord worldResourceTree = WorldResourceIdentifier.Tree.GetRecord();
        WorldResourceRecord worldResourceIronOre = WorldResourceIdentifier.IronOre.GetRecord();
        WorldResourceRecord worldResourceBerryBush = WorldResourceIdentifier.BerryBush.GetRecord();

        biomeMap = new int[width * height];
        fogOfWarMap = new float[width * height];
        resourceTypeMap = new WorldResourceRecord[width * height];

        for (int i = 0; i < biomeMap.Length; i++)
        {
            biomeMap[i] = (int) BiomeType.Grass;
            fogOfWarMap[i] = 1;
            resourceTypeMap[i] = worldResourceTree;
        }

        for (int x = 40; x <= 60; x++)
        {
            for (int y = 40; y <= 60; y++)
            {
                fogOfWarMap[y * width + x] = 0;
            }
        }

        for (int x = 42; x <= 58; x++)
        {
            for (int y = 42; y <= 58; y++)
            {
                resourceTypeMap[y * width + x] = worldResourceNone;
            }
        }

        resourceTypeMap[49 * width + 48] = worldResourceIronOre;
        resourceTypeMap[48 * width + 48] = worldResourceIronOre;
        resourceTypeMap[49 * width + 47] = worldResourceIronOre;
        resourceTypeMap[48 * width + 47] = worldResourceIronOre;
        resourceTypeMap[47 * width + 47] = worldResourceIronOre;
        resourceTypeMap[46 * width + 47] = worldResourceIronOre;
        resourceTypeMap[46 * width + 46] = worldResourceIronOre;
        resourceTypeMap[46 * width + 45] = worldResourceIronOre;


        resourceTypeMap[55 * width + 51] = worldResourceBerryBush;
        resourceTypeMap[56 * width + 51] = worldResourceBerryBush;
        resourceTypeMap[57 * width + 51] = worldResourceBerryBush;
        resourceTypeMap[55 * width + 52] = worldResourceBerryBush;
        resourceTypeMap[56 * width + 52] = worldResourceBerryBush;
        resourceTypeMap[56 * width + 53] = worldResourceBerryBush;

        fogOfWarComputeBuffer.SetData(fogOfWarMap);
        biomeMapComputeBuffer.SetData(biomeMap);

        terrainMaterial.SetBuffer("biomeMap", biomeMapComputeBuffer);
        terrainMaterial.SetInt("biomeWidth", width);
        terrainMaterial.SetInt("biomeHeight", height);

        terrainMeshRenderer.transform.localScale = new Vector3(gridSize * width, gridSize * height, 1);
        terrainMeshRenderer.transform.position = new Vector3(gridSize * width / 2, 0, gridSize * height / 2);

        fogOfWarMeshRenderer.transform.localScale = new Vector3(gridSize * width * 2, gridSize * height * 2, 1);
        fogOfWarMeshRenderer.transform.position = new Vector3(gridSize * width / 2, fogOfWarHeight, gridSize * height / 2);

        fogOfWarMaterial.SetBuffer("fogOfWarMap", fogOfWarComputeBuffer);
        fogOfWarMaterial.SetInt("biomeWidth", width);
        fogOfWarMaterial.SetInt("biomeHeight", height);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                WorldResourceRecord record = resourceTypeMap[y * width + x];
                if (record == null)
                    continue;

                GameObject resourceModel = record.ModelVariations.GetRandomItem();
                Vector3 worldPosition = new Vector3((x + 0.5f) * gridSize, 0, (y + 0.5f) * gridSize);
                Quaternion rotation = Quaternion.Euler(0, Random.value * 360, 0);
                GameObject instance = GameObject.Instantiate(resourceModel, worldPosition, rotation);
                instance.transform.parent = terrainMeshRenderer.transform;
            }
        }
    }


    private void DisposeBuffers()
    {
        if (biomeMapComputeBuffer == null)
            return;

        biomeMapComputeBuffer.Dispose();
        fogOfWarComputeBuffer.Dispose();
    }

    private void OnDestroy()
    {
        DisposeBuffers();
    }

    public void SetBiomeType(Point point, BiomeType biomeType)
    {
        biomeMap[point.yIndex * width + point.xIndex] = (int) biomeType;
        biomeMapComputeBuffer.SetData(biomeMap);
    }

    public void SetFogOfWar(Point point, float fogAmount)
    {
        fogOfWarMap[point.yIndex * width + point.xIndex] = fogAmount;
        fogOfWarComputeBuffer.SetData(fogOfWarMap);
    }
}