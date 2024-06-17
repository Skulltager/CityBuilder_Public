using SheetCodes;
using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class InstancedWorldResource
{
    private const int BUFFER_SIZE_INCREASE = 100;
    private const int BUFFER_SIZE_START = 100;

    public readonly WorldResourceRecord record;
    public readonly Mesh mesh;
    public readonly Material[] materials;
    public readonly uint[][] argumentArrays;
    public readonly ComputeBuffer[] argumentBuffers;
    public ComputeBuffer positionBuffer;
    public ComputeBuffer rotationBuffer;
    public Vector3[] positionArray;
    public Quaternion[] rotationArray;
    public ChunkMapPointContent_WorldResource[] resourceArray;
    private uint instancedCount;
    private bool dirty;

    public InstancedWorldResource(WorldResourceRecord record, int modelIndex)
    {
        this.record = record;

        mesh = record.ModelVariations[modelIndex].GetComponent<MeshFilter>().sharedMesh;
        materials = new Material[record.Material.Length];
        argumentBuffers = new ComputeBuffer[record.Material.Length];
        argumentArrays = new uint[record.Material.Length][];
        for (int i = 0; i < materials.Length; i++)
        {
            argumentBuffers[i] = new ComputeBuffer(1, 5 * sizeof(uint), ComputeBufferType.IndirectArguments);
            materials[i] = new Material(record.Material[i]);
            argumentArrays[i] = new uint[5];
            argumentArrays[i][0] = mesh.GetIndexCount(i);
            argumentArrays[i][2] = mesh.GetIndexStart(i);
            argumentArrays[i][3] = mesh.GetBaseVertex(i);
            argumentArrays[i][4] = 0; // offset
        }

        rotationArray = new Quaternion[BUFFER_SIZE_START];
        resourceArray = new ChunkMapPointContent_WorldResource[BUFFER_SIZE_START];
        positionArray = new Vector3[BUFFER_SIZE_START];
        float scale = record.ModelVariations[modelIndex].transform.localScale.x;

        rotationBuffer = new ComputeBuffer(BUFFER_SIZE_START, Marshal.SizeOf(typeof(Matrix4x4)));
        positionBuffer = new ComputeBuffer(BUFFER_SIZE_START, Marshal.SizeOf(typeof(Vector3)));

        for (int i = 0; i < materials.Length; i++)
        {
            Material material = materials[i];
            material.SetFloat("scale", scale);
            material.SetBuffer("rotationBuffer", rotationBuffer);
            material.SetBuffer("positionBuffer", positionBuffer);
        }
    }

    public void AddWorldResource(ChunkMapPointContent_WorldResource worldResource)
    {
        if (worldResource.isInstanced)
            return;

        if(instancedCount == resourceArray.Length)
        {
            rotationBuffer.Dispose();
            positionBuffer.Dispose();
            rotationBuffer = new ComputeBuffer((int) instancedCount + BUFFER_SIZE_INCREASE, Marshal.SizeOf(typeof(Quaternion)));
            positionBuffer = new ComputeBuffer((int)instancedCount + BUFFER_SIZE_INCREASE, Marshal.SizeOf(typeof(Vector3)));

            ChunkMapPointContent_WorldResource[] newResourceArray = new ChunkMapPointContent_WorldResource[instancedCount + BUFFER_SIZE_INCREASE];
            Quaternion[] newRotationArray = new Quaternion[instancedCount + BUFFER_SIZE_INCREASE];
            Vector3[] newPositionArray = new Vector3[instancedCount + BUFFER_SIZE_INCREASE];
            Array.Copy(resourceArray, newResourceArray, instancedCount);
            Array.Copy(rotationArray, newRotationArray, instancedCount);
            Array.Copy(positionArray, newPositionArray, instancedCount);
            rotationArray = newRotationArray;
            resourceArray = newResourceArray;
            positionArray = newPositionArray;

            for (int i = 0; i < materials.Length; i++)
            {
                Material material = materials[i];
                material.SetBuffer("rotationBuffer", rotationBuffer);
                material.SetBuffer("positionBuffer", positionBuffer);
            }
        }

        positionArray[instancedCount] = worldResource.position;
        rotationArray[instancedCount] = worldResource.rotation;
        resourceArray[instancedCount] = worldResource;
        worldResource.instancedIndex = instancedCount;
        worldResource.isInstanced = true;
        instancedCount++;
        dirty = true;
    }

    public void RemoveWorldResource(ChunkMapPointContent_WorldResource worldResource)
    {
        if (!worldResource.isInstanced)
            return;

        if (worldResource.instancedIndex == instancedCount - 1)
        {
            instancedCount--;
            return;
        }

        ChunkMapPointContent_WorldResource otherWorldResource = resourceArray[instancedCount - 1];

        resourceArray[worldResource.instancedIndex] = otherWorldResource;
        positionArray[worldResource.instancedIndex] = otherWorldResource.position;
        rotationArray[worldResource.instancedIndex] = otherWorldResource.rotation;
        otherWorldResource.instancedIndex = worldResource.instancedIndex;

        worldResource.isInstanced = false;
        instancedCount--;
        dirty = true;
    }

    public void Draw()
    {
        if (dirty)
        {
            rotationBuffer.SetData(rotationArray);
            positionBuffer.SetData(positionArray);
            dirty = false;
        }

        for (int i = 0; i < materials.Length; i++)
        {
            argumentArrays[i][1] = instancedCount;
            argumentBuffers[i].SetData(argumentArrays[i]);

            Material material = materials[i];
            Graphics.DrawMeshInstancedIndirect(mesh, 0, material, new Bounds(Vector3.zero, Vector3.one * 10000), argumentBuffers[i], 0, default, UnityEngine.Rendering.ShadowCastingMode.TwoSided);
        }
    }

    public void Dispose()
    {
        rotationBuffer.Dispose();
        positionBuffer.Dispose();
        foreach (ComputeBuffer argumentsBuffer in argumentBuffers)
            argumentsBuffer.Dispose();
    }
}