using System;
using System.Runtime.InteropServices;
using UnityEngine;
using SheetCodes;

public class InstancedBlocker
{
    private const int BUFFER_SIZE_INCREASE = 100;
    private const int BUFFER_SIZE_START = 100;

    public readonly Mesh mesh;
    public readonly Material[] materials;
    public readonly uint[][] argumentArrays;
    public readonly ComputeBuffer[] argumentBuffers;
    public ComputeBuffer positionBuffer;
    public ComputeBuffer rotationBuffer;
    public Vector3[] positionArray;
    public Quaternion[] rotationArray;
    public ChunkMapPointContent_Blocker[] blockerArray;
    private uint instancedCount;
    private bool dirty;

    public InstancedBlocker()
    {
        BlockerRecord record = BlockerIdentifier.Wall.GetRecord();
        mesh = record.Prefab.GetComponent<MeshFilter>().sharedMesh;
        materials = new Material[record.Materials.Length];
        argumentBuffers = new ComputeBuffer[record.Materials.Length];
        argumentArrays = new uint[record.Materials.Length][];
        for (int i = 0; i < materials.Length; i++)
        {
            argumentBuffers[i] = new ComputeBuffer(1, 5 * sizeof(uint), ComputeBufferType.IndirectArguments);
            materials[i] = new Material(record.Materials[i]);
            argumentArrays[i] = new uint[5];
            argumentArrays[i][0] = mesh.GetIndexCount(i);
            argumentArrays[i][2] = mesh.GetIndexStart(i);
            argumentArrays[i][3] = mesh.GetBaseVertex(i);
            argumentArrays[i][4] = 0; // offset
        }

        rotationArray = new Quaternion[BUFFER_SIZE_START];
        blockerArray = new ChunkMapPointContent_Blocker[BUFFER_SIZE_START];
        positionArray = new Vector3[BUFFER_SIZE_START];

        rotationBuffer = new ComputeBuffer(BUFFER_SIZE_START, Marshal.SizeOf(typeof(Matrix4x4)));
        positionBuffer = new ComputeBuffer(BUFFER_SIZE_START, Marshal.SizeOf(typeof(Vector3)));

        for (int i = 0; i < materials.Length; i++)
        {
            Material material = materials[i];
            material.SetFloat("scale", record.Prefab.transform.lossyScale.x);
            material.SetBuffer("rotationBuffer", rotationBuffer);
            material.SetBuffer("positionBuffer", positionBuffer);
        }
    }

    public void AddBlocker(ChunkMapPointContent_Blocker blocker)
    {
        if (blocker.isInstanced)
            return;

        if (instancedCount == blockerArray.Length)
        {
            rotationBuffer.Dispose();
            positionBuffer.Dispose();
            rotationBuffer = new ComputeBuffer((int)instancedCount + BUFFER_SIZE_INCREASE, Marshal.SizeOf(typeof(Quaternion)));
            positionBuffer = new ComputeBuffer((int)instancedCount + BUFFER_SIZE_INCREASE, Marshal.SizeOf(typeof(Vector3)));

            ChunkMapPointContent_Blocker[] newBlockerArray = new ChunkMapPointContent_Blocker[instancedCount + BUFFER_SIZE_INCREASE];
            Quaternion[] newRotationArray = new Quaternion[instancedCount + BUFFER_SIZE_INCREASE];
            Vector3[] newPositionArray = new Vector3[instancedCount + BUFFER_SIZE_INCREASE];
            Array.Copy(blockerArray, newBlockerArray, instancedCount);
            Array.Copy(rotationArray, newRotationArray, instancedCount);
            Array.Copy(positionArray, newPositionArray, instancedCount);
            rotationArray = newRotationArray;
            blockerArray = newBlockerArray;
            positionArray = newPositionArray;

            for (int i = 0; i < materials.Length; i++)
            {
                Material material = materials[i];
                material.SetBuffer("rotationBuffer", rotationBuffer);
                material.SetBuffer("positionBuffer", positionBuffer);
            }
        }

        positionArray[instancedCount] = blocker.position;
        rotationArray[instancedCount] = blocker.rotation;
        blockerArray[instancedCount] = blocker;
        blocker.instancedIndex = instancedCount;
        blocker.isInstanced = true;
        instancedCount++;
        dirty = true;
    }

    public void RemoveBlocker(ChunkMapPointContent_Blocker blocker)
    {
        if (!blocker.isInstanced)
            return;

        if (blocker.instancedIndex == instancedCount - 1)
        {
            instancedCount--;
            return;
        }

        ChunkMapPointContent_Blocker otherBlocker = blockerArray[instancedCount - 1];

        blockerArray[blocker.instancedIndex] = otherBlocker;
        positionArray[blocker.instancedIndex] = otherBlocker.position;
        rotationArray[blocker.instancedIndex] = otherBlocker.rotation;
        otherBlocker.instancedIndex = blocker.instancedIndex;

        blocker.isInstanced = false;
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