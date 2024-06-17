
using UnityEngine;

public struct WorldContentSizeBounds
{
    public readonly int width;
    public readonly int height;
    public readonly int xOffset;
    public readonly int yOffset;

    public readonly float xCenter;
    public readonly float yCenter;

    public WorldContentSizeBounds(WorldContentSize size, CardinalDirection direction)
    {
        Point rotatedBottomLeft = new Point(0, 0).RotateByDirection(direction);
        Point rotatedTopRight = new Point(size.width - 1, size.height - 1).RotateByDirection(direction);

        int xMin = Mathf.Min(rotatedBottomLeft.xIndex, rotatedTopRight.xIndex);
        int xMax = Mathf.Max(rotatedBottomLeft.xIndex, rotatedTopRight.xIndex);
        int yMin = Mathf.Min(rotatedBottomLeft.yIndex, rotatedTopRight.yIndex);
        int yMax = Mathf.Max(rotatedBottomLeft.yIndex, rotatedTopRight.yIndex);

        yOffset = yMin;
        xOffset = xMin;

        width = xMax - xMin + 1;
        height = yMax - yMin + 1;

        xCenter = (xMin + xMax + 1) / 2f;
        yCenter = (yMin + yMax + 1) / 2f;
    }
}
