using System.Collections.Generic;
using UnityEngine;

public class VillagerVisualizer : MonoBehaviour
{
    public static VillagerVisualizer instance { private set; get; }

    [SerializeField] private Transform villagersContainer;
    private bool destroyed;

    private readonly List<VillagerVisual> villagerInstances;

    private VillagerVisualizer()
        : base()
    {
        villagerInstances = new List<VillagerVisual>();
    }

    private void Awake()
    {
        instance = this;
    }

    public void Show(Villager villager)
    {
        if (destroyed)
            return;

        VillagerVisual instance = GameObject.Instantiate(villager.record.Prefab, villagersContainer);
        instance.data = villager;
        villagerInstances.Add(instance);
    }

    public void Hide(Villager villager)
    {
        if (destroyed)
            return;

        VillagerVisual instance = villagerInstances.Find(i => i.data == villager);
        villagerInstances.Remove(instance);
        GameObject.Destroy(instance.gameObject);
    }

    private void OnDestroy()
    {
        destroyed = true;
    }
}