using System;

[Serializable]
public struct WorldContentSize
{
    public int width;
    public int height;

    public WorldContentSize(int width, int height)
    {
        this.width = width;
        this.height = height;
    }

    public WorldContentSizeBounds GetSizeBounds(CardinalDirection direction)
    {
        return new WorldContentSizeBounds(this, direction);
    }
}