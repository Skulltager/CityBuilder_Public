public enum TilePosition
{
    Center,
    BottomLeft,
    BottomCenter,
    BottomRight,
    CenterLeft,
    CenterRight,
    TopLeft,
    TopCenter,
    TopRight,
}

public static class TilePositionExtension
{ 
    public static TilePosition Flip(this TilePosition tilePosition)
    {
        switch (tilePosition)
        {
            case TilePosition.Center:
                return TilePosition.Center;
            case TilePosition.BottomLeft:
                return TilePosition.TopRight;
            case TilePosition.BottomCenter:
                return TilePosition.TopCenter;
            case TilePosition.BottomRight:
                return TilePosition.TopLeft;
            case TilePosition.CenterLeft:
                return TilePosition.CenterRight;
            case TilePosition.CenterRight:
                return TilePosition.CenterLeft;
            case TilePosition.TopLeft:
                return TilePosition.BottomRight;
            case TilePosition.TopCenter:
                return TilePosition.BottomCenter;
            case TilePosition.TopRight:
                return TilePosition.BottomLeft;
        }

        throw new System.Exception(string.Format("Flip is missing implementation for {0}", tilePosition));
    }

}