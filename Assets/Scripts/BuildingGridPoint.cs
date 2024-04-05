
using System;
using UnityEngine;

[Serializable]
public class BuildingGridPoint
{
    public bool blocked => !canMoveLeft && !canMoveRight && !canMoveDown && !canMoveUp;
    [field: SerializeField] public Point centerOffset;
    [field: SerializeField] public bool canMoveLeft;
    [field: SerializeField] public bool canMoveRight;
    [field: SerializeField] public bool canMoveUp;
    [field: SerializeField] public bool canMoveDown;

}