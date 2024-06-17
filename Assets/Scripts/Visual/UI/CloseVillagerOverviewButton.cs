
using UnityEngine;
using UnityEngine.UI;

public class CloseVillagerOverviewButton : MonoBehaviour
{
    [SerializeField] private Button button;

    private void Awake()
    {
        button.onClick.AddListener(OnPress_Button);
    }

    private void OnPress_Button()
    {
        VillagerVisual.selectedVillager.value = null;
    }

    private void OnDestroy()
    {
        button.onClick.RemoveListener(OnPress_Button);
    }
}