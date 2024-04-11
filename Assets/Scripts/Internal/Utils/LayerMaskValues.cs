public enum LayerMaskValue
{
    [Identifier("Default")] Default = 1 << 0,
    [Identifier("TransparentVFX")] TransparentVFX = 1 << 1,
    [Identifier("Ignore Raycast")] IgnoreRaycast = 1 << 2,

    [Identifier("Water")] Water = 1 << 4,
    [Identifier("UI")] UI = 1 << 5,
    [Identifier("Floor")] Floor = 1 << 6,
    [Identifier("Player")] Player = 1 << 7,
    [Identifier("Environment")] Environment = 1 << 8,
    [Identifier("Interactable")] Interactable = 1 << 9,
}