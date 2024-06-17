public enum LayerMaskValue
{
    [Identifier("Default")] Default = 1 << 0,
    [Identifier("TransparentVFX")] TransparentVFX = 1 << 1,
    [Identifier("Ignore Raycast")] IgnoreRaycast = 1 << 2,

    [Identifier("Water")] Water = 1 << 4,
    [Identifier("UI")] UI = 1 << 5,
    [Identifier("Villagers")] Villagers = 1 << 6,
}