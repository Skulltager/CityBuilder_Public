using UnityEngine;

public class ChunkMapPointContent_Blocker : ChunkMapPointContent
{
    public override string name => "blocker";
    public override ChunkMapPointContentType contentType => ChunkMapPointContentType.WorldResource;
    public override ContentTaskLocation contentTaskLocation => ContentTaskLocation.Edge;
    public readonly Vector3 position;
    public readonly Quaternion rotation;

    public bool isInstanced;
    public uint instancedIndex;

    public ChunkMapPointContent_Blocker(Map map, Point pivotPoint)
        : base(map, pivotPoint, CardinalDirection.Right, new WorldContentSize(1, 1))
    {
        position = new Vector3(pivotPoint.xIndex + 0.5f, 0, pivotPoint.yIndex + 0.5f);
        rotation = Quaternion.Euler(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f));
    }

    public override void Hide()
    {
        InstancingManager.instance.RemoveInstancingItem(this);
    }

    public override void Show()
    {
        InstancingManager.instance.AddInstancingItem(this);
    }
}