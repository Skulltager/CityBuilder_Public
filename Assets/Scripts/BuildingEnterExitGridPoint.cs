
using System;
using UnityEngine;

[Serializable]
public class BuildingEnterExitGridPoint
{
    [field: SerializeField] public Point centerOffset;
    [field: SerializeField] public CardinalDirection direction;
}