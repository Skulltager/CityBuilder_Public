
using UnityEngine;

public class FogOfWarRepositioner : MonoBehaviour
{
    [SerializeField] private float height;

    [SerializeField] private Transform surfaceTransform;

    public void LateUpdate()
    {
        Camera camera = Camera.main;
        if (camera == null)
            return;

        Vector3 direction = (camera.transform.position - surfaceTransform.position).normalized;
        transform.position = direction * height / Mathf.Abs(direction.y) + surfaceTransform.position;
    }
}