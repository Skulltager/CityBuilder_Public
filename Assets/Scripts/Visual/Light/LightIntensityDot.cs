
using UnityEngine;

public class LightIntensityDot : MonoBehaviour
{
    [SerializeField] private new Light light;
    [SerializeField] private float rotateSpeed;
    [SerializeField] private float lowestLightDot;

    private void Update()
    {
        transform.Rotate(rotateSpeed * Time.deltaTime, 0, 0);
        light.intensity = Mathf.Clamp01((Vector3.Dot(Vector3.down, light.transform.forward) - lowestLightDot) * (1 / lowestLightDot));
    }
}