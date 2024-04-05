
public enum CardinalDirection
{
    Top,
    Right,
    Bottom,
    Left,
}

public static class CardinalDirectionExtensions
{ 
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

        throw new System.Exception(string.Format("Rotate90Degrees is missing implementation for {0}", direction));
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

        throw new System.Exception(string.Format("RotateLeft is missing implementation for {0}", direction));
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

        throw new System.Exception(string.Format("Rotate270Degrees is missing implementation for {0}", direction));
    }
}