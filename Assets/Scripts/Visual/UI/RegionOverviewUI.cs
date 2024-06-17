
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RegionOverviewUIData
{
    public readonly Player player;
    public readonly ChunkRegion chunkRegion;

    public RegionOverviewUIData(Player player, ChunkRegion chunkRegion)
    {
        this.player = player;
        this.chunkRegion = chunkRegion;
    }
}

public class RegionOverviewUI : DataDrivenUI<RegionOverviewUIData>
{
    public static RegionOverviewUI instance { private set; get; }

    private readonly EventVariable<RegionOverviewUI, bool> unlockable;

    [SerializeField] private Button unlockRegionButton;
    [SerializeField] private TextMeshProUGUI regionNameText;
    [SerializeField] private TextMeshProUGUI buttonText;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private RegionContentItem regionContentItemPrefab;
    [SerializeField] private RectTransform regionContentItemContainer;

    private readonly List<RegionContentItem> regionContentItemInstances;

    private RegionOverviewUI()
    {
        unlockable = new EventVariable<RegionOverviewUI, bool>(this, false);
        regionContentItemInstances = new List<RegionContentItem>();
    }

    private void Awake()
    {
        unlockRegionButton.onClick.AddListener(OnPress_UnlockRegionButton);
        unlockable.onValueChangeImmediate += OnValueChanged_Unlockable;
        instance = this;
    }

    protected override void OnValueChanged_Data(RegionOverviewUIData oldValue, RegionOverviewUIData newValue)
    {
        if (oldValue != null)
        {
            oldValue.player.playerRegion.adjacentRegions.onAdd -= OnAdd_AdjacentRegion;
            oldValue.player.playerRegion.adjacentRegions.onRemove -= OnRemove_AdjacentRegion;

            foreach (RegionContentItem instance in regionContentItemInstances)
                GameObject.Destroy(instance.gameObject);

            regionContentItemInstances.Clear();
        }

        if (newValue != null)
        {
            newValue.player.playerRegion.adjacentRegions.onAdd += OnAdd_AdjacentRegion;
            newValue.player.playerRegion.adjacentRegions.onRemove += OnRemove_AdjacentRegion;
            regionNameText.text = newValue.chunkRegion.record.Name;

            foreach (ChunkRegionContentInfo contentInfo in newValue.chunkRegion.chunkRegionContentInfos)
            {
                RegionContentItem instance = GameObject.Instantiate(regionContentItemPrefab, regionContentItemContainer);
                instance.data = contentInfo;
                regionContentItemInstances.Add(instance);

            }

            SetUnlockableState();
            canvasGroup.Show();
        }
        else
        {
            canvasGroup.Hide();
        }
    }

    private void OnValueChanged_Unlockable(bool oldValue, bool newValue)
    {
        if(newValue)
        {
            unlockRegionButton.interactable = true;
            buttonText.text = "Unlock Region";
        }
        else
        {
            unlockRegionButton.interactable = false;
            buttonText.text = "Not neightboring this region"; 
        }
    }

    private void OnAdd_AdjacentRegion(ChunkRegion item)
    {
        SetUnlockableState();
    }

    private void OnRemove_AdjacentRegion(ChunkRegion item)
    {
        SetUnlockableState();
    }

    private void SetUnlockableState()
    {
        if (!data.player.playerRegion.adjacentRegions.Contains(data.chunkRegion))
        {
            unlockable.value = false;
            return;
        }

        unlockable.value = true;
    }

    private void OnPress_UnlockRegionButton()
    {
        data.chunkRegion.map.MergeRegionsTogether(data.player.playerRegion, data.chunkRegion);
        data = null;
    }

    private void OnDestroy()
    {
        unlockable.onValueChangeImmediate -= OnValueChanged_Unlockable;
        unlockRegionButton.onClick.RemoveListener(OnPress_UnlockRegionButton);
    }
}