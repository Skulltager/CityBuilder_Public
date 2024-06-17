
using System;
using UnityEngine;

[Serializable]
public class BuildingGridPoint
{
    [field: SerializeField] public Point centerOffset;
    [field: SerializeField] public TileTraversalEdgeDirection travelEdgeDirection;

    public Point Get0DegreesRotated()
    {
        return centerOffset;
    }

    public Point Get90DegreesRotated()
    {
          return centerOffset.GetRotated90DegreesOffset().AddDirection(CardinalDirection.Bottom);
    }

    public Point Get180DegreesRotated()
    {

        return centerOffset.GetRotated180DegreesOffset().AddDirection(DiagonalDirection.BottomLeft);
    }

    public Point Get270DegreesRotated()
    {
        return centerOffset.GetRotated270DegreesOffset().AddDirection(CardinalDirection.Left);
    }
}