
using System;

public enum CardinalDirection
{
    Top,
    Right,
    Bottom,
    Left,
}

public static class CardinalDirectionExtensions
{
    public static readonly CardinalDirection[] ALL_DIRECTIONS;
    static CardinalDirectionExtensions()
    {
        ALL_DIRECTIONS = Enum.GetValues(typeof(CardinalDirection)) as CardinalDirection[];
    }

    public static CardinalDirection RotateByDirection(this CardinalDirection direction, CardinalDirection otherDirection)
    {
        switch (otherDirection)
        {
            case CardinalDirection.Right:
                return direction;
            case CardinalDirection.Bottom:
                return direction.Rotate90Degrees();
            case CardinalDirection.Left:
                return direction.Rotate180Degrees();
            case CardinalDirection.Top:
                return direction.Rotate270Degrees();
        }

        throw new Exception(string.Format("GetEnterExitDirection is missing implementation for {0}", direction));
    }

    public static EnterExitDirection GetEnterExitDirection(this CardinalDirection direction)
    {
        switch (direction)
        {
            case CardinalDirection.Top:
                return EnterExitDirection.Top;
            case CardinalDirection.Right:
                return EnterExitDirection.Right;
            case CardinalDirection.Bottom:
                return EnterExitDirection.Bottom;
            case CardinalDirection.Left:
                return EnterExitDirection.Left;
        }

        throw new Exception(string.Format("GetEnterExitDirection is missing implementation for {0}", direction));
    }

    public static CardinalDirection Rotate90Degrees(this CardinalDirection direction)
    {
        switch (direction)
        {
            case CardinalDirection.Top:
                return CardinalDirection.Right;
            case CardinalDirection.Right:
                return CardinalDirection.Bottom;
            case CardinalDirection.Bottom:
                return CardinalDirection.Left;
            case CardinalDirection.Left:
                return CardinalDirection.Top;
        }

        throw new Exception(string.Format("Rotate90Degrees is missing implementation for {0}", direction));
    }

    public static CardinalDirection Rotate180Degrees(this CardinalDirection direction)
    {
        switch (direction)
        {
            case CardinalDirection.Top:
                return CardinalDirection.Bottom;
            case CardinalDirection.Right:
                return CardinalDirection.Left;
            case CardinalDirection.Bottom:
                return CardinalDirection.Top;
            case CardinalDirection.Left:
                return CardinalDirection.Right;
        }

        throw new Exception(string.Format("RotateLeft is missing implementation for {0}", direction));
    }

    public static CardinalDirection Rotate270Degrees(this CardinalDirection direction)
    {
        switch (direction)
        {
            case CardinalDirection.Top:
                return CardinalDirection.Left;
            case CardinalDirection.Right:
                return CardinalDirection.Top;
            case CardinalDirection.Bottom:
                return CardinalDirection.Right;
            case CardinalDirection.Left:
                return CardinalDirection.Bottom;
        }

        throw new Exception(string.Format("Rotate270Degrees is missing implementation for {0}", direction));
    }
}