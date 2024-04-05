using SheetCodes;
using System.Runtime.InteropServices;
using UnityEngine;

public class ChunkMap
{
    public readonly ChunkDataMap chunkDataMap;
    public readonly ComputeBuffer biomeTypeMapBuffer;
    public readonly ComputeBuffer fogOfWarMapBuffer;
    public readonly ChunkMapPoint[] chunkMapPoints;
    public readonly EventList<Building> buildings;

    public int chunkWidth => chunkDataMap.chunkWidth;
    public int chunkHeight => chunkDataMap.chunkHeight;

    public int xChunkPosition => chunkDataMap.xChunkPosition;
    public int yChunkPosition => chunkDataMap.yChunkPosition;

    public ChunkMap(ChunkDataMap chunkDataMap)
    {
        this.chunkDataMap = chunkDataMap;
        buildings = new EventList<Building>();
        biomeTypeMapBuffer = new ComputeBuffer(chunkWidth * chunkWidth, Marshal.SizeOf(typeof(int)));
        fogOfWarMapBuffer = new ComputeBuffer(chunkWidth * chunkWidth, Marshal.SizeOf(typeof(float)));

        chunkMapPoints = new ChunkMapPoint[chunkWidth * chunkHeight];

        int[] biomeTypeMap = new int[chunkWidth * chunkHeight];
        float[] fogOfWarMap = new float[chunkWidth * chunkHeight];

        for (int x = 0; x < chunkWidth; x++)
        {
            for (int y = 0; y < chunkHeight; y++)
            {
                int index = y * chunkWidth + x;
                biomeTypeMap[index] = (int)chunkDataMap.biomeTypeMap[x, y];
                fogOfWarMap[index] = chunkDataMap.fogOfWarMap[x, y] ? 1 : 0;
                WorldResourceRecord resourceRecord = chunkDataMap.resourceMap[x, y];

                if (resourceRecord != null)
                    chunkMapPoints[index] = new ChunkMapPoint(new Point(x, y), resourceRecord);
                else
                    chunkMapPoints[index] = new ChunkMapPoint(new Point(x, y));
            }
        }

        biomeTypeMapBuffer.SetData(biomeTypeMap);
        fogOfWarMapBuffer.SetData(fogOfWarMap);
        buildings.onAdd += OnAdd_Building;
        buildings.onRemove += OnRemove_Building;
    }

    private void OnAdd_Building(Building item)
    {
        item.PlaceOnMap();
    }

    private void OnRemove_Building(Building item)
    {
        item.RemoveFromMap();
    }

    public void DisposeBuffers()
    {
        biomeTypeMapBuffer.Dispose();
        fogOfWarMapBuffer.Dispose();
    }

    public bool TryGetChunkMapPoint(Point point, out ChunkMapPoint chunkMapPoint)
    {
        if (!IsInBounds(point))
        {
            chunkMapPoint = default;
            return false;
        }

        chunkMapPoint = chunkMapPoints[point.yIndex * chunkWidth + point.xIndex];
        return true;
    }

    public ChunkMapPoint GetChunkMapPoint(Point point)
    {
        return chunkMapPoints[point.yIndex * chunkWidth + point.xIndex];
    }

    public bool IsInBounds(Point point)
    {
        if (point.xIndex < 0)
            return false;

        if (point.xIndex >= chunkWidth)
            return false;

        if (point.yIndex < 0)
            return false;

        if (point.yIndex >= chunkHeight)
            return false;

        return true;
    }
}