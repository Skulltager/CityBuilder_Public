using UnityEngine;

public class GameDataManager : MonoBehaviour
{
    [SerializeField] private MapVisual mapVisual;
    [SerializeField] private CameraController cameraController;
    [SerializeField] private BuildingPlacer buildingPlacer;
    [SerializeField] private int chunkWidth;
    [SerializeField] private int chunkHeight;
    [SerializeField] private int minRegionSize;
    [SerializeField] private int maxRegionSize;
    [SerializeField] private int fogOfWarRadius;
    [SerializeField] private int spawnAttempts;

    private Map map;

    private void Awake()
    {
        GenerateMap();
    }

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.P))
            GenerateMap();

        //if (Input.GetKeyDown(KeyCode.O))
        //    buildingPlacer.data = BuildingIdentifier.House;
        //
        //if (Input.GetKeyDown(KeyCode.U))
        //    buildingPlacer.data = BuildingIdentifier.None;
    }

    private void GenerateMap()
    {   
        DisposeBuffers();

        map = new Map(chunkWidth, chunkHeight, minRegionSize, maxRegionSize);
        for(int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                map.GenerateChunkMap(x, y);
            }
        }
        mapVisual.data = map;

        Vector2 bottomLeftBounds = new Vector2(-chunkWidth, -chunkHeight);
        Vector2 topRightBounds = new Vector2(chunkWidth * 2, chunkHeight * 2);

        cameraController.InitializeCameraBounds(bottomLeftBounds, topRightBounds);
        Vector2 cameraPosition = new Vector2(chunkWidth / 2, chunkHeight / 2);
        cameraController.SetCameraPosition(cameraPosition);
    }

    private void DisposeBuffers()
    {
        if (map != null)
            map.Dispose();
    }

    private void OnDestroy()
    {
        DisposeBuffers();
    }
}