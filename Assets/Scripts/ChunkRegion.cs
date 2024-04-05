
using SheetCodes;
using System.Collections.Generic;
using UnityEngine;

public class ChunkRegion
{
    public readonly List<Point> points;
    public int xMin { private set; get; }
    public int yMin { private set; get; }
    public int xMax { private set; get; }
    public int yMax { private set; get; }
    public bool isBorderRegion;

    public ChunkRegion(bool isBorderRegion)
    {
        points = new List<Point>();
        xMin = int.MaxValue;
        yMin = int.MaxValue;
        xMax = int.MinValue;
        yMax = int.MinValue;
        this.isBorderRegion = isBorderRegion;
    }

    public void Clear()
    {
        points.Clear();
        xMin = int.MaxValue;
        yMin = int.MaxValue;
        xMax = int.MinValue;
        yMax = int.MinValue;
    }

    public void AddPoint(Point point)
    {
        points.Add(point);
        xMin = Mathf.Min(xMin, point.xIndex);
        yMin = Mathf.Min(yMin, point.yIndex);
        xMax = Mathf.Max(xMax, point.xIndex);
        yMax = Mathf.Max(yMax, point.yIndex);
    }

    public void RemovePoint(Point point)
    {
        Point foundPoint = points.Find(i => i.Equals(point));
        if (foundPoint != null)
            points.Remove(foundPoint);
    }

    public void GenerateContent(Map map)
    {
        List<Point> pointsToGenerate = new List<Point>(points);

        WorldResourceRecord treeRecord = WorldResourceIdentifier.Tree.GetRecord();
        for(int i = pointsToGenerate.Count - 1; i >= 0; i--)
        {
            Point point = pointsToGenerate[i];
            map.SetWorldResource(point.xIndex, point.yIndex, treeRecord);
            //if (map.GetRegion(point.xIndex - 1, point.yIndex) != this)
            //{
            //    map.SetWorldResource(point.xIndex, point.yIndex, treeRecord);
            //    pointsToGenerate.RemoveAt(i);
            //    continue;
            //}
            //
            //if (map.GetRegion(point.xIndex + 1, point.yIndex) != this)
            //{
            //    map.SetWorldResource(point.xIndex, point.yIndex, treeRecord);
            //    pointsToGenerate.RemoveAt(i);
            //    continue;
            //}
            //
            //if (map.GetRegion(point.xIndex, point.yIndex - 1) != this)
            //{
            //    map.SetWorldResource(point.xIndex, point.yIndex, treeRecord);
            //    pointsToGenerate.RemoveAt(i);
            //    continue;
            //}
            //
            //if (map.GetRegion(point.xIndex, point.yIndex + 1) != this)
            //{
            //    map.SetWorldResource(point.xIndex, point.yIndex, treeRecord);
            //    pointsToGenerate.RemoveAt(i);
            //    continue;
            //}
        }
    }
}
