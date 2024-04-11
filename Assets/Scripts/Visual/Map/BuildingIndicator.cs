
using UnityEngine;

public class BuildingIndicator : DataDrivenBehaviour<ChunkMapPoint>
{
    [SerializeField] private Material viableMaterial;
    [SerializeField] private Material notViableMaterial;
    [SerializeField] private MeshRenderer meshRenderer;

    public readonly EventVariable<BuildingIndicator, bool> viable;

    private BuildingIndicator()
        : base()
    {
        viable = new EventVariable<BuildingIndicator, bool>(this, default);
    }

    private void Awake()
    {
        viable.onValueChangeImmediate += OnValueChanged_Viable;
    }

    protected override void OnValueChanged_Data(ChunkMapPoint oldValue, ChunkMapPoint newValue)
    {
        if (oldValue != null)
        {
            oldValue.canBeBlocked.onValueChange -= OnValueChanged_ChunkMapPoint_CanBeBlocked;
            meshRenderer.enabled = false;
        }

        if (newValue != null)
        {
            transform.position = new Vector3(newValue.globalPoint.xIndex + 0.5f, 0, newValue.globalPoint.yIndex + 0.5f);
            newValue.canBeBlocked.onValueChange += OnValueChanged_ChunkMapPoint_CanBeBlocked;
            meshRenderer.enabled = true;
        }

        SetViableStatus();
    }

    private void OnValueChanged_ChunkMapPoint_CanBeBlocked(bool oldValue, bool newValue)
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

        if (!data.canBeBlocked.value)
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