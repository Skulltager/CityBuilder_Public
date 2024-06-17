
using System;

public enum DiagonalDirection
{
    BottomLeft,
    BottomCenter,
    BottomRight,
    CenterLeft,
    CenterRight,
    TopLeft,
    TopCenter,
    TopRight,
}


public static class DiagonalDirectionExtensions
{
    public static readonly DiagonalDirection[] ALL_DIRECTIONS;

    static DiagonalDirectionExtensions()
    {
        ALL_DIRECTIONS = Enum.GetValues(typeof(DiagonalDirection)) as DiagonalDirection[];
    }
}
