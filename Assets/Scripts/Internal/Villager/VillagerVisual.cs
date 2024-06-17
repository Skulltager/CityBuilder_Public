
using UnityEngine;

public class VillagerVisual : DataDrivenBehaviour<Villager>
{
    private const string ANIMATOR_PROPERTY_MOVEMENT_SPEED = "Movement Speed";
    private const string ANIMATOR_PROPERTY_CARRYING_RESOURCE = "Carrying Resource";

    [SerializeField] private Animator animator;
    [SerializeField] private SkinnedMeshRenderer meshRenderer;
    [SerializeField] private VillagerTool[] villagerTools;
    [SerializeField] private float movementSpeedAnimationMultiplier;
    [SerializeField] private Transform carryingResourceContainer;
    [SerializeField] private Material hoveringMaterial;
    [SerializeField] private Material selectedMaterial;
    [SerializeField] private Material defaultMaterial;

    public static readonly EventVariable<VillagerVisual, VillagerVisual> hoveringVillager;
    public static readonly EventVariable<VillagerVisual, VillagerVisual> selectedVillager;

    static VillagerVisual()
    {
        hoveringVillager = new EventVariable<VillagerVisual, VillagerVisual>(null, null);
        selectedVillager = new EventVariable<VillagerVisual, VillagerVisual>(null, null);
    }

    private GameObject carryingResourceInstance;

    private float lastAnimationUpdateMoment;

    private void Awake()
    {
        hoveringVillager.onValueChange += OnValueChanged_HoveringVillager;
        selectedVillager.onValueChange += OnValueChanged_SelectedVillager;
        SetMaterial();
    }

    protected override void OnValueChanged_Data(Villager oldValue, Villager newValue)
    {
        if (oldValue != null)
        {
            oldValue.position.onValueChange -= OnValueChanged_Position;
            oldValue.currentAnimation.onValueChange -= OnValueChanged_CurrentAnimation;
            oldValue.currentRotation.onValueChange -= OnValueChanged_Rotation;
            oldValue.currentMovementSpeed.onValueChange -= OnValueChanged_CurrentMovementSpeed;
            oldValue.inventory.onValueChange -= OnValueChanged_Inventory;
        }

        if (newValue != null)
        {
            newValue.position.onValueChangeImmediate += OnValueChanged_Position;
            newValue.currentAnimation.onValueChangeImmediate += OnValueChanged_CurrentAnimation;
            newValue.currentRotation.onValueChangeImmediate += OnValueChanged_Rotation;
            newValue.currentMovementSpeed.onValueChangeImmediate += OnValueChanged_CurrentMovementSpeed;
            newValue.inventory.onValueChangeImmediate += OnValueChanged_Inventory;
        }

        foreach (VillagerTool villagerTool in villagerTools)
            villagerTool.data = newValue;
    }

    private void OnValueChanged_Inventory(ResourceCount oldValue, ResourceCount newValue)
    {
        if (oldValue != null)
        {
            GameObject.Destroy(carryingResourceInstance);
            carryingResourceInstance = null;
        }

        if (newValue != null)
        {
            carryingResourceInstance = GameObject.Instantiate(newValue.record.Prefab, carryingResourceContainer);
            animator.SetBool(ANIMATOR_PROPERTY_CARRYING_RESOURCE, true);
        }
        else
            animator.SetBool(ANIMATOR_PROPERTY_CARRYING_RESOURCE, false);
    }

    private void OnValueChanged_CurrentMovementSpeed(float oldValue, float newValue)
    {
        animator.SetFloat(ANIMATOR_PROPERTY_MOVEMENT_SPEED, newValue * movementSpeedAnimationMultiplier);
    }

    private void OnValueChanged_Rotation(float oldValue, float newValue)
    {
        transform.rotation = Quaternion.Euler(0, newValue, 0);
    }

    private void OnValueChanged_Position(Vector2 oldValue, Vector2 newValue)
    {
        transform.position = new Vector3(newValue.x, 0, newValue.y);
    }

    private void OnValueChanged_CurrentAnimation(AnimationType_Villager oldValue, AnimationType_Villager newValue)
    {
        if (Time.time == lastAnimationUpdateMoment)
        {
            string oldAnimationTrigger = oldValue.GetIdentifier();
            animator.ResetTrigger(oldAnimationTrigger);
        }

        string newAnimationTrigger = newValue.GetIdentifier();
        animator.SetTrigger(newAnimationTrigger);
        lastAnimationUpdateMoment = Time.time;

        switch (newValue)
        {
            case AnimationType_Villager.Idle:
                carryingResourceContainer.gameObject.SetActive(true);
                break;
            case AnimationType_Villager.Walk:
                carryingResourceContainer.gameObject.SetActive(true);
                break;
            case AnimationType_Villager.Mining:
                carryingResourceContainer.gameObject.SetActive(false);
                break;
            case AnimationType_Villager.Woodcutting:
                carryingResourceContainer.gameObject.SetActive(false);
                break;
            case AnimationType_Villager.Gathering:
                carryingResourceContainer.gameObject.SetActive(false);
                break;
            case AnimationType_Villager.Building:
                carryingResourceContainer.gameObject.SetActive(false);
                break;
        }
    }

    private void OnValueChanged_HoveringVillager(VillagerVisual oldValue, VillagerVisual newValue)
    {
        SetMaterial();
    }

    private void OnValueChanged_SelectedVillager(VillagerVisual oldValue, VillagerVisual newValue)
    {
        SetMaterial();
    }

    private void SetMaterial()
    {
        if (selectedVillager.value == this)
        {
            meshRenderer.sharedMaterial = selectedMaterial;
            return;
        }

        if (hoveringVillager.value == this)
        {
            meshRenderer.sharedMaterial = hoveringMaterial;
            return;
        }

        meshRenderer.sharedMaterial = defaultMaterial;
    }

    private void FixedUpdate()
    {
        if (data != null)
            data.FixedUpdate();
    }

    private void OnDestroy()
    {
        hoveringVillager.onValueChange -= OnValueChanged_HoveringVillager;
        selectedVillager.onValueChange -= OnValueChanged_SelectedVillager;
    }
}