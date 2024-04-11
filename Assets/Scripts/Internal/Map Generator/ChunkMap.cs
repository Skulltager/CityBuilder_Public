using SheetCodes;
using System.Runtime.InteropServices;
using UnityEngine;

public class ChunkMap
{
    public readonly ChunkDataMap chunkDataMap;
    public readonly ComputeBuffer biomeTypeMapBuffer;
    public readonly ComputeBuffer fogOfWarMapBuffer;
    public readonly ChunkMapPoint[,] chunkMapPoints;
    public readonly EventList<Building> buildings;

    public bool usedCheck;

    public int chunkWidth => chunkDataMap.chunkWidth;
    public int chunkHeight => chunkDataMap.chunkHeight;

    public int xChunkPosition => chunkDataMap.xChunkPosition;
    public int yChunkPosition => chunkDataMap.yChunkPosition;

    public ChunkMap(ChunkDataMap chunkDataMap)
    {
        this.chunkDataMap = chunkDataMap;
        buildings = new EventList<Building>();
        biomeTypeMapBuffer = new ComputeBuffer(chunkDataMap.biomeTypeMap.Length, Marshal.SizeOf(typeof(int)));
        fogOfWarMapBuffer = new ComputeBuffer(chunkDataMap.biomeTypeMap.Length, Marshal.SizeOf(typeof(float)));

        chunkMapPoints = new ChunkMapPoint[chunkWidth, chunkHeight];

        int[] biomeTypeMap = new int[chunkDataMap.biomeTypeMap.Length];
        float[] fogOfWarMap = new float[chunkDataMap.biomeTypeMap.Length];


        for (int x = 0; x < chunkDataMap.biomeTypeMap.GetLength(0); x++)
        {
            for (int y = 0; y < chunkDataMap.biomeTypeMap.GetLength(1); y++)
            {
                int index = y * chunkDataMap.biomeTypeMap.GetLength(0) + x;
                biomeTypeMap[index] = (int)chunkDataMap.biomeTypeMap[x, y];
                fogOfWarMap[index] = chunkDataMap.fogOfWarMap[x, y].GetFogValue();
            }
        }

        for (int x = 0; x < chunkWidth; x++)
        {
            for (int y = 0; y < chunkHeight; y++)
            {
                WorldResourceRecord resourceRecord = chunkDataMap.resourceMap[x, y];

                if (resourceRecord != null)
                    chunkMapPoints[x, y] = new ChunkMapPoint(this, new Point(x, y), resourceRecord);
                else
                    chunkMapPoints[x, y] = new ChunkMapPoint(this, new Point(x, y));
            }
        }

        biomeTypeMapBuffer.SetData(biomeTypeMap);
        fogOfWarMapBuffer.SetData(fogOfWarMap);
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

        chunkMapPoint = chunkMapPoints[point.xIndex, point.yIndex];
        return true;
    }

    public ChunkMapPoint GetChunkMapPoint(Point point)
    {
        return chunkMapPoints[point.xIndex, point.yIndex];
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

    public void UpdateFogOfWarBuffer()
    {
        float[] fogOfWarMap = new float[chunkDataMap.fogOfWarMap.Length];
        for (int x = 0; x < chunkDataMap.fogOfWarMap.GetLength(0); x++)
        {
            for (int y = 0; y < chunkDataMap.fogOfWarMap.GetLength(1); y++)
            {
                int index = y * chunkDataMap.fogOfWarMap.GetLength(0) + x;
                fogOfWarMap[index] = chunkDataMap.fogOfWarMap[x, y].GetFogValue();
            }
        }
        fogOfWarMapBuffer.SetData(fogOfWarMap);
    }
}