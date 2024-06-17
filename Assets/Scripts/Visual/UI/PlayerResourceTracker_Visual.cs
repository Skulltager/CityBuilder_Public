
using System.Collections.Generic;
using UnityEngine;

public class PlayerResourceTracker_Visual : DataDrivenUI<PlayerResourceTracker>
{
    [SerializeField] private TotalResourceData_Visual prefab;
    [SerializeField] private RectTransform container;

    private readonly List<TotalResourceData_Visual> instances;

    private PlayerResourceTracker_Visual()
    {
        instances = new List<TotalResourceData_Visual>();
    }

    protected override void OnValueChanged_Data(PlayerResourceTracker oldValue, PlayerResourceTracker newValue)
    {
        if (oldValue != null)
        {
            foreach (TotalResourceData_Visual instance in instances)
                GameObject.Destroy(instance.gameObject);

            instances.Clear();
        }    

        if (newValue != null)
        {
            foreach(TotalResourceData resourceData in newValue.totalResourceData.Values)
            {
                TotalResourceData_Visual instance = GameObject.Instantiate(prefab, container);
                instance.data = resourceData;
                instances.Add(instance);
            }
        }
    }
}