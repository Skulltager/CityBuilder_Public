
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RegionContentItem : DataDrivenUI<ChunkRegionContentInfo>
{
    [SerializeField] private Image contentIcon;
    [SerializeField] private TextMeshProUGUI contentText;

    protected override void OnValueChanged_Data(ChunkRegionContentInfo oldValue, ChunkRegionContentInfo newValue)
    {
        if (newValue != null)
        {
            contentIcon.sprite = newValue.record.Icon;
            contentText.text = string.Format("{0} x {1}", newValue.record.Name, newValue.count);
        }
    }
}