using System;

public enum TileTraversalEdgeDirection
{
    Free = 0,
    BottomLeft = 1,
    BottomCenter = 2,
    BottomRight = 3,
    CenterLeft = 4,
    CenterRight = 5,
    TopLeft = 6,
    TopCenter = 7,
    TopRight = 8,
    AllEdges = 9,
    TopBottom = 10,
    LeftRight = 11,
    TopRightBottom = 12,
    RightBottomLeft = 13,
    BottomLeftTop = 14,
    LeftTopRight = 15,
    Impossible = 16,
}

public static class TileTraversalEdgeDirectionExtension
{
    public static bool CanTravelInDirection(this TileTraversalEdgeDirection direction, CardinalDirection travelDirection)
    {
        switch(travelDirection)
        {
            case CardinalDirection.Left:
                return direction.CanTravelLeft();
            case CardinalDirection.Right:
                return direction.CanTravelRight();
            case CardinalDirection.Top:
                return direction.CanTravelUp();
            case CardinalDirection.Bottom:
                return direction.CanTravelDown();
        }

        throw new Exception(string.Format("CanTravelInDirection is missing implementation for {0}", travelDirection));
    }

    private static bool CanTravelUp(this TileTraversalEdgeDirection direction)
    {
        switch (direction)
        {
            case TileTraversalEdgeDirection.Free:
            case TileTraversalEdgeDirection.BottomLeft:
            case TileTraversalEdgeDirection.BottomRight:
            case TileTraversalEdgeDirection.TopLeft:
            case TileTraversalEdgeDirection.TopRight:
            case TileTraversalEdgeDirection.CenterLeft:
            case TileTraversalEdgeDirection.CenterRight:
            case TileTraversalEdgeDirection.AllEdges:
            case TileTraversalEdgeDirection.LeftTopRight:
            case TileTraversalEdgeDirection.TopRightBottom:
            case TileTraversalEdgeDirection.TopBottom:
            case TileTraversalEdgeDirection.TopCenter:
            case TileTraversalEdgeDirection.RightBottomLeft:
            case TileTraversalEdgeDirection.BottomLeftTop:
            case TileTraversalEdgeDirection.LeftRight:
                return true;
            case TileTraversalEdgeDirection.BottomCenter:
            case TileTraversalEdgeDirection.Impossible:
                return false;
        }

        throw new Exception(string.Format("CanTravelUp is missing implementation for {0}", direction));
    }

    private static bool CanTravelDown(this TileTraversalEdgeDirection direction)
    {
        switch (direction)
        {
            case TileTraversalEdgeDirection.Free:
            case TileTraversalEdgeDirection.BottomLeft:
            case TileTraversalEdgeDirection.BottomRight:
            case TileTraversalEdgeDirection.TopLeft:
            case TileTraversalEdgeDirection.TopRight:
            case TileTraversalEdgeDirection.CenterLeft:
            case TileTraversalEdgeDirection.CenterRight:
            case TileTraversalEdgeDirection.AllEdges:
            case TileTraversalEdgeDirection.LeftTopRight:
            case TileTraversalEdgeDirection.RightBottomLeft:
            case TileTraversalEdgeDirection.TopRightBottom:
            case TileTraversalEdgeDirection.BottomLeftTop:
            case TileTraversalEdgeDirection.TopBottom:
            case TileTraversalEdgeDirection.BottomCenter:
            case TileTraversalEdgeDirection.LeftRight:
                return true;
            case TileTraversalEdgeDirection.TopCenter:
            case TileTraversalEdgeDirection.Impossible:
                return false;
        }

        throw new Exception(string.Format("CanTravelDown is missing implementation for {0}", direction));
    }

    private static bool CanTravelRight(this TileTraversalEdgeDirection direction)
    {

        switch (direction)
        {
            case TileTraversalEdgeDirection.Free:
            case TileTraversalEdgeDirection.BottomLeft:
            case TileTraversalEdgeDirection.BottomRight:
            case TileTraversalEdgeDirection.TopLeft:
            case TileTraversalEdgeDirection.TopRight:
            case TileTraversalEdgeDirection.BottomCenter:
            case TileTraversalEdgeDirection.TopCenter:
            case TileTraversalEdgeDirection.AllEdges:
            case TileTraversalEdgeDirection.LeftTopRight:
            case TileTraversalEdgeDirection.RightBottomLeft:
            case TileTraversalEdgeDirection.TopRightBottom:
            case TileTraversalEdgeDirection.BottomLeftTop:
            case TileTraversalEdgeDirection.TopBottom:
            case TileTraversalEdgeDirection.LeftRight:
            case TileTraversalEdgeDirection.CenterRight:
                return true;
            case TileTraversalEdgeDirection.CenterLeft:
            case TileTraversalEdgeDirection.Impossible:
                return false;
        }

        throw new Exception(string.Format("CanTravelRight is missing implementation for {0}", direction));
    }

    private static bool CanTravelLeft(this TileTraversalEdgeDirection direction)
    {

        switch (direction)
        {
            case TileTraversalEdgeDirection.Free:
            case TileTraversalEdgeDirection.BottomLeft:
            case TileTraversalEdgeDirection.BottomRight:
            case TileTraversalEdgeDirection.TopLeft:
            case TileTraversalEdgeDirection.TopRight:
            case TileTraversalEdgeDirection.BottomCenter:
            case TileTraversalEdgeDirection.TopCenter:
            case TileTraversalEdgeDirection.AllEdges:
            case TileTraversalEdgeDirection.LeftTopRight:
            case TileTraversalEdgeDirection.RightBottomLeft:
            case TileTraversalEdgeDirection.TopRightBottom:
            case TileTraversalEdgeDirection.BottomLeftTop:
            case TileTraversalEdgeDirection.TopBottom:
            case TileTraversalEdgeDirection.LeftRight:
            case TileTraversalEdgeDirection.CenterLeft:
                return true;
            case TileTraversalEdgeDirection.CenterRight:
            case TileTraversalEdgeDirection.Impossible:
                return false;
        }

        throw new Exception(string.Format("CanTravelLeft is missing implementation for {0}", direction));
    }

    public static TileTraversalEdgeDirection Rotate90Degrees(this  TileTraversalEdgeDirection direction)
    {
        switch (direction)
        {
            case TileTraversalEdgeDirection.Free:
                return TileTraversalEdgeDirection.Free;
            case TileTraversalEdgeDirection.AllEdges:
                return TileTraversalEdgeDirection.AllEdges;
            case TileTraversalEdgeDirection.BottomLeft:
                return TileTraversalEdgeDirection.TopLeft;
            case TileTraversalEdgeDirection.BottomCenter:
                return TileTraversalEdgeDirection.CenterLeft;
            case TileTraversalEdgeDirection.BottomRight:
                return TileTraversalEdgeDirection.BottomLeft;
            case TileTraversalEdgeDirection.CenterLeft:
                return TileTraversalEdgeDirection.TopCenter;
            case TileTraversalEdgeDirection.CenterRight:
                return TileTraversalEdgeDirection.BottomCenter;
            case TileTraversalEdgeDirection.TopLeft:
                return TileTraversalEdgeDirection.TopRight;
            case TileTraversalEdgeDirection.TopCenter:
                return TileTraversalEdgeDirection.CenterRight;
            case TileTraversalEdgeDirection.TopRight:
                return TileTraversalEdgeDirection.BottomRight;
            case TileTraversalEdgeDirection.TopBottom:
                return TileTraversalEdgeDirection.LeftRight;
            case TileTraversalEdgeDirection.LeftRight:
                return TileTraversalEdgeDirection.TopBottom;
            case TileTraversalEdgeDirection.LeftTopRight:
                return TileTraversalEdgeDirection.TopRightBottom;
            case TileTraversalEdgeDirection.TopRightBottom:
                return TileTraversalEdgeDirection.RightBottomLeft;
            case TileTraversalEdgeDirection.RightBottomLeft:
                return TileTraversalEdgeDirection.BottomLeftTop;
            case TileTraversalEdgeDirection.BottomLeftTop:
                return TileTraversalEdgeDirection.LeftTopRight;
            case TileTraversalEdgeDirection.Impossible:
                return TileTraversalEdgeDirection.Impossible;
        }

        throw new Exception(string.Format("Rotate90Degrees is missing implementation for {0}", direction));
    }

    public static TileTraversalEdgeDirection Rotate180Degrees(this TileTraversalEdgeDirection direction)
    {
        switch (direction)
        {
            case TileTraversalEdgeDirection.Free:
                return TileTraversalEdgeDirection.Free;
            case TileTraversalEdgeDirection.AllEdges:
                return TileTraversalEdgeDirection.AllEdges;
            case TileTraversalEdgeDirection.BottomLeft:
                return TileTraversalEdgeDirection.TopRight;
            case TileTraversalEdgeDirection.BottomCenter:
                return TileTraversalEdgeDirection.TopCenter;
            case TileTraversalEdgeDirection.BottomRight:
                return TileTraversalEdgeDirection.TopLeft;
            case TileTraversalEdgeDirection.CenterLeft:
                return TileTraversalEdgeDirection.CenterRight;
            case TileTraversalEdgeDirection.CenterRight:
                return TileTraversalEdgeDirection.CenterLeft;
            case TileTraversalEdgeDirection.TopLeft:
                return TileTraversalEdgeDirection.BottomRight;
            case TileTraversalEdgeDirection.TopCenter:
                return TileTraversalEdgeDirection.BottomCenter;
            case TileTraversalEdgeDirection.TopRight:
                return TileTraversalEdgeDirection.BottomLeft;
            case TileTraversalEdgeDirection.TopBottom:
                return TileTraversalEdgeDirection.TopBottom;
            case TileTraversalEdgeDirection.LeftRight:
                return TileTraversalEdgeDirection.LeftRight;
            case TileTraversalEdgeDirection.LeftTopRight:
                return TileTraversalEdgeDirection.RightBottomLeft;
            case TileTraversalEdgeDirection.TopRightBottom:
                return TileTraversalEdgeDirection.BottomLeftTop;
            case TileTraversalEdgeDirection.RightBottomLeft:
                return TileTraversalEdgeDirection.LeftTopRight;
            case TileTraversalEdgeDirection.BottomLeftTop:
                return TileTraversalEdgeDirection.TopRightBottom;
            case TileTraversalEdgeDirection.Impossible:
                return TileTraversalEdgeDirection.Impossible;
        }

        throw new Exception(string.Format("Rotate180Degrees is missing implementation for {0}", direction));
    }

    public static TileTraversalEdgeDirection Rotate270Degrees(this  TileTraversalEdgeDirection direction)
    {
        switch (direction)
        {
            case TileTraversalEdgeDirection.Free:
                return TileTraversalEdgeDirection.Free;
            case TileTraversalEdgeDirection.AllEdges:
                return TileTraversalEdgeDirection.AllEdges;
            case TileTraversalEdgeDirection.BottomLeft:
                return TileTraversalEdgeDirection.BottomRight;
            case TileTraversalEdgeDirection.BottomCenter:
                return TileTraversalEdgeDirection.CenterRight;
            case TileTraversalEdgeDirection.BottomRight:
                return TileTraversalEdgeDirection.TopRight;
            case TileTraversalEdgeDirection.CenterLeft:
                return TileTraversalEdgeDirection.BottomCenter;
            case TileTraversalEdgeDirection.CenterRight:
                return TileTraversalEdgeDirection.TopCenter;
            case TileTraversalEdgeDirection.TopLeft:
                return TileTraversalEdgeDirection.BottomLeft;
            case TileTraversalEdgeDirection.TopCenter:
                return TileTraversalEdgeDirection.CenterLeft;
            case TileTraversalEdgeDirection.TopRight:
                return TileTraversalEdgeDirection.TopLeft;
            case TileTraversalEdgeDirection.TopBottom:
                return TileTraversalEdgeDirection.LeftRight;
            case TileTraversalEdgeDirection.LeftRight:
                return TileTraversalEdgeDirection.TopBottom;
            case TileTraversalEdgeDirection.LeftTopRight:
                return TileTraversalEdgeDirection.BottomLeftTop;
            case TileTraversalEdgeDirection.TopRightBottom:
                return TileTraversalEdgeDirection.LeftTopRight;
            case TileTraversalEdgeDirection.RightBottomLeft:
                return TileTraversalEdgeDirection.TopRightBottom;
            case TileTraversalEdgeDirection.BottomLeftTop:
                return TileTraversalEdgeDirection.RightBottomLeft;
            case TileTraversalEdgeDirection.Impossible:
                return TileTraversalEdgeDirection.Impossible;
        }

        throw new Exception(string.Format("Rotate90Degrees is missing implementation for {0}", direction));
    }

    public static bool CanGoThroughMiddle(this TileTraversalEdgeDirection direction)
    {
        switch (direction)
        {
            case TileTraversalEdgeDirection.Free:
                return true;

            case TileTraversalEdgeDirection.BottomLeft:
            case TileTraversalEdgeDirection.BottomCenter:
            case TileTraversalEdgeDirection.BottomRight:
            case TileTraversalEdgeDirection.CenterLeft:
            case TileTraversalEdgeDirection.CenterRight:
            case TileTraversalEdgeDirection.TopLeft:
            case TileTraversalEdgeDirection.TopCenter:
            case TileTraversalEdgeDirection.TopRight:
            case TileTraversalEdgeDirection.AllEdges:
            case TileTraversalEdgeDirection.TopBottom:
            case TileTraversalEdgeDirection.LeftRight:
            case TileTraversalEdgeDirection.TopRightBottom:
            case TileTraversalEdgeDirection.RightBottomLeft:
            case TileTraversalEdgeDirection.BottomLeftTop:
            case TileTraversalEdgeDirection.LeftTopRight:
            case TileTraversalEdgeDirection.Impossible:
                return false;
        }

        throw new Exception(string.Format("CanGoThroughMiddle is missing implementation for {0}", direction));
    }

    public static bool CanGoThroughBottom(this TileTraversalEdgeDirection direction)
    {
        switch (direction)
        {
            case TileTraversalEdgeDirection.Free:
            case TileTraversalEdgeDirection.BottomLeft:
            case TileTraversalEdgeDirection.BottomCenter:
            case TileTraversalEdgeDirection.BottomRight:
            case TileTraversalEdgeDirection.TopBottom:
            case TileTraversalEdgeDirection.TopRightBottom:
            case TileTraversalEdgeDirection.RightBottomLeft:
            case TileTraversalEdgeDirection.BottomLeftTop:
            case TileTraversalEdgeDirection.AllEdges:
                return true;

            case TileTraversalEdgeDirection.CenterLeft:
            case TileTraversalEdgeDirection.CenterRight:
            case TileTraversalEdgeDirection.TopLeft:
            case TileTraversalEdgeDirection.TopCenter:
            case TileTraversalEdgeDirection.TopRight:
            case TileTraversalEdgeDirection.LeftRight:
            case TileTraversalEdgeDirection.LeftTopRight:
            case TileTraversalEdgeDirection.Impossible:
                return false;
        }

        throw new Exception(string.Format("CanGoThroughBottom is missing implementation for {0}", direction));
    }

    public static bool CanGoThroughTop(this TileTraversalEdgeDirection direction)
    {
        switch (direction)
        {
            case TileTraversalEdgeDirection.Free:
            case TileTraversalEdgeDirection.TopLeft:
            case TileTraversalEdgeDirection.TopCenter:
            case TileTraversalEdgeDirection.TopRight:
            case TileTraversalEdgeDirection.AllEdges:
            case TileTraversalEdgeDirection.TopBottom:
            case TileTraversalEdgeDirection.TopRightBottom:
            case TileTraversalEdgeDirection.BottomLeftTop:
            case TileTraversalEdgeDirection.LeftTopRight:
                return true;

            case TileTraversalEdgeDirection.BottomLeft:
            case TileTraversalEdgeDirection.BottomCenter:
            case TileTraversalEdgeDirection.BottomRight:
            case TileTraversalEdgeDirection.CenterLeft:
            case TileTraversalEdgeDirection.CenterRight:
            case TileTraversalEdgeDirection.LeftRight:
            case TileTraversalEdgeDirection.RightBottomLeft:
            case TileTraversalEdgeDirection.Impossible:
                return false;
        }

        throw new Exception(string.Format("CanGoThroughTop is missing implementation for {0}", direction));
    }

    public static bool CanGoThroughLeft(this TileTraversalEdgeDirection direction)
    {
        switch (direction)
        {
            case TileTraversalEdgeDirection.Free:
            case TileTraversalEdgeDirection.BottomLeft:
            case TileTraversalEdgeDirection.CenterLeft:
            case TileTraversalEdgeDirection.TopLeft:
            case TileTraversalEdgeDirection.AllEdges:
            case TileTraversalEdgeDirection.LeftRight:
            case TileTraversalEdgeDirection.RightBottomLeft:
            case TileTraversalEdgeDirection.BottomLeftTop:
            case TileTraversalEdgeDirection.LeftTopRight:
                return true;

            case TileTraversalEdgeDirection.BottomCenter:
            case TileTraversalEdgeDirection.BottomRight:
            case TileTraversalEdgeDirection.CenterRight:
            case TileTraversalEdgeDirection.TopCenter:
            case TileTraversalEdgeDirection.TopRight:
            case TileTraversalEdgeDirection.TopBottom:
            case TileTraversalEdgeDirection.TopRightBottom:
            case TileTraversalEdgeDirection.Impossible:
                return false;
        }

        throw new Exception(string.Format("CanGoThroughLeft is missing implementation for {0}", direction));
    }

    public static bool CanGoThroughRight(this TileTraversalEdgeDirection direction)
    {
        switch (direction)
        {
            case TileTraversalEdgeDirection.Free:
            case TileTraversalEdgeDirection.BottomRight:
            case TileTraversalEdgeDirection.CenterRight:
            case TileTraversalEdgeDirection.TopRight:
            case TileTraversalEdgeDirection.AllEdges:
            case TileTraversalEdgeDirection.LeftRight:
            case TileTraversalEdgeDirection.TopRightBottom:
            case TileTraversalEdgeDirection.RightBottomLeft:
            case TileTraversalEdgeDirection.LeftTopRight:
                return true;

            case TileTraversalEdgeDirection.BottomLeft:
            case TileTraversalEdgeDirection.BottomCenter:
            case TileTraversalEdgeDirection.CenterLeft:
            case TileTraversalEdgeDirection.TopLeft:
            case TileTraversalEdgeDirection.TopCenter:
            case TileTraversalEdgeDirection.TopBottom:
            case TileTraversalEdgeDirection.BottomLeftTop:
            case TileTraversalEdgeDirection.Impossible:
                return false;
        }

        throw new Exception(string.Format("CanGoThroughRight is missing implementation for {0}", direction));
    }

    public static bool IsViablePosition(this TileTraversalEdgeDirection edgeDirection, TilePosition tilePosition)
    {
        switch (tilePosition)
        {
            case TilePosition.Center:
                return edgeDirection.IsViablePosition_Center();
            case TilePosition.BottomLeft:
                return edgeDirection.IsViablePosition_BottomLeft();
            case TilePosition.BottomCenter:
                return edgeDirection.IsViablePosition_BottomCenter();
            case TilePosition.BottomRight:
                return edgeDirection.IsViablePosition_BottomRight();
            case TilePosition.CenterLeft:
                return edgeDirection.IsViablePosition_CenterLeft();
            case TilePosition.CenterRight:
                return edgeDirection.IsViablePosition_CenterRight();
            case TilePosition.TopLeft:
                return edgeDirection.IsViablePosition_TopLeft();
            case TilePosition.TopCenter:
                return edgeDirection.IsViablePosition_TopCenter();
            case TilePosition.TopRight:
                return edgeDirection.IsViablePosition_TopRight();
        }

        throw new Exception(string.Format("IsViablePosition is missing implementation for {0}", tilePosition));
    }

    private static bool IsViablePosition_TopLeft(this TileTraversalEdgeDirection edgeDirection)
    {
        switch (edgeDirection)
        {
            case TileTraversalEdgeDirection.Free:
            case TileTraversalEdgeDirection.BottomLeft:
            case TileTraversalEdgeDirection.CenterLeft:
            case TileTraversalEdgeDirection.TopLeft:
            case TileTraversalEdgeDirection.TopCenter:
            case TileTraversalEdgeDirection.TopRight:
            case TileTraversalEdgeDirection.AllEdges:
            case TileTraversalEdgeDirection.TopBottom:
            case TileTraversalEdgeDirection.LeftRight:
            case TileTraversalEdgeDirection.TopRightBottom:
            case TileTraversalEdgeDirection.RightBottomLeft:
            case TileTraversalEdgeDirection.BottomLeftTop:
            case TileTraversalEdgeDirection.LeftTopRight:
                return true;
            case TileTraversalEdgeDirection.BottomCenter:
            case TileTraversalEdgeDirection.BottomRight:
            case TileTraversalEdgeDirection.CenterRight:
            case TileTraversalEdgeDirection.Impossible:
                return false;
        }

        throw new Exception(string.Format("IsViablePosition_TopLeft is missing implementation for {0}", edgeDirection));
    }

    private static bool IsViablePosition_TopCenter(this TileTraversalEdgeDirection edgeDirection)
    {
        switch (edgeDirection)
        {
            case TileTraversalEdgeDirection.Free:
            case TileTraversalEdgeDirection.TopLeft:
            case TileTraversalEdgeDirection.TopCenter:
            case TileTraversalEdgeDirection.TopRight:
            case TileTraversalEdgeDirection.AllEdges:
            case TileTraversalEdgeDirection.TopBottom:
            case TileTraversalEdgeDirection.BottomLeftTop:
            case TileTraversalEdgeDirection.LeftTopRight:
            case TileTraversalEdgeDirection.TopRightBottom:
                return true;
            case TileTraversalEdgeDirection.CenterRight:
            case TileTraversalEdgeDirection.BottomRight:
            case TileTraversalEdgeDirection.BottomCenter:
            case TileTraversalEdgeDirection.BottomLeft:
            case TileTraversalEdgeDirection.CenterLeft:
            case TileTraversalEdgeDirection.LeftRight:
            case TileTraversalEdgeDirection.RightBottomLeft:
            case TileTraversalEdgeDirection.Impossible:
                return false;
        }

        throw new Exception(string.Format("IsViablePosition_TopCenter is missing implementation for {0}", edgeDirection));
    }

    private static bool IsViablePosition_TopRight(this TileTraversalEdgeDirection edgeDirection)
    {
        switch (edgeDirection)
        {
            case TileTraversalEdgeDirection.Free:
            case TileTraversalEdgeDirection.BottomRight:
            case TileTraversalEdgeDirection.CenterRight:
            case TileTraversalEdgeDirection.TopLeft:
            case TileTraversalEdgeDirection.TopCenter:
            case TileTraversalEdgeDirection.TopRight:
            case TileTraversalEdgeDirection.AllEdges:
            case TileTraversalEdgeDirection.TopBottom:
            case TileTraversalEdgeDirection.LeftRight:
            case TileTraversalEdgeDirection.TopRightBottom:
            case TileTraversalEdgeDirection.RightBottomLeft:
            case TileTraversalEdgeDirection.BottomLeftTop:
            case TileTraversalEdgeDirection.LeftTopRight:
                return true;
            case TileTraversalEdgeDirection.BottomCenter:
            case TileTraversalEdgeDirection.CenterLeft:
            case TileTraversalEdgeDirection.BottomLeft:
            case TileTraversalEdgeDirection.Impossible:
                return false;
        }

        throw new Exception(string.Format("IsViablePosition_TopRight is missing implementation for {0}", edgeDirection));
    }

    private static bool IsViablePosition_CenterLeft(this TileTraversalEdgeDirection edgeDirection)
    {
        switch (edgeDirection)
        {
            case TileTraversalEdgeDirection.Free:
            case TileTraversalEdgeDirection.BottomLeft:
            case TileTraversalEdgeDirection.AllEdges:
            case TileTraversalEdgeDirection.LeftRight:
            case TileTraversalEdgeDirection.RightBottomLeft:
            case TileTraversalEdgeDirection.BottomLeftTop:
            case TileTraversalEdgeDirection.LeftTopRight:
            case TileTraversalEdgeDirection.CenterLeft:
            case TileTraversalEdgeDirection.TopLeft:
                return true;
            case TileTraversalEdgeDirection.TopCenter:
            case TileTraversalEdgeDirection.TopRight:
            case TileTraversalEdgeDirection.CenterRight:
            case TileTraversalEdgeDirection.BottomRight:
            case TileTraversalEdgeDirection.BottomCenter:
            case TileTraversalEdgeDirection.TopBottom:
            case TileTraversalEdgeDirection.TopRightBottom:
            case TileTraversalEdgeDirection.Impossible:
                return false;
        }

        throw new Exception(string.Format("IsViablePosition_CenterLeft is missing implementation for {0}", edgeDirection));
    }

    private static bool IsViablePosition_Center(this TileTraversalEdgeDirection edgeDirection)
    {
        switch (edgeDirection)
        {
            case TileTraversalEdgeDirection.Free:
                return true;
            case TileTraversalEdgeDirection.TopLeft:
            case TileTraversalEdgeDirection.TopCenter:
            case TileTraversalEdgeDirection.TopRight:
            case TileTraversalEdgeDirection.AllEdges:
            case TileTraversalEdgeDirection.TopBottom:
            case TileTraversalEdgeDirection.LeftRight:
            case TileTraversalEdgeDirection.TopRightBottom:
            case TileTraversalEdgeDirection.RightBottomLeft:
            case TileTraversalEdgeDirection.BottomLeftTop:
            case TileTraversalEdgeDirection.LeftTopRight:
            case TileTraversalEdgeDirection.CenterRight:
            case TileTraversalEdgeDirection.BottomRight:
            case TileTraversalEdgeDirection.BottomCenter:
            case TileTraversalEdgeDirection.CenterLeft:
            case TileTraversalEdgeDirection.BottomLeft:
            case TileTraversalEdgeDirection.Impossible:
                return false;
        }

        throw new Exception(string.Format("IsViablePosition_Center is missing implementation for {0}", edgeDirection));
    }

    private static bool IsViablePosition_CenterRight(this TileTraversalEdgeDirection edgeDirection)
    {
        switch (edgeDirection)
        {
            case TileTraversalEdgeDirection.Free:
            case TileTraversalEdgeDirection.AllEdges:
            case TileTraversalEdgeDirection.LeftRight:
            case TileTraversalEdgeDirection.RightBottomLeft:
            case TileTraversalEdgeDirection.LeftTopRight:
            case TileTraversalEdgeDirection.TopRight:
            case TileTraversalEdgeDirection.CenterRight:
            case TileTraversalEdgeDirection.BottomRight:
            case TileTraversalEdgeDirection.TopRightBottom:
                return true;
            case TileTraversalEdgeDirection.BottomCenter:
            case TileTraversalEdgeDirection.BottomLeft:
            case TileTraversalEdgeDirection.CenterLeft:
            case TileTraversalEdgeDirection.TopLeft:
            case TileTraversalEdgeDirection.TopCenter:
            case TileTraversalEdgeDirection.TopBottom:
            case TileTraversalEdgeDirection.BottomLeftTop:
            case TileTraversalEdgeDirection.Impossible:
                return false;
        }

        throw new Exception(string.Format("IsViablePosition_CenterRight is missing implementation for {0}", edgeDirection));
    }

    private static bool IsViablePosition_BottomLeft(this TileTraversalEdgeDirection edgeDirection)
    {
        switch (edgeDirection)
        {
            case TileTraversalEdgeDirection.Free:
            case TileTraversalEdgeDirection.BottomRight:
            case TileTraversalEdgeDirection.BottomLeft:
            case TileTraversalEdgeDirection.BottomCenter:
            case TileTraversalEdgeDirection.AllEdges:
            case TileTraversalEdgeDirection.TopBottom:
            case TileTraversalEdgeDirection.TopLeft:
            case TileTraversalEdgeDirection.CenterLeft:
            case TileTraversalEdgeDirection.LeftRight:
            case TileTraversalEdgeDirection.TopRightBottom:
            case TileTraversalEdgeDirection.RightBottomLeft:
            case TileTraversalEdgeDirection.BottomLeftTop:
            case TileTraversalEdgeDirection.LeftTopRight:
                return true;
            case TileTraversalEdgeDirection.TopCenter:
            case TileTraversalEdgeDirection.TopRight:
            case TileTraversalEdgeDirection.CenterRight:
            case TileTraversalEdgeDirection.Impossible:
                return false;
        }

        throw new Exception(string.Format("IsViablePosition_BottomLeft is missing implementation for {0}", edgeDirection));
    }

    private static bool IsViablePosition_BottomCenter(this TileTraversalEdgeDirection edgeDirection)
    {
        switch (edgeDirection)
        {
            case TileTraversalEdgeDirection.Free:
            case TileTraversalEdgeDirection.AllEdges:
            case TileTraversalEdgeDirection.TopBottom:
            case TileTraversalEdgeDirection.BottomLeftTop:
            case TileTraversalEdgeDirection.TopRightBottom:
            case TileTraversalEdgeDirection.BottomRight:
            case TileTraversalEdgeDirection.BottomCenter:
            case TileTraversalEdgeDirection.BottomLeft:
            case TileTraversalEdgeDirection.RightBottomLeft:
                return true;
            case TileTraversalEdgeDirection.CenterLeft:
            case TileTraversalEdgeDirection.TopCenter:
            case TileTraversalEdgeDirection.TopLeft:
            case TileTraversalEdgeDirection.TopRight:
            case TileTraversalEdgeDirection.CenterRight:
            case TileTraversalEdgeDirection.LeftRight:
            case TileTraversalEdgeDirection.LeftTopRight:
            case TileTraversalEdgeDirection.Impossible:
                return false;
        }


        throw new Exception(string.Format("IsViablePosition_BottomCenter is missing implementation for {0}", edgeDirection));
    }

    private static bool IsViablePosition_BottomRight(this TileTraversalEdgeDirection edgeDirection)
    {
        switch (edgeDirection)
        {
            case TileTraversalEdgeDirection.Free:
            case TileTraversalEdgeDirection.BottomRight:
            case TileTraversalEdgeDirection.BottomLeft:
            case TileTraversalEdgeDirection.BottomCenter:
            case TileTraversalEdgeDirection.AllEdges:
            case TileTraversalEdgeDirection.TopBottom:
            case TileTraversalEdgeDirection.LeftRight:
            case TileTraversalEdgeDirection.TopRightBottom:
            case TileTraversalEdgeDirection.RightBottomLeft:
            case TileTraversalEdgeDirection.BottomLeftTop:
            case TileTraversalEdgeDirection.LeftTopRight:
            case TileTraversalEdgeDirection.TopRight:
            case TileTraversalEdgeDirection.CenterRight:
                return true;
            case TileTraversalEdgeDirection.CenterLeft:
            case TileTraversalEdgeDirection.TopLeft:
            case TileTraversalEdgeDirection.TopCenter:
            case TileTraversalEdgeDirection.Impossible:
                return false;
        }

        throw new Exception(string.Format("IsViablePosition_BottomRight is missing implementation for {0}", edgeDirection));
    }

    public static TileTraversalEdgeDirection GetTileTraversalEdgeDirection(bool leftOpen, bool rightOpen, bool bottomOpen, bool topOpen)
    {
        if (leftOpen)
        {
            if (rightOpen)
            {
                if (topOpen)
                {
                    if (bottomOpen)
                        return TileTraversalEdgeDirection.AllEdges;
                    else
                        return TileTraversalEdgeDirection.LeftTopRight;
                }
                else
                {
                    if (bottomOpen)
                        return TileTraversalEdgeDirection.RightBottomLeft;
                    else
                        return TileTraversalEdgeDirection.LeftRight;
                }
            }
            else
            {
                if (topOpen)
                {
                    if (bottomOpen)
                        return TileTraversalEdgeDirection.BottomLeftTop;
                    else
                        return TileTraversalEdgeDirection.TopLeft;
                }
                else
                {
                    if (bottomOpen)
                        return TileTraversalEdgeDirection.BottomLeft;
                    else
                        return TileTraversalEdgeDirection.CenterLeft;
                }
            }
        }
        else
        {
            if (rightOpen)
            {
                if (topOpen)
                {
                    if (bottomOpen)
                        return TileTraversalEdgeDirection.TopRightBottom;
                    else
                        return TileTraversalEdgeDirection.TopRight;
                }
                else
                {
                    if (bottomOpen)
                        return TileTraversalEdgeDirection.BottomRight;
                    else
                        return TileTraversalEdgeDirection.CenterRight;
                }
            }
            else
            {
                if (topOpen)
                {
                    if (bottomOpen)
                        return TileTraversalEdgeDirection.TopBottom;
                    else
                        return TileTraversalEdgeDirection.TopCenter;
                }
                else
                {
                    if (bottomOpen)
                        return TileTraversalEdgeDirection.BottomCenter;
                    else
                        return TileTraversalEdgeDirection.Impossible;
                }
            }
        }
    }
}