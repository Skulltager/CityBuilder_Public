using UnityEngine;

public class BuildingEnterExitIndicatorData
{
    public readonly ChunkMapPoint chunkMapPoint;
    public readonly CardinalDirection enterExitDirection;

    public BuildingEnterExitIndicatorData(ChunkMapPoint chunkMapPoint, CardinalDirection enterExitDirection)
    {
        this.chunkMapPoint = chunkMapPoint;
        this.enterExitDirection = enterExitDirection;
    }
}

public class BuildingEnterExitIndicator : DataDrivenBehaviour<BuildingEnterExitIndicatorData>
{
    [SerializeField] private Material viableMaterial;
    [SerializeField] private Material notViableMaterial;
    [SerializeField] private MeshRenderer meshRenderer;

    public readonly EventVariable<BuildingEnterExitIndicator, bool> viable;

    private BuildingEnterExitIndicator()
        :base()
    {
        viable = new EventVariable<BuildingEnterExitIndicator, bool>(this, default);
    }

    private void Awake()
    {
        viable.onValueChangeImmediate += OnValueChanged_Viable;
    }

    protected override void OnValueChanged_Data(BuildingEnterExitIndicatorData oldValue, BuildingEnterExitIndicatorData newValue)
    {
        if (oldValue != null)
        {
            meshRenderer.enabled = false;
            oldValue.chunkMapPoint.blocked.onValueChange -= OnValueChanged_ChunkMapPoint_Blocked;
        }

        if (newValue != null)
        {
            meshRenderer.enabled = true;
            newValue.chunkMapPoint.blocked.onValueChange += OnValueChanged_ChunkMapPoint_Blocked;
            switch(newValue.enterExitDirection)
            {
                case CardinalDirection.Right:
                    transform.position = new Vector3(newValue.chunkMapPoint.globalPoint.xIndex, 0, newValue.chunkMapPoint.globalPoint.yIndex + 0.5f);
                    transform.rotation = Quaternion.Euler(0, 90, 0);
                    break;

                case CardinalDirection.Bottom:
                    transform.position = new Vector3(newValue.chunkMapPoint.globalPoint.xIndex + 0.5f, 0, newValue.chunkMapPoint.globalPoint.yIndex + 1);
                    transform.rotation = Quaternion.Euler(0, 180, 0);
                    break;

                case CardinalDirection.Left:
                    transform.position = new Vector3(newValue.chunkMapPoint.globalPoint.xIndex + 1, 0, newValue.chunkMapPoint.globalPoint.yIndex + 0.5f);
                    transform.rotation = Quaternion.Euler(0, 270, 0);
                    break;

                case CardinalDirection.Top:
                    transform.position = new Vector3(newValue.chunkMapPoint.globalPoint.xIndex + 0.5f, 0, newValue.chunkMapPoint.globalPoint.yIndex);
                    transform.rotation = Quaternion.Euler(0, 0, 0);
                    break;
            }
        }

        SetViableStatus();
    }

    private void OnValueChanged_ChunkMapPoint_Blocked(bool oldValue, bool newValue)
    {
        SetViableStatus();
    }

    private void SetViableStatus()
    {
        if (data == null)
        {
            viable.value = false;
            return;
        }

        if (data.chunkMapPoint.blocked.value)
        {
            viable.value = false;
            return;
        }

        viable.value = true;
    }

    private void OnValueChanged_Viable(bool oldValue, bool newValue)
    {
        meshRenderer.sharedMaterial = newValue ? viableMaterial : notViableMaterial;
    }

    private void OnDestroy()
    {
        viable.onValueChange -= OnValueChanged_Viable;
    }
}
