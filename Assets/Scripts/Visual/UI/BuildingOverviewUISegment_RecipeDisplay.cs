
using System.Collections.Generic;
using UnityEngine;

public class BuildingOverviewUISegment_RecipeDisplay : DataDrivenUI<ChunkMapPointContent_Building_CraftingStation>
{
    [SerializeField] private RecipeInputUI recipeInputUIPrefab;
    [SerializeField] private RecipeOutputUI recipeOutputUIPrefab;

    [SerializeField] private GameObject plusPrefab;
    [SerializeField] private GameObject arrowPrefab;

    [SerializeField] private RectTransform recipeItemsContainer;

    private readonly List<RecipeInputUI> recipeInputInstances;
    private readonly List<RecipeOutputUI> recipeOutputInstances;
    private readonly List<GameObject> otherInstances;

    private BuildingOverviewUISegment_RecipeDisplay()
    {
        recipeInputInstances = new List<RecipeInputUI>();
        recipeOutputInstances = new List<RecipeOutputUI>();
        otherInstances = new List<GameObject>();
    }

    protected override void OnValueChanged_Data(ChunkMapPointContent_Building_CraftingStation oldValue, ChunkMapPointContent_Building_CraftingStation newValue)
    {

        if (oldValue != null)
        {

            foreach (GameObject instance in otherInstances)
                GameObject.Destroy(instance);

            foreach (RecipeOutputUI instance in recipeOutputInstances)
                GameObject.Destroy(instance.gameObject);

            foreach (RecipeInputUI instance in recipeInputInstances)
                GameObject.Destroy(instance.gameObject);

            recipeInputInstances.Clear();
            recipeOutputInstances.Clear();
            otherInstances.Clear();
        }

        if (newValue != null)
        {
            for(int i = 0; i < newValue.craftingSettings.Length; i++)
            {
                CraftingSetting craftingSetting = newValue.craftingSettings[i];
                if (i > 0)
                {
                    GameObject plusInstance = GameObject.Instantiate(plusPrefab, recipeItemsContainer);
                    otherInstances.Add(plusInstance);
                }

                RecipeInputUI recipeInputInstance = GameObject.Instantiate(recipeInputUIPrefab, recipeItemsContainer);
                recipeInputInstance.data = craftingSetting;
                recipeInputInstances.Add(recipeInputInstance);
            }

            GameObject arrowInstance = GameObject.Instantiate(arrowPrefab, recipeItemsContainer);
            otherInstances.Add(arrowInstance);

            RecipeOutputUI recipeOutputInstance = GameObject.Instantiate(recipeOutputUIPrefab, recipeItemsContainer);
            recipeOutputInstance.data = newValue.record.CraftingRecipes;
        }
    }
}