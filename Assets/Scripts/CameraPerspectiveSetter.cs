
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraPerspectiveSetter : MonoBehaviour
{
    private const string PROPERTY_INVERSE_PROJECTION_VIEW = "viewProjInv";
    private new Camera camera;

    private void Awake()
    {
        camera = GetComponent<Camera>();
    }

    private void OnPreRender()
    {
        Matrix4x4 viewMat = camera.worldToCameraMatrix;
        Matrix4x4 projMat = GL.GetGPUProjectionMatrix(camera.projectionMatrix, false);
        Matrix4x4 viewProjMat = (projMat * viewMat);
        Shader.SetGlobalMatrix(PROPERTY_INVERSE_PROJECTION_VIEW, viewProjMat.inverse);
    }
}