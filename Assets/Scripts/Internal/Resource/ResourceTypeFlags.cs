
using System;

[Flags]
public enum ResourceTypeFlags
{
    None = 0,
    Tree = 1 << 0,
    Rock = 1 << 1,
    Ore = 1 << 2,
    Bush = 1 << 3,
    ProcessedMaterial = 1 << 4,
    Product = 1 << 5,
    All = int.MaxValue,
}

public static class ResourceTypeExtension
{
    public static bool MatchesFlags(this ResourceType resourceType, ResourceTypeFlags resourceTypeFlags)
    {
        switch (resourceType)
        {
            case ResourceType.Tree:
                return resourceTypeFlags.HasFlag(ResourceTypeFlags.Tree);
            case ResourceType.Rock:
                return resourceTypeFlags.HasFlag(ResourceTypeFlags.Rock);
            case ResourceType.Ore:
                return resourceTypeFlags.HasFlag(ResourceTypeFlags.Ore);
            case ResourceType.Bush:
                return resourceTypeFlags.HasFlag(ResourceTypeFlags.Bush);
            case ResourceType.ProcessedMaterial:
                return resourceTypeFlags.HasFlag(ResourceTypeFlags.ProcessedMaterial);
            case ResourceType.Product:
                return resourceTypeFlags.HasFlag(ResourceTypeFlags.Product);
        }

        throw new System.Exception(string.Format("MathchesFlags is missing implementation for {0}", resourceType));
    }
}