using SheetCodes;
using System;
using UnityEngine;

public class GameDataManager : MonoBehaviour
{
    public static float LAST_MOUSE_INTERACTION_HANDLED_MOMENT;

    [SerializeField] private MapVisual mapVisual;
    [SerializeField] private CameraController cameraController;
    [SerializeField] private BuildingPlacer buildingPlacer;
    [SerializeField] private BuildingRemover buildingRemover;
    [SerializeField] private PlayerHUD playerHUD;
    [SerializeField] private int chunkWidth;
    [SerializeField] private int chunkHeight;
    [SerializeField] private int villagers;
    [SerializeField] private int playerRegionSize;
    [SerializeField] private int minRegionSize;
    [SerializeField] private int maxRegionSize;
    [SerializeField] private int seed;

    private Map map;
    private System.Random random;
    private Player playerOne;
    private Player playerTwo;

    private void Start()
    {
        GenerateMap();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
            GenerateMap();

        if (Input.GetKey(KeyCode.O))
        {
            while (true)
            {
                ChunkRegion adjacentRegion = playerOne.playerRegion.adjacentRegions.GetRandomItem(random);
                if (adjacentRegion.owner != null)
                    continue;

                if (adjacentRegion.merged)
                    continue;

                map.MergeRegionsTogether(playerOne.playerRegion, adjacentRegion);
                break;
            }

            while (true)
            {
                ChunkRegion adjacentRegion = playerTwo.playerRegion.adjacentRegions.GetRandomItem(random);
                if (adjacentRegion.owner != null)
                    continue;
            
                if (adjacentRegion.merged)
                    continue;
            
                map.MergeRegionsTogether(playerTwo.playerRegion, adjacentRegion);
                break;
            }
        }
    }

    private void GenerateMap()
    {
        DisposeBuffers();

        buildingPlacer.data = null;
        buildingRemover.data = null;

        random = new System.Random(seed);
        map = new Map(random, chunkWidth, chunkHeight, minRegionSize, maxRegionSize);

        ChunkRegion playerRegion;
        if (!map.TryGenerateSingleRegion(0, 0, RegionsIdentifier.PlayerStart.GetRecord(), out playerRegion))
        {
            mapVisual.data = null;
            Debug.Log("Failed to generate a player region");
            return;
        }
        playerOne = new Player(playerRegion);
        playerHUD.data = playerOne;
        playerRegion.contents.Insert(0, new ChunkRegionContent_TownCenterAndVillagers(playerRegion, villagers));

        if (!map.TryGenerateSingleRegion(0, 0, RegionsIdentifier.PlayerStart.GetRecord(), out playerRegion))
        {
            mapVisual.data = null;
            Debug.Log("Failed to generate a player region");
            return;
        }
        playerTwo = new Player(playerRegion);
        playerRegion.contents.Insert(0, new ChunkRegionContent_TownCenterAndVillagers(playerRegion, 1));

        map.GenerateChunkMap(0, 0);
        map.MakeChunkRegionVisible(playerOne.playerRegion);
        map.MakeChunkRegionVisible(playerTwo.playerRegion);

        mapVisual.data = map;

        Vector2 bottomLeftBounds = new Vector2(-chunkWidth * 10, -chunkHeight * 10);
        Vector2 topRightBounds = new Vector2(chunkWidth * 10, chunkHeight * 10);

        cameraController.InitializeCameraBounds(bottomLeftBounds, topRightBounds);
        Vector2 cameraPosition = new Vector2(chunkWidth / 2, chunkHeight / 2);
        cameraController.SetCameraPosition(playerOne.townCenterBuilding.centerPosition);
    }

    private void DisposeBuffers()
    {
        if (map == null)
            return;

        map.Dispose();
        playerOne.Dispose();
        playerTwo.Dispose();
    }

    private void OnDestroy()
    {
        DisposeBuffers();
    }
}