
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/BuildingGridData", order = 1)]
public class BuildingGridData : ScriptableObject
{
    [field: SerializeField] public BuildingEnterExitGridPoint[] enterExitPoints { private set; get; }
    [field: SerializeField] public WorldContentSize size { private set; get; }
}