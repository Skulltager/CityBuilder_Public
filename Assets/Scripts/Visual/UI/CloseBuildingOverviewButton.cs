
using UnityEngine;
using UnityEngine.UI;

public class CloseBuildingOverviewButton : MonoBehaviour
{
    [SerializeField] private Button button;

    private void Awake()
    {
        button.onClick.AddListener(OnPress_Button);
    }

    private void OnPress_Button()
    {
        BuildingOverviewUIManager.instance.data = null;
    }

    private void OnDestroy()
    {
        button.onClick.RemoveListener(OnPress_Button);
    }
}