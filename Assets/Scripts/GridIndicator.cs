
using UnityEngine;

public class GridIndicator : MonoBehaviour
{
    [SerializeField] private Material notViableIndicator;
    [SerializeField] private Material viableIndicator;
    [SerializeField] private MeshRenderer meshRenderer;

    public void SetBlocked()
    {
        meshRenderer.sharedMaterial = notViableIndicator;
    }

    public void SetViable()
    {
        meshRenderer.sharedMaterial = viableIndicator;
    }
}