
public enum BuildingTaskPriority
{
    [Identifier("Very High")] VeryHigh,
    [Identifier("High")] High,
    [Identifier("Medium")] Medium,
    [Identifier("Low")] Low,
    [Identifier("Very Low")] VeryLow,
}

public static class BuildingTaskPriorityExtension
{
    public static BuildingTaskPriority DecreasePriority(this BuildingTaskPriority priority)
    {
        switch (priority)
        {
            case BuildingTaskPriority.VeryHigh:
                return BuildingTaskPriority.High;
            case BuildingTaskPriority.High:
                return BuildingTaskPriority.Medium;
            case BuildingTaskPriority.Medium:
                return BuildingTaskPriority.Low;
            case BuildingTaskPriority.Low:
                return BuildingTaskPriority.VeryLow;
        }

        throw new System.Exception(string.Format("DecreasePriority is missing implementation for {0}", priority));
    }

    public static BuildingTaskPriority IncreasePriority(this BuildingTaskPriority priority)
    {
        switch (priority)
        {
            case BuildingTaskPriority.High:
                return BuildingTaskPriority.VeryHigh;
            case BuildingTaskPriority.Medium:
                return BuildingTaskPriority.High;
            case BuildingTaskPriority.Low:
                return BuildingTaskPriority.Medium;
            case BuildingTaskPriority.VeryLow:
                return BuildingTaskPriority.Low;
        }

        throw new System.Exception(string.Format("IncreasePriority is missing implementation for {0}", priority));
    }
}