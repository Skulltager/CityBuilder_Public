
using System;
using UnityEngine;

[Serializable]
public class BuildingEnterExitGridPoint
{
    [field: SerializeField] public Point centerOffset;
    [field: SerializeField] public CardinalDirection direction;

    public Point RotateByDirection(CardinalDirection direction)
    {
        switch (direction)
        {
            case CardinalDirection.Right:
                return Get0DegreesRotated();
            case CardinalDirection.Bottom:
                return Get90DegreesRotated();
            case CardinalDirection.Left:
                return Get180DegreesRotated();
            case CardinalDirection.Top:
                return Get270DegreesRotated();
        }

        throw new System.Exception(string.Format("RotateByDirection missing implementation for {0}", direction));
    }

    private Point Get0DegreesRotated()
    {
        return centerOffset.AddDirection(direction);
    }

    private Point Get90DegreesRotated()
    {
        return centerOffset.AddDirection(direction).GetRotated90DegreesOffset();
    }

    private Point Get180DegreesRotated()
    {
        return centerOffset.AddDirection(direction).GetRotated180DegreesOffset();
    }

    private Point Get270DegreesRotated()
    {
        return centerOffset.AddDirection(direction).GetRotated270DegreesOffset();
    }
}