
using System.Collections.Generic;
using UnityEngine;

public class MapVisual : DataDrivenBehaviour<Map>
{
    [SerializeField] private ChunkMapVisual chunkMapVisualPrefab;
    [SerializeField] private Transform chunkMapVisualContainer;

    private readonly List<ChunkMapVisual> chunkMapVisualInstances;

    private MapVisual()
    {
        chunkMapVisualInstances = new List<ChunkMapVisual>();
    }

    protected override void OnValueChanged_Data(Map oldValue, Map newValue)
    {
        if (oldValue != null)
        {
            foreach (ChunkMapVisual instance in chunkMapVisualInstances)
                GameObject.Destroy(instance.gameObject);

            chunkMapVisualInstances.Clear();
            oldValue.generatedChunkMaps.onAdd -= OnAdd_ChunkMap;
            oldValue.generatedChunkMaps.onRemove -= OnRemove_ChunkMap;
        }

        if (newValue != null)
        {
            foreach (ChunkMap chunkMap in newValue.generatedChunkMaps)
                OnAdd_ChunkMap(chunkMap);
            
            newValue.generatedChunkMaps.onAdd += OnAdd_ChunkMap;
            newValue.generatedChunkMaps.onRemove += OnRemove_ChunkMap;
        }
    }

    private void OnAdd_ChunkMap(ChunkMap item)
    {
        ChunkMapVisual instance = GameObject.Instantiate(chunkMapVisualPrefab, chunkMapVisualContainer);
        instance.data = item;
        chunkMapVisualInstances.Add(instance);
    }

    private void OnRemove_ChunkMap(ChunkMap item)
    {
        ChunkMapVisual instance = chunkMapVisualInstances.Find(i => i.data == item);
        GameObject.Destroy(instance.gameObject);
        chunkMapVisualInstances.Remove(instance);
    }

    public bool TryGetMouseGridPoint(out Point point, float xOffset, float yOffset)
    {
        Plane plane = new Plane(Vector3.up, transform.position.y);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!plane.Raycast(ray, out float distance))
        {
            point = default;
            return false;
        }

        Vector3 hitPosition = ray.origin + ray.direction * distance;
        int xIndex = Mathf.FloorToInt(hitPosition.x - transform.position.x + xOffset);
        int yIndex = Mathf.FloorToInt(hitPosition.z - transform.position.z + yOffset);

        point = new Point(xIndex, yIndex);
        return true;
    }
}
