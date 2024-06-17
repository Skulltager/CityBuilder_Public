using System.Runtime.InteropServices;
using UnityEngine;

public class ChunkMap
{
    public readonly ChunkDataMap chunkDataMap;
    public readonly ComputeBuffer biomeTypeMapBuffer;
    public readonly ComputeBuffer fogOfWarMapBuffer;
    public readonly ComputeBuffer roadsBuffer;
    public readonly ChunkMapPoint[,] chunkMapPoints;
    public readonly PathFindingPointData[,] pathFindingPreviousPointBuffer;

    public bool usedCheck;

    public int chunkWidth => chunkDataMap.chunkWidth;
    public int chunkHeight => chunkDataMap.chunkHeight;

    public int xChunkPosition => chunkDataMap.xChunkPosition;
    public int yChunkPosition => chunkDataMap.yChunkPosition;

    public ChunkMap(ChunkDataMap chunkDataMap)
    {
        this.chunkDataMap = chunkDataMap;
        biomeTypeMapBuffer = new ComputeBuffer(chunkDataMap.biomeTypeMap.Length, Marshal.SizeOf(typeof(int)));
        fogOfWarMapBuffer = new ComputeBuffer(chunkDataMap.biomeTypeMap.Length, Marshal.SizeOf(typeof(float)));
        roadsBuffer = new ComputeBuffer(chunkDataMap.biomeTypeMap.Length, Marshal.SizeOf(typeof(float)));
        pathFindingPreviousPointBuffer = new PathFindingPointData[chunkWidth, chunkHeight];

        chunkMapPoints = new ChunkMapPoint[chunkWidth, chunkHeight];

        int[] biomeTypeMap = new int[chunkDataMap.biomeTypeMap.Length];
        float[] fogOfWarMap = new float[chunkDataMap.biomeTypeMap.Length];
        float[] roadsMap = new float[chunkDataMap.biomeTypeMap.Length];


        for (int x = 0; x < chunkDataMap.biomeTypeMap.GetLength(0); x++)
        {
            for (int y = 0; y < chunkDataMap.biomeTypeMap.GetLength(1); y++)
            {
                int index = y * chunkDataMap.biomeTypeMap.GetLength(0) + x;
                biomeTypeMap[index] = (int)chunkDataMap.biomeTypeMap[x, y];
                fogOfWarMap[index] = chunkDataMap.fogOfWarMap[x, y].GetFogValue();
                roadsMap[index] = chunkDataMap.roadsMap[x, y] ? 1 : 0;
            }
        }

        for (int x = 0; x < chunkWidth; x++)
        {
            for (int y = 0; y < chunkHeight; y++)
            {
                ChunkRegion chunkRegion = chunkDataMap.regionMap.assignedRegions[x, y];
                chunkMapPoints[x, y] = new ChunkMapPoint(this, chunkRegion, new Point(x, y));
            }
        }

        biomeTypeMapBuffer.SetData(biomeTypeMap);
        fogOfWarMapBuffer.SetData(fogOfWarMap);
        roadsBuffer.SetData(roadsMap);
    }

    public void GenerateContent()
    {
        for (int x = 0; x < chunkWidth; x++)
        {
            for (int y = 0; y < chunkHeight; y++)
            {
                ChunkRegionPointContent regionPointContent = chunkDataMap.contentMap[x, y];

                if (regionPointContent == null)
                    continue;

                ChunkMapPoint chunkMapPoint = chunkMapPoints[x, y];
                regionPointContent.SetChunkMapPointData(chunkMapPoint);
            }
        }
    }

    public void DisposeBuffers()
    {
        biomeTypeMapBuffer.Dispose();
        fogOfWarMapBuffer.Dispose();
        roadsBuffer.Dispose();
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

    public void UpdateRoadsBuffer()
    {
        float[] roadsMap = new float[chunkDataMap.roadsMap.Length];
        for (int x = 0; x < chunkDataMap.roadsMap.GetLength(0); x++)
        {
            for (int y = 0; y < chunkDataMap.roadsMap.GetLength(1); y++)
            {
                int index = y * chunkDataMap.roadsMap.GetLength(0) + x;
                roadsMap[index] = chunkDataMap.roadsMap[x, y] ? 1 : 0;
            }
        }

        roadsBuffer.SetData(roadsMap);
    }
}