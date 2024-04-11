
public class BuildingGridSize
{
    public int currentWidth { private set; get; }
    public int currentHeight { private set; get; }
    public float currentXOffset { private set; get; }
    public float currentYOffset { private set; get; }
    public CardinalDirection currentRotation { private set; get; }

    public BuildingGridSize(int width, int height, float xOffset, float yOffset, CardinalDirection startRotation)
    {
        currentHeight = height;
        currentWidth = width;
        currentXOffset = xOffset;
        currentYOffset = yOffset;
        currentRotation = startRotation;
    }

    public void SetRotation(CardinalDirection rotation)
    {
        switch (currentRotation)
        {
            case CardinalDirection.Top:
                switch (rotation)
                {
                    case CardinalDirection.Right:
                        SetRotation90Degrees();
                        break;
                    case CardinalDirection.Bottom:
                        SetRotation180Degrees();
                        break;
                    case CardinalDirection.Left:
                        SetRotation270Degrees();
                        break;
                }
                break;
            case CardinalDirection.Right:
                switch (rotation)
                {
                    case CardinalDirection.Bottom:
                        SetRotation90Degrees();
                        break;
                    case CardinalDirection.Left:
                        SetRotation180Degrees();
                        break;
                    case CardinalDirection.Top:
                        SetRotation270Degrees();
                        break;
                }
                break;
            case CardinalDirection.Bottom:
                switch (rotation)
                {
                    case CardinalDirection.Left:
                        SetRotation90Degrees();
                        break;
                    case CardinalDirection.Top:
                        SetRotation180Degrees();
                        break;
                    case CardinalDirection.Right:
                        SetRotation270Degrees();
                        break;
                }
                break;
            case CardinalDirection.Left:
                switch (rotation)
                {
                    case CardinalDirection.Top:
                        SetRotation90Degrees();
                        break;
                    case CardinalDirection.Right:
                        SetRotation180Degrees();
                        break;
                    case CardinalDirection.Bottom:
                        SetRotation270Degrees();
                        break;
                }
                break;
        }

        currentRotation = rotation;
    }

    private void SetRotation90Degrees()
    {
        int tempSize = currentWidth;
        currentWidth = currentHeight;
        currentHeight = tempSize;

        float tempOffset = currentXOffset;
        currentXOffset = currentYOffset;
        currentYOffset = -tempOffset;
    }

    private void SetRotation180Degrees()
    {
        currentXOffset = -currentXOffset;
        currentYOffset = -currentYOffset;
    }

    private void SetRotation270Degrees()
    {
        int temp = currentWidth;
        currentWidth = currentHeight;
        currentHeight = temp;

        float tempOffset = currentXOffset;
        currentXOffset = -currentYOffset;
        currentYOffset = tempOffset;
    }
}
